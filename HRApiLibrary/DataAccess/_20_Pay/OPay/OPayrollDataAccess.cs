using HRApiLibrary.DataAccess._90_Utils;
using System.Data;
using Dapper;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay.M0605;
using HRApiLibrary.Models._20_Pay.OPay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

//--- Payroll on the MBPay (FoxPro) tables.
//---   Load : forms/payrollnew4.scx bLoad.Click  (paymain_save and every *_fetch)
//---   Save : forms/payroll4.scx    bSave.Click  (save_paymaindtl, finalize_manhour, save_01_manhour, save_01_tmpdata)
//---   Lock : forms/payroll4.scx    bLock.Click  (save_*, lock_01 .. lock_11)
//--- pay.tmptbltran and pay.loansref are MyISAM in MBPay and are not rolled back.
public class OPayrollDataAccess : IOPayrollDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OPayrollDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public static string MakeTrn(string yy4, string mm, string prd, string clNumber)
        => $"{yy4[^2..]}{mm.PadLeft(2, '0')}{prd.PadLeft(2, '0')}-{clNumber.Trim()}";

    //--- p1..p5 : which payroll period a setting applies to ---
    static string PrdFld(string? prd) => prd switch { "02" => "p2", "03" => "p3", "04" => "p4", "05" => "p5", _ => "p1" };

    //--- payrollgrp left join client : one client row per ClNumber, so a duplicate ClNumber in client cannot repeat a group ---
    static string ClientSql(string paydb, string pisdb) =>
        $@"select g.Id, trim(g.Code) Code, trim(g.ClNumber) ClNumber, trim(g.Name) ClName, ifnull(g.RatePerDay,0) Rate,
                  ifnull(g.MinDailyRate,0) MinRate, trim(ifnull(c.payprd,'')) PayPrd, ifnull(c.colarate,0) ColaRate,
                  ifnull(c.ecolaRevised,0) EcolaRevised, ifnull(c.ctpaRate,0) CtpaRate, ifnull(c.withCTPA,0) WithCtpa,
                  ifnull(c.seaRate,0) SeaRate, ifnull(c.withSEA,0) WithSea, ifnull(c.MealAllow,0) MealAllow,
                  ifnull(c.withSIL,0) WithSil, ifnull(c.billRate,0) BillRate
           from {paydb}.payrollgrp g
           left join (select trim(ClNumber) ClNumber, max(payprd) payprd, max(colarate) colarate, max(ecolaRevised) ecolaRevised,
                             max(ctpaRate) ctpaRate, max(withCTPA) withCTPA, max(seaRate) seaRate, max(withSEA) withSEA,
                             max(MealAllow) MealAllow, max(withSIL) withSIL, max(billRate) billRate
                      from {pisdb}.client group by trim(ClNumber)) c on c.ClNumber = trim(g.ClNumber)";

    //--- Payroll groups : {OldPay}.payrollgrp (filled from {OldPis}.client by the migration when empty) ------------------
    public async Task<List<OPayClient>> _02Clients(string paydb, string pisdb, string conn)
        => await _sql.FetchLegacy<OPayClient, dynamic>(ClientSql(paydb, pisdb) + " where g.Status = 'A' order by g.Name", new { }, conn);

    // -- Payroll periods (MBPay frmpayrollnew : period where pdNumber != '00') --
    public async Task<List<OPayPeriod>> _02Periods(string paydb, string conn)
        => await _sql.FetchLegacy<OPayPeriod, dynamic>(
               $"select trim(PdNumber) PdNumber, trim(PdName) PdName from {paydb}.period where PdNumber != '00' order by PdNumber",
               new { }, conn);

    //--- Employee master and rate fields the computation needs (payroll4.Load) --------------------------------------------
    private static string EmpSql(string paydb, string pisdb) =>
        $@"select e.client_ ClNumber, e.EmpNumber, concat(trim(e.EmpLastNm), ', ', trim(e.EmpFirstNm), ' ', trim(ifnull(e.EmpMidNm,''))) Name,
                  ifnull(e.AllowRate,0) AllowRate, ifnull(e.EmpBasicRate,0) BasicRate, ifnull(e.RateId,1) RateId, e.TaxCode,
                  ifnull(e.IsTaxable,0) IsTaxable, e.EmpStat_ StatCode, ifnull(e.IsMaxSss,0) IsMaxSss, ifnull(e.IsWithSss,1) IsWithSss,
                  ifnull(e.IsWithPhic,1) IsWithPhic, ifnull(e.IsWithPagibig,1) IsWithPagibig, e.SgCode, ifnull(g.DailyRate,0) SgDailyRate,
                  e.Desig_ Desig, (select st.name from {pisdb}.empstat st where st.code = e.EmpStat_ limit 1) StatName
           from {pisdb}.empmas e
           left join {paydb}.salgrade g on g.sgcode = e.sgcode ";

    static Dictionary<string, object?> Ci(IDictionary<string, object> row)
        => new(row.ToDictionary(k => k.Key, k => (object?)k.Value), StringComparer.OrdinalIgnoreCase);

    static double Dbl(object? v) => v == null || v is DBNull ? 0 : Convert.ToDouble(v);
    static string Str(object? v) => v == null || v is DBNull ? "" : (Convert.ToString(v) ?? "").Trim();

    //========================================================================================================================
    // Load
    //========================================================================================================================
    public async Task<OPayrollSession> _01Load(string yy4, string mm, string prd, string grpCode, DateTime attStart, DateTime attEnd,
                                               string paydb, string pisdb, string conn, IProgress<string>? progress = null)
    {
        var s = new OPayrollSession
        {
            Trn = MakeTrn(yy4, mm, prd, grpCode), Yy = yy4, Mm = mm.PadLeft(2, '0'), Prd = prd.PadLeft(2, '0'),
            AttStart = attStart, AttEnd = attEnd, Paydb = paydb, Pisdb = pisdb, Conn = conn
        };
        var p = new { Trn = s.Trn, Code = grpCode.Trim() };

        progress?.Report("Loading payroll group...");
        s.Client = (await _sql.FetchLegacy<OPayClient, dynamic>(ClientSql(paydb, pisdb) + " where g.Code = @Code limit 1", p, conn))
                   .FirstOrDefault() ?? new OPayClient { Code = grpCode.Trim() };

        //--- Status: locked, draft or new (setPayrollStatus) ---------------------------------------------------------------
        progress?.Report($"Checking payroll {s.Trn}...");
        var locked = await _sql.FetchLegacy<string, dynamic>($"select trn from {paydb}.paymainhdr where trn = @Trn", p, conn);
        //--- Dates as text: MBPay tables hold zero dates, which MySql.Data cannot convert without Allow Zero Datetime ------
        var draft  = await _sql.FetchLegacy<(string AttStart, string AttEnd), dynamic>(
                         $"select date_format(attStart,'%Y-%m-%d') AttStart, date_format(attEnd,'%Y-%m-%d') AttEnd from {paydb}.tmppaymainhdr where trn = @Trn", p, conn);
        s.Status = locked.Count > 0 ? "Lock" : draft.Count > 0 ? "Updated" : "New";
        if (s.Status == "Updated")
        {
            if (DateTime.TryParse(draft[0].AttStart, out var d1)) s.AttStart = d1;
            if (DateTime.TryParse(draft[0].AttEnd,   out var d2)) s.AttEnd   = d2;
        }

        //--- payroll4.Load : paymaindtlsetup, chartofacct, ssstbl, pagibigtbl, medtbl, semimonthlytax ----------------------
        progress?.Report("Loading payroll setup and chart of accounts...");
        var setup = (await _sql.FetchLegacy<dynamic, dynamic>($"select * from {paydb}.paymaindtlsetup limit 1", new { }, conn)).FirstOrDefault();
        if (setup is IDictionary<string, object> sd)
            foreach (var kv in sd) s.Setup[kv.Key] = Str(kv.Value);

        var acfg = (await _sql.FetchLegacy<dynamic, dynamic>($"select * from {paydb}.acctconfig limit 1", new { }, conn)).FirstOrDefault();
        if (acfg is IDictionary<string, object> ad)
            foreach (var kv in ad) s.AcctConfig[kv.Key] = Str(kv.Value);

        //--- create_tmp_data_storage reads chartofacct without a status filter ---
        s.Coas = await _sql.FetchLegacy<OPayCoa, dynamic>(
            $@"select trim(AcctNumber) AcctNumber, AcctName, trim(AcctType) AcctType, IsTaxable, ifnull(WithSss,0) WithSss, ifnull(WithPhic,0) WithPhic,
                      ifnull(WithPagibig,0) WithPagibig, IsLock, ifnull(DedSort,0) DedSort, ifnull(OtRate,0) OtRate, ifnull(IsTH,0) IsTh,
                      ifnull(show01,0) Show01, ifnull(show02,0) Show02, ShortDesc
               from {paydb}.chartofacct order by AcctNumber", new { }, conn);

        progress?.Report("Loading SSS, PhilHealth, Pag-IBIG and tax tables...");
        s.SssTbl     = await _sql.FetchLegacy<PremiumBracket, dynamic>($"select FStart, FEnd, FSSSEE Ee, FSSSER Er, FECC Ecc from {paydb}.ssstbl order by FStart", new { }, conn);
        s.PhicTbl    = await _sql.FetchLegacy<PremiumBracket, dynamic>($"select FStart, FEnd, FMEDEE Ee, FMEDER Er, ifnull(Percent,0) Percent from {paydb}.medtbl", new { }, conn);
        s.PagibigTbl = await _sql.FetchLegacy<PremiumBracket, dynamic>($"select FStart, FEnd, EmpCont Ee, Emplyr Er from {paydb}.pagibigtbl order by FStart", new { }, conn);
        s.TaxTbl     = await _sql.FetchLegacy<TaxBracket, dynamic>($"select SAmt, EAmt, Fix, Percentage from {paydb}.semimonthlytax", new { }, conn);

        //--- paymain_save : the header, and for a new payroll the employee list, are written when the payroll is loaded ------
        if (!s.IsLocked) await PaymainSave(s);

        //--- Employees: from the draft/locked TRN, otherwise the group's employees ({OldPis}.deprec.PayrollgrpId) in payroll -
        progress?.Report("Loading employees...");
        var dtlTbl  = s.IsLocked ? "paymaindtl" : "tmppaymaindtl";
        var dtlRows = (await _sql.FetchLegacy<dynamic, dynamic>($"select * from {paydb}.{dtlTbl} where trn = @Trn", p, conn))
                      .Select(d => Ci((IDictionary<string, object>)d)).ToList();

        var empSql = EmpSql(paydb, pisdb);
        if (dtlRows.Count > 0)
        {
            var nums = dtlRows.Select(d => Str(d["empnumber"])).ToList();
            s.Employees = await _sql.FetchLegacy<OPayEmp, dynamic>(empSql + " where e.EmpNumber in @Nums order by Name", new { Nums = nums }, conn);
        }
        else if (!s.IsLocked)
        {
            s.Employees = await _sql.FetchLegacy<OPayEmp, dynamic>(
                empSql + $@" join {pisdb}.deprec dp on dp.EmpNumber = e.EmpNumber
                             where dp.PayrollgrpId = @GrpId and e.EmpStat_ in (select code from {pisdb}.empstat where isInPayroll = 1) order by Name",
                new { GrpId = s.Client.Id }, conn);
        }

        var cols = OPayrollCompute.HourColumns.Concat(OPayrollCompute.OtherHourColumns).ToList();
        foreach (var e in s.Employees)
        {
            var r = new OPayEmpResult { EmpNumber = e.EmpNumber, Name = e.Name };
            var row = dtlRows.FirstOrDefault(d => Str(d["empnumber"]) == e.EmpNumber);
            foreach (var c in cols)
                r.Hours[c] = row != null && row.TryGetValue(c, out var v) ? Dbl(v) : 0;
            if (row != null)
            {
                r.PayStatus = Str(row.GetValueOrDefault("paystatus"));
                r.CoId = Str(row.GetValueOrDefault("coid")); r.BrId = Str(row.GetValueOrDefault("brid"));
                r.DepId = Str(row.GetValueOrDefault("depid")); r.SecId = Str(row.GetValueOrDefault("secid"));
            }
            s.Results.Add(r);
        }

        await _01LoadEmployeeData(s, progress);
        if (s.IsLocked) { progress?.Report("Loading locked amounts..."); await _01LoadLockedAmounts(s); }
        return s;
    }

    //--- payrollnew4.paymain_save ---
    async Task PaymainSave(OPayrollSession s)
    {
        var db = s.Paydb;
        var c = s.Client;
        using var cn = _sql.GetConnString(s.Conn);
        cn.Open();
        await cn.ExecuteAsync(LegacySqlMode.Set);
        try
        {
            var exists = (await cn.QueryAsync<string>($"select trn from {db}.tmppaymainhdr where trn = @Trn", new { s.Trn })).Any();
            var prm = new
            {
                s.Trn, ClRate = c.Rate, MinRate = c.MinRate, WithSea = c.WithSea, SeaRate = c.SeaRate, WithCtpa = c.WithCtpa, CtpaRate = c.CtpaRate,
                EcolaRevised = c.EcolaRevised, BillRate = c.BillRate, User = "", s.AttStart, s.AttEnd
            };
            if (!exists)
                await cn.ExecuteAsync(
                    $@"insert into {db}.tmppaymainhdr (trn, clrate, minrate, withSEA, SEARate, withCTPA, CTPARate, ecolaRevised, billrate, user, status, datecreated, attStart, attEnd)
                       values (@Trn, @ClRate, @MinRate, @WithSea, @SeaRate, @WithCtpa, @CtpaRate, @EcolaRevised, @BillRate, @User, 'New', now(), @AttStart, @AttEnd)", prm);
            else
                await cn.ExecuteAsync(
                    $@"update {db}.tmppaymainhdr set clrate = @ClRate, minrate = @MinRate, withSEA = @WithSea, SEARate = @SeaRate, withCTPA = @WithCtpa,
                              CTPARate = @CtpaRate, ecolaRevised = @EcolaRevised, billrate = @BillRate, datecreated = now(), status = 'Updated',
                              attStart = @AttStart, attEnd = @AttEnd where trn = @Trn", prm);

            //--- Details : only a payroll whose status is 'New' gets its employee list written ---
            if (!exists)
            {
                var nums = (await cn.QueryAsync<string>(
                    $@"select e.EmpNumber from {s.Pisdb}.empmas e join {s.Pisdb}.deprec dp on dp.EmpNumber = e.EmpNumber
                       where dp.PayrollgrpId = @GrpId and e.EmpStat_ in (select code from {s.Pisdb}.empstat where isInPayroll = 1)", new { GrpId = c.Id })).ToList();
                await cn.ExecuteAsync($"delete from {db}.tmppaymaindtl where trn = @Trn", new { s.Trn });
                if (nums.Count > 0)
                    await cn.ExecuteAsync($"insert into {db}.tmppaymaindtl (trn, empnumber) values (@Trn, @EmpNumber)", nums.Select(n => new { s.Trn, EmpNumber = n.Trim() }));
            }
        }
        finally { await cn.ExecuteAsync(LegacySqlMode.Restore); }
    }

    //--- Employees by number (13T/502 Upload Excel : employees added from the file) ---------------------------------------
    public async Task<List<OPayEmp>> _02Employees(OPayrollSession s, IEnumerable<string> empNumbers)
    {
        var nums = empNumbers.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).Distinct().ToList();
        if (nums.Count == 0) return [];
        return await _sql.FetchLegacy<OPayEmp, dynamic>(EmpSql(s.Paydb!, s.Pisdb!) + " where e.EmpNumber in @Nums order by Name", new { Nums = nums }, s.Conn);
    }

    //--- PayrollAddEmp4.Init : empmas not in xcurpaylst, narrowed by the Find box (name starts with the text) ---
    public async Task<List<OPayEmp>> _02AddCandidates(OPayrollSession s, string? find)
    {
        var have = s.Results.Select(r => r.EmpNumber!).ToList();
        var like = (find ?? "").Trim() + "%";
        return await _sql.FetchLegacy<OPayEmp, dynamic>(
            EmpSql(s.Paydb!, s.Pisdb!) +
            $@" where e.EmpStat_ in (select code from {s.Pisdb}.empstat where isInPayroll = 1) and e.EmpNumber not in @Have
                  and (@Find = '' or concat(trim(e.EmpLastNm), ', ', trim(e.EmpFirstNm)) like @Like or e.EmpNumber like @Like)
                order by Name limit 300", new { Have = have, Find = (find ?? "").Trim(), Like = like }, s.Conn);
    }

    //--- Everything payrollnew4 fetches for the employees in the session ----------------------------------------------------
    public async Task _01LoadEmployeeData(OPayrollSession s, IProgress<string>? progress = null)
    {
        var nums = s.Results.Select(r => r.EmpNumber!).ToList();
        s.SavedMonth = []; s.SavedEr = []; s.Loans = []; s.OtherDeds = []; s.FixedEarnings = []; s.FixedEarningsGrp = []; s.OtherEarnings = [];
        s.Retros = []; s.MealOpts = []; s.RetirementOpts = []; s.UniformOpts = []; s.DedExemptions = []; s.MandExemptAccts = [];
        s.DedMandatories = []; s.MandLocked = []; s.HospDtls = []; s.HospSavings = []; s.HospSavedThisPeriod = []; s.Thirteenth = []; s.RatesAmt = new(StringComparer.OrdinalIgnoreCase);

        var db  = s.Paydb;
        var cn  = s.Conn;
        var fld = PrdFld(s.Prd);
        var cl  = s.Client.ClNumber ?? "";
        var p = new { s.Trn, Nums = nums, s.AttStart, s.AttEnd, Year = int.Parse(s.Yy!), Month = int.Parse(s.Mm!), s.Prd, ClNumber = cl, Prd6 = (s.Trn ?? "")[..6] };

        //--- Rates that do not depend on the employee list ---
        var rates = (await _sql.FetchLegacy<dynamic, dynamic>($"select * from {db}.ratesamt where clnumber = @ClNumber limit 1", p, cn)).FirstOrDefault();
        if (rates is IDictionary<string, object> rd)
            foreach (var kv in rd) if (!string.Equals(kv.Key, "clnumber", StringComparison.OrdinalIgnoreCase)) s.RatesAmt[kv.Key] = Dbl(kv.Value);

        s.DedExemptions = await _sql.FetchLegacy<OPayEmpAcct, dynamic>(
            $"select trim(empnumber) EmpNumber, trim(acctnumber) AcctNumber from {db}.empdedexemption where clnumber = @ClNumber", p, cn);
        s.MandExemptAccts = (await _sql.FetchLegacy<string, dynamic>(
            $@"select trim(d.acctNumber) from {db}.dedmandatoryexemption e left join {db}.dedmandatory d on d.idDedMandatory = e.idDedMandatory
               where e.clNumber = @ClNumber and d.acctNumber is not null", p, cn)).ToHashSet();
        s.DedMandatories = await _sql.FetchLegacy<OPayDedMandatory, dynamic>(
            $"select idDedMandatory Id, trim(acctNumber) AcctNumber, ifnull(ContAmt,0) ContAmt, ifnull(MaxAmt,0) MaxAmt from {db}.dedmandatory where status = 'A' and {fld} = 1", p, cn);

        if (nums.Count == 0) return;

        progress?.Report($"Loading month-to-date amounts ({nums.Count} employees)...");
        s.SavedMonth = await _sql.FetchLegacy<OPayTran, dynamic>(
            $@"select Trn, trim(AcctNumber) AcctNumber, EmpNumber, Amount, 'saved' Stat from {db}.tbltran where left(trn,4) = left(@Trn,4) and EmpNumber in @Nums
               union all
               select Trn, trim(AcctNumber) AcctNumber, EmpNumber, Amount, 'tmp' Stat from {db}.tmptbltran where left(trn,4) = left(@Trn,4) and EmpNumber in @Nums", p, cn);

        //--- prevPayMainDtl : locked paymaindtl of the same period, other TRNs ---
        progress?.Report("Loading employer shares...");
        s.SavedEr = await _sql.FetchLegacy<OPayEr, dynamic>(
            $@"select EmpNumber, sum(ifnull(SSSer,0)) SssEr, sum(ifnull(phicer,0)) PhicEr, sum(ifnull(pagibiger,0)) PagibigEr, sum(ifnull(phic,0)) Phic
               from {db}.paymaindtl where left(trn,6) = left(@Trn,6) and trn != @Trn and EmpNumber in @Nums group by EmpNumber", p, cn);

        //--- payrollnew4.loans_fetch ---
        progress?.Report("Loading loans...");
        s.Loans = await _sql.FetchLegacy<OPayLoan, dynamic>(
            $@"select Number, EmpNumber, trim(DedNCode) DedNCode, ifnull(Amort,0) Amort, ifnull(Balance,0) Balance from {db}.loans
               where left(trnLastPosted,6) != left(@Trn,6) and Status = 'A' and Balance > 0 and {fld} = 1
                 and payRes <= @AttEnd and payStart <= @AttStart and EmpNumber in @Nums", p, cn);

        //--- payrollnew4.fetcth_tblotherdeductions : exempted employee/account removed ---
        progress?.Report("Loading other deductions...");
        var ods = await _sql.FetchLegacy<OPayOtherDed, dynamic>(
            $@"select Year, Month, PeriodId, EmpNumber, trim(AcctNumber) AcctNumber, Amount, Remarks from {db}.tblotherdeductions
               where ((processed = 0 and year = @Year and Month = @Month and (PeriodID = @Prd or {fld} = 1) and length(trnPosted) = 0)
                  or (contproc = 1 and {fld} = 1 and left(trnPosted,6) != left(@Trn,6))) and EmpNumber in @Nums", p, cn);
        s.OtherDeds = ods.Where(d => !s.DedExemptions.Any(x => x.EmpNumber == (d.EmpNumber ?? "").Trim() && x.AcctNumber == (d.AcctNumber ?? "").Trim())).ToList();

        //--- payrollnew4.fixedearnings_fetch / fixedearningsgrp_fetch / tblotherearnings_fetch / retropay_fetch ---
        progress?.Report("Loading fixed earnings, other earnings and retro pay...");
        s.FixedEarnings = await _sql.FetchLegacy<OPayFixedEarning, dynamic>(
            $@"select idfixed_earnings Id, trim(empnumber) EmpNumber, trim(acctNumber) AcctNumber, ifnull(amount,0) Amount, ifnull(dayspara,0) DaysPara
               from {db}.fixed_earnings
               where dStart <= @AttEnd and (left(trnPosted,6) != left(@Trn,6) or dayspara > 0) and {fld} = 1 and dEnd = '0000-00-00' and empnumber in @Nums", p, cn);
        s.FixedEarningsGrp = await _sql.FetchLegacy<OPayFixedEarningGrp, dynamic>(
            $@"select id Id, trim(acctNumber) AcctNumber, ifnull(amount,0) Amount, ifnull(dayspara,0) DaysPara, ifnull(perdayearnings,0) PerDayEarnings, desig_ Desig
               from {db}.fixed_earnings_grp where dStart <= @AttEnd and clnumber = @ClNumber and {fld} = 1 and dstart <= @AttStart", p, cn);
        s.OtherEarnings = await _sql.FetchLegacy<OPayOtherEarning, dynamic>(
            $@"select CONTROL_NO ControlNo, trim(empNumber) EmpNumber, trim(acctNumber) AcctNumber, ifnull(amount,0) Amount from {db}.tblotherearnings
               where left(trnposted,6) != left(@Trn,6) and processed = 0 and year = @Year and month = @Month and {fld} = 1 and amount != 0 and empNumber in @Nums", p, cn);
        s.Retros = await _sql.FetchLegacy<OPayRetro, dynamic>(
            $@"select idretropay Id, trim(empnumber) EmpNumber, trim(acctNumber) AcctNumber, ifnull(amt,0) Amt from {db}.retropay
               where yr = @Year and mo = @Month and prd = @Prd and isposted = 0 and empnumber in @Nums", p, cn);

        //--- payrollnew4 mealallowance_fetch, retirementopt_fetch, uniformopt_fetch ---
        progress?.Report("Loading meal allowance, retirement and uniform settings...");
        s.MealOpts = await _sql.FetchLegacy<OPayMealOpt, dynamic>(
            $"select trim(empnumber) EmpNumber, ifnull(MealAllow,0) MealAllow, ifnull(isMgr,0) IsMgr from {db}.mealallowopt where empnumber in @Nums", p, cn);
        s.RetirementOpts = (await _sql.FetchLegacy<string, dynamic>(
            $"select trim(empnumber) from {db}.retirementopt where withretirement = 1 and empnumber in @Nums", p, cn)).ToHashSet();
        s.UniformOpts = (await _sql.FetchLegacy<string, dynamic>(
            $"select trim(empnumber) from {db}.uniformopt where empnumber in @Nums", p, cn)).ToHashSet();

        //--- dedmandatory_trans (locked), 13thmogrp ---
        var mt = await _sql.FetchLegacy<(string EmpNumber, string AcctNumber, double Amount), dynamic>(
            $@"select trim(empnumber) EmpNumber, trim(acctnumber) AcctNumber, sum(amount) Amount from {db}.dedmandatory_trans
               where islock = 1 and empnumber in @Nums group by empnumber, acctnumber", p, cn);
        foreach (var m in mt) s.MandLocked[$"{m.EmpNumber}|{m.AcctNumber}"] = m.Amount;
        s.Thirteenth = (await _sql.FetchLegacy<string, dynamic>($"select trim(empnumber) from {db}.`13thmogrp` where trn = @Trn", p, cn)).ToHashSet();

        //--- hospitalization : deductions of the period, then the savings (payroll4.hospitalization) ---
        progress?.Report("Loading hospitalization...");
        s.HospDtls = await _sql.FetchLegacy<OPayHospDtl, dynamic>(
            $@"select trim(empnumber) EmpNumber, trim(acctnumber) AcctNumber, ifnull(ded,0) Ded from {db}.hospitalizationdtl
               where concat(right(year,2),mo,prd) = @Prd6 and length(trnposted) < 1 and empnumber in @Nums", p, cn);

        var grp  = await _sql.FetchLegacy<OPayEmpAcct, dynamic>($"select trim(empnumber) EmpNumber, trim(acctnumber) AcctNumber from {db}.hospitalizationgrp", p, cn);
        var accts = await _sql.FetchLegacy<(string AcctNumber, string SavingsAcct, double Fund, double Amort, int IsDefault), dynamic>(
            $"select trim(acctnumber) AcctNumber, trim(savingsacct) SavingsAcct, ifnull(fund,0) Fund, ifnull(amort,0) Amort, ifnull(isdefault,0) IsDefault from {db}.hospitalizationacct", p, cn);
        var sav  = await _sql.FetchLegacy<(string EmpNumber, double Saving), dynamic>($"select trim(empnumber) EmpNumber, ifnull(saving,0) Saving from {db}.hospitalizationsavings", p, cn);
        var def  = accts.FirstOrDefault(a => a.IsDefault == 1);
        foreach (var emp in nums)
        {
            var g = grp.FirstOrDefault(x => x.EmpNumber == emp);
            var a = g != null ? accts.FirstOrDefault(x => x.AcctNumber == g.AcctNumber) : def;
            if (a.AcctNumber == null) continue;
            s.HospSavings.Add(new OPayHospSaving
            {
                EmpNumber = emp, SavingsAcct = a.SavingsAcct, Fund = a.Fund, Amort = a.Amort, Saving = sav.FirstOrDefault(x => x.EmpNumber == emp).Saving
            });
        }
        var prd6 = (s.Trn ?? "")[..6];
        foreach (var h in s.HospSavings)
            if (s.SavedMonth.Any(t => t.Stat == "saved" && t.EmpNumber?.Trim() == h.EmpNumber && (t.AcctNumber ?? "") == h.SavingsAcct && (t.Trn ?? "").StartsWith(prd6)))
                s.HospSavedThisPeriod.Add(h.EmpNumber!);
    }

    async Task _01LoadLockedAmounts(OPayrollSession s)
    {
        var rows = await _sql.FetchLegacy<OPayTran, dynamic>($"select Trn, trim(AcctNumber) AcctNumber, EmpNumber, Amount from {s.Paydb}.tbltran where trn = @Trn", new { s.Trn }, s.Conn);
        foreach (var r in s.Results)
            foreach (var t in rows.Where(t => t.EmpNumber == r.EmpNumber))
                r.Amounts[t.AcctNumber!] = t.Amount;
    }

    //========================================================================================================================
    // Add / remove employees (payroll4 bAddEmployee, bRemoveSelectedEmployee)
    //========================================================================================================================
    public async Task _05AddEmployees(OPayrollSession s, IEnumerable<string> empNumbers)
    {
        var nums = empNumbers.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).Distinct().ToList();
        if (nums.Count == 0 || s.IsLocked) return;
        using var cn = _sql.GetConnString(s.Conn);
        cn.Open();
        await cn.ExecuteAsync(LegacySqlMode.Set);
        try
        {
            var have = (await cn.QueryAsync<string>($"select trim(empnumber) from {s.Paydb}.tmppaymaindtl where trn = @Trn", new { s.Trn })).ToHashSet();
            var add  = nums.Where(n => !have.Contains(n)).Select(n => new { s.Trn, EmpNumber = n }).ToList();
            if (add.Count > 0) await cn.ExecuteAsync($"insert into {s.Paydb}.tmppaymaindtl (trn, empnumber) values (@Trn, @EmpNumber)", add);
        }
        finally { await cn.ExecuteAsync(LegacySqlMode.Restore); }
    }

    public async Task<string?> _05RemoveEmployees(OPayrollSession s, IEnumerable<string> empNumbers)
    {
        if (s.IsLocked) return "Unable to delete posted transaction";
        var nums = empNumbers.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).Distinct().ToList();
        if (nums.Count == 0) return null;
        using var cn = _sql.GetConnString(s.Conn);
        cn.Open();
        await cn.ExecuteAsync(LegacySqlMode.Set);
        try
        {
            foreach (var t in new[] { "tmppaymaindtl", "tmptbltran", "tmptbltrandtl" })
                await cn.ExecuteAsync($"delete from {s.Paydb}.{t} where trn = @Trn and empnumber in @Nums", new { s.Trn, Nums = nums });
            return null;
        }
        finally { await cn.ExecuteAsync(LegacySqlMode.Restore); }
    }

    //========================================================================================================================
    // Save (bProcess then bSave)
    //========================================================================================================================
    public async Task<string?> _03SaveDraft(OPayrollSession s, string user)
    {
        if (s.IsLocked) return "Payroll already locked. Unable to save.";
        OPayrollCompute.Compute(s);
        OPayrollCompute.FinalizeManhour(s);
        using var cn = _sql.GetConnString(s.Conn);
        cn.Open();
        await cn.ExecuteAsync(LegacySqlMode.Set);      //--- MBPay tables : see LegacySqlMode
        try
        {
            using var tx = cn.BeginTransaction();
            try
            {
                if (await IsLocked(cn, tx, s)) { tx.Rollback(); return "Payroll already locked. Unable to save."; }
                await SaveDraft(cn, tx, s, user);
                tx.Commit();
                s.Status = "Updated";
                return null;
            }
            catch { tx.Rollback(); throw; }
        }
        finally { await cn.ExecuteAsync(LegacySqlMode.Restore); }
    }

    static async Task<bool> IsLocked(IDbConnection cn, IDbTransaction tx, OPayrollSession s)
        => (await cn.QueryAsync<string>($"select trn from {s.Paydb}.tbltran where trn = @Trn limit 1", new { s.Trn }, tx)).Any()
        || (await cn.QueryAsync<string>($"select trn from {s.Paydb}.paymainhdr where trn = @Trn limit 1", new { s.Trn }, tx)).Any();

    static double R4(double v) => OPayrollCompute.R4(v);

    //--- save_paymaindtl, save_01_manhour, save_01_tmpdata and the tmptbltransource rows written while processing ---
    static async Task SaveDraft(IDbConnection cn, IDbTransaction tx, OPayrollSession s, string user)
    {
        var db = s.Paydb;
        var p  = new { s.Trn };

        //--- save_paymaindtl ---------------------------------------------------------------------------------------------
        await cn.ExecuteAsync($"delete from {db}.tmppaymaindtl where trn = @Trn", p, tx);
        var cols = OPayrollCompute.HourColumns.Concat(OPayrollCompute.OtherHourColumns).ToList();
        var colList = string.Join(", ", cols);
        var valList = string.Join(", ", cols.Select((_, i) => $"@H{i}"));
        foreach (var r in s.Results)
        {
            var prm = new DynamicParameters(new
            {
                s.Trn, r.EmpNumber, Sss = R4(r.Sss), SssEr = R4(r.SssEr), Pagibig = R4(r.Pagibig), PagibigEr = R4(r.PagibigEr),
                Phic = R4(r.Phic), PhicEr = R4(r.PhicEr), PayStatus = string.IsNullOrEmpty(r.PayStatus) ? "0" : r.PayStatus,
                r.CoId, r.BrId, r.DepId, r.SecId
            });
            for (int i = 0; i < cols.Count; i++) prm.Add($"H{i}", R4(r.Hours.TryGetValue(cols[i], out var h) ? h : 0));
            await cn.ExecuteAsync(
                $@"insert into {db}.tmppaymaindtl (trn, empnumber, SSS, SSSer, pagibig, pagibiger, phic, phicer, paystatus, coid, brid, depid, secid, {colList})
                   values (@Trn, @EmpNumber, @Sss, @SssEr, @Pagibig, @PagibigEr, @Phic, @PhicEr, @PayStatus, @CoId, @BrId, @DepId, @SecId, {valList})", prm, tx);
        }

        //--- save_01_tmpdata : amounts > 0 only ----------------------------------------------------------------------------
        await cn.ExecuteAsync($"delete from {db}.tmptbltran where trn = @Trn", p, tx);
        var trans = s.Results.SelectMany(r => r.Amounts.Where(a => a.Value > 0)
                        .Select(a => new { s.Trn, AcctNumber = a.Key, r.EmpNumber, Amount = R4(a.Value), User = Trunc(user, 45) })).ToList();
        if (trans.Count > 0)
            await cn.ExecuteAsync($"insert into {db}.tmptbltran (trn, acctNumber, empNumber, amount, dTimeStamp, postedBy) values (@Trn, @AcctNumber, @EmpNumber, @Amount, now(), @User)", trans, tx);

        //--- save_01_manhour : detail rows with a value, detail code padded to 7 like the FoxPro field ---------------------
        await cn.ExecuteAsync($"delete from {db}.tmptbltrandtl where trn = @Trn", p, tx);
        var dtls = s.Results.SelectMany(r => r.Dtls.Where(d => d.NVal > 0)
                        .Select(d => new { s.Trn, r.EmpNumber, DtlCd = (d.DtlCd ?? "").PadRight(7), NVal = R4(d.NVal), Uom = d.Uom ?? "" })).ToList();
        if (dtls.Count > 0)
            await cn.ExecuteAsync($"insert into {db}.tmptbltrandtl (trn, empnumber, dtlcd, nval, uom) values (@Trn, @EmpNumber, @DtlCd, @NVal, @Uom)", dtls, tx);

        //--- delete_tbltransource, insert_tbltransource (Incentive Leave) ---
        await cn.ExecuteAsync($"delete from {db}.tmptbltransource where trn = @Trn", p, tx);
        var src = s.Results.SelectMany(r => r.Sources.Select(x => new { s.Trn, r.EmpNumber, x.AcctNumber, x.Amount, x.Source })).ToList();
        if (src.Count > 0)
            await cn.ExecuteAsync($"insert into {db}.tmptbltransource (trn, empnumber, acctnumber, amount, source) values (@Trn, @EmpNumber, @AcctNumber, @Amount, @Source)", src, tx);
    }

    //========================================================================================================================
    // Lock
    //========================================================================================================================

    //--- check_negative_pay, check_40_percent ---
    public OPayLockCheck _04LockCheck(OPayrollSession s)
    {
        OPayrollCompute.Compute(s);
        var chk = new OPayLockCheck { AlreadyLocked = s.IsLocked };
        foreach (var r in s.Results.OrderBy(x => x.Name))
        {
            if (r.NetStatus == NetPayStatus.Negative) chk.Negative.Add(r.Name ?? "");
            else if (r.NetStatus == NetPayStatus.BelowMinimum) chk.BelowMinimum.Add(r.Name ?? "");
            if (r.Earnings != 0 && r.Deductions / r.Earnings * 100 >= OPayrollConstants.MaxDeductionPercent) chk.Over40Percent.Add(r.Name ?? "");
        }
        return chk;
    }

    //--- Returns an error message, or null when locked. The confirmations (below 500, 40%) are asked by the caller. ---
    public async Task<string?> _04Lock(OPayrollSession s, string user)
    {
        if (s.IsLocked) return "Payroll already locked. Unable to process.";
        if (s.Results.Count == 0) return "No employees to lock.";
        OPayrollCompute.Compute(s);
        OPayrollCompute.FinalizeManhour(s);
        var negative = s.Results.Where(r => r.NetStatus == NetPayStatus.Negative).Select(r => r.Name).ToList();
        if (negative.Count > 0) return "Unable to lock transaction. Found record(s) with negative net pay: " + string.Join(", ", negative);

        var db = s.Paydb;
        var p  = new { s.Trn, User = Trunc(user, 25) };
        using var cn = _sql.GetConnString(s.Conn);
        cn.Open();
        await cn.ExecuteAsync(LegacySqlMode.Set);      //--- MBPay tables : see LegacySqlMode
        try
        {
            using var tx = cn.BeginTransaction();
            try
            {
                if (await IsLocked(cn, tx, s)) { tx.Rollback(); return "Payroll already locked. Unable to process."; }

                //--- the header is written when the payroll is loaded; make sure it is there ---
                if (!(await cn.QueryAsync<string>($"select trn from {db}.tmppaymainhdr where trn = @Trn", p, tx)).Any())
                    await cn.ExecuteAsync(
                        $@"insert into {db}.tmppaymainhdr (trn, clrate, minrate, withSEA, SEARate, withCTPA, CTPARate, ecolaRevised, billrate, user, status, datecreated, attStart, attEnd)
                           values (@Trn, @ClRate, @MinRate, @WithSea, @SeaRate, @WithCtpa, @CtpaRate, @EcolaRevised, @BillRate, @User, 'Updated', now(), @AttStart, @AttEnd)",
                        new { s.Trn, ClRate = s.Client.Rate, MinRate = s.Client.MinRate, WithSea = s.Client.WithSea, SeaRate = s.Client.SeaRate, WithCtpa = s.Client.WithCtpa,
                              CtpaRate = s.Client.CtpaRate, EcolaRevised = s.Client.EcolaRevised, BillRate = s.Client.BillRate, p.User, s.AttStart, s.AttEnd }, tx);

                //--- save_01_tmpdata, save_01_manhour, save_paymaindtl ---
                await SaveDraft(cn, tx, s, user);

                //--- save_02_loans_tmp : the loan rows deducted, with the full amortization ---
                await cn.ExecuteAsync($"delete from {db}.tmploans where trn = @Trn", p, tx);
                var loans = s.Results.SelectMany(r => r.Loans).Select(l => new { s.Trn, l.Number }).ToList();
                if (loans.Count > 0)
                    await cn.ExecuteAsync(
                        $@"insert into {db}.tmploans (NUMBER, EMPNUMBER, DATE, DEDNCODE, DEDNDESC, AMOUNT, AMORT, BALANCE, STATUS, ENCODEBY, ENCODEDT,
                                                      CHANGEBY, CHANGEDT, POSTED, EMPLASTNM, POSTFLAG, REMARKS, payMode, payStart, payRes, trn)
                           select NUMBER, EMPNUMBER, DATE, DEDNCODE, DEDNDESC, AMOUNT, AMORT, BALANCE, STATUS, ENCODEBY, ENCODEDT,
                                  CHANGEBY, CHANGEDT, POSTED, EMPLASTNM, POSTFLAG, REMARKS, payMode, payStart, payRes, @Trn
                           from {db}.loans where NUMBER = @Number", loans, tx);

                //--- save_03_otherdeductions_tmp ---
                await cn.ExecuteAsync($"delete from {db}.tmptblotherdeductions where trn = @Trn", p, tx);
                var oded = s.Results.SelectMany(r => r.OtherDeds)
                            .Select(d => new { d.Year, d.Month, d.PeriodId, d.EmpNumber, d.AcctNumber, d.Amount, Remarks = d.Remarks ?? "", s.Trn }).ToList();
                if (oded.Count > 0)
                    await cn.ExecuteAsync(
                        $@"insert into {db}.tmptblotherdeductions (year, month, periodID, empNumber, acctNumber, amount, Processed, Remarks, trn)
                           values (@Year, @Month, @PeriodId, @EmpNumber, @AcctNumber, @Amount, 0, @Remarks, @Trn)", oded, tx);

                //--- save_04_tmpdedmandatory_trans ---
                await cn.ExecuteAsync($"delete from {db}.tmpdedmandatory_trans where trn = @Trn", p, tx);
                var mand = s.Results.SelectMany(r => r.MandTrans.Select(m => new { s.Trn, r.EmpNumber, AcctNumber = m.Key, Amount = m.Value })).ToList();
                if (mand.Count > 0)
                    await cn.ExecuteAsync($"insert into {db}.tmpdedmandatory_trans (trn, empnumber, acctnumber, amount, islock) values (@Trn, @EmpNumber, @AcctNumber, @Amount, 1)", mand, tx);

                //--- lock_01_tbltran ---
                await cn.ExecuteAsync($@"insert into {db}.tbltran (trn, acctNumber, empNumber, amount, dTimeStamp, source)
                                         select trn, acctNumber, empNumber, amount, now(), 'main' from {db}.tmptbltran where trn = @Trn", p, tx);
                await cn.ExecuteAsync($@"insert into {db}.tbltrandtl (trn, empnumber, dtlCd, nval, uom)
                                         select trn, empnumber, dtlCd, nval, uom from {db}.tmptbltrandtl where trn = @Trn", p, tx);
                await cn.ExecuteAsync($"delete from {db}.tmptbltran where trn = @Trn", p, tx);
                await cn.ExecuteAsync($"delete from {db}.tmptbltrandtl where trn = @Trn", p, tx);

                //--- lock_02_loans ---
                await cn.ExecuteAsync($"delete from {db}.loansref where trn = @Trn", p, tx);
                await cn.ExecuteAsync($@"insert into {db}.loansref (number, EMPNUMBER, DATE, DEDNCODE, AMOUNT, AMORT, BALANCE, REMARKS, TRN, LoanAmt)
                                         select number, EMPNUMBER, DATE, DEDNCODE, AMOUNT, AMORT, BALANCE - AMORT, REMARKS, trn, AMOUNT
                                         from {db}.tmploans where trn = @Trn", p, tx);
                await cn.ExecuteAsync($@"update {db}.loans set balance = balance - amort, TRNLastPosted = @Trn
                                         where number in (select number from {db}.tmploans where trn = @Trn)", p, tx);
                await cn.ExecuteAsync($"update {db}.loans set status = 'I' where balance < 1", null, tx);
                await cn.ExecuteAsync($"delete from {db}.tmploans where trn = @Trn", p, tx);

                //--- lock_03_otherdeductions ---
                await cn.ExecuteAsync($@"insert into {db}.tblotherdedref (trn, year, month, period, empnumber, acctnumber, amount)
                                         select trn, year, month, periodid, empnumber, acctnumber, amount from {db}.tmptblotherdeductions where trn = @Trn", p, tx);
                await cn.ExecuteAsync($@"update {db}.tblotherdeductions d join {db}.tmptblotherdeductions t
                                           on t.trn = @Trn and t.year = d.year and t.month = d.month and t.periodID = d.periodID
                                          and t.empNumber = d.empNumber and t.acctNumber = d.acctNumber and t.amount = d.amount
                                         set d.processed = 1, d.TRNPosted = @Trn", p, tx);
                await cn.ExecuteAsync($"delete from {db}.tmptblotherdeductions where trn = @Trn", p, tx);

                //--- lock_04_tmpdedmandatory_trans ---
                await cn.ExecuteAsync($@"insert into {db}.dedmandatory_trans (trn, empnumber, acctnumber, amount, islock)
                                         select trn, empnumber, acctnumber, amount, 1 from {db}.tmpdedmandatory_trans where trn = @Trn", p, tx);
                await cn.ExecuteAsync($"delete from {db}.tmpdedmandatory_trans where trn = @Trn", p, tx);

                //--- lock_05_otherearnings, lock_06_retropay, lock_07_fixedearnings ---
                foreach (var o in s.PostedOther)
                    await cn.ExecuteAsync($"update {db}.tblOtherEarnings set processed = 1, trnposted = @Trn where control_no = @ControlNo", new { s.Trn, o.ControlNo }, tx);
                foreach (var o in s.PostedRetro)
                    await cn.ExecuteAsync($"update {db}.retropay set isposted = 1 where idretropay = @Id", new { o.Id }, tx);
                foreach (var f in s.PostedFixed)
                    await cn.ExecuteAsync($"update {db}.fixed_earnings set trnPosted = @Trn where idfixed_earnings = @Id", new { s.Trn, f.Id }, tx);

                //--- lock_09_paymainhdr, lock_10_paymaindtl ---
                await cn.ExecuteAsync($"insert into {db}.paymainhdr select * from {db}.tmppaymainhdr where trn = @Trn", p, tx);
                await cn.ExecuteAsync($"update {db}.paymainhdr set user = left(concat(trim(ifnull(user,'')),'/',@User),25), status = 'Lock', dateposted = now() where trn = @Trn", p, tx);
                await cn.ExecuteAsync($"delete from {db}.tmppaymainhdr where trn = @Trn", p, tx);
                await cn.ExecuteAsync($"insert into {db}.paymaindtl select * from {db}.tmppaymaindtl where trn = @Trn", p, tx);
                await cn.ExecuteAsync($"delete from {db}.tmppaymaindtl where trn = @Trn", p, tx);

                //--- lock_11_hospitalization ---
                var year = "20" + s.Trn![..2]; var mo = s.Trn![2..4]; var prd = s.Trn![4..6];
                foreach (var h in s.PostedHosp.Select(x => new { x.EmpNumber, x.AcctNumber }).Distinct())
                    await cn.ExecuteAsync($@"update {db}.hospitalizationdtl set trnposted = @Trn
                                             where acctnumber = @AcctNumber and empnumber = @EmpNumber and year = @Year and mo = @Mo and prd = @Prd",
                                          new { s.Trn, h.AcctNumber, h.EmpNumber, Year = year, Mo = mo, Prd = prd }, tx);
                foreach (var h in s.PostedHospSavings)
                    await cn.ExecuteAsync($@"insert into {db}.hospitalizationsavings (empnumber, saving) values (@EmpNumber, @Amount)
                                             on duplicate key update saving = @Amount", new { h.EmpNumber, Amount = h.Saving + h.Amort }, tx);

                tx.Commit();
                s.Status = "Lock";
            }
            catch { tx.Rollback(); throw; }

            //--- save_deployment : history only, a failure here does not undo the lock ---
            await SaveDeployment(cn, s, p.User);
            return null;
        }
        finally { await cn.ExecuteAsync(LegacySqlMode.Restore); }
    }

    //--- save_deployment : tbltrandeployment and PISVal_EmpDeploymentHist ---
    async Task SaveDeployment(IDbConnection cn, OPayrollSession s, string user)
    {
        var db = s.Paydb; var pis = s.Pisdb;
        var cl = (s.Client.ClNumber ?? "").Trim();
        try
        {
            await cn.ExecuteAsync(
                $@"insert into {db}.tbltrandeployment (trn, clnumber)
                   select concat(left(@Trn,4),'-',trim(t.empnumber)) trn, e.client_ clnumber from {db}.paymaindtl t
                   left join {pis}.empmas e on e.empnumber = t.empnumber
                   where ifnull(e.client,'-1') != '-1' and t.trn = @Trn
                   on duplicate key update clnumber = trim(e.client_)", new { s.Trn });
            await cn.ExecuteAsync(
                $@"insert into {db}.PISVal_EmpDeploymentHist
                   select @Trn, EmpNumber, client_, EmpStat_, @User, now() from {pis}.empmas where client_ = @ClNumber", new { s.Trn, User = user, ClNumber = cl });
        }
        catch { /* FoxPro ignores the SQL error here as well */ }
    }

    static string Trunc(string? v, int len) => (v ?? "").Length > len ? v![..len] : v ?? "";
}
