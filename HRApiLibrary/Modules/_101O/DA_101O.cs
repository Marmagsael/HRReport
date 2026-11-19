using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._101O;

public class DA_101O : IDA_101O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_101O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    private async Task<int> Count(string sql, string conn)
    {
        try
        {
            var r = await _sql.FetchLegacy<long, dynamic>(sql, new { }, conn);
            return (int)(r?.FirstOrDefault() ?? 0);
        }
        catch { return 0; }
    }

    private async Task<List<T>> Rows<T>(string sql, string conn)
    {
        try { return await _sql.FetchLegacy<T, dynamic>(sql, new { }, conn) ?? []; }
        catch { return []; }
    }

    public async Task<M101_Home> _02Home(string pis, string opis, string opay, string conn, string? period = null)
    {
        var h = new M101_Home { Today = DateTime.Today };

        h.Groups = await Count($"select count(*) from {opay}.payrollgrp where Status = 'A'", conn);
        h.Drafts = await Count($"select count(*) from {opay}.tmppaymainhdr", conn);

        var cur = (await Rows<M101_Run>($@"select * from (
                        select trim(trn) Trn, attStart AttStart, attEnd AttEnd, 'Locked' Status, trim(`user`) UserName, datecreated Created from {opay}.paymainhdr
                        union all
                        select trim(trn), attStart, attEnd, 'Draft', trim(`user`), datecreated from {opay}.tmppaymainhdr) x
                        order by Created desc limit 6", conn));
        h.Runs = cur;

        h.Periods = (await Rows<string>($@"select distinct left(trim(trn), 6) from (select trn from {opay}.paymainhdr union all select trn from {opay}.tmppaymainhdr) x order by 1 desc limit 36", conn))
                    .Where(t => t.Length == 6).ToList();

        var c = cur.FirstOrDefault();
        if (period is { Length: 6 } && period.All(char.IsDigit))
        {
            var sel = await Rows<M101_Run>($@"select * from (
                        select trim(trn) Trn, attStart AttStart, attEnd AttEnd, 'Locked' Status, trim(`user`) UserName, datecreated Created from {opay}.paymainhdr
                        union all
                        select trim(trn), attStart, attEnd, 'Draft', trim(`user`), datecreated from {opay}.tmppaymainhdr) x
                        where left(Trn, 6) = '{period}' order by Created desc limit 1", conn);
            if (sel.Count > 0) c = sel[0];
        }
        if (c != null && c.Trn.Length >= 6)
        {
            string P = c.Trn.Substring(0, 6);
            h.CurrentPeriod = P;
            h.PeriodStart = c.AttStart;
            h.PeriodEnd = c.AttEnd;
            if (c.AttEnd is { } ae) h.DaysLeft = (ae.Date - DateTime.Today).Days;

            int yy = 2000 + int.Parse(P.Substring(0, 2)), mm = int.Parse(P.Substring(2, 2));
            string pd2 = P.Substring(4, 2), pd1 = pd2.TrimStart('0');

            h.LockedInPeriod = await Count($"select count(*) from {opay}.paymainhdr where left(trim(trn), 6) = '{P}'", conn);
            h.DraftsInPeriod = await Count($"select count(*) from {opay}.tmppaymainhdr where left(trim(trn), 6) = '{P}'", conn);

            //--- payroll group status for the period (trn = period + '-' + group code) ---
            h.GroupStatus = await Rows<M101_Grp>($@"select g.Name Name,
                        (select count(*) from {opis}.deprec d where d.PayrollgrpId = g.Id) Employees,
                        case when exists (select 1 from {opay}.paymainhdr x where trim(x.trn) = concat('{P}-', trim(g.Code))) then 'Locked'
                             when exists (select 1 from {opay}.tmppaymainhdr y where trim(y.trn) = concat('{P}-', trim(g.Code))) then 'Draft'
                             else 'None' end Status
                        from {opay}.payrollgrp g where g.Status = 'A' order by g.Name", conn);

            //--- totals of the period : earnings (E) less deductions (D) ---
            var lk = await Rows<M101_Totals>($@"select ifnull(sum(case when c.AcctType = 'E' then t.amount else 0 end), 0) Gross,
                        ifnull(sum(case when c.AcctType = 'D' then t.amount else 0 end), 0) Deductions
                        from {opay}.tbltran t join {opay}.chartofacct c on trim(c.AcctNumber) = trim(t.acctNumber)
                        where left(trim(t.trn), 6) = '{P}'", conn);
            h.Locked = lk.FirstOrDefault() ?? new();
            var dr = await Rows<M101_Totals>($@"select ifnull(sum(case when c.AcctType = 'E' then t.amount else 0 end), 0) Gross,
                        ifnull(sum(case when c.AcctType = 'D' then t.amount else 0 end), 0) Deductions
                        from {opay}.tmptbltran t join {opay}.chartofacct c on trim(c.AcctNumber) = trim(t.acctNumber)
                        where left(trim(t.trn), 6) = '{P}'", conn);
            h.Draft = dr.FirstOrDefault() ?? new();

            h.OpenEarnings = await Count($"select count(*) from {opay}.tblotherearnings where ifnull(processed, 0) = 0 and year = {yy} and month = {mm} and periodID in ('{pd2}', '{pd1}')", conn);
            h.OpenDeductions = await Count($"select count(*) from {opay}.tblotherdeductions where ifnull(processed, 0) = 0 and year = {yy} and month = {mm} and periodID in ('{pd2}', '{pd1}')", conn);

            h.NoEntry = await Count($@"select count(*) from {opis}.empmas e join {opis}.empstat s on s.code = e.empStat_ and ifnull(s.isInPayroll, 0) = 1
                        where e.empNumber is not null
                          and exists (select 1 from {opis}.deprec d where d.EmpNumber = e.EmpNumber and ifnull(d.PayrollgrpId, 0) > 0)
                          and not exists (select 1 from {opay}.paymaindtl m where trim(m.empnumber) = trim(e.empNumber) and left(trim(m.trn), 6) = '{P}')
                          and not exists (select 1 from {opay}.tmppaymaindtl m where trim(m.empnumber) = trim(e.empNumber) and left(trim(m.trn), 6) = '{P}')", conn);

            if (c.AttStart != null && c.AttEnd != null)
            {
                string a = c.AttStart.Value.ToString("yyyy-MM-dd"), b = c.AttEnd.Value.ToString("yyyy-MM-dd");
                h.NewHires = await Count($"select count(*) from {opis}.empmas where DATEHIRED between '{a}' and '{b}'", conn);
                h.Separated = await Count($"select count(*) from {opis}.empmas where `SEPARATE` between '{a}' and '{b}'", conn);
            }
        }

        h.Undeployed = await Count($@"select count(*) from {opis}.empmas e join {opis}.empstat s on s.code = e.empStat_
                                      where e.empNumber is not null and ifnull(s.isInPayroll, 0) = 1
                                        and not exists (select 1 from {opis}.deprec d where d.EmpNumber = e.EmpNumber and ifnull(d.PayrollgrpId, 0) > 0)", conn);

        h.NoRate = await Count($@"select count(*) from {opis}.empmas e join {opis}.empstat s on s.code = e.empStat_ and ifnull(s.isInPayroll, 0) = 1
                                  join {opis}.deprec d on d.EmpNumber = e.EmpNumber and ifnull(d.PayrollgrpId, 0) > 0
                                  join {opay}.payrollgrp g on g.Id = d.PayrollgrpId
                                  where e.empNumber is not null and ifnull(e.EmpBasicRate, 0) = 0 and ifnull(g.RatePerDay, 0) = 0", conn);
        h.NoSalgrade = await Count($@"select count(*) from {opis}.empmas e join {opis}.empstat s on s.code = e.empStat_ and ifnull(s.isInPayroll, 0) = 1
                                  left join {opay}.salgrade g on g.sgcode = e.sgcode
                                  where e.empNumber is not null and g.sgcode is null
                                    and exists (select 1 from {opis}.deprec d where d.EmpNumber = e.EmpNumber and ifnull(d.PayrollgrpId, 0) > 0)", conn);

        h.PendingAttendance = await Count($"select count(*) from {pis}.LeaveApplication where Status in ('S','F')", conn)
                            + await Count($"select count(*) from {pis}.ObApplication where Status in ('S','F')", conn)
                            + await Count($"select count(*) from {pis}.OtReqHdr where Status in ('S','F')", conn);

        h.FixedEarnings = await Count($"select count(*) from {opay}.fixed_earnings", conn)
                        + await Count($"select count(*) from {opay}.fixed_earnings_grp", conn);
        h.Mandatory = await Count($"select count(*) from {opay}.dedmandatory where status = 'A'", conn);
        h.ActiveLoans = await Count($"select count(*) from {opay}.loans where ifnull(BALANCE, 0) > 0", conn);
        h.LoansEnding = await Count($"select count(*) from {opay}.loans where ifnull(BALANCE, 0) > 0 and ifnull(AMORT, 0) > 0 and BALANCE <= AMORT * 2", conn);
        h.UnpaidInstallments = await Count($"select count(*) from {opay}.loansschedule where TRN is null or trim(TRN) = ''", conn);

        h.Trend = await Rows<M101_Trend>($@"select left(trim(t.trn), 6) Period,
                        sum(case when c.AcctType = 'E' then t.amount when c.AcctType = 'D' then -t.amount else 0 end) Net
                        from {opay}.tbltran t join {opay}.chartofacct c on trim(c.AcctNumber) = trim(t.acctNumber)
                        group by left(trim(t.trn), 6) order by Period desc limit 6", conn);
        h.Trend.Reverse();

        h.Audit = await Rows<M101_Audit>($"select trim(trn) Trn, tdate Date, action Action, trim(`user`) UserName from {opay}.audit_payroll_lock order by tdate desc limit 5", conn);

        h.Holidays = await Rows<M101_Holiday>($@"select HDate, Name, TypeCode from {opay}.holidaysetting
                        where Scope = 'G' and HDate >= curdate() order by HDate limit 5", conn);

        return h;
    }
}

public interface IDA_101O
{
    Task<M101_Home> _02Home(string pis, string opis, string opay, string conn, string? period = null);
}

public class M101_Run
{
    public string Trn { get; set; } = "";
    public DateTime? AttStart { get; set; }
    public DateTime? AttEnd { get; set; }
    public string Status { get; set; } = "";
    public string UserName { get; set; } = "";
    public DateTime? Created { get; set; }
}

public class M101_Holiday
{
    public DateTime HDate { get; set; }
    public string Name { get; set; } = "";
    public string TypeCode { get; set; } = "";
}

public class M101_Grp
{
    public string Name { get; set; } = "";
    public int Employees { get; set; }
    public string Status { get; set; } = "None";
}

public class M101_Totals
{
    public decimal Gross { get; set; }
    public decimal Deductions { get; set; }
    public decimal Net => Gross - Deductions;
}

public class M101_Trend
{
    public string Period { get; set; } = "";
    public decimal Net { get; set; }
}

public class M101_Audit
{
    public string Trn { get; set; } = "";
    public DateTime? Date { get; set; }
    public string Action { get; set; } = "";
    public string UserName { get; set; } = "";
}

public class M101_Home
{
    public DateTime Today { get; set; }
    public int Groups { get; set; }
    public int Drafts { get; set; }
    public string CurrentPeriod { get; set; } = "";
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public int DaysLeft { get; set; }
    public int LockedInPeriod { get; set; }
    public int DraftsInPeriod { get; set; }
    public M101_Totals Locked { get; set; } = new();
    public M101_Totals Draft { get; set; } = new();
    public int Undeployed { get; set; }
    public int NoRate { get; set; }
    public int NoSalgrade { get; set; }
    public List<string> Periods { get; set; } = new();
    public int NoEntry { get; set; }
    public int PendingAttendance { get; set; }
    public int OpenEarnings { get; set; }
    public int OpenDeductions { get; set; }
    public int FixedEarnings { get; set; }
    public int Mandatory { get; set; }
    public int ActiveLoans { get; set; }
    public int LoansEnding { get; set; }
    public int UnpaidInstallments { get; set; }
    public int NewHires { get; set; }
    public int Separated { get; set; }
    public List<M101_Run> Runs { get; set; } = new();
    public List<M101_Grp> GroupStatus { get; set; } = new();
    public List<M101_Trend> Trend { get; set; } = new();
    public List<M101_Audit> Audit { get; set; } = new();
    public List<M101_Holiday> Holidays { get; set; } = new();
}
