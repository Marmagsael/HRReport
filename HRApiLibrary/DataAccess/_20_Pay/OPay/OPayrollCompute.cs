using HRApiLibrary.Models._20_Pay.M0605;
using HRApiLibrary.Models._20_Pay.OPay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

//--- Port of MBPay forms/payroll4.scx bProcess.Click and the procedures it calls, in the same order. ------------------
//--- The session already holds everything payrollnew4 fetched (OPayrollDataAccess._01Load). Nothing here reads the DB.
//---   Before the employee loop : zero_tmpdata_amt, post_fixedEarnings, post_otherearnings, post_retroPay
//---   Per employee             : compute_basic, compute_absent, compute_fixed_earnings_grp, compute_ordinaryday, compute_ot x30,
//---                              compute_allowance, compute_mealallowance, ecola, ctpa, sea, incentive leave, retirement,
//---                              free uniform, SSS, (d02gsis does not exist in payroll4), Pag-IBIG, PhilHealth, tax
//---   After the loop           : grp_deduction, calc_loans, hospitalization, empdedexemption, dedMandatory, process_13thMo_added
public static class OPayrollCompute
{
    //--- paymaindtl hour/day columns, in the order of the FoxPro grid (grdPayrollDtls columns 4-45) ---
    public static readonly string[] HourColumns =
    [
        "daywrk", "rn", "absent", "rot", "rd", "rdot", "lh", "lhot", "rdlh", "rdlhot", "sh", "shot", "rdsh", "rdshot",
        "sea", "sealh", "seardlh", "ctpa", "ctpalh", "ctpardlh", "custom1", "custom2", "custom3", "late", "utime",
        "nd", "ndot", "ndlh", "ndlhot", "ndsh", "ndshot", "ndrd", "ndrdot", "ndrdlh", "ndrdlhot", "ndrdsh", "ndrdshot",
        "dh", "dhot", "rddh", "rddhot"
    ];

    //--- paymaindtl columns saved besides the ones in HourColumns ---
    public static readonly string[] OtherHourColumns = ["rnot"];

    //--- Order of the compute_ot calls in bProcess.Click ---
    static readonly string[] OtOrder =
    [
        "rot", "rd", "rdot", "lh", "lhot", "rdlh", "rdlhot", "sh", "shot", "rdsh", "rdshot",
        "nd", "ndot", "ndlh", "ndlhot", "ndsh", "ndshot",
        "ndrd", "ndrdot", "ndrdlh", "ndrdlhot", "ndrdsh", "ndrdshot",
        "custom1", "custom2", "custom3", "dh", "dhot", "rddh", "rddhot"
    ];

    //--- Columns the FoxPro grid shows (hide_payacct) : account set, account found, and show01/show02 for periods 01/02 ---
    public static List<string> InputColumns(OPayrollSession s)
    {
        var coa = s.Coas.Where(c => c.AcctNumber != null).GroupBy(c => c.AcctNumber!.Trim()).ToDictionary(g => g.Key, g => g.First());
        return HourColumns.Where(c =>
        {
            var a = Acct(s, c);
            if (a == "" || !coa.TryGetValue(a, out var x)) return false;
            if (c.StartsWith("custom")) return true;
            if (s.Prd == "01") return x.Show01 != 0;
            if (s.Prd == "02") return x.Show02 != 0;
            return true;
        }).ToList();
    }

    public static string Acct(OPayrollSession s, string column)
        => s.Setup.TryGetValue(column, out var a) ? (a ?? "").Trim() : "";

    public static double R4(double v) => Math.Round(v, OPayrollConstants.AmountDecimals, MidpointRounding.AwayFromZero);

    static double Hrs(OPayEmpResult r, string column) => r.Hours.TryGetValue(column, out var v) ? v : 0;
    static string Left4(string? s) => (s ?? "").Trim() is { Length: > 4 } t ? t[..4] : (s ?? "").Trim();
    static bool Between(double x, double a, double b) => x >= a && x <= b;

    //--- tmpdata : only accounts of chartofacct exist as rows, REPLACE ... FOR touches nothing for any other account ---
    sealed class Ctx
    {
        public required OPayrollSession S;
        public required HashSet<string> CoaSet;
        public required Dictionary<string, OPayCoa> Coa;
        public required HashSet<string> SssAccts, PhicAccts, PagibigAccts, TaxAccts;
        public required string Prd6;
        public required HashSet<string> EmpNumbers;
        public required Dictionary<string, OPayEmp> Emps;
    }

    static void Set(Ctx c, OPayEmpResult r, string acct, double v)
    {
        if (acct == "" || !c.CoaSet.Contains(acct)) return;
        r.Amounts[acct] = R4(v);
    }

    static void Add(Ctx c, OPayEmpResult r, string acct, double v)
    {
        if (acct == "" || !c.CoaSet.Contains(acct)) return;
        r.Amounts[acct] = R4((r.Amounts.TryGetValue(acct, out var o) ? o : 0) + v);
    }

    static double Amt(OPayEmpResult r, string acct) => r.Amounts.TryGetValue(acct, out var v) ? v : 0;
    static double Sum(OPayEmpResult r, IEnumerable<string> accts) => accts.Sum(a => Amt(r, a));

    static OPayDtl? DtlExact(OPayEmpResult r, string acct) => r.Dtls.FirstOrDefault(d => (d.DtlCd ?? "").Trim() == acct);
    static OPayDtl? DtlLeft4(OPayEmpResult r, string acct) => r.Dtls.FirstOrDefault(d => Left4(d.DtlCd) == acct);

    //--- UPDATE tmpdatadtl SET nval = v, dtlCd = LEFT(ALLTRIM(dtlCd),4)+suffix WHERE ALLTRIM(dtlCd) == acct ---
    static void DtlRename(OPayEmpResult r, string acct, double? nval, string suffix)
    {
        if (acct == "") return;
        foreach (var d in r.Dtls.Where(d => (d.DtlCd ?? "").Trim() == acct))
        {
            if (nval.HasValue) d.NVal = nval.Value;
            d.DtlCd = Left4(d.DtlCd) + suffix;
        }
    }

    public static void Compute(OPayrollSession s)
    {
        var coa = s.Coas.Where(c => c.AcctNumber != null).GroupBy(c => c.AcctNumber!.Trim()).ToDictionary(g => g.Key, g => g.First());
        HashSet<string> Accts(Func<OPayCoa, bool> f) => s.Coas.Where(c => (c.AcctType ?? "").Trim() == "E" && f(c)).Select(c => c.AcctNumber!.Trim()).ToHashSet();
        var c = new Ctx
        {
            S = s, Coa = coa, CoaSet = coa.Keys.ToHashSet(),
            SssAccts = Accts(x => x.WithSss == 1), PhicAccts = Accts(x => x.WithPhic == 1), PagibigAccts = Accts(x => x.WithPagibig == 1),
            TaxAccts = Accts(x => (x.IsTaxable ?? "").Trim() == "1"),
            Prd6 = (s.Trn ?? "").Length >= 6 ? s.Trn![..6] : "",
            EmpNumbers = s.Results.Select(r => r.EmpNumber!).ToHashSet(),
            Emps = s.Employees.GroupBy(e => e.EmpNumber!).ToDictionary(g => g.Key, g => g.First())
        };

        //--- create_tmp_data_storage + zero_tmpdata_amt : every employee starts with zero amounts and the E-account detail rows ---
        var eAccts = s.Coas.Where(a => (a.AcctNumber ?? "").Trim().StartsWith("E")).Select(a => a.AcctNumber!.Trim()).OrderBy(a => a, StringComparer.Ordinal).ToList();
        s.PostedFixed = []; s.PostedOther = []; s.PostedRetro = []; s.PostedHosp = []; s.PostedHospSavings = [];
        foreach (var r in s.Results)
        {
            r.Amounts.Clear(); r.Loans.Clear(); r.OtherDeds.Clear(); r.Sources.Clear(); r.MandTrans.Clear();
            r.Sss = r.SssEr = r.Phic = r.PhicEr = r.Pagibig = r.PagibigEr = 0;
            c.Emps.TryGetValue(r.EmpNumber!, out var e);
            r.Dtls = eAccts.Select(a => new OPayDtl
            {
                Acct = a, DtlCd = a, NVal = 0,
                Uom = a == OPayrollConstants.BasicAcct ? (e?.RateId == 3 ? "mo" : "day") : "hr"
            }).ToList();
            r.Dtls.Add(new OPayDtl { Acct = "TH",   DtlCd = "TH",   NVal = 0,                                                     Uom = "" });
            r.Dtls.Add(new OPayDtl { Acct = "RATE", DtlCd = "RATE", NVal = (e?.BasicRate ?? 0) == 0 ? s.Client.Rate : e!.BasicRate, Uom = "" });
        }

        PostFixedEarnings(c);
        PostOtherEarnings(c);
        PostRetroPay(c);

        foreach (var r in s.Results)
        {
            if (!c.Emps.TryGetValue(r.EmpNumber!, out var e)) continue;
            //--- No rate (null or zero) : the payroll group's rate is the employee's daily rate ---
            if (e.BasicRate == 0) { e = e.Clone(); e.BasicRate = s.Client.Rate; e.RateId = 2; }

            var basicSal = ComputeBasic(c, r, e, Acct(s, "daywrk"));
            ComputeAbsent(c, r, e);
            ComputeFixedEarningsGrp(c, r, e);
            ComputeOrdinaryDay(c, r, e, Acct(s, "rn"));
            foreach (var col in OtOrder) ComputeOt(c, r, e, Acct(s, col), col);

            ComputeAllowance(c, r, e, Acct(s, "allowance"));
            ComputeMealAllowance(c, r);

            var ecola = ComputeEcola(c, r, e);
            var ctpa  = ComputeCtpa(c, r, e);
            var sea   = ComputeSea(c, r, e);
            if (s.Client.WithSil == 1) ComputeIncentiveLeave(c, r, basicSal, ecola, ctpa, sea);
            ComputeProvisionForRetirement(c, r, e);
            ComputeFreeUniform(c, r);

            ComputeSss(c, r, e);
            ComputePagibig(c, r, e);
            ComputePhic(c, r, e);
            ComputeTax(c, r, e);
        }

        GrpDeduction(c);
        CalcLoans(c);
        Hospitalization(c);
        EmpDedExemption(c);
        DedMandatory(c);
        Process13thMonthAdded(c);

        foreach (var r in s.Results) r.NetStatus = NetStatus(r.Net);
    }

    //--- check_negative_pay : BETWEEN(net,0,499) is a warning, below 0 blocks ---
    public static NetPayStatus NetStatus(double net)
        => net < OPayrollConstants.LockWarnNetFrom ? NetPayStatus.Negative
         : Between(net, OPayrollConstants.LockWarnNetFrom, OPayrollConstants.LockWarnNetTo) ? NetPayStatus.BelowMinimum
         : NetPayStatus.Ok;

    //========================================================================================================================
    // Before the employee loop
    //========================================================================================================================

    //--- post_fixedEarnings ---
    static void PostFixedEarnings(Ctx c)
    {
        foreach (var f in c.S.FixedEarnings)
        {
            var r = c.S.Results.FirstOrDefault(x => x.EmpNumber == (f.EmpNumber ?? "").Trim());
            if (r == null) continue;
            var acct = (f.AcctNumber ?? "").Trim();
            var days = Hrs(r, "daywrk");
            if (days == 0) days = f.DaysPara;
            if (f.DaysPara == 0) Add(c, r, acct, f.Amount);
            else Add(c, r, acct, f.Amount / f.DaysPara * days);
            c.S.PostedFixed.Add(f);
        }
    }

    //--- post_otherearnings ---
    static void PostOtherEarnings(Ctx c)
    {
        foreach (var o in c.S.OtherEarnings)
        {
            var r = c.S.Results.FirstOrDefault(x => x.EmpNumber == (o.EmpNumber ?? "").Trim());
            if (r == null) continue;
            Add(c, r, (o.AcctNumber ?? "").Trim(), o.Amount);
            c.S.PostedOther.Add(o);
        }
    }

    //--- post_retroPay ---
    static void PostRetroPay(Ctx c)
    {
        foreach (var o in c.S.Retros)
        {
            var r = c.S.Results.FirstOrDefault(x => x.EmpNumber == (o.EmpNumber ?? "").Trim());
            if (r == null) continue;
            Add(c, r, (o.AcctNumber ?? "").Trim(), o.Amt);
            c.S.PostedRetro.Add(o);
        }
    }

    //========================================================================================================================
    // Per employee
    //========================================================================================================================

    //--- compute_basic ---
    static double ComputeBasic(Ctx c, OPayEmpResult r, OPayEmp e, string acct)
    {
        var dayWrk = Hrs(r, "daywrk");
        var clientRate = c.S.Client.Rate;
        var empRate = e.BasicRate;
        var rateId = e.RateId;

        if (Left4(acct) == OPayrollConstants.BasicAcct)
        {
            foreach (var d in r.Dtls.Where(d => (d.DtlCd ?? "").Trim().StartsWith("TH") || Left4(d.DtlCd) == OPayrollConstants.BasicAcct)) d.NVal = 0;
            foreach (var d in r.Dtls.Where(d => Left4(d.DtlCd) == OPayrollConstants.BasicAcct)) d.NVal = dayWrk;
        }

        if (acct == "") return 0;

        double amt;
        if (dayWrk == 0)
        {
            amt = 0;
            if (rateId == 1) amt = empRate / 2;
            if (rateId == 3) amt = empRate;
            if (rateId == 3 || rateId == 1)
            {
                Set(c, r, acct, amt);
                DtlRename(r, acct, null, "_02");
                return amt;
            }
            return 0;
        }

        if (empRate == 0)
        {
            var msal = clientRate * dayWrk;
            Set(c, r, acct, msal);
            DtlRename(r, acct, dayWrk, "_02");
            foreach (var d in r.Dtls.Where(d => (d.DtlCd ?? "").Trim() == "TH")) d.NVal = dayWrk * OPayrollConstants.HoursPerDay;
            return msal;
        }

        amt = empRate * dayWrk;
        if (rateId == 1) amt = empRate / 2;
        if (rateId == 3) amt = empRate;
        Set(c, r, acct, amt);
        DtlRename(r, acct, dayWrk, "_02");
        foreach (var d in r.Dtls.Where(d => (d.DtlCd ?? "").Trim() == "TH")) d.NVal = dayWrk * OPayrollConstants.HoursPerDay;
        return amt;
    }

    //--- compute_absent ---
    static void ComputeAbsent(Ctx c, OPayEmpResult r, OPayEmp e)
    {
        var rate = e.SgDailyRate / OPayrollConstants.HoursPerDay;
        var hrWrk = Hrs(r, "absent");
        var acct = Acct(c.S, "absent");
        if (rate == 0) rate = c.S.Client.Rate / OPayrollConstants.HoursPerDay;
        if (rate == 0 || hrWrk == 0 || acct == "") return;

        Set(c, r, acct, hrWrk * rate * -1);
        DtlRename(r, acct, hrWrk, "_01");
    }

    //--- compute_fixed_earnings_grp : rows of the employee's designation ---
    static void ComputeFixedEarningsGrp(Ctx c, OPayEmpResult r, OPayEmp e)
    {
        if (c.S.FixedEarningsGrp.Count == 0) return;
        var dayWrk = Hrs(r, "daywrk");
        var desig = (e.Desig ?? "").Trim().ToUpperInvariant();
        foreach (var g in c.S.FixedEarningsGrp.Where(g => (g.Desig ?? "").Trim().ToUpperInvariant() == desig))
        {
            double amount;
            if (g.PerDayEarnings == 1) amount = dayWrk * g.Amount;
            else if (g.DaysPara != 0) amount = g.Amount / g.DaysPara * dayWrk;
            else amount = g.Amount;
            Add(c, r, (g.AcctNumber ?? "").Trim(), amount);
        }
    }

    //--- compute_ordinaryday : rn hours x salary-grade daily rate / 8 ---
    static double ComputeOrdinaryDay(Ctx c, OPayEmpResult r, OPayEmp e, string acct)
    {
        var rn = Hrs(r, "rn");
        var empRate = e.SgDailyRate;

        foreach (var d in r.Dtls.Where(d => acct != "" && Left4(d.DtlCd) == Left4(acct))) d.NVal = rn;
        if (acct == "") return 0;
        if (rn == 0) return 0;

        if (empRate == 0)
        {
            var msal = c.S.Client.Rate * rn / OPayrollConstants.HoursPerDay;
            Set(c, r, acct, msal);
            DtlRename(r, acct, rn, "_01");
            var d2 = DtlLeft4(r, acct);
            if (d2 != null) d2.NVal = rn / OPayrollConstants.HoursPerDay;
            return msal;
        }

        var amt = empRate * rn / OPayrollConstants.HoursPerDay;
        Set(c, r, acct, amt);
        DtlRename(r, acct, rn, "_01");
        var d3 = DtlLeft4(r, acct);
        if (d3 != null) d3.NVal += rn;
        return amt;
    }

    //--- compute_ot ---
    static void ComputeOt(Ctx c, OPayEmpResult r, OPayEmp e, string acct, string fld)
    {
        if (acct == "") return;
        c.Coa.TryGetValue(acct, out var x);
        var otRate = x?.OtRate ?? 0;

        var basicRate = e.BasicRate;
        if (basicRate == 0) basicRate = c.S.Client.Rate;
        else if (e.RateId != 2) basicRate = e.SgDailyRate;
        if (basicRate == 0) return;

        var hourly = basicRate / OPayrollConstants.HoursPerDay;
        var hr = Hrs(r, fld);
        var otAmt = hourly * otRate * hr;
        if (fld == "rot" && c.S.RatesAmt.TryGetValue("rot", out var fixedRot)) otAmt = fixedRot * hr;

        Set(c, r, acct, otAmt);
        DtlRename(r, acct, hr, "_01");
    }

    //--- compute_allowance : allowRate / 30 * daywrk ---
    static void ComputeAllowance(Ctx c, OPayEmpResult r, OPayEmp e, string acct)
    {
        if (acct == "") return;
        var dayWrk = Hrs(r, "daywrk");
        if (e.AllowRate == 0 || dayWrk == 0) return;
        Add(c, r, acct, e.AllowRate / OPayrollConstants.AllowanceDaysPerMonth * dayWrk);
        DtlRename(r, acct, dayWrk, "_02");
    }

    //--- compute_mealallowance ---
    static void ComputeMealAllowance(Ctx c, OPayEmpResult r)
    {
        var clientMeal = c.S.Client.MealAllow;
        var acct = Acct(c.S, "mealAllowance");
        if (clientMeal == 0)
        {
            Set(c, r, acct, 0);
            var d = r.Dtls.FirstOrDefault(d => acct != "" && Left4(d.DtlCd) == Left4(acct));
            if (d != null) d.NVal = 0;
            return;
        }

        var opt = c.S.MealOpts.FirstOrDefault(m => (m.EmpNumber ?? "").Trim() == r.EmpNumber);
        var indMeal = opt?.MealAllow ?? 0;
        if (opt == null) indMeal = clientMeal;
        if (indMeal == 0) return;

        var dayWrk = Hrs(r, "daywrk");
        var otWrk = Hrs(r, "rot") + Hrs(r, "rdot") + Hrs(r, "lhot") + Hrs(r, "shot") + Hrs(r, "rdlhot") + Hrs(r, "rdshot") + Hrs(r, "rnot");
        var amt = dayWrk * indMeal;
        if ((opt?.IsMgr ?? 0) == 0) amt += otWrk / 4 * (indMeal / 2);
        else amt += indMeal / 8 * otWrk;

        Add(c, r, acct, amt);
        DtlRename(r, acct, dayWrk, "_02");
    }

    //--- Below minimum rate : ecola, ctpa and sea are paid only to employees at or under the group's minimum rate ---
    static bool PaidEcolaCtpaSea(Ctx c, OPayEmp e)
    {
        var minRate = c.S.Client.MinRate;
        return !(e.BasicRate > 0 && e.BasicRate > minRate);
    }

    static double RegularAllowance(Ctx c, OPayEmpResult r, double rate, string acctReg, string acctRd, string acctLh, string acctRdLh, bool withRd)
    {
        double amount = 0;
        var dayWrk = Hrs(r, "daywrk");
        if (acctReg != "" && dayWrk > 0)
        {
            var v = dayWrk * rate;
            amount += v;
            Add(c, r, acctReg, v);
            DtlRename(r, acctReg, dayWrk, "_02");
        }
        if (withRd)
        {
            var rd = Hrs(r, "rd");
            if (acctRd != "" && rd > 0)
            {
                var v = rate / OPayrollConstants.HoursPerDay * rd;
                amount += v;
                Add(c, r, acctRd, v);
                DtlRename(r, acctRd, rd, "_02");
            }
        }
        var lh = Hrs(r, "lh");
        if (acctLh != "" && lh > 0)
        {
            var v = rate / OPayrollConstants.HoursPerDay * lh;
            amount += v;
            Add(c, r, acctLh, v);
            DtlRename(r, acctLh, lh, "_02");
        }
        var rdlh = Hrs(r, "rdlh");
        if (acctRdLh != "" && rdlh > 0)
        {
            var v = rate / OPayrollConstants.HoursPerDay * rdlh;
            amount += v;
            Add(c, r, acctRdLh, v);
            DtlRename(r, acctRdLh, rdlh, "_02");
        }
        return amount;
    }

    static string Cfg(OPayrollSession s, string key) => s.AcctConfig.TryGetValue(key, out var v) ? (v ?? "").Trim() : "";

    //--- compute_ecola : regular day, legal holiday and restday legal holiday (the restday part is commented out in payroll4) ---
    static double ComputeEcola(Ctx c, OPayEmpResult r, OPayEmp e)
    {
        if (c.S.Client.EcolaRevised != 1) return 0;
        var rate = c.S.Client.ColaRate;
        if (rate == 0) return 0;
        if (!PaidEcolaCtpaSea(c, e)) return 0;
        return RegularAllowance(c, r, rate, Cfg(c.S, "ecola"), "", Cfg(c.S, "ecola_lh"), Cfg(c.S, "ecola_rdlh"), false);
    }

    //--- compute_ctpa : regular day, restday, legal holiday, restday legal holiday ---
    static double ComputeCtpa(Ctx c, OPayEmpResult r, OPayEmp e)
    {
        if (c.S.Client.WithCtpa != 1) return 0;
        var rate = c.S.Client.CtpaRate;
        if (rate == 0) return 0;
        if (!PaidEcolaCtpaSea(c, e)) return 0;
        return RegularAllowance(c, r, rate, Cfg(c.S, "ctpa"), Cfg(c.S, "ctpa_rd"), Cfg(c.S, "ctpa_lh"), Cfg(c.S, "ctpa_rdlh"), true);
    }

    //--- compute_sea : regular day, restday, legal holiday, restday legal holiday ---
    static double ComputeSea(Ctx c, OPayEmpResult r, OPayEmp e)
    {
        if (c.S.Client.WithSea != 1) return 0;
        var rate = c.S.Client.SeaRate;
        if (rate == 0) return 0;
        if (!PaidEcolaCtpaSea(c, e)) return 0;
        return RegularAllowance(c, r, rate, Cfg(c.S, "sea"), Cfg(c.S, "sea_rd"), Cfg(c.S, "sea_lh"), Cfg(c.S, "sea_rdlh"), true);
    }

    //--- compute_incentive_leave : (basic + ecola + ctpa + sea) / daywrk x 5 / 365 x daywrk ---
    static void ComputeIncentiveLeave(Ctx c, OPayEmpResult r, double basicSal, double ecola, double ctpa, double sea)
    {
        var acct = Acct(c.S, "incentLeave");
        var dayWrk = Hrs(r, "daywrk");
        if (acct == "" || dayWrk == 0) return;

        var perDay = (basicSal + ecola + ctpa + sea) / dayWrk;
        var annual = perDay * OPayrollConstants.IncentiveLeaveDays;
        var il = annual / OPayrollConstants.IncentiveLeaveYearDays * dayWrk;

        Add(c, r, acct, il);
        if (il > 0) r.Sources.Add(new OPaySource { AcctNumber = acct, Amount = R4(il), Source = "Incentive Leave" });
        DtlRename(r, acct, dayWrk, "_02");
    }

    //--- compute_provision_for_retirement ---
    static void ComputeProvisionForRetirement(Ctx c, OPayEmpResult r, OPayEmp e)
    {
        var acct = Acct(c.S, "provForRetirement");
        var dayWrk = Hrs(r, "daywrk");
        var rate = e.SgDailyRate;
        if (rate == 0) rate = c.S.Client.Rate;
        if (acct == "" || dayWrk == 0) return;
        if (!c.S.RetirementOpts.Contains(r.EmpNumber!)) return;

        var amt = rate * OPayrollConstants.RetirementDays / OPayrollConstants.RetirementMonths * OPayrollConstants.RetirementMonths
                  / OPayrollConstants.RetirementYearDays * dayWrk;
        Add(c, r, acct, amt);
        DtlRename(r, acct, dayWrk, "_02");
    }

    //--- compute_freeuniform ---
    static void ComputeFreeUniform(Ctx c, OPayEmpResult r)
    {
        var acct = Acct(c.S, "freeUniform");
        var dayWrk = Hrs(r, "daywrk");
        if (acct == "" || dayWrk == 0) return;
        if (!c.S.UniformOpts.Contains(r.EmpNumber!)) return;

        Add(c, r, acct, OPayrollConstants.FreeUniformPerDay * dayWrk);
        DtlRename(r, acct, dayWrk, "_02");
    }

    //========================================================================================================================
    // Premiums and tax
    //========================================================================================================================

    static double SavedEe(Ctx c, string emp, IEnumerable<string> accts)
    {
        var set = accts as ISet<string> ?? accts.ToHashSet();
        return c.S.SavedMonth.Where(t => t.Stat != "tmp" && (t.EmpNumber ?? "").Trim() == emp && set.Contains((t.AcctNumber ?? "").Trim())).Sum(t => t.Amount);
    }

    static PremiumBracket? Bracket(IEnumerable<PremiumBracket> tbl, double x) => tbl.FirstOrDefault(b => Between(x, b.FStart, b.FEnd));

    //--- compute_d01sss ---
    static void ComputeSss(Ctx c, OPayEmpResult r, OPayEmp e)
    {
        if (e.IsWithSss != 1) return;
        var acct = Acct(c.S, "sss");
        if (acct == "") return;

        var emp = r.EmpNumber!;
        var spremee = SavedEe(c, emp, [acct]);
        var spremer = c.S.SavedEr.FirstOrDefault(x => x.EmpNumber == emp)?.SssEr ?? 0;
        double premEe, premEr;

        if (e.IsMaxSss == 1)
        {
            var top = c.S.SssTbl.OrderBy(b => b.FStart).LastOrDefault();
            premEe = (top?.Ee ?? 0) - spremee;
            premEr = (top?.Er ?? 0) - spremer;
            if (premEe < 1) premEe = 0;
            if (premEr < 1) premEr = 0;
        }
        else
        {
            var total = SavedEe(c, emp, c.SssAccts) + Sum(r, c.SssAccts);
            var b = Bracket(c.S.SssTbl.OrderBy(x => x.FStart), total);
            premEe = (b?.Ee ?? 0) - spremee;
            premEr = (b?.Er ?? 0) - spremer;
        }

        r.Sss = premEe; r.SssEr = premEr;
        Set(c, r, acct, premEe);
    }

    //--- compute_d03pagibig ---
    static void ComputePagibig(Ctx c, OPayEmpResult r, OPayEmp e)
    {
        var acct = Acct(c.S, "pagibig");
        if (e.IsWithPagibig != 1 || acct == "") return;

        var emp = r.EmpNumber!;
        var total = SavedEe(c, emp, c.PagibigAccts) + Sum(r, c.PagibigAccts);
        var b = Bracket(c.S.PagibigTbl.OrderBy(x => x.FStart), total);
        var spremee = SavedEe(c, emp, [acct]);
        var spremer = c.S.SavedEr.FirstOrDefault(x => x.EmpNumber == emp)?.PagibigEr ?? 0;

        var premEe = (b?.Ee ?? 0) - spremee;
        var premEr = (b?.Er ?? 0) - spremer;
        r.Pagibig = premEe; r.PagibigEr = premEr;
        Set(c, r, acct, premEe);
    }

    //--- compute_d04phic : fixed + (earnings x percent / 100) / 2 each for employee and employer ---
    static void ComputePhic(Ctx c, OPayEmpResult r, OPayEmp e)
    {
        var acct = Acct(c.S, "phic");
        if (e.IsWithPhic != 1 || acct == "") return;

        var emp = r.EmpNumber!;
        var total = SavedEe(c, emp, c.PhicAccts) + Sum(r, c.PhicAccts);
        if (total == 0) return;

        var b = Bracket(c.S.PhicTbl.OrderBy(x => x.FStart), total);
        var half = b == null ? 0 : total * b.Percent / 100 / 2;
        var mmEe = (b?.Ee ?? 0) + half;
        var mmEr = (b?.Er ?? 0) + half;

        var spremee = SavedEe(c, emp, [acct]);
        var spremer = c.S.SavedEr.FirstOrDefault(x => x.EmpNumber == emp)?.Phic ?? 0;

        var premEe = mmEe - spremee;
        var premEr = mmEr - spremer;
        r.Phic = premEe; r.PhicEr = premEr;
        Set(c, r, acct, premEe);
    }

    //--- compute_d00tax : taxable earnings less D002-D004, semimonthlytax bracket, no tax-code filter ---
    static void ComputeTax(Ctx c, OPayEmpResult r, OPayEmp e)
    {
        if (e.IsTaxable != 1) return;

        var taxable = Sum(r, c.TaxAccts) - Sum(r, OPayrollConstants.GovDeductionAccts);
        var b = c.S.TaxTbl.OrderBy(x => x.SAmt).FirstOrDefault(x => Between(taxable, x.SAmt, x.EAmt));
        if (b == null) return;

        var excess = taxable - b.SAmt;
        if (excess < 1) excess = 0;
        Set(c, r, OPayrollConstants.TaxAcct, b.Fix + excess * b.Percentage);
    }

    //========================================================================================================================
    // After the employee loop
    //========================================================================================================================

    //--- grp_deduction : other deductions of the period, unless the same employee/account is already posted in a locked TRN of the period ---
    static void GrpDeduction(Ctx c)
    {
        foreach (var d in c.S.OtherDeds)
        {
            var r = c.S.Results.FirstOrDefault(x => x.EmpNumber == (d.EmpNumber ?? "").Trim());
            if (r == null) continue;
            var acct = (d.AcctNumber ?? "").Trim();

            var posted = c.S.SavedMonth.Any(t => t.Stat != "tmp" && (t.EmpNumber ?? "").Trim() == r.EmpNumber &&
                                                 (t.AcctNumber ?? "").Trim() == acct && (t.Trn ?? "").StartsWith(c.Prd6));
            if (posted) continue;

            Add(c, r, acct, d.Amount);
            r.OtherDeds.Add(d);
        }
    }

    //--- calc_loans : the amortization, or the balance when that is smaller, unless the same amount is already saved for the period ---
    static void CalcLoans(Ctx c)
    {
        foreach (var l in c.S.Loans.OrderBy(x => x.Number))
        {
            var r = c.S.Results.FirstOrDefault(x => x.EmpNumber == (l.EmpNumber ?? "").Trim());
            if (r == null) continue;

            var deduction = l.Balance < l.Amort ? l.Balance : l.Amort;
            var acct = (l.DedNCode ?? "").Trim();

            var saved = c.S.SavedMonth.Any(t => t.Stat == "saved" && (t.Trn ?? "").StartsWith(c.Prd6) &&
                                                (t.AcctNumber ?? "").Trim() == acct && (t.EmpNumber ?? "").Trim() == r.EmpNumber &&
                                                Math.Abs(t.Amount - deduction) < 0.00005);
            if (saved) continue;

            Add(c, r, acct, deduction);
            r.Loans.Add(new OPayLoan { Number = l.Number, EmpNumber = l.EmpNumber, DedNCode = acct, Amort = l.Amort, Balance = l.Balance });
        }
    }

    //--- hospitalization : deductions of the period, then the savings amortization ---
    static void Hospitalization(Ctx c)
    {
        var hosp = c.S.HospDtls.Where(h => c.EmpNumbers.Contains((h.EmpNumber ?? "").Trim()))
                    .GroupBy(h => ((h.EmpNumber ?? "").Trim(), (h.AcctNumber ?? "").Trim()))
                    .Select(g => (Emp: g.Key.Item1, Acct: g.Key.Item2, Ded: g.Sum(x => x.Ded), Rows: g.ToList()));
        foreach (var h in hosp)
        {
            var already = c.S.SavedMonth.Any(t => (t.EmpNumber ?? "").Trim() == h.Emp && (t.AcctNumber ?? "").Trim() == h.Acct &&
                                                  (t.Trn ?? "").StartsWith(c.Prd6) && t.Trn != c.S.Trn);
            if (already) continue;
            var r = c.S.Results.First(x => x.EmpNumber == h.Emp);
            Add(c, r, h.Acct, h.Ded);
            c.S.PostedHosp.AddRange(h.Rows);
        }

        foreach (var sv in c.S.HospSavings)
        {
            var emp = (sv.EmpNumber ?? "").Trim();
            if (!c.EmpNumbers.Contains(emp)) continue;
            if (!(sv.Fund > sv.Saving)) continue;
            if (c.S.HospSavedThisPeriod.Contains(emp)) continue;
            var r = c.S.Results.First(x => x.EmpNumber == emp);
            Add(c, r, (sv.SavingsAcct ?? "").Trim(), sv.Amort);
            c.S.PostedHospSavings.Add(sv);
        }
    }

    //--- empdedexemption : exempted employee/account is set to zero after everything else was posted ---
    static void EmpDedExemption(Ctx c)
    {
        foreach (var x in c.S.DedExemptions)
        {
            var r = c.S.Results.FirstOrDefault(y => y.EmpNumber == (x.EmpNumber ?? "").Trim());
            if (r == null) continue;
            Set(c, r, (x.AcctNumber ?? "").Trim(), 0);
        }
    }

    //--- dedMandatory : fixed contribution per period, until the account's maximum is reached ---
    static void DedMandatory(Ctx c)
    {
        foreach (var m in c.S.DedMandatories)
        {
            var acct = (m.AcctNumber ?? "").Trim();
            if (c.S.MandExemptAccts.Contains(acct)) continue;

            foreach (var r in c.S.Results)
            {
                var emp = r.EmpNumber!;
                if (c.S.MandLocked.TryGetValue($"{emp}|{acct}", out var locked) && locked >= m.MaxAmt) continue;

                var posted = c.S.SavedMonth.Any(t => t.Stat != "tmp" && (t.AcctNumber ?? "").Trim() == acct &&
                                                     (t.EmpNumber ?? "").Trim() == emp && (t.Trn ?? "").StartsWith(c.Prd6));
                if (posted) continue;

                if (c.S.DedExemptions.Any(x => (x.EmpNumber ?? "").Trim() == emp && (x.AcctNumber ?? "").Trim() == acct)) continue;

                var sameTrn = c.S.SavedMonth.Any(t => t.Stat != "tmp" && t.Trn == c.S.Trn && (t.AcctNumber ?? "").Trim() == acct && (t.EmpNumber ?? "").Trim() == emp);
                if (sameTrn) continue;

                Set(c, r, acct, m.ContAmt);
                r.MandTrans[acct] = R4(m.ContAmt);
            }
        }
    }

    //--- process_13thMo_added : basic pay / 12 for the employees of 13thmogrp ---
    static void Process13thMonthAdded(Ctx c)
    {
        var acct = Acct(c.S, "_13thMo");
        if (c.S.Thirteenth.Count == 0 || acct == "") return;

        foreach (var emp in c.S.Thirteenth)
        {
            var r = c.S.Results.FirstOrDefault(x => x.EmpNumber == emp);
            if (r == null) continue;
            var basic = Amt(r, OPayrollConstants.BasicAcct);
            if (basic == 0) continue;
            Set(c, r, acct, basic / OPayrollConstants.ThirteenthMonthDivisor);
        }
    }

    //========================================================================================================================
    // Save step : finalize_manhour (bSave.Click)
    //========================================================================================================================

    //--- Hours typed in the grid replace the detail rows (paymaindtlsetup fields 3 onward, when > 0), then TH and RATE are rebuilt ---
    public static void FinalizeManhour(OPayrollSession s)
    {
        var coaTh = s.Coas.Where(a => a.IsTh == 1 && (a.AcctNumber ?? "").Trim() != OPayrollConstants.BasicAcct)
                          .Select(a => a.AcctNumber!.Trim()).ToHashSet();
        foreach (var r in s.Results)
        {
            foreach (var col in HourColumns.Concat(OtherHourColumns).Where(x => x != "daywrk"))
            {
                var acct = Acct(s, col);
                if (acct == "" || !acct.StartsWith("E")) continue;
                var v = Hrs(r, col);
                if (v <= 0) continue;
                foreach (var d in r.Dtls.Where(d => Left4(d.DtlCd) == acct)) d.NVal = v;
            }

            //--- UPDATE ... FROM xmanhr : the join only has rows when some account has isth = 1 ---
            if (coaTh.Count == 0) continue;
            var other = r.Dtls.Where(d => coaTh.Contains(Left4(d.DtlCd))).Sum(d => d.NVal);
            var e001  = r.Dtls.Where(d => Left4(d.DtlCd) == OPayrollConstants.BasicAcct).Sum(d => d.NVal) * OPayrollConstants.HoursPerDay;
            foreach (var d in r.Dtls.Where(d => (d.DtlCd ?? "").Trim() == "TH")) d.NVal = other + e001;
        }
    }
}
