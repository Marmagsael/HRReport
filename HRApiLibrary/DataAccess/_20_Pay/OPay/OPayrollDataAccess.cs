using System.Data;
using Dapper;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay.M0605;
using HRApiLibrary.Models._20_Pay.OPay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

//--- Payroll process on the MBPay (FoxPro) tables, following forms/payrollnew4.scx (load) and forms/payroll4.scx ---------
//---   Load  : everything for the TRN in one pass (payrollnew4 *_fetch, payroll4.Load)
//---   Save  : draft in tmppaymainhdr, tmppaymaindtl, tmptbltran, tmptbltrandtl, tmploans, tmptblotherdeductions
//---   Lock  : draft -> tbltran, tbltrandtl, loansref/loans, tblotherdedref, paymainhdr, paymaindtl (payroll4 bLock.Click)
//--- Save and Lock run in one transaction. pay.tmptbltran and pay.loansref are MyISAM in MBPay and are not rolled back.
public class OPayrollDataAccess : IOPayrollDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OPayrollDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public static string MakeTrn(string yy4, string mm, string prd, string clNumber)
        => $"{yy4[^2..]}{mm.PadLeft(2, '0')}{prd.PadLeft(2, '0')}-{clNumber.Trim()}";

    public async Task<List<OPayClient>> _02Clients(string pisdb, string conn)
        => await _sql.FetchData<OPayClient, dynamic>(
               $"select ClNumber, ClName, ifnull(Rate,0) Rate, ifnull(MinRate,0) MinRate from {pisdb}.client where Status = 'Active' order by ClName",
               new { }, conn);

    //--- Load ------------------------------------------------------------------------------------------------------------
    public async Task<OPayrollSession> _01Load(string yy4, string mm, string prd, string clNumber, DateTime attStart, DateTime attEnd,
                                               string paydb, string pisdb, string conn)
    {
        var s = new OPayrollSession
        {
            Trn = MakeTrn(yy4, mm, prd, clNumber), Yy = yy4, Mm = mm.PadLeft(2, '0'), Prd = prd.PadLeft(2, '0'),
            AttStart = attStart, AttEnd = attEnd, Paydb = paydb, Pisdb = pisdb, Conn = conn
        };
        var p = new { Trn = s.Trn, ClNumber = clNumber.Trim() };

        s.Client = (await _sql.FetchData<OPayClient, dynamic>(
                       $"select ClNumber, ClName, ifnull(Rate,0) Rate, ifnull(MinRate,0) MinRate from {pisdb}.client where ClNumber = @ClNumber", p, conn))
                   .FirstOrDefault() ?? new OPayClient { ClNumber = clNumber };

        //--- Status: locked, draft or new ---------------------------------------------------------------------------------
        var locked = await _sql.FetchData<string, dynamic>($"select trn from {paydb}.paymainhdr where trn = @Trn", p, conn);
        //--- Dates as text: MBPay tables hold zero dates, which MySql.Data cannot convert without Allow Zero Datetime ------
        var draft  = await _sql.FetchData<(string AttStart, string AttEnd), dynamic>(
                         $"select date_format(attStart,'%Y-%m-%d') AttStart, date_format(attEnd,'%Y-%m-%d') AttEnd from {paydb}.tmppaymainhdr where trn = @Trn", p, conn);
        s.Status = locked.Count > 0 ? "Lock" : draft.Count > 0 ? "Updated" : "New";
        if (s.Status == "Updated")
        {
            if (DateTime.TryParse(draft[0].AttStart, out var d1)) s.AttStart = d1;
            if (DateTime.TryParse(draft[0].AttEnd,   out var d2)) s.AttEnd   = d2;
        }

        //--- Setup and reference tables -----------------------------------------------------------------------------------
        var setup = (await _sql.FetchData<dynamic, dynamic>($"select * from {paydb}.paymaindtlsetup limit 1", new { }, conn)).FirstOrDefault();
        if (setup is IDictionary<string, object> sd)
            foreach (var kv in sd) s.Setup[kv.Key] = Convert.ToString(kv.Value)?.Trim() ?? "";

        s.Coas = await _sql.FetchData<OPayCoa, dynamic>(
            $@"select trim(AcctNumber) AcctNumber, AcctName, trim(AcctType) AcctType, IsTaxable, ifnull(WithSss,0) WithSss, ifnull(WithPhic,0) WithPhic,
                      ifnull(WithPagibig,0) WithPagibig, IsLock, ifnull(DedSort,0) DedSort, ifnull(OtRate,0) OtRate
               from {paydb}.chartofacct where status_ = 1 order by AcctNumber", new { }, conn);

        s.SssTbl     = await _sql.FetchData<PremiumBracket, dynamic>($"select FStart, FEnd, FSSSEE Ee, FSSSER Er, FECC Ecc from {paydb}.ssstbl order by FStart", new { }, conn);
        s.PhicTbl    = await _sql.FetchData<PremiumBracket, dynamic>($"select FStart, FEnd, FMEDEE Ee, FMEDER Er, ifnull(Percent,0) Percent from {paydb}.medtbl order by FStart", new { }, conn);
        s.PagibigTbl = await _sql.FetchData<PremiumBracket, dynamic>($"select FStart, FEnd, EmpCont Ee, Emplyr Er from {paydb}.pagibigtbl order by FStart", new { }, conn);
        s.TaxTbl     = await _sql.FetchData<TaxBracket, dynamic>($"select SAmt, EAmt, Fix, Percentage from {paydb}.semimonthlytax order by SAmt", new { }, conn);

        //--- Employees: from the draft/locked TRN, otherwise the client's employees whose status is in payroll ------------
        var dtlTbl  = s.IsLocked ? "paymaindtl" : "tmppaymaindtl";
        var dtlRows = (await _sql.FetchData<dynamic, dynamic>($"select * from {paydb}.{dtlTbl} where trn = @Trn", p, conn))
                      .Cast<IDictionary<string, object>>().ToList();

        var empSql = $@"select e.client_ ClNumber, e.EmpNumber, concat(trim(e.EmpLastNm), ', ', trim(e.EmpFirstNm), ' ', trim(ifnull(e.EmpMidNm,''))) Name,
                               ifnull(e.AllowRate,0) AllowRate, ifnull(e.EmpBasicRate,0) BasicRate, ifnull(e.RateId,2) RateId, e.TaxCode,
                               ifnull(e.IsTaxable,0) IsTaxable, e.EmpStat_ StatCode, ifnull(e.IsMaxSss,0) IsMaxSss, ifnull(e.IsWithSss,1) IsWithSss,
                               ifnull(e.IsWithPhic,1) IsWithPhic, ifnull(e.IsWithPagibig,1) IsWithPagibig, e.SgCode, ifnull(g.DailyRate,0) SgDailyRate
                        from {pisdb}.empmas e
                        left join {paydb}.salgrade g on g.sgcode = e.sgcode ";
        if (dtlRows.Count > 0)
        {
            var nums = dtlRows.Select(d => Convert.ToString(d["empnumber"])!.Trim()).ToList();
            s.Employees = await _sql.FetchData<OPayEmp, dynamic>(empSql + " where e.EmpNumber in @Nums order by Name", new { Nums = nums }, conn);
        }
        else if (!s.IsLocked)
        {
            s.Employees = await _sql.FetchData<OPayEmp, dynamic>(
                empSql + $" where e.client_ = @ClNumber and e.EmpStat_ in (select code from {pisdb}.empstat where isInPayroll = 1) order by Name", p, conn);
        }

        var cols = OPayrollCompute.InputColumns(s);
        foreach (var e in s.Employees)
        {
            var r = new OPayEmpResult { EmpNumber = e.EmpNumber, Name = e.Name };
            var row = dtlRows.FirstOrDefault(d => Convert.ToString(d["empnumber"])?.Trim() == e.EmpNumber);
            foreach (var c in cols)
                r.Hours[c] = row != null && row.TryGetValue(c, out var v) && v != null ? Convert.ToDouble(v) : 0;
            s.Results.Add(r);
        }

        await _01LoadEmployeeData(s);
        if (s.IsLocked) await _01LoadLockedAmounts(s);
        return s;
    }

    //--- Month-to-date (locked), ER shares, loans, other deductions for the employees in the session --------------------
    public async Task _01LoadEmployeeData(OPayrollSession s)
    {
        var nums = s.Results.Select(r => r.EmpNumber).ToList();
        s.SavedMonth = []; s.SavedEr = []; s.Loans = []; s.OtherDeds = [];
        if (nums.Count == 0) return;

        var prdFld = s.Prd switch { "02" => "p2", "03" => "p3", "04" => "p4", "05" => "p5", _ => "p1" };
        var p = new { s.Trn, Nums = nums, s.AttStart, s.AttEnd, Year = int.Parse(s.Yy!), Month = int.Parse(s.Mm!), s.Prd };

        s.SavedMonth = await _sql.FetchData<OPayTran, dynamic>(
            $@"select Trn, trim(AcctNumber) AcctNumber, EmpNumber, Amount from {s.Paydb}.tbltran
               where left(trn,4) = left(@Trn,4) and trn != @Trn and EmpNumber in @Nums", p, s.Conn);

        s.SavedEr = await _sql.FetchData<OPayEr, dynamic>(
            $@"select EmpNumber, sum(ifnull(SSSer,0)) SssEr, sum(ifnull(phicer,0)) PhicEr, sum(ifnull(pagibiger,0)) PagibigEr from {s.Paydb}.paymaindtl
               where left(trn,4) = left(@Trn,4) and trn != @Trn and EmpNumber in @Nums group by EmpNumber", p, s.Conn);

        //--- payrollnew4.loans_fetch ----------------------------------------------------------------------------------------
        s.Loans = await _sql.FetchData<OPayLoan, dynamic>(
            $@"select Number, EmpNumber, trim(DedNCode) DedNCode, ifnull(Amort,0) Amort, ifnull(Balance,0) Balance from {s.Paydb}.loans
               where left(ifnull(TrnLastPosted,''),6) != left(@Trn,6) and Status = 'A' and Balance > 0 and {prdFld} = 1
                 and payRes <= @AttEnd and payStart <= @AttStart and EmpNumber in @Nums", p, s.Conn);

        //--- payroll4.grp_deduction ------------------------------------------------------------------------------------------
        s.OtherDeds = await _sql.FetchData<OPayOtherDed, dynamic>(
            $@"select Year, Month, PeriodId, EmpNumber, trim(AcctNumber) AcctNumber, Amount, Remarks from {s.Paydb}.tblotherdeductions
               where Processed = 0 and Year = @Year and Month = @Month and PeriodId = @Prd and EmpNumber in @Nums", p, s.Conn);
    }

    async Task _01LoadLockedAmounts(OPayrollSession s)
    {
        var rows = await _sql.FetchData<OPayTran, dynamic>($"select Trn, trim(AcctNumber) AcctNumber, EmpNumber, Amount from {s.Paydb}.tbltran where trn = @Trn", new { s.Trn }, s.Conn);
        foreach (var r in s.Results)
            foreach (var t in rows.Where(t => t.EmpNumber == r.EmpNumber))
                r.Amounts[t.AcctNumber!] = t.Amount;
    }

    //--- Save draft --------------------------------------------------------------------------------------------------------
    public async Task<string?> _03SaveDraft(OPayrollSession s, string user)
    {
        if (s.IsLocked) return "Payroll already locked. Unable to save.";
        OPayrollCompute.Compute(s);         //--- amounts always follow the manhours being saved (payroll4: bProcess then bSave)
        using var cn = _sql.GetConnString(s.Conn);
        cn.Open();
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

    static async Task<bool> IsLocked(IDbConnection cn, IDbTransaction tx, OPayrollSession s)
        => (await cn.QueryAsync<string>($"select trn from {s.Paydb}.paymainhdr where trn = @Trn limit 1", new { s.Trn }, tx)).Any();

    static async Task SaveDraft(IDbConnection cn, IDbTransaction tx, OPayrollSession s, string user)
    {
        var db = s.Paydb;
        var p  = new { s.Trn };
        foreach (var t in new[] { "tmppaymainhdr", "tmppaymaindtl", "tmptbltran", "tmptbltrandtl", "tmploans", "tmptblotherdeductions" })
            await cn.ExecuteAsync($"delete from {db}.{t} where trn = @Trn", p, tx);

        await cn.ExecuteAsync(
            $@"insert into {db}.tmppaymainhdr (trn, clrate, minrate, user, status, datecreated, attStart, attEnd)
               values (@Trn, @ClRate, @MinRate, @User, 'Updated', now(), @AttStart, @AttEnd)",
            new { s.Trn, ClRate = s.Client.Rate, MinRate = s.Client.MinRate, User = Trunc(user, 25), s.AttStart, s.AttEnd }, tx);

        //--- Column names come from the fixed list in OPayrollCompute, not from user input ------------------------------
        var cols = OPayrollCompute.InputColumns(s);
        var colList = string.Join(", ", cols);
        var valList = string.Join(", ", cols.Select((_, i) => $"@H{i}"));
        foreach (var r in s.Results)
        {
            var prm = new DynamicParameters(new
            {
                s.Trn, r.EmpNumber,
                Sss     = Amt(r, OPayrollCompute.Acct(s, "sss")),     SssEr     = r.SssEr,
                Pagibig = Amt(r, OPayrollCompute.Acct(s, "pagibig")), PagibigEr = r.PagibigEr,
                Phic    = Amt(r, OPayrollCompute.Acct(s, "phic")),    PhicEr    = r.PhicEr
            });
            for (int i = 0; i < cols.Count; i++) prm.Add($"H{i}", r.Hours.TryGetValue(cols[i], out var h) ? h : 0);
            await cn.ExecuteAsync(
                $@"insert into {db}.tmppaymaindtl (trn, empnumber, SSS, SSSer, pagibig, pagibiger, phic, phicer, paystatus{(cols.Count > 0 ? ", " + colList : "")})
                   values (@Trn, @EmpNumber, @Sss, @SssEr, @Pagibig, @PagibigEr, @Phic, @PhicEr, 'Saved'{(cols.Count > 0 ? ", " + valList : "")})", prm, tx);
        }

        //--- save_01_tmpdata: amounts > 0 only, as in FoxPro ---------------------------------------------------------------
        var trans = s.Results.SelectMany(r => r.Amounts.Where(a => a.Value > 0)
                        .Select(a => new { s.Trn, AcctNumber = a.Key, r.EmpNumber, Amount = a.Value, User = Trunc(user, 45) })).ToList();
        if (trans.Count > 0)
            await cn.ExecuteAsync($"insert into {db}.tmptbltran (trn, acctNumber, empNumber, amount, dTimeStamp, postedBy) values (@Trn, @AcctNumber, @EmpNumber, @Amount, now(), @User)", trans, tx);

        var dtls = s.Results.SelectMany(r => r.Dtls.Where(d => !string.IsNullOrWhiteSpace(d.DtlCd) && !d.DtlCd!.StartsWith("_"))
                        .GroupBy(d => d.DtlCd).Select(g => g.First())
                        .Select(d => new { s.Trn, r.EmpNumber, d.DtlCd, d.NVal, d.Uom })).ToList();
        if (dtls.Count > 0)
            await cn.ExecuteAsync($"insert into {db}.tmptbltrandtl (trn, empnumber, dtlcd, nval, uom) values (@Trn, @EmpNumber, @DtlCd, @NVal, @Uom)", dtls, tx);

        //--- save_02_loans_tmp: loan row with the amount deducted as AMORT -------------------------------------------------
        var loans = s.Results.SelectMany(r => r.Loans).Select(l => new { s.Trn, l.Number, l.Amort }).ToList();
        if (loans.Count > 0)
            await cn.ExecuteAsync(
                $@"insert into {db}.tmploans (NUMBER, EMPNUMBER, DATE, DEDNCODE, DEDNDESC, AMOUNT, AMORT, BALANCE, STATUS, ENCODEBY, ENCODEDT,
                                              CHANGEBY, CHANGEDT, POSTED, EMPLASTNM, POSTFLAG, REMARKS, payMode, payStart, payRes, trn)
                   select NUMBER, EMPNUMBER, DATE, DEDNCODE, DEDNDESC, AMOUNT, @Amort, BALANCE, STATUS, ENCODEBY, ENCODEDT,
                          CHANGEBY, CHANGEDT, POSTED, EMPLASTNM, POSTFLAG, REMARKS, payMode, payStart, payRes, @Trn
                   from {db}.loans where NUMBER = @Number", loans, tx);

        //--- save_03_otherdeductions_tmp ----------------------------------------------------------------------------------
        var oded = s.Results.SelectMany(r => r.OtherDeds)
                    .Select(d => new { d.Year, d.Month, d.PeriodId, d.EmpNumber, d.AcctNumber, d.Amount, Remarks = d.Remarks ?? "", s.Trn }).ToList();
        if (oded.Count > 0)
            await cn.ExecuteAsync(
                $@"insert into {db}.tmptblotherdeductions (year, month, periodID, empNumber, acctNumber, amount, Processed, Remarks, trn)
                   values (@Year, @Month, @PeriodId, @EmpNumber, @AcctNumber, @Amount, 0, @Remarks, @Trn)", oded, tx);
    }

    //--- Lock --------------------------------------------------------------------------------------------------------------
    //--- Returns an error message, or null when locked.
    public async Task<string?> _04Lock(OPayrollSession s, string user)
    {
        if (s.IsLocked) return "Payroll already locked. Unable to process.";
        if (s.Results.Count == 0) return "No employees to lock.";
        OPayrollCompute.Compute(s);
        var negative = s.Results.Where(r => r.NetStatus == NetPayStatus.Negative).Select(r => r.Name).ToList();
        if (negative.Count > 0) return "Unable to lock transaction. Found record(s) with negative net pay: " + string.Join(", ", negative);

        var db = s.Paydb;
        var p  = new { s.Trn, User = Trunc(user, 25) };
        using var cn = _sql.GetConnString(s.Conn);
        cn.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            if (await IsLocked(cn, tx, s)) { tx.Rollback(); return "Payroll already locked. Unable to process."; }
            await SaveDraft(cn, tx, s, user);

            //--- lock_01_tbltran ------------------------------------------------------------------------------------------
            await cn.ExecuteAsync($@"insert into {db}.tbltran (trn, acctNumber, empNumber, amount, dTimeStamp, source, postedby)
                                     select trn, acctNumber, empNumber, amount, now(), 'main', @User from {db}.tmptbltran where trn = @Trn", p, tx);
            await cn.ExecuteAsync($@"insert into {db}.tbltrandtl (trn, empnumber, dtlCd, nval, uom)
                                     select trn, empnumber, dtlCd, nval, uom from {db}.tmptbltrandtl where trn = @Trn", p, tx);

            //--- lock_02_loans --------------------------------------------------------------------------------------------
            await cn.ExecuteAsync($"delete from {db}.loansref where trn = @Trn", p, tx);
            await cn.ExecuteAsync($@"insert into {db}.loansref (number, EMPNUMBER, DATE, DEDNCODE, AMOUNT, AMORT, BALANCE, REMARKS, TRN, LoanAmt)
                                     select number, EMPNUMBER, DATE, DEDNCODE, AMOUNT, AMORT, BALANCE - AMORT, REMARKS, trn, AMOUNT
                                     from {db}.tmploans where trn = @Trn", p, tx);
            await cn.ExecuteAsync($@"update {db}.loans l join {db}.tmploans t on t.number = l.number and t.trn = @Trn
                                     set l.balance = l.balance - t.amort, l.TRNLastPosted = @Trn", p, tx);
            await cn.ExecuteAsync($@"update {db}.loans set status = 'I'
                                     where balance < 1 and number in (select number from {db}.tmploans where trn = @Trn)", p, tx);

            //--- lock_03_otherdeductions ----------------------------------------------------------------------------------
            await cn.ExecuteAsync($@"insert into {db}.tblotherdedref (trn, year, month, period, empnumber, acctnumber, amount)
                                     select trn, year, month, periodid, empnumber, acctnumber, amount from {db}.tmptblotherdeductions where trn = @Trn", p, tx);
            await cn.ExecuteAsync($@"update {db}.tblotherdeductions d join {db}.tmptblotherdeductions t
                                       on t.trn = @Trn and t.year = d.year and t.month = d.month and t.periodID = d.periodID
                                      and t.empNumber = d.empNumber and t.acctNumber = d.acctNumber
                                     set d.processed = 1, d.TRNPosted = @Trn", p, tx);

            //--- lock_09_paymainhdr, lock_10_paymaindtl -------------------------------------------------------------------
            await cn.ExecuteAsync($"insert into {db}.paymainhdr select * from {db}.tmppaymainhdr where trn = @Trn", p, tx);
            await cn.ExecuteAsync($"update {db}.paymainhdr set user = left(concat(trim(ifnull(user,'')),'/',@User),25), status = 'Lock', dateposted = now() where trn = @Trn", p, tx);
            await cn.ExecuteAsync($"insert into {db}.paymaindtl select * from {db}.tmppaymaindtl where trn = @Trn", p, tx);

            foreach (var t in new[] { "tmppaymainhdr", "tmppaymaindtl", "tmptbltran", "tmptbltrandtl", "tmploans", "tmptblotherdeductions" })
                await cn.ExecuteAsync($"delete from {db}.{t} where trn = @Trn", p, tx);

            tx.Commit();
            s.Status = "Lock";
            return null;
        }
        catch { tx.Rollback(); throw; }
    }

    static double Amt(OPayEmpResult r, string acct) => acct != "" && r.Amounts.TryGetValue(acct, out var v) ? v : 0;
    static string Trunc(string? v, int len) => (v ?? "").Length > len ? v![..len] : v ?? "";
}
