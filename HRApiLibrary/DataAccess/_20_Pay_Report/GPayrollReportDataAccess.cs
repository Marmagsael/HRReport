using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._20_PayGeneric;
using System.Security.AccessControl;

namespace HRApiLibrary.DataAccess._20_Pay_Report;

public class GPayrollReportDataAccess : IGPayrollReportDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public GPayrollReportDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<GChartofacctModel?>?> _02s(string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.Chartofacct order by Type Desc, AcctName ";
        var data = await _sql.FetchData<GChartofacctModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    public async Task<List<GChartofacctModel?>?> _02ByAcctTypes(string? acctType, string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.Chartofacct where AcctType = @AcctType order by AcctName ";
        var data = await _sql.FetchData<GChartofacctModel?, dynamic>(sql, new { AcctType = acctType }, conn);
        return data;
    }


    public async Task<List<RSssPremModel?>?> _02SSSPrem_ByPGrps_ByYYMM(List<string> pgrps, int? yyyy, string? mm, string? acctNumber, string? opaydb, string? opisdb, string? conn)
    {




        string? prd = $"{yyyy.ToString().Trim().Substring(2, 2)}{mm}";
        string? sql = $@" DROP TEMPORARY TABLE IF EXISTS tTbltran; 
                        DROP TEMPORARY TABLE IF EXISTS tRefssstbl; 
                        DROP TEMPORARY TABLE IF EXISTS tEmplist;

                        CREATE TEMPORARY TABLE tTbltran AS 
                        SELECT TRN, AcctNumber, EmpNumber, Amount FROM {opaydb}.tbltran  WHERE LEFT(trn,4) = @Prd; 

                        CREATE TEMPORARY TABLE tRefssstbl AS 
                        SELECT * FROM {opaydb}.refssstbl WHERE @Yyyy BETWEEN yrstart AND yrend; 

                        CREATE TEMPORARY TABLE tEmplist as 
                        SELECT DISTINCT empnumber FROM tTbltran WHERE RIGHT(trn,5) IN @Pgrps; 

                        SELECT  
                            t.EmpNumber, 
                            e.EmpLastnm, 
                            e.EmpFirstNm, 
                            e.EmpMidNm,  
                            c.ClName Payrollgrp, 
                            IF(e.DateHired IS NULL OR e.DateHired < '1000-01-01', NULL, e.DateHired) AS DateHired,
                            t.Ee,  
                            m.Ecc AS Ec, 
                            m.Er   
                        FROM (
                            SELECT EmpNumber, SUM(Amount) Ee  
                            FROM tTbltran 
                            WHERE AcctNumber = @AcctNumber 
                            GROUP BY EmpNumber
                        ) t 
                        LEFT JOIN {opisdb}.Empmas e ON e.empnumber = t.EmpNumber
                        LEFT JOIN {opisdb}.Client c ON c.ClNumber = e.Client_
                        LEFT JOIN tRefssstbl m ON m.ee = t.Ee 
                        WHERE t.empnumber IN (select * from tEmplist) 
                        Order by EmplastNm, EmpfirstNm; ";


        var data = await _sql.FetchData<RSssPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy, AcctNumber = acctNumber }, conn);
        return data;
    }

    public async Task<List<RPhicPremModel?>?> _02PHICPrem_ByPGrps_ByYYMM(List<string> pgrps, int? yyyy, string? mm, string? acctNumber, string? opaydb, string? opisdb, string? conn)
    {
        string? prd = $"{yyyy.ToString().Trim().Substring(2, 2)}{mm}";
        string? sql = $@"DROP TEMPORARY TABLE IF EXISTS tTbltran; 
                        DROP TEMPORARY TABLE IF EXISTS t2Tbltran;
                        DROP TEMPORARY TABLE IF EXISTS tEmplist;

                        CREATE TEMPORARY TABLE tTbltran AS 
                        SELECT TRN, AcctNumber, EmpNumber, Amount FROM {opaydb}.tbltran  WHERE LEFT(trn,4) = @Prd; 

                        CREATE TEMPORARY TABLE t2Tbltran AS 
                        SELECT TRN, AcctNumber, EmpNumber, Amount FROM {opaydb}.tbltran  WHERE LEFT(trn,4) = @Prd; 

                        CREATE TEMPORARY TABLE tEmplist as 
                        SELECT DISTINCT empnumber FROM tTbltran WHERE RIGHT(trn,5) IN @Pgrps; 
                        

                        SELECT  
                            t.EmpNumber, 
                            e.EmpLastnm, 
                            e.EmpFirstNm, 
                            e.EmpMidNm,  
                            e.Phic, 
                            e.EmpBirth, 
                            e.Sex_ Gender,
                            t.Ee,  
                            t.Ee AS Er, 
                            t1.Compensation   
                        FROM (SELECT EmpNumber, SUM(Amount) Ee  
                            FROM tTbltran 
                            WHERE AcctNumber = @AcctNumber 
                            GROUP BY EmpNumber
                        ) t 
                        LEFT JOIN {opisdb}.Empmas e ON e.empnumber = t.EmpNumber
                        LEFT JOIN (SELECT EmpNumber, sum(Amount) Compensation 
                                     FROM t2Tbltran 
                                     WHERE AcctNumber = 'E001' 
                                     GROUP BY EmpNumber) t1 on t1.EmpNumber = t.EmpNumber
                        WHERE t.empnumber IN (select * from tEmplist)
                        Order by EmplastNm, EmpfirstNm; ";

        var data = await _sql.FetchData<RPhicPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy, AcctNumber = acctNumber }, conn);
        return data;
    }

    public async Task<List<RPagIbigPremModel?>?> _02PagIbigPrem_ByPGrps_ByYYMM(List<string> pgrps, int? yyyy, string? mm, string? acctNumber, string? opaydb, string? opisdb, string? conn)
    {
        string? prd = $"{yyyy.ToString().Trim().Substring(2, 2)}{mm}";
        string? sql = $@" DROP TEMPORARY TABLE IF EXISTS tTbltran; 
                        DROP TEMPORARY TABLE IF EXISTS tRefpagibigtbl; 
                        DROP TEMPORARY TABLE IF EXISTS tEmplist;

                        CREATE TEMPORARY TABLE tTbltran AS 
                        SELECT TRN, AcctNumber, EmpNumber, Amount FROM {opaydb}.tbltran WHERE LEFT(trn,4) = @Prd; 

                        CREATE TEMPORARY TABLE tRefpagibigtbl AS 
                        SELECT * FROM {opaydb}.refpagibigtbl WHERE @Yyyy BETWEEN yrstart AND yrend; 

                        CREATE TEMPORARY TABLE tEmplist as 
                        SELECT DISTINCT empnumber FROM tTbltran WHERE RIGHT(trn,5) IN @Pgrps; 

                        SELECT  
                            t.EmpNumber, 
                            e.EmpLastnm, 
                            e.EmpFirstNm, 
                            e.EmpMidNm,  
                            e.PagIbiGNo, 
                            e.EmpBirth, 
                            e.Tin,
                              IF(e.DateHired IS NULL OR e.DateHired < '1000-01-01', NULL, e.DateHired) AS DateHired,
                           IF(e.DateHired IS NULL OR e.DateHired < '1000-01-01', NULL, e.DateHired) AS DateHired,
                            t.Ee,  
                            t.Ee AS Er, 
                            m.Compensation   
                        FROM (
                            SELECT EmpNumber, SUM(Amount) Ee  
                            FROM tTbltran 
                            WHERE AcctNumber = @AcctNumber 
                            GROUP BY EmpNumber
                        ) t 
                        LEFT JOIN {opisdb}.Empmas e ON e.empnumber = t.EmpNumber
                        LEFT JOIN tRefpagibigtbl m ON m.Fee = t.Ee 
                        WHERE t.empnumber IN (select * from tEmplist)
                        Order by EmplastNm, EmpfirstNm; ";  

        var data = await _sql.FetchData<RPagIbigPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy, AcctNumber = acctNumber }, conn);
        return data;
    }

    // --- Payroll Register (MBPay frmpayrollregister: per Client / per Batch -> rptPYR_04b) ----------------------------
    // --- TRNs of a period (yymmpp) to choose from, like MBPay's curBatch grid ---
    public async Task<List<RPayrollRegisterTrnModel>> _02PayrollRegisterTrns(string yymmpp, string? opaydb, string? opisdb, string? conn)
    {
        string? sql = $@"SELECT  h.TRN, b.Batch, c.ClName
                         FROM    (SELECT DISTINCT TRN FROM {opaydb}.tbltran WHERE LEFT(TRN,6) = @Prd) h
                                 LEFT JOIN {opaydb}.tblbatch b ON b.TRN = h.TRN
                                 LEFT JOIN {opisdb}.client   c ON c.ClNumber = RIGHT(h.TRN,5)
                         ORDER BY c.ClName, h.TRN ";
        return await _sql.FetchData<RPayrollRegisterTrnModel, dynamic>(sql, new { Prd = yymmpp }, conn);
    }

    // --- One row per employee per TRN with the used accounts placed in E/D slots ---
    // --- Differences from MBPay (bugs there): amounts are matched by TRN + employee (MBPay matched by employee only, so
    // --- an employee in two selected TRNs got the other TRN's amounts) and Gross/Ded/Net are computed (MBPay's loop
    // --- ran over the closed alias 'd' and left them 0) ---
    public async Task<List<RPayrollRegisterModel>> _02PayrollRegister(List<string> trns, string yymmpp, string? coName, string? opaydb, string? opisdb, string? conn)
    {
        if (trns.Count == 0) return [];

        string? sql = $@"SELECT AcctNumber, TRIM(AcctName) AcctName, TRIM(ShortDesc) ShortDesc,
                                CAST(Sort AS SIGNED) Sort, CAST(DedSort AS SIGNED) DedSort
                         FROM   {opaydb}.chartofacct WHERE status_ = 1 ";
        var coa = (await _sql.FetchData<RPayrollRegisterAcctModel, dynamic>(sql, new { }, conn))
                    .ToDictionary(x => x.AcctNumber!.Trim(), StringComparer.OrdinalIgnoreCase);

        sql = $@"SELECT  a.TRN, a.EmpNumber, a.AcctNumber, a.Amount, b.Batch,
                         CONCAT(TRIM(c.EmpLastNm),', ',TRIM(c.EmpFirstNm),' ',LEFT(c.EmpMidNm,1),'.') Empname,
                         d.ClName Clname,
                         DATE_FORMAT(h.attStart,'%d %M %Y') Dstart, DATE_FORMAT(h.attEnd,'%d %M %Y') Dend
                 FROM    {opaydb}.tbltran a
                         LEFT JOIN {opaydb}.tblbatch   b ON b.TRN = a.TRN
                         LEFT JOIN {opisdb}.empmas     c ON c.EmpNumber = a.EmpNumber
                         LEFT JOIN {opisdb}.client     d ON d.ClNumber = RIGHT(a.TRN,5)
                         LEFT JOIN {opaydb}.paymainhdr h ON h.TRN = a.TRN
                 WHERE   LEFT(a.TRN,6) = @Prd AND a.TRN IN @Trns
                 ORDER BY a.TRN, c.EmpLastNm, c.EmpFirstNm ";
        var trans = (await _sql.FetchData<RPayrollRegisterTranModel, dynamic>(sql, new { Prd = yymmpp, Trns = trns }, conn))
                    .Where(t => coa.ContainsKey(t.AcctNumber?.Trim() ?? ""))
                    .ToList();

        //--- Slots: accounts used in the run; earnings by Sort, deductions by DedSort ---
        var used = trans.Select(t => coa[t.AcctNumber!.Trim()]).DistinctBy(a => a.AcctNumber).ToList();
        var earn = used.Where(a => a.AcctNumber!.StartsWith("E", StringComparison.OrdinalIgnoreCase)).OrderBy(a => a.Sort).ThenBy(a => a.AcctNumber).Take(RPayrollRegisterModel.Slots).ToList();
        var ded  = used.Where(a => a.AcctNumber!.StartsWith("D", StringComparison.OrdinalIgnoreCase)).OrderBy(a => a.DedSort).ThenBy(a => a.AcctNumber).Take(RPayrollRegisterModel.Slots).ToList();
        var slotOf = earn.Select((a, i) => (a.AcctNumber!, (true, i + 1)))
                         .Concat(ded.Select((a, i) => (a.AcctNumber!, (false, i + 1))))
                         .ToDictionary(x => x.Item1, x => x.Item2, StringComparer.OrdinalIgnoreCase);
        string Caption(RPayrollRegisterAcctModel a) => string.IsNullOrWhiteSpace(a.ShortDesc) ? a.AcctName ?? "" : a.ShortDesc;

        var dtl = (await _02ReportSetting("REGISTERACK", opaydb, conn)).Replace("{company}", coName?.Trim());
        var rows = new Dictionary<(string, string), RPayrollRegisterModel>();
        foreach (var t in trans)
        {
            var key = (t.Trn!, t.EmpNumber!.Trim());
            if (!rows.TryGetValue(key, out var r))
            {
                r = new RPayrollRegisterModel { Trn = t.Trn, Batch = t.Batch, EmpNumber = t.EmpNumber, Empname = t.Empname, Clname = t.Clname, Dstart = t.Dstart, Dend = t.Dend, HDtl = dtl };
                for (int i = 0; i < earn.Count; i++) r.SetCaption(true, i + 1, Caption(earn[i]));
                for (int i = 0; i < ded.Count; i++) r.SetCaption(false, i + 1, Caption(ded[i]));
                rows[key] = r;
            }
            if (slotOf.TryGetValue(t.AcctNumber!.Trim(), out var s)) r.AddSlot(s.Item1, s.Item2, t.Amount);
        }

        //--- MBPay slots are n(12,2): each amount is rounded to centavos before the totals ---
        foreach (var r in rows.Values)
        {
            for (int i = 1; i <= earn.Count; i++) r.SetSlot(true, i, Cents(r.GetSlot(true, i)));
            for (int i = 1; i <= ded.Count; i++) r.SetSlot(false, i, Cents(r.GetSlot(false, i)));
            r.GrossPay = Math.Round(Enumerable.Range(1, earn.Count).Sum(i => r.GetSlot(true, i)), 2);
            r.GrossDed = Math.Round(Enumerable.Range(1, ded.Count).Sum(i => r.GetSlot(false, i)), 2);
            r.Netpay   = Math.Round(r.GrossPay - r.GrossDed, 2);
        }

        return rows.Values.OrderBy(r => r.Clname).ThenBy(r => r.Trn).ThenBy(r => r.Empname).ToList();
    }

    // --- Payroll Register (Per Account) (MBPay frmpayrollregisterperacct -> rptPYR.frx) ---------------------------------
    // --- TRNs are picked like the register (_02PayrollRegisterTrns). Difference from MBPay: hours are matched by the first
    // --- 4 characters of DtlCd. MBPay built Ennn_<timedmode> columns (E001_2) but tbltrandtl has E001_02, so they never met ---
    public async Task<List<RPayrollRegisterPerAcctModel>> _02PayrollRegisterPerAcct(List<string> trns, string yymmpp, string? coName, string? opaydb, string? opisdb, string? conn)
    {
        if (trns.Count == 0) return [];

        var coa = (await _sql.FetchData<string, dynamic>($"SELECT TRIM(AcctNumber) FROM {opaydb}.chartofacct", new { }, conn))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

        string? sql = $@"SELECT  a.TRN, a.EmpNumber, a.AcctNumber, a.Amount, b.Batch,
                                 CONCAT(TRIM(c.EmpLastNm),', ',TRIM(c.EmpFirstNm),' ',LEFT(c.EmpMidNm,1),'.') Empname,
                                 d.ClName Clname
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opaydb}.tblbatch b ON b.TRN = a.TRN
                                 LEFT JOIN {opisdb}.empmas   c ON c.EmpNumber = a.EmpNumber
                                 LEFT JOIN {opisdb}.client   d ON d.ClNumber = RIGHT(a.TRN,5)
                         WHERE   LEFT(a.TRN,6) = @Prd AND a.TRN IN @Trns
                         ORDER BY a.TRN, c.EmpLastNm, c.EmpFirstNm ";
        var trans = (await _sql.FetchData<RPayrollRegisterTranModel, dynamic>(sql, new { Prd = yymmpp, Trns = trns }, conn))
                    .Where(t => coa.Contains(t.AcctNumber?.Trim() ?? "")).ToList();

        sql = $@"SELECT TRN, EmpNumber, TRIM(DtlCd) DtlCd, nVal FROM {opaydb}.tbltrandtl WHERE LEFT(TRN,6) = @Prd AND TRN IN @Trns ";
        var dtls = (await _sql.FetchData<RPayslipDtlModel, dynamic>(sql, new { Prd = yymmpp, Trns = trns }, conn))
                    .Where(d => d.DtlCd!.Length >= 4 && coa.Contains(d.DtlCd[..4]) && !d.DtlCd.StartsWith("E016", StringComparison.OrdinalIgnoreCase));

        //--- Period Covered comes from the TRN (MBPay functions.prg PeriodCovered) ---
        int yy = 2000 + int.Parse(yymmpp[..2]), mm = int.Parse(yymmpp.Substring(2, 2));
        var from = new DateTime(yy, mm, yymmpp[4..6] == "01" ? 1 : 16);
        var to   = yymmpp[4..6] == "01" ? new DateTime(yy, mm, 15) : new DateTime(yy, mm, DateTime.DaysInMonth(yy, mm));
        var periodCovered = $"Period Covered: {from:MMMM dd, yyyy} to {to:MMMM dd, yyyy}";
        var dtl = (await _02ReportSetting("REGISTERACK", opaydb, conn)).Replace("{company}", coName?.Trim());

        var rows = new Dictionary<(string, string), RPayrollRegisterPerAcctModel>();
        foreach (var t in trans)
        {
            var key = (t.Trn!, t.EmpNumber!.Trim());
            if (!rows.TryGetValue(key, out var r))
                rows[key] = r = new RPayrollRegisterPerAcctModel { Trn = t.Trn, Batch = t.Batch, EmpNumber = t.EmpNumber, Empname = t.Empname, Clname = t.Clname, PeriodCovered = periodCovered, HDtl = dtl };

            var acct = t.AcctNumber!.Trim();
            var amt  = Cents(t.Amount);
            r.Add(acct, amt);
            if (acct.StartsWith("E", StringComparison.OrdinalIgnoreCase)) r.Gross += amt;
            else if (acct.StartsWith("D", StringComparison.OrdinalIgnoreCase)) r.Net -= amt;
        }
        foreach (var d in dtls)
            if (rows.TryGetValue((d.Trn!, d.EmpNumber!.Trim()), out var r)) r.Add(d.DtlCd![..4] + "H", d.NVal ?? 0);

        foreach (var r in rows.Values)
        {
            r.Net = r.Gross + r.Net;
            r.Round(Cents);
        }
        return rows.Values.OrderBy(r => r.Trn).ThenBy(r => r.Empname).ToList();
    }

    // --- Bank Advise / Cash Basis / Bank Charge / Bank Text File (MBPay frmbankadvise, frmbankcharge, frmbanktextfile) ----
    // --- MBPay picked the TRNs of one batch code; here the TRNs are picked from the list (_02PayrollRegisterTrns) ---
    // --- Net pay per TRN + employee for employees with earnings. IsFirstTrn: the employee's TRN with the earliest
    // --- dTimeStamp in the period (all TRNs of the period, like MBPay; MBPay read `pay`.tbltran there, any company) ---
    private async Task<List<RBankAdviseModel>> _02BankRows(List<string> trns, string? opaydb, string? opisdb, string? conn)
    {
        if (trns.Count == 0) return [];

        string? sql = $@"SELECT  a.TRN, a.EmpNumber,
                                 SUM(IF(LEFT(a.AcctNumber,1) = 'E', a.Amount, 0)) E, SUM(IF(LEFT(a.AcctNumber,1) = 'D', a.Amount, 0)) D,
                                 b.Batch, c.ClName, TRIM(d.EmpLastNm) EmpLastNm, TRIM(d.EmpFirstNm) EmpFirstNm, IFNULL(d.AcctCode,'') AcctCode,
                                 DATE_FORMAT(h.attStart,'%M %d, %Y') AttStart, DATE_FORMAT(h.attEnd,'%M %d, %Y') AttEnd
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opaydb}.tblbatch   b ON b.TRN = a.TRN
                                 LEFT JOIN {opisdb}.client     c ON c.ClNumber = RIGHT(TRIM(a.TRN),5)
                                 LEFT JOIN {opisdb}.empmas     d ON d.EmpNumber = a.EmpNumber
                                 LEFT JOIN {opaydb}.paymainhdr h ON h.TRN = a.TRN
                         WHERE   a.TRN IN @Trns
                         GROUP BY a.TRN, a.EmpNumber
                         HAVING  SUM(LEFT(a.AcctNumber,1) = 'E') > 0 ";
        var trans = await _sql.FetchData<RBankTranModel, dynamic>(sql, new { Trns = trns }, conn);
        if (trans.Count == 0) return [];

        //--- Earliest TRN of each employee in the period(s); a tie keeps the first TRN in TRN order ---
        sql = $@"SELECT TRN, EmpNumber, DATE_FORMAT(MIN(dTimeStamp),'%Y%m%d%H%i%s') FirstStamp
                 FROM   {opaydb}.tbltran
                 WHERE  LEFT(TRN,6) IN @Prds AND EmpNumber IN @Emps
                 GROUP BY EmpNumber, TRN ORDER BY TRN, EmpNumber ";
        var stamps = await _sql.FetchData<RBankTranModel, dynamic>(sql, new { Prds = trns.Select(t => t[..6]).Distinct().ToList(), Emps = trans.Select(t => t.EmpNumber).Distinct().ToList() }, conn);
        var firstTrn = stamps.GroupBy(s => (s.Trn![..6], s.EmpNumber!.Trim()))
                             .ToDictionary(g => g.Key, g => g.Aggregate((lo, s) => string.CompareOrdinal(s.FirstStamp, lo.FirstStamp) < 0 ? s : lo).Trn);

        return trans.Select(t => new RBankAdviseModel
        {
            Trn = t.Trn, Batch = t.Batch, Clname = t.ClName, Empnumber = t.EmpNumber,
            Empname = $"{t.EmpLastNm}, {t.EmpFirstNm}", EmpLastNm = t.EmpLastNm, EmpFirstNm = t.EmpFirstNm,
            Acctcode = t.AcctCode?.Trim(), Period = $"{t.AttStart} - {t.AttEnd}", Amt = t.E - t.D,
            IsFirstTrn = firstTrn.GetValueOrDefault((t.Trn![..6], t.EmpNumber!.Trim())) == t.Trn
        }).ToList();
    }

    // --- Bank Advise (employees with an ATM account, all chosen TRNs summed) or Cash Basis (no account, per TRN) ---
    public async Task<List<RBankAdviseModel>> _02BankAdvise(List<string> trns, bool cashBasis, string? opaydb, string? opisdb, string? conn)
    {
        var rows = await _02BankRows(trns, opaydb, opisdb, conn);
        if (cashBasis)
            return rows.Where(r => string.IsNullOrEmpty(r.Acctcode)).Select(r => { r.Amt = Cents(r.Amt); return r; })
                       .OrderBy(r => r.Empname).ToList();

        return rows.Where(r => !string.IsNullOrEmpty(r.Acctcode))
                   .GroupBy(r => (r.Empnumber!.Trim(), r.Empname, r.Acctcode))
                   .Select(g => new RBankAdviseModel
                   {
                       Empnumber = g.First().Empnumber, Empname = g.First().Empname, EmpLastNm = g.First().EmpLastNm, EmpFirstNm = g.First().EmpFirstNm,
                       Acctcode = g.First().Acctcode, Batch = g.First().Batch, Period = g.First().Period, Amt = Cents(g.Sum(r => r.Amt))
                   })
                   .OrderBy(r => r.Empname).ToList();
    }

    // --- Bank Charge: one line per employee with an ATM account, on the employee's first TRN of the period ---
    public async Task<List<RBankAdviseModel>> _02BankCharge(List<string> trns, string? opaydb, string? opisdb, string? conn)
        => (await _02BankRows(trns, opaydb, opisdb, conn)).Where(r => !string.IsNullOrEmpty(r.Acctcode) && r.IsFirstTrn).OrderBy(r => r.Empname).ToList();

    // --- Bank Text File (bank upload sheet): the Bank Advise amounts ordered by last name, first name ---
    public async Task<List<RBankAdviseModel>> _02BankTextFile(List<string> trns, string? opaydb, string? opisdb, string? conn)
        => (await _02BankAdvise(trns, false, opaydb, opisdb, conn)).OrderBy(r => r.EmpLastNm).ThenBy(r => r.EmpFirstNm).ToList();

    // --- Payslip (MBPay frmps "Export to PDF" -> payslip2pdf.frx) -------------------------------------------------------
    // --- TRNs are picked like the register (_02PayrollRegisterTrns). Differences from MBPay: a missing RATE is computed
    // --- (E001 amount / E001 duration) but not inserted into tbltrandtl as MBPay's fixzerorate did ---
    public async Task<List<RPayslipModel>> _02Payslip(List<string> trns, string? opaydb, string? opisdb, string? conn)
    {
        if (trns.Count == 0) return [];

        string? sql = $@"SELECT  a.TRN, a.EmpNumber, a.AcctNumber, TRIM(c.AcctName) AcctName, a.Amount,
                                 b.EmpLastNm, b.EmpFirstNm, b.EmpMidNm, b.TaxCode, b.Tin, b.Sss, d.ClName,
                                 DATE_FORMAT(h.attStart,'%M %d, %Y') AttStart, DATE_FORMAT(h.attEnd,'%M %d, %Y') AttEnd
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opaydb}.chartofacct c ON c.AcctNumber = a.AcctNumber
                                 LEFT JOIN {opisdb}.empmas      b ON b.EmpNumber  = a.EmpNumber
                                 LEFT JOIN {opisdb}.client      d ON d.ClNumber   = RIGHT(a.TRN,5)
                                 LEFT JOIN {opaydb}.paymainhdr  h ON h.TRN        = a.TRN
                         WHERE   a.TRN IN @Trns
                         ORDER BY a.EmpNumber, b.EmpLastNm, b.EmpFirstNm, a.AcctNumber ";
        var trans = await _sql.FetchData<RPayslipTranModel, dynamic>(sql, new { Trns = trns }, conn);

        sql = $@"SELECT TRN, EmpNumber, TRIM(DtlCd) DtlCd, nVal FROM {opaydb}.tbltrandtl WHERE TRN IN @Trns ";
        var dtls = (await _sql.FetchData<RPayslipDtlModel, dynamic>(sql, new { Trns = trns }, conn))
                    .GroupBy(d => (d.Trn, d.EmpNumber!.Trim())).ToDictionary(g => g.Key, g => g.ToList());

        int minLines = int.Parse(await _02ReportSetting("PAYSLIPMINLINES", opaydb, conn));
        var res = new List<RPayslipModel>();
        var slips = trans.GroupBy(t => (t.Trn, Emp: t.EmpNumber!.Trim()))
                         .OrderBy(g => g.First().ClName).ThenBy(g => g.First().EmpLastNm).ThenBy(g => g.First().EmpFirstNm).ThenBy(g => g.First().EmpMidNm);
        foreach (var slip in slips)
        {
            var h   = slip.First();
            var dtl = dtls.GetValueOrDefault(slip.Key) ?? [];
            var earn = slip.Where(t => t.AcctNumber!.StartsWith("E", StringComparison.OrdinalIgnoreCase)).ToList();
            var ded  = slip.Where(t => t.AcctNumber!.StartsWith("D", StringComparison.OrdinalIgnoreCase)).ToList();

            //--- RATE from tbltrandtl; when missing, E001 amount / E001 duration (MBPay fixzerorate) ---
            double? rate = dtl.FirstOrDefault(d => d.DtlCd == "RATE")?.NVal;
            if (rate == null)
            {
                var e001 = earn.FirstOrDefault(t => t.AcctNumber!.Trim() == "E001");
                var dur  = dtl.FirstOrDefault(d => d.DtlCd!.StartsWith("E001"))?.NVal;
                rate = e001 != null && dur is > 0 ? e001.Amount / dur : 0;
            }
            int rateDec = h.Trn!.Trim().EndsWith("00000") ? 0 : 2;

            var head = new RPayslipModel
            {
                Trn = h.Trn, Empnumber = h.EmpNumber, Emplastnm = h.EmpLastNm, Empfirstnm = h.EmpFirstNm, Empmidnm = h.EmpMidNm,
                Clname = h.ClName, Taxcode = h.TaxCode, Tin = h.Tin, Sss = h.Sss, Period = $"{h.AttStart} - {h.AttEnd}",
                Rate = (double)Math.Round((decimal)rate.Value, rateDec, MidpointRounding.AwayFromZero),
                Th   = dtl.FirstOrDefault(d => d.DtlCd == "TH")?.NVal ?? 0
            };

            int lines = Math.Max(earn.Count, ded.Count);
            for (int i = 0; i < Math.Max(lines, minLines); i++)
            {
                var r = Line(head);
                if (i < earn.Count)
                {
                    var e = earn[i];
                    r.Acctnumber = e.AcctNumber!.Trim(); r.Acctname = e.AcctName; r.Amount = Cents(e.Amount);
                    //--- duration of the earning: "88 hrs" when the code ends in 1 (hours), "15 days" otherwise ---
                    var d = dtl.FirstOrDefault(x => x.DtlCd!.Length >= 4 && x.DtlCd[..4].Equals(r.Acctnumber, StringComparison.OrdinalIgnoreCase));
                    if (d?.NVal != null) r.DurationText = $"{Math.Round(d.NVal.Value, 0, MidpointRounding.AwayFromZero)}{(d.DtlCd!.EndsWith("1") ? " hrs" : " days")}";
                }
                if (i < ded.Count)
                {
                    var x = ded[i];
                    r.Acctnumber2 = x.AcctNumber!.Trim(); r.Acctname2 = x.AcctName; r.Amount2 = Cents(x.Amount);
                }
                res.Add(r);
            }
        }
        return res;

        static RPayslipModel Line(RPayslipModel h) => new()
        {
            Trn = h.Trn, Empnumber = h.Empnumber, Emplastnm = h.Emplastnm, Empfirstnm = h.Empfirstnm, Empmidnm = h.Empmidnm,
            Clname = h.Clname, Taxcode = h.Taxcode, Tin = h.Tin, Sss = h.Sss, Period = h.Period, Rate = h.Rate, Th = h.Th
        };
    }

    // --- Account Total (MBPay frmaccounttotals -> rptAcctTotals / rptAcctTotals1) ----------------------------------------
    // --- All TRNs of the period, employees with (ATM) or without (cash) an account. 2nd period: the SSS employer share is
    // --- looked up in ssstbl from the month's SSS (1st + 2nd period); PhilHealth and Pag-IBIG employer share = employee's.
    // --- Differences from MBPay: hours are counted once per employee + account (MBPay joined them to every tbltran row, so
    // --- an account in two TRNs doubled them) and a duration in days is x HOURSPERDAY (MBPay's IF(RIGHT(DtlCd,1 = '1'), ..)
    // --- never multiplied) ---
    public async Task<List<RAcctTotalModel>> _02AccountTotal(string yymmpp, bool isAtm, string? opaydb, string? opisdb, string? conn)
    {
        var set = await _02ReportSettings(opaydb, conn);
        string S(string code) => set.TryGetValue(code, out var v) ? v : throw new InvalidOperationException($"Report setting {code} is missing in {opaydb}.reportsettings.");
        string sss = S("ACCT_SSS"), phic = S("ACCT_PHIC"), hdmf = S("ACCT_HDMF");
        double hoursPerDay = double.Parse(S("HOURSPERDAY"));
        var excluded = S("ACCTTOTAL_EXCLUDEDTL").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

        string? sql = $@"SELECT  a.EmpNumber, TRIM(a.AcctNumber) AcctNumber, TRIM(b.AcctName) AcctName, a.Amount,
                                 IF(TRIM(IFNULL(c.AcctCode,'')) = '', 0, 1) IsAtm
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opaydb}.chartofacct b ON b.AcctNumber = a.AcctNumber
                                 LEFT JOIN {opisdb}.empmas      c ON c.EmpNumber  = a.EmpNumber
                         WHERE   LEFT(a.TRN,6) = @Prd ";
        var trans = (await _sql.FetchData<RAcctTotalTranModel, dynamic>(sql, new { Prd = yymmpp }, conn)).Where(t => t.IsAtm == isAtm).ToList();
        if (trans.Count == 0) return [];

        //--- Hours per employee + account; DtlCd ending in 1 = hours, otherwise days ---
        sql = $@"SELECT EmpNumber, TRIM(DtlCd) DtlCd, nVal FROM {opaydb}.tbltrandtl WHERE LEFT(TRN,6) = @Prd AND TRIM(DtlCd) NOT IN @Excluded ";
        var hrs = (await _sql.FetchData<RPayslipDtlModel, dynamic>(sql, new { Prd = yymmpp, Excluded = excluded }, conn))
                    .Where(d => d.DtlCd!.Length >= 4)
                    .GroupBy(d => (d.EmpNumber!.Trim(), d.DtlCd![..4]))
                    .ToDictionary(g => g.Key, g => g.Sum(d => d.DtlCd!.EndsWith("1") ? d.NVal ?? 0 : (d.NVal ?? 0) * hoursPerDay));

        //--- Per employee + account ---
        var empAcct = trans.GroupBy(t => (Emp: t.EmpNumber!.Trim(), Acct: t.AcctNumber!))
                           .Select(g => (g.Key.Emp, g.Key.Acct, Name: g.First().AcctName, Amount: g.Sum(t => t.Amount), Er: 0d, Hrs: hrs.GetValueOrDefault((g.Key.Emp, g.Key.Acct))))
                           .ToList();

        //--- 2nd period: SSS employer share from the month's SSS employee share ---
        if (yymmpp.EndsWith("02"))
        {
            sql = $@"SELECT EmpNumber, SUM(Amount) Amount FROM {opaydb}.tbltran WHERE LEFT(TRN,6) = @Prd1 AND TRIM(AcctNumber) = @Sss GROUP BY EmpNumber ";
            var prev = (await _sql.FetchData<RAcctTotalTranModel, dynamic>(sql, new { Prd1 = yymmpp[..4] + "01", Sss = sss }, conn))
                        .ToDictionary(p => p.EmpNumber!.Trim(), p => p.Amount);
            var table = (await _sql.FetchData<(double Ee, double Er), dynamic>($"SELECT FSSSEE + 0e0, FSSSER + 0e0 FROM {opaydb}.ssstbl", new { }, conn))
                        .GroupBy(x => Cents(x.Ee)).ToDictionary(g => g.Key, g => g.First().Er);
            empAcct = empAcct.Select(x => x.Acct == sss ? x with { Er = table.GetValueOrDefault(Cents(x.Amount + prev.GetValueOrDefault(x.Emp))) } : x).ToList();
        }

        double earn = empAcct.Where(x => x.Acct.StartsWith("E", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Amount);
        double ded  = empAcct.Where(x => x.Acct.StartsWith("D", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Amount);
        int emps    = trans.Select(t => t.EmpNumber!.Trim()).Distinct().Count();

        return empAcct.GroupBy(x => x.Acct)
                      .Select(g => new RAcctTotalModel
                      {
                          IsAtm = isAtm, Acctnumber = g.Key, Acctname = g.First().Name,
                          Section = g.Key.StartsWith("D", StringComparison.OrdinalIgnoreCase) ? "DEDUCTIONS" : "EARNINGS",
                          Amount = Cents(g.Sum(x => x.Amount)),
                          Sss_ershare = g.Key == phic || g.Key == hdmf ? Cents(g.Sum(x => x.Amount)) : Cents(g.Sum(x => x.Er)),
                          Hrs = Math.Round(g.Sum(x => x.Hrs), 2), NetPay = Cents(earn - ded), TotalEmp = emps
                      })
                      .OrderBy(r => r.Section == "EARNINGS" ? 1 : 2).ThenBy(r => r.Acctnumber).ToList();
    }

    // --- Grand Total (MBPay frmtotalded -> rptGTCL / rptGTTT) -------------------------------------------------------------
    // --- Periods from..to (yymmpp), the chosen accounts; clients are the ones in those TRNs (MBPay "SEARCH") ---
    public async Task<List<RGrandTotalModel>> _02GrandTotalClients(string fromPrd, string toPrd, List<string> accts, string? opaydb, string? opisdb, string? conn)
    {
        if (accts.Count == 0) return [];
        string? sql = $@"SELECT  DISTINCT RIGHT(TRIM(a.TRN),5) Clnumber, c.ClName Clname
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opisdb}.client c ON c.ClNumber = RIGHT(TRIM(a.TRN),5)
                         WHERE   LEFT(a.TRN,6) BETWEEN @From AND @To AND TRIM(a.AcctNumber) IN @Accts
                         ORDER BY c.ClName ";
        return await _sql.FetchData<RGrandTotalModel, dynamic>(sql, new { From = fromPrd, To = toPrd, Accts = accts }, conn);
    }

    public async Task<List<RGrandTotalModel>> _02GrandTotal(string fromPrd, string toPrd, List<string> accts, List<string> clnumbers, bool byClient, string? opaydb, string? opisdb, string? conn)
    {
        if (accts.Count == 0 || clnumbers.Count == 0) return [];
        string? sql = $@"SELECT  RIGHT(TRIM(a.TRN),5) Clnumber, c.ClName Clname, TRIM(a.AcctNumber) Acctnumber, TRIM(b.AcctName) Acctname, SUM(a.Amount) Amount
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opaydb}.chartofacct b ON b.AcctNumber = a.AcctNumber
                                 LEFT JOIN {opisdb}.client      c ON c.ClNumber = RIGHT(TRIM(a.TRN),5)
                         WHERE   LEFT(a.TRN,6) BETWEEN @From AND @To AND TRIM(a.AcctNumber) IN @Accts AND RIGHT(TRIM(a.TRN),5) IN @Cls
                         GROUP BY RIGHT(TRIM(a.TRN),5), c.ClName, TRIM(a.AcctNumber), TRIM(b.AcctName) ";
        var rows = await _sql.FetchData<RGrandTotalModel, dynamic>(sql, new { From = fromPrd, To = toPrd, Accts = accts, Cls = clnumbers }, conn);

        bool Ded(RGrandTotalModel r) => r.Acctnumber!.StartsWith("D", StringComparison.OrdinalIgnoreCase);
        var res = rows.GroupBy(r => (Cl: byClient ? r.Clname : null, r.Acctnumber))
                      .Select(g => new RGrandTotalModel
                      {
                          Clnumber = byClient ? g.First().Clnumber : null, Clname = g.Key.Cl, Acctnumber = g.Key.Acctnumber, Acctname = g.First().Acctname,
                          Section = Ded(g.First()) ? "Deductions" : "Earnings",
                          Amount = Cents(Ded(g.First()) ? -g.Sum(r => r.Amount) : g.Sum(r => r.Amount))
                      });
        return res.OrderBy(r => r.Clname).ThenBy(r => r.Section == "Earnings" ? 1 : 2).ThenBy(r => r.Acctnumber).ToList();
    }

    // --- Pag-IBIG Contribution Remittance (MBPay frmpagibigcontremit / frmpagibigperclient -> rptPAGIgBIGContRemit) ------
    // --- Employer share = employee share up to PAGIBIG_ER_MAX. Clients: only those TRNs (MBPay per client grouped first
    // --- and then took the client of any one TRN of the employee) ---
    public async Task<List<RLoanRemitModel>> _02PagibigContRemit(string yymm, List<string>? clients, string? opaydb, string? opisdb, string? conn)
    {
        double cap = double.Parse(await _02ReportSetting("PAGIBIG_ER_MAX", opaydb, conn));
        var rows = await _02Remittance(["ACCT_HDMF"], yymm, clients, opaydb, opisdb, conn);
        foreach (var r in rows) r.Er = Math.Min(r.Amount, cap);
        return rows;
    }

    // --- Employees to pick from (MBPay curEmpMas: name, SSS no.) --------------------------------------------------------
    public async Task<List<RContributionSummaryModel>> _02ReportEmployees(string? opisdb, string? conn)
        => await _sql.FetchData<RContributionSummaryModel, dynamic>(
               $@"SELECT EmpNumber Empnumber, CONCAT(TRIM(EmpLastNm),', ',TRIM(EmpFirstNm),' ',TRIM(EmpMidNm)) Empname, SSS Sss
                  FROM {opisdb}.empmas ORDER BY EmpLastNm, EmpFirstNm ", new { }, conn);

    // --- Contribution summary of the chosen employees, months fromYymm..toYymm. acctSetting: ACCT_HDMF / ACCT_PHIC.
    // --- perMonth: PhilHealth sums the TRNs of a month; Pag-IBIG lists every TRN ---
    public async Task<List<RContributionSummaryModel>> _02ContributionSummary(string acctSetting, string fromYymm, string toYymm, List<string> emps, bool perMonth, string? opaydb, string? opisdb, string? conn)
    {
        if (emps.Count == 0) return [];
        var acct = await _02ReportSetting(acctSetting, opaydb, conn);
        string? sql = $@"SELECT  TRIM(t.EmpNumber) Empnumber, {(perMonth ? "LEFT(t.TRN,4)" : "t.TRN")} Trn, SUM(t.Amount) Amount
                         FROM    {opaydb}.tbltran t
                         WHERE   LEFT(t.TRN,4) BETWEEN @From AND @To AND TRIM(t.AcctNumber) = @Acct AND TRIM(t.EmpNumber) IN @Emps
                         GROUP BY TRIM(t.EmpNumber), {(perMonth ? "LEFT(t.TRN,4)" : "t.TRN")} ";
        var rows = await _sql.FetchData<RContributionSummaryModel, dynamic>(sql, new { From = fromYymm, To = toYymm, Acct = acct, Emps = emps }, conn);
        var names = (await _02ReportEmployees(opisdb, conn)).ToDictionary(e => e.Empnumber!.Trim(), StringComparer.OrdinalIgnoreCase);
        foreach (var r in rows)
        {
            if (names.TryGetValue(r.Empnumber!, out var e)) { r.Empname = e.Empname; r.Sss = e.Sss; }
            r.Year      = 2000 + int.Parse(r.Trn![..2]);
            r.MonthName = new DateTime(r.Year, int.Parse(r.Trn.Substring(2, 2)), 1).ToString("MMMM");
            r.Amount    = Cents(r.Amount);
        }
        return rows.OrderBy(r => r.Empname).ThenBy(r => r.Trn).ToList();
    }

    // --- Remittance of one account for a month (yymm): one row per employee, by last/first name ----------------------
    // --- acctSettings: ACCT_HDMFLOAN, ACCT_PAGIBIGLOAN, ACCT_SSSLOAN ... (MBPay had the account codes in each form);
    // --- clients: only those TRNs (RIGHT(TRN,5)) when given ---
    public async Task<List<RLoanRemitModel>> _02Remittance(List<string> acctSettings, string yymm, List<string>? clients, string? opaydb, string? opisdb, string? conn)
    {
        var accts = new List<string>();
        foreach (var s in acctSettings) accts.Add(await _02ReportSetting(s, opaydb, conn));
        string? sql = $@"SELECT  TRIM(a.EmpNumber) Empnumber, SUM(a.Amount) Amount,
                                 CONCAT(TRIM(b.EmpLastNm),', ',TRIM(b.EmpFirstNm),' ',TRIM(b.EmpMidNm)) Empname,
                                 TRIM(b.EmpLastNm) EmpLastNm, TRIM(b.EmpFirstNm) EmpFirstNm, TRIM(b.EmpMidNm) EmpMidNm,
                                 b.TIN Tin, b.SSS Sss, b.PAGIBIGNO PagibigNo,
                                 IF(b.EmpBirth IS NULL OR b.EmpBirth < '1000-01-01', NULL, DATE_FORMAT(b.EmpBirth,'%m/%d/%Y')) Empbirth
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opisdb}.empmas b ON b.EmpNumber = a.EmpNumber
                         WHERE   TRIM(a.AcctNumber) IN @Accts AND LEFT(a.TRN,4) = @Yymm {(clients == null ? "" : "AND RIGHT(TRIM(a.TRN),5) IN @Cls")}
                         GROUP BY TRIM(a.EmpNumber)
                         ORDER BY b.EmpLastNm, b.EmpFirstNm ";
        var rows = await _sql.FetchData<RLoanRemitModel, dynamic>(sql, new { Accts = accts, Yymm = yymm, Cls = clients ?? [] }, conn);
        foreach (var r in rows) r.Amount = Cents(r.Amount);
        return rows;
    }

    // --- Earnings Summary / Deductions Summary (MBPay frmearnsum -> rptEarnSum, frmdedsum -> rptDedSum) ---------------
    // --- All TRNs of the month (yymm), chosen accounts, ordered by employee name ---
    public async Task<List<REarnDedSummaryModel>> _02EarnDedSummary(string yymm, List<string> accts, string? opaydb, string? opisdb, string? conn)
    {
        if (accts.Count == 0) return [];
        string? sql = $@"SELECT  TRIM(c.EmpLastNm) Emplastnm, TRIM(c.EmpFirstNm) Empfirstnm, LEFT(TRIM(c.EmpMidNm),1) Empmidnm,
                                 TRIM(a.AcctNumber) Acctnumber, TRIM(b.AcctName) Acctname, a.Amount, TRIM(a.TRN) Trn
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opaydb}.chartofacct b ON b.AcctNumber = a.AcctNumber
                                 LEFT JOIN {opisdb}.empmas      c ON c.EmpNumber  = a.EmpNumber
                         WHERE   LEFT(a.TRN,4) = @Yymm AND TRIM(a.AcctNumber) IN @Accts
                         ORDER BY c.EmpLastNm, c.EmpFirstNm ";
        var rows = await _sql.FetchData<REarnDedSummaryModel, dynamic>(sql, new { Yymm = yymm, Accts = accts }, conn);
        foreach (var r in rows) r.Amount = Cents(r.Amount);
        return rows;
    }

    // --- Earnings History / Deductions History (MBPay frmearnsumemp / frmdedsumemp -> rptEarnHist / rptDedHist) ---------
    // --- One employee, chosen accounts, periods whose date (1st for period 01, 16th otherwise) is within from..to ---
    public async Task<List<REarnDedHistoryModel>> _02EarnDedHistory(string empNumber, DateTime from, DateTime to, List<string> accts, string? opaydb, string? conn)
    {
        if (accts.Count == 0) return [];
        string? sql = $@"SELECT  LEFT(TRIM(a.TRN),6) Period, TRIM(a.AcctNumber) Acctnumber, TRIM(b.AcctName) Acctname, a.Amount, TRIM(a.TRN) Trn
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opaydb}.chartofacct b ON b.AcctNumber = a.AcctNumber
                         WHERE   TRIM(a.EmpNumber) = @Emp AND TRIM(a.AcctNumber) IN @Accts
                         ORDER BY LEFT(TRIM(a.TRN),6) ";
        var rows = await _sql.FetchData<REarnDedHistoryModel, dynamic>(sql, new { Emp = empNumber.Trim(), Accts = accts }, conn);
        foreach (var r in rows)
        {
            r.Pdate     = new DateTime(2000 + int.Parse(r.Period![..2]), int.Parse(r.Period![2..4]), r.Period![4..] == "01" ? 1 : 16);
            r.Year      = r.Pdate.Year;
            r.MonthName = r.Pdate.ToString("MMMM", System.Globalization.CultureInfo.InvariantCulture);
            r.Pp        = r.Period![4..];
            r.Amount    = Cents(r.Amount);
        }
        return rows.Where(r => r.Pdate >= from.Date && r.Pdate <= to.Date).ToList();
    }

    // --- Monthly Earnings / Monthly Deductions (MBPay frmearnmonthly / frmdedmonthly -> rptDedMonthly) ---------------------
    // --- One account, month yymm; per employee the sum of period 01 and of period 02. MBPay made one row per TRN of
    // --- period 01 (an employee in two TRNs printed twice) and put period 02 on every one of them; here the TRNs are added ---
    public async Task<List<REarnDedMonthlyModel>> _02EarnDedMonthly(string yymm, string acct, string? opaydb, string? opisdb, string? conn)
    {
        string? sql = $@"SELECT  TRIM(c.EmpLastNm) Ln, TRIM(c.EmpFirstNm) Fn, LEFT(TRIM(c.EmpMidNm),1) Mi, TRIM(b.AcctName) Acctname,
                                 SUM(IF(SUBSTRING(TRIM(a.TRN),5,2) = '01', ROUND(a.Amount,2), 0)) Amount01,
                                 SUM(IF(SUBSTRING(TRIM(a.TRN),5,2) = '02', ROUND(a.Amount,2), 0)) Amount02
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opaydb}.chartofacct b ON b.AcctNumber = a.AcctNumber
                                 LEFT JOIN {opisdb}.empmas      c ON c.EmpNumber  = a.EmpNumber
                         WHERE   LEFT(a.TRN,4) = @Yymm AND TRIM(a.AcctNumber) = @Acct AND SUBSTRING(TRIM(a.TRN),5,2) IN ('01','02')
                         GROUP BY a.EmpNumber, c.EmpLastNm, c.EmpFirstNm, c.EmpMidNm, b.AcctName
                         ORDER BY c.EmpLastNm, c.EmpFirstNm ";
        var rows = await _sql.FetchData<REarnDedMonthlyModel, dynamic>(sql, new { Yymm = yymm, Acct = acct.Trim() }, conn);
        foreach (var r in rows) { r.Amount01 = Cents(r.Amount01); r.Amount02 = Cents(r.Amount02); }
        return rows;
    }

    // --- Deduction Summary (MBPay rptdedsummary; a screen grid, every TRN ever posted) ---------------------------------
    public async Task<List<RConsolDedModel>> _02EmpStatuses(string? opisdb, string? conn)
        => await _sql.FetchData<RConsolDedModel, dynamic>($"SELECT TRIM(Code) Code, TRIM(Name) Name FROM {opisdb}.empstat WHERE TRIM(IFNULL(Name,'')) <> '' ORDER BY Name", new { }, conn);

    public async Task<List<RConsolDedModel>> _02ConsolidatedDeduction(List<string> statuses, List<string> accts, string? opaydb, string? opisdb, string? conn)
    {
        if (statuses.Count == 0 || accts.Count == 0) return [];
        string? sql = $@"SELECT  TRIM(a.EmpNumber) Empnumber, SUM(a.Amount) Amount,
                                 CONCAT(TRIM(IFNULL(e.EmpLastNm,' ')), ', ', TRIM(IFNULL(e.EmpFirstNm,' ')), ' ', TRIM(LEFT(IFNULL(e.EmpMidNm,''),1))) Empname
                         FROM    {opaydb}.tbltran a
                                 JOIN {opisdb}.empmas e ON e.EmpNumber = a.EmpNumber
                         WHERE   TRIM(a.AcctNumber) IN @Accts AND TRIM(e.EmpStat_) IN @Statuses
                         GROUP BY TRIM(a.EmpNumber), e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm
                         ORDER BY e.EmpLastNm, e.EmpFirstNm ";
        var rows = await _sql.FetchData<RConsolDedModel, dynamic>(sql, new { Accts = accts, Statuses = statuses }, conn);
        foreach (var r in rows) r.Amount = Cents(r.Amount);
        return rows;
    }

    // --- Loan Summary (MBPay loans_management) -------------------------------------------------------------------------
    public async Task<List<RLoanSummaryModel>> _02LoanTypes(string? opaydb, string? conn)
        => await _sql.FetchData<RLoanSummaryModel, dynamic>($@"SELECT DISTINCT TRIM(l.DEDNCODE) Code, TRIM(c.AcctName) Name FROM {opaydb}.loans l
                  LEFT JOIN {opaydb}.chartofacct c ON c.AcctNumber = l.DEDNCODE WHERE LENGTH(TRIM(IFNULL(c.AcctName,''))) > 0 ORDER BY Name", new { }, conn);

    // --- Employees of the list: the resigned ones only when asked (MBPay "View Resigned Employees") ---
    public async Task<List<RLoanSummaryModel>> _02LoanEmployees(bool includeResigned, string? opisdb, string? conn)
        => await _sql.FetchData<RLoanSummaryModel, dynamic>($@"SELECT TRIM(e.EmpNumber) Empnumber, CONCAT(TRIM(e.EmpLastNm), ', ', TRIM(e.EmpFirstNm)) Empname, c.ClName Clname
                  FROM {opisdb}.empmas e LEFT JOIN {opisdb}.client c ON c.ClNumber = e.Client_
                  {(includeResigned ? "" : $"WHERE e.EmpStat_ IN (SELECT Code FROM {opisdb}.empstat WHERE IFNULL(IsResigned,'0') IN ('0',''))")}
                  ORDER BY e.EmpLastNm, e.EmpFirstNm", new { }, conn);

    // --- Latest loan (highest NUMBER) of each chosen employee for the loan account; Pymt = loandtl amort of that loan date ---
    public async Task<List<RLoanSummaryModel>> _02LoanSummary(string loanCode, List<string> emps, string? opaydb, string? opisdb, string? conn)
    {
        if (emps.Count == 0) return [];
        string? sql = $@"SELECT  TRIM(l.EmpNumber) Empnumber, CONCAT(TRIM(e.EmpLastNm), ', ', TRIM(e.EmpFirstNm)) Empname, c.ClName Clname,
                                 l.Date, IFNULL(l.Amount,0) Amount, IFNULL(l.Balance,0) Balance, IFNULL(l.Amort,0) Amort, l.Status,
                                 IFNULL((SELECT SUM(d.Amort) FROM {opaydb}.loandtl d WHERE d.AcctNumber = @Code AND d.EmpNumber = l.EmpNumber AND d.LoanDate = l.Date), 0) Pymt
                         FROM    {opaydb}.loans l
                                 LEFT JOIN {opisdb}.empmas e ON e.EmpNumber = l.EmpNumber
                                 LEFT JOIN {opisdb}.client c ON c.ClNumber  = e.Client_
                         WHERE   l.DEDNCODE = @Code AND TRIM(l.EmpNumber) IN @Emps
                                 AND l.Number IN (SELECT MAX(Number) FROM {opaydb}.loans WHERE DEDNCODE = @Code GROUP BY EmpNumber)
                         ORDER BY e.EmpLastNm, e.EmpFirstNm ";
        var rows = await _sql.FetchData<RLoanSummaryModel, dynamic>(sql, new { Code = loanCode.Trim(), Emps = emps }, conn);
        foreach (var r in rows) r.Pymt = Cents(r.Pymt);
        return rows;
    }

    // --- 13th Month (MBPay frm13thmonthdetailed / summary / register / per client / per employee / bank advice) --------
    // --- half "01" = June..November of year, "02" = December of the year before..May. Basic pay (E001) per employee per period ---
    public async Task<List<R13thMonthModel>> _02ActiveClients(string? opisdb, string? conn)
        => await _sql.FetchData<R13thMonthModel, dynamic>($"SELECT TRIM(ClNumber) Code, TRIM(ClName) Name FROM {opisdb}.client WHERE LEFT(Status,1) = 'A' ORDER BY ClName", new { }, conn);

    // --- Months of the half in slot order: slot 2k-1 = period 01, slot 2k = period 02 of the k-th month ---
    private static List<string> ThirteenthMonths(string half, int year)
    {
        int yy = year % 100;
        return half == "02"
            ? [$"{(yy + 99) % 100:00}12", $"{yy:00}01", $"{yy:00}02", $"{yy:00}03", $"{yy:00}04", $"{yy:00}05"]
            : [$"{yy:00}06", $"{yy:00}07", $"{yy:00}08", $"{yy:00}09", $"{yy:00}10", $"{yy:00}11"];
    }

    // --- clients: only these clients (null = any); noClient: employees without a client also (MBPay "View employees with no deployment");
    // --- empNumber: only that employee (the per employee reports, no status / client filter) ---
    public async Task<List<R13thMonthModel>> _02ThirteenthMonth(string half, int year, List<string>? statuses, List<string>? clients, bool noClient, string? empNumber, string? opaydb, string? opisdb, string? conn)
    {
        var months = ThirteenthMonths(half, year);
        string? sql = $@"SELECT  LEFT(TRIM(a.TRN),6) Period, TRIM(a.EmpNumber) Empnumber, a.Amount,
                                 TRIM(IFNULL(b.EmpLastNm,'')) Emplastnm, TRIM(IFNULL(b.EmpFirstNm,'')) Empfirstnm,
                                 TRIM(IFNULL(b.EmpStat_,'')) Empstat, TRIM(IFNULL(b.Client_,'')) Clnumber, TRIM(IFNULL(c.ClName,'')) Clname, TRIM(IFNULL(b.AcctCode,'')) Acctcode
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opisdb}.empmas b ON b.EmpNumber = a.EmpNumber
                                 LEFT JOIN {opisdb}.client c ON c.ClNumber  = b.Client_
                         WHERE   LEFT(a.TRN,4) BETWEEN @From AND @To AND TRIM(a.AcctNumber) = 'E001' {(empNumber == null ? "" : "AND TRIM(a.EmpNumber) = @Emp")} ";
        var raw = await _sql.FetchData<R13thTranModel, dynamic>(sql, new { From = months[0], To = months[5], Emp = empNumber?.Trim() }, conn);

        if (empNumber == null)
            raw = raw.Where(r => (statuses == null || statuses.Contains(r.Empstat!))
                              && (r.Clnumber == "" ? noClient : clients == null || clients.Contains(r.Clnumber!))).ToList();

        var res = new List<R13thMonthModel>();
        foreach (var g in raw.GroupBy(r => r.Empnumber!))
        {
            var f = g.First();
            var row = new R13thMonthModel { Clnumber = f.Clnumber, Clname = f.Clname, Empnumber = g.Key, Emplastnm = f.Emplastnm, Empfirstnm = f.Empfirstnm, Acctcode = f.Acctcode };
            var slots = new double[12];
            foreach (var t in g)
            {
                //--- MBPay: periods 03, 04, 05 of a month are added to period 02 ---
                string pp = t.Period![4..];
                string key = t.Period![..4] + (pp is "03" or "04" or "05" ? "02" : pp);
                int m = months.IndexOf(key[..4]);
                if (m < 0 || (key[4..] != "01" && key[4..] != "02")) continue;
                slots[m * 2 + (key[4..] == "02" ? 1 : 0)] += Cents(t.Amount);
            }
            row.M1 = slots[0]; row.M2 = slots[1]; row.M3 = slots[2]; row.M4 = slots[3]; row.M5 = slots[4]; row.M6 = slots[5];
            row.M7 = slots[6]; row.M8 = slots[7]; row.M9 = slots[8]; row.M10 = slots[9]; row.M11 = slots[10]; row.M12 = slots[11];
            res.Add(row);
        }
        return res.OrderBy(r => r.Emplastnm).ThenBy(r => r.Empfirstnm).ToList();
    }

    // --- SSS Contribution Remittance (MBPay frmssscontremit / frmssscontremit2 -> rptSSSContRemit) -----------------------
    // --- Employee share of the month (ACCT_SSS) per employee; the employer share and EC come from ssstbl where its employee
    // --- share equals the amount (none = 0). adjusted: first the amount is moved to the nearest ssstbl employee share (MBPay
    // --- frmssscontremit2; kept as is when no share is within 1000). clients: only the TRNs of these clients (MBPay
    // --- frmssscontremitperclient: the employee share is the total of those clients, the employer share looked up from it) ---
    public async Task<List<RSssRemitModel>> _02SssRemit(string yymm, bool adjusted, string? opaydb, string? opisdb, string? conn, List<string>? clients = null)
    {
        string acct = await _02ReportSetting("ACCT_SSS", opaydb, conn);
        string? sql = $@"SELECT  TRIM(a.EmpNumber) Empnumber, a.Amount,
                                 TRIM(b.EmpLastNm) Emplastnm, TRIM(b.EmpFirstNm) Empfirstnm, TRIM(b.EmpMidNm) Empmidnm, b.SSS Sss,
                                 CONCAT(TRIM(b.EmpLastNm),', ',TRIM(b.EmpFirstNm),' ',TRIM(b.EmpMidNm)) Empname
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opisdb}.empmas b ON b.EmpNumber = a.EmpNumber
                         WHERE   TRIM(a.AcctNumber) = @Acct AND LEFT(a.TRN,4) = @Yymm {(clients == null ? "" : "AND RIGHT(TRIM(a.TRN),5) IN @Cls")} ";
        var lines = await _sql.FetchData<RSssRemitModel, dynamic>(sql, new { Acct = acct, Yymm = yymm, Cls = clients ?? [] }, conn);
        var rows = lines.GroupBy(l => l.Empnumber!)
                        .Select(g => { var f = g.First(); f.Amount = Cents(g.Sum(l => Cents(l.Amount))); return f; })
                        .OrderBy(r => r.Emplastnm).ThenBy(r => r.Empfirstnm).ToList();

        var table = await _sql.FetchData<(double Ee, double Er, double Ec), dynamic>($"SELECT FSSSEE + 0e0, FSSSER + 0e0, FECC + 0e0 FROM {opaydb}.ssstbl ORDER BY FSSSEE", new { }, conn);
        foreach (var r in rows)
        {
            if (adjusted)
            {
                double best = 1000;
                foreach (var t in table)
                {
                    double d = Math.Abs(r.Amount - t.Ee);
                    if (d < best) { best = d; r.Amount = Cents(t.Ee); }
                }
            }
            var m = table.FirstOrDefault(t => Cents(t.Ee) == r.Amount);
            r.Fssser = m.Er; r.Fecc = m.Ec;
        }
        return rows;
    }

    // --- SSS Loan Remittance (MBPay frmsssloanremittance -> rptSSSREmit) -------------------------------------------------
    // --- Clients of the month's TRNs (MBPay filled its client list from them) ---
    public async Task<List<RSssLoanRemitModel>> _02MonthClients(string yymm, string? opaydb, string? opisdb, string? conn)
        => await _sql.FetchData<RSssLoanRemitModel, dynamic>($@"SELECT DISTINCT RIGHT(TRIM(a.TRN),5) Clnumber, c.ClName Clname FROM {opaydb}.tbltran a
                  LEFT JOIN {opisdb}.client c ON c.ClNumber = RIGHT(TRIM(a.TRN),5) WHERE LEFT(a.TRN,4) = @Yymm ORDER BY c.ClName", new { Yymm = yymm }, conn);

    // --- loansref rows of the SSS loan account (ACCT_SSSLOAN) whose TRN is of the month and was really deducted (same TRN, employee and
    // --- account in tbltran), clients: those TRNs only; amortization added per employee ---
    public async Task<List<RSssLoanRemitModel>> _02SssLoanRemit(string yymm, List<string> clients, string? opaydb, string? opisdb, string? conn)
        => await _02LoanRefRemit(await _02ReportSetting("ACCT_SSSLOAN", opaydb, conn), yymm, clients, opaydb, opisdb, conn);

    public async Task<List<RSssLoanRemitModel>> _02LoanRefRemit(string acct, string yymm, List<string> clients, string? opaydb, string? opisdb, string? conn)
    {
        if (clients.Count == 0) return [];
        string? sql = $@"SELECT  TRIM(a.EmpNumber) Empnumber, DATE_FORMAT(a.Date,'%m/%d/%y') Date, IFNULL(a.Amount,0) Amount, IFNULL(a.Amort,0) Amort,
                                 TRIM(b.EmpLastNm) Emplastnm, TRIM(b.EmpFirstNm) Empfirstnm, LEFT(TRIM(b.EmpMidNm),1) Empmidnm, b.SSS Sss
                         FROM    {opaydb}.loansref a
                                 LEFT JOIN {opisdb}.empmas b ON b.EmpNumber = a.EmpNumber
                         WHERE   TRIM(a.DednCode) = @Acct AND LEFT(a.TRN,4) = @Yymm AND RIGHT(TRIM(a.TRN),5) IN @Cls
                                 AND EXISTS (SELECT 1 FROM {opaydb}.tbltran x WHERE x.TRN = a.TRN AND x.EmpNumber = a.EmpNumber AND x.AcctNumber = a.DednCode)
                         ORDER BY a.TRN, a.Date ";
        var lines = await _sql.FetchData<RSssLoanRemitModel, dynamic>(sql, new { Acct = acct, Yymm = yymm, Cls = clients }, conn);
        return lines.GroupBy(l => l.Empnumber!)
                    .Select(g => { var f = g.First(); f.Amort = Cents(g.Sum(l => Cents(l.Amort))); f.Amount = Cents(f.Amount); return f; })
                    .OrderBy(r => r.Emplastnm).ThenBy(r => r.Empfirstnm).ToList();
    }

    // --- Monthly PHIC Report per payroll group (MBPay frmphmoperclient -> rptPhMo) ----------------------------------------
    // --- Employees with a PhilHealth premium (ACCT_PHIC) in a TRN of the chosen clients; their premium is the total of the
    // --- whole month (MBPay took every TRN of the month, not only those of the clients) ---
    public async Task<List<RPhicMonthlyModel>> _02PhicMonthly(string yymm, List<string> clients, string? opaydb, string? opisdb, string? conn)
    {
        if (clients.Count == 0) return [];
        string acct = await _02ReportSetting("ACCT_PHIC", opaydb, conn);
        string? sql = $@"SELECT  TRIM(a.EmpNumber) Empnumber, a.Amount, RIGHT(TRIM(a.TRN),5) Clnumber,
                                 CONCAT(TRIM(IFNULL(b.EmpLastNm,'')), ', ', TRIM(IFNULL(b.EmpFirstNm,'')), ' ', TRIM(IFNULL(b.EmpMidNm,''))) Empname, b.PHIC Phic,
                                 b.EmpLastNm, b.EmpFirstNm
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opisdb}.empmas b ON b.EmpNumber = a.EmpNumber
                         WHERE   TRIM(a.AcctNumber) = @Acct AND LEFT(a.TRN,4) = @Yymm ";
        var lines = await _sql.FetchData<RPhicLineModel, dynamic>(sql, new { Acct = acct, Yymm = yymm }, conn);
        var inGroup = lines.Where(l => clients.Contains(l.Clnumber!)).Select(l => l.Empnumber!).ToHashSet();
        return lines.Where(l => inGroup.Contains(l.Empnumber!))
                    .GroupBy(l => l.Empnumber!)
                    .Select(g => { var f = g.First(); return new { f.EmpLastNm, f.EmpFirstNm, Row = new RPhicMonthlyModel { Empnumber = g.Key, Empname = f.Empname, Phic = f.Phic, Amount = Cents(g.Sum(l => Cents(l.Amount))) } }; })
                    .OrderBy(x => x.EmpLastNm).ThenBy(x => x.EmpFirstNm).Select(x => x.Row).ToList();
    }

    // --- PhilHealth Employer's Quarterly Remittance Report (MBPay frmphicquarterly -> rptphquarter) ----------------------
    // --- quarter 1..4 of year; the premium (ACCT_PHIC) of each employee per month of the quarter (all TRNs of the month added) ---
    public async Task<List<RPhicQuarterModel>> _02PhicQuarter(int quarter, int year, string? opaydb, string? opisdb, string? conn)
    {
        string acct = await _02ReportSetting("ACCT_PHIC", opaydb, conn);
        int m1 = (quarter - 1) * 3 + 1;
        var months = new[] { m1, m1 + 1, m1 + 2 }.Select(m => $"{year % 100:00}{m:00}").ToList();
        string? sql = $@"SELECT  LEFT(TRIM(a.TRN),4) Trn, TRIM(a.EmpNumber) Empnumber, a.Amount, b.SSS Sss,
                                 TRIM(IFNULL(b.EmpLastNm,'')) Ln, TRIM(IFNULL(b.EmpFirstNm,'')) Fn, LEFT(TRIM(IFNULL(b.EmpMidNm,'')),1) Mi
                         FROM    {opaydb}.tbltran a
                                 LEFT JOIN {opisdb}.empmas b ON b.EmpNumber = a.EmpNumber
                         WHERE   TRIM(a.AcctNumber) = @Acct AND LEFT(a.TRN,4) IN @Months ";
        var lines = await _sql.FetchData<RPhicQuarterModel, dynamic>(sql, new { Acct = acct, Months = months }, conn);
        var res = new List<RPhicQuarterModel>();
        foreach (var g in lines.GroupBy(l => l.Empnumber!))
        {
            var f = g.First();
            double Sum(string m) => Cents(g.Where(l => l.Trn == m).Sum(l => Cents(l.Amount)));
            res.Add(new RPhicQuarterModel { Empnumber = g.Key, Sss = f.Sss, Ln = f.Ln, Fn = f.Fn, Mi = f.Mi, P1ee = Sum(months[0]), P2ee = Sum(months[1]), P3ee = Sum(months[2]) });
        }
        return res.OrderBy(r => r.Ln).ThenBy(r => r.Fn).ToList();
    }

    // --- Report settings ({opaydb}.reportsettings, made by OldPay_TblMaker._01ReportSettings) ---------------------------
    public async Task<Dictionary<string, string>> _02ReportSettings(string? opaydb, string? conn)
        => (await _sql.FetchData<(string Code, string Value), dynamic>($"SELECT TRIM(Code), IFNULL(Value,'') FROM {opaydb}.reportsettings", new { }, conn))
            .ToDictionary(x => x.Code, x => x.Value, StringComparer.OrdinalIgnoreCase);

    // --- One setting; a missing one is an error (no value is assumed in code) ---
    public async Task<string> _02ReportSetting(string code, string? opaydb, string? conn)
        => (await _02ReportSettings(opaydb, conn)).TryGetValue(code, out var v) ? v
           : throw new InvalidOperationException($"Report setting {code} is missing in {opaydb}.reportsettings.");

    //--- MBPay report cursors are n(_,2): amounts are rounded to centavos as decimal, so a stored 16.775 (16.77499.. as a
    //--- double) gives 16.78 like FoxPro ---
    private static double Cents(double v) => (double)Math.Round((decimal)v, 2, MidpointRounding.AwayFromZero);

}


public interface IGPayrollReportDataAccess
{
    Task<List<GChartofacctModel?>?> _02ByAcctTypes(string? acctType, string? schema, string? conn);
    Task<List<GChartofacctModel?>?> _02s(string? schema, string? conn);
    Task<List<RSssPremModel?>?> _02SSSPrem_ByPGrps_ByYYMM(List<string> pgrps, int? yyyy, string? mm, string? acctType, string? opaydb, string? opisdb, string? conn);
    Task<List<RPhicPremModel?>?> _02PHICPrem_ByPGrps_ByYYMM(List<string> pgrps, int? yyyy, string? mm, string? acctType, string? opaydb, string? opisdb, string? conn);
    Task<List<RPagIbigPremModel?>?> _02PagIbigPrem_ByPGrps_ByYYMM(List<string> pgrps, int? yyyy, string? mm, string? acctType, string? opaydb, string? opisdb, string? conn);
    Task<List<RPayrollRegisterTrnModel>> _02PayrollRegisterTrns(string yymmpp, string? opaydb, string? opisdb, string? conn);
    Task<List<RPayrollRegisterModel>> _02PayrollRegister(List<string> trns, string yymmpp, string? coName, string? opaydb, string? opisdb, string? conn);
    Task<List<RPayrollRegisterPerAcctModel>> _02PayrollRegisterPerAcct(List<string> trns, string yymmpp, string? coName, string? opaydb, string? opisdb, string? conn);
    Task<List<RBankAdviseModel>> _02BankAdvise(List<string> trns, bool cashBasis, string? opaydb, string? opisdb, string? conn);
    Task<List<RBankAdviseModel>> _02BankCharge(List<string> trns, string? opaydb, string? opisdb, string? conn);
    Task<List<RBankAdviseModel>> _02BankTextFile(List<string> trns, string? opaydb, string? opisdb, string? conn);
    Task<List<RAcctTotalModel>> _02AccountTotal(string yymmpp, bool isAtm, string? opaydb, string? opisdb, string? conn);
    Task<List<RGrandTotalModel>> _02GrandTotalClients(string fromPrd, string toPrd, List<string> accts, string? opaydb, string? opisdb, string? conn);
    Task<List<RGrandTotalModel>> _02GrandTotal(string fromPrd, string toPrd, List<string> accts, List<string> clnumbers, bool byClient, string? opaydb, string? opisdb, string? conn);
    Task<List<RLoanRemitModel>> _02Remittance(List<string> acctSettings, string yymm, List<string>? clients, string? opaydb, string? opisdb, string? conn);
    Task<List<RLoanRemitModel>> _02PagibigContRemit(string yymm, List<string>? clients, string? opaydb, string? opisdb, string? conn);
    Task<List<RContributionSummaryModel>> _02ReportEmployees(string? opisdb, string? conn);
    Task<List<RContributionSummaryModel>> _02ContributionSummary(string acctSetting, string fromYymm, string toYymm, List<string> emps, bool perMonth, string? opaydb, string? opisdb, string? conn);
    Task<Dictionary<string, string>> _02ReportSettings(string? opaydb, string? conn);
    Task<string> _02ReportSetting(string code, string? opaydb, string? conn);
    Task<List<REarnDedSummaryModel>> _02EarnDedSummary(string yymm, List<string> accts, string? opaydb, string? opisdb, string? conn);
    Task<List<REarnDedHistoryModel>> _02EarnDedHistory(string empNumber, DateTime from, DateTime to, List<string> accts, string? opaydb, string? conn);
    Task<List<REarnDedMonthlyModel>> _02EarnDedMonthly(string yymm, string acct, string? opaydb, string? opisdb, string? conn);
    Task<List<RConsolDedModel>> _02EmpStatuses(string? opisdb, string? conn);
    Task<List<RConsolDedModel>> _02ConsolidatedDeduction(List<string> statuses, List<string> accts, string? opaydb, string? opisdb, string? conn);
    Task<List<RLoanSummaryModel>> _02LoanTypes(string? opaydb, string? conn);
    Task<List<RLoanSummaryModel>> _02LoanEmployees(bool includeResigned, string? opisdb, string? conn);
    Task<List<RLoanSummaryModel>> _02LoanSummary(string loanCode, List<string> emps, string? opaydb, string? opisdb, string? conn);
    Task<List<R13thMonthModel>> _02ActiveClients(string? opisdb, string? conn);
    Task<List<R13thMonthModel>> _02ThirteenthMonth(string half, int year, List<string>? statuses, List<string>? clients, bool noClient, string? empNumber, string? opaydb, string? opisdb, string? conn);
    Task<List<RSssRemitModel>> _02SssRemit(string yymm, bool adjusted, string? opaydb, string? opisdb, string? conn, List<string>? clients = null);
    Task<List<RSssLoanRemitModel>> _02MonthClients(string yymm, string? opaydb, string? opisdb, string? conn);
    Task<List<RSssLoanRemitModel>> _02SssLoanRemit(string yymm, List<string> clients, string? opaydb, string? opisdb, string? conn);
    Task<List<RSssLoanRemitModel>> _02LoanRefRemit(string acct, string yymm, List<string> clients, string? opaydb, string? opisdb, string? conn);
    Task<List<RPhicMonthlyModel>> _02PhicMonthly(string yymm, List<string> clients, string? opaydb, string? opisdb, string? conn);
    Task<List<RPhicQuarterModel>> _02PhicQuarter(int quarter, int year, string? opaydb, string? opisdb, string? conn);
    Task<List<RPayslipModel>> _02Payslip(List<string> trns, string? opaydb, string? opisdb, string? conn);

}
