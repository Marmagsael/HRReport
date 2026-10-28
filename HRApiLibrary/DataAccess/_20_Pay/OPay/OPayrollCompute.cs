using HRApiLibrary.DataAccess._20_Pay.DA0605;
using HRApiLibrary.Models._20_Pay.M0605;
using HRApiLibrary.Models._20_Pay.OPay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

//--- Process step of MBPay forms/payroll4.scx (bProcess.Click), on an already loaded OPayrollSession ----------------------
//--- Done: basic, ordinary day, OT columns, absent, SSS, Pag-IBIG, PhilHealth, tax, group deductions, loans.
//--- Not yet: allowance, meal, ECOLA, CTPA, SEA, incentive leave, retirement, uniform, fixed/other earnings, retro pay,
//---          hospitalization, deduction exemptions, mandatory deductions, 13th month, ratesamt fixed ROT.
public static class OPayrollCompute
{
    //--- paymaindtl columns computed with compute_ot (hours x hourly rate x chartofacct.otrate) -------------------------
    public static readonly string[] OtColumns =
    [
        "rot", "rd", "rdot", "lh", "lhot", "rdlh", "rdlhot", "sh", "shot", "rdsh", "rdshot",
        "nd", "ndot", "ndlh", "ndlhot", "ndsh", "ndshot", "ndrd", "ndrdot", "ndrdlh", "ndrdlhot", "ndrdsh", "ndrdshot",
        "custom1", "custom2", "custom3", "dh", "dhot", "rddh", "rddhot"
    ];

    //--- Manhour columns shown for entry: days worked, ordinary hours, absent and the OT columns that have an account ---
    public static List<string> InputColumns(OPayrollSession s)
        => new[] { "daywrk", "rn", "absent" }.Concat(OtColumns).Where(c => Acct(s, c) != "").ToList();

    public static string Acct(OPayrollSession s, string column)
        => s.Setup.TryGetValue(column, out var a) ? (a ?? "").Trim() : "";

    static double Hrs(OPayEmpResult r, string column) => r.Hours.TryGetValue(column, out var v) ? v : 0;

    static void Add(OPayEmpResult r, string acct, double amt)
    {
        if (acct == "" || amt == 0) return;
        r.Amounts[acct] = PayrollCalculator.Round2((r.Amounts.TryGetValue(acct, out var v) ? v : 0) + amt);
    }

    public static PayCalcEmployee ToCalcEmployee(OPayEmp e, OPayClient c) => new()
    {
        EmpNumber            = e.EmpNumber,
        EmpName              = e.Name,
        RateBasis            = PayrollCalculator.RateBasisFromFoxProRateId(e.RateId),
        Rate                 = e.BasicRate,
        ClientRate           = c.Rate,
        SalaryGradeDailyRate = e.SgDailyRate > 0 ? e.SgDailyRate : c.Rate,     //--- loadnewpayroll: sgDailyRate = client rate when 0
        WithTax              = e.IsTaxable == 1,
        WithSss              = e.IsWithSss == 1,
        WithPhic             = e.IsWithPhic == 1,
        WithPagibig          = e.IsWithPagibig == 1,
        IsMaxSss             = e.IsMaxSss == 1
    };

    public static void Compute(OPayrollSession s, PayCalcOptions? o = null)
    {
        o ??= new PayCalcOptions();
        var coa = s.Coas.Where(c => c.AcctNumber != null).GroupBy(c => c.AcctNumber!.Trim()).ToDictionary(g => g.Key, g => g.First());
        HashSet<string> Accts(Func<OPayCoa, bool> f) => s.Coas.Where(c => (c.AcctType ?? "").Trim() == "E" && f(c)).Select(c => c.AcctNumber!.Trim()).ToHashSet();
        var sssAccts     = Accts(c => c.WithSss == 1);
        var phicAccts    = Accts(c => c.WithPhic == 1);
        var pagibigAccts = Accts(c => c.WithPagibig == 1);
        var taxAccts     = Accts(c => (c.IsTaxable ?? "").Trim() == "1");

        string sssAcct = Acct(s, "sss"), phicAcct = Acct(s, "phic"), pagibigAcct = Acct(s, "pagibig");

        foreach (var r in s.Results)
        {
            var e = s.Employees.FirstOrDefault(x => x.EmpNumber == r.EmpNumber);
            if (e == null) continue;
            var ce = ToCalcEmployee(e, s.Client);

            r.Amounts.Clear(); r.Dtls.Clear(); r.Loans.Clear(); r.OtherDeds.Clear();

            //--- post_fixedEarnings / post_otherearnings run before the employee loop in payroll4 ------------------------
            foreach (var x in r.ExtraEarnings) Add(r, x.Key, x.Value);

            //--- compute_basic --------------------------------------------------------------------------------------------
            var days = Hrs(r, "daywrk");
            Add(r, Acct(s, "daywrk"), PayrollCalculator.BasicPay(ce, days, o));
            r.Dtls.Add(new OPayDtl { DtlCd = Acct(s, "daywrk") + "_02", NVal = days, Uom = e.RateId == 3 ? "mo" : "day" });
            r.Dtls.Add(new OPayDtl { DtlCd = "TH",   NVal = days * o.HoursPerDay, Uom = "" });
            r.Dtls.Add(new OPayDtl { DtlCd = "RATE", NVal = e.BasicRate == 0 ? s.Client.Rate : e.BasicRate, Uom = "" });

            //--- compute_absent -------------------------------------------------------------------------------------------
            Add(r, Acct(s, "absent"), PayrollCalculator.Absent(ce, Hrs(r, "absent"), o));

            //--- compute_ordinaryday: rn hours x salary-grade daily rate / 8 ----------------------------------------------
            var rn = Hrs(r, "rn");
            Add(r, Acct(s, "rn"), PayrollCalculator.Round2(ce.SalaryGradeDailyRate * rn / o.HoursPerDay));

            //--- compute_ot -----------------------------------------------------------------------------------------------
            foreach (var col in OtColumns)
            {
                var acct = Acct(s, col);
                var hrs  = Hrs(r, col);
                if (acct == "" || hrs == 0) continue;
                var otRate = coa.TryGetValue(acct, out var a) ? a.OtRate : 0;
                Add(r, acct, PayrollCalculator.Overtime(ce, hrs, otRate, null, o));
                r.Dtls.Add(new OPayDtl { DtlCd = acct + "_01", NVal = hrs, Uom = "hr" });
            }

            //--- Premiums: month-to-date earnings, less what was deducted earlier this month ------------------------------
            double Cur(ICollection<string> accts)   => r.Amounts.Where(x => accts.Contains(x.Key)).Sum(x => x.Value);
            double Saved(ICollection<string> accts) => s.SavedMonth.Where(t => t.EmpNumber == r.EmpNumber && accts.Contains((t.AcctNumber ?? "").Trim())).Sum(t => t.Amount);
            var er = s.SavedEr.FirstOrDefault(x => x.EmpNumber == r.EmpNumber) ?? new OPayEr();

            var sss = PayrollCalculator.Sss(ce, Cur(sssAccts) + Saved(sssAccts), Saved([sssAcct]), er.SssEr, s.SssTbl);
            var pag = PayrollCalculator.Pagibig(ce, Cur(pagibigAccts) + Saved(pagibigAccts), Saved([pagibigAcct]), er.PagibigEr, s.PagibigTbl);
            var phc = PayrollCalculator.Phic(ce, Cur(phicAccts) + Saved(phicAccts), Saved([phicAcct]), er.PhicEr, s.PhicTbl);
            Add(r, sssAcct, sss.Ee);         r.SssEr     = sss.Er;
            Add(r, pagibigAcct, pag.Ee);     r.PagibigEr = pag.Er;
            Add(r, phicAcct, phc.Ee);        r.PhicEr    = phc.Er;

            //--- compute_d00tax: current-period taxable earnings less current premiums ------------------------------------
            Add(r, o.TaxAcct, PayrollCalculator.Tax(ce, Cur(taxAccts), sss.Ee, phc.Ee, pag.Ee, s.TaxTbl));
        }

        //--- grp_deduction: unprocessed other deductions for this period, unless already in a locked TRN of the period ---
        var prd6 = (s.Trn ?? "").Length >= 6 ? s.Trn![..6] : "";
        foreach (var d in s.OtherDeds)
        {
            var r = s.Results.FirstOrDefault(x => x.EmpNumber == d.EmpNumber);
            if (r == null) continue;
            var already = s.SavedMonth.Any(t => t.EmpNumber == d.EmpNumber && (t.AcctNumber ?? "").Trim() == (d.AcctNumber ?? "").Trim() &&
                                                (t.Trn ?? "").StartsWith(prd6));
            if (already) continue;
            Add(r, (d.AcctNumber ?? "").Trim(), d.Amount);
            r.OtherDeds.Add(d);
        }

        //--- calc_loans ---------------------------------------------------------------------------------------------------
        foreach (var r in s.Results)
        {
            var mine = s.Loans.Where(l => l.EmpNumber == r.EmpNumber)
                              .Select(l => new PayCalcLoan { Id = l.Number, DedNCode = (l.DedNCode ?? "").Trim(), Amort = l.Amort, Balance = l.Balance });
            foreach (var line in PayrollCalculator.Loans(mine, new HashSet<int>()))
            {
                Add(r, line.AcctNumber!, line.Amount);
                var src = s.Loans.First(l => l.Number == line.RefId);
                r.Loans.Add(new OPayLoan { Number = src.Number, EmpNumber = src.EmpNumber, DedNCode = line.AcctNumber, Amort = line.Amount, Balance = src.Balance });
            }
            r.NetStatus = PayrollCalculator.CheckNet(r.Net, o);
        }
    }
}
