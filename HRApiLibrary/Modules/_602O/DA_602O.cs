using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._602O;

public class DA_602O : IDA_602O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_602O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    private static string Fld(int pd) => "p" + Math.Clamp(pd, 1, 5);
    private static string Prdx(int year, int month) => year.ToString() + month.ToString("00");
    private static string Cl(string? clNumber) => string.IsNullOrWhiteSpace(clNumber) ? "--1" : clNumber.Trim();

    public async Task<List<M602_Emp>> _02Employees(string opis, string conn)
    {
        string sql = $@"select trim(e.EmpNumber) EmpNumber,
                               concat(trim(ifnull(e.EmpLastNm, '')), ', ', trim(ifnull(e.EmpFirstNm, '')), ' ', trim(ifnull(e.EmpMidNm, ''))) EmpName,
                               trim(ifnull(e.CLIENT_, '')) ClNumber
                        from {opis}.Empmas e
                        where e.EmpStat_ in (select code from {opis}.empstat where isinPayroll = 1)
                        order by e.EmpLastNm, e.EmpFirstNm";
        var r = await _sql.FetchLegacy<M602_Emp, dynamic>(sql, new { }, conn) ?? [];
        foreach (var e in r) e.EmpName = (e.EmpName ?? "").Trim();
        return r;
    }

    public async Task<List<M602_Acct>> _02Accounts(string opay, string conn)
    {
        string sql = $"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName from {opay}.chartofacct where AcctType = 'D' order by AcctName";
        return await _sql.FetchLegacy<M602_Acct, dynamic>(sql, new { }, conn) ?? [];
    }

    private async Task<HashSet<string>> Exempt(string emp, string clNumber, string opay, string conn)
    {
        var r = await _sql.FetchLegacy<string, dynamic>($"select trim(acctnumber) from {opay}.empdedexemption where empnumber = @Emp and clnumber != @Cl",
                    new { Emp = emp, Cl = Cl(clNumber) }, conn) ?? [];
        return r.ToHashSet();
    }

    public async Task<List<M602_Other>> _02Others(string emp, string clNumber, int year, int month, int pd, string opay, string conn)
    {
        string f = Fld(pd);
        string sql = $@"select d.year Year, d.month Month, d.periodID PeriodId, trim(d.acctNumber) AcctNumber, trim(ifnull(c.AcctName, d.acctNumber)) AcctName, d.amount Amount
                        from {opay}.tblotherdeductions d left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(d.acctNumber)
                        where ((d.processed = 0 and concat(d.year, lpad(trim(d.month), 2, '0')) <= @Prdx and d.{f} = 1)
                            or (d.contproc = 1 and concat(d.year, lpad(trim(d.month), 2, '0')) <= @Prdx and d.{f} = 1))
                          and trim(d.empNumber) = @Emp
                        order by AcctName";
        var r = await _sql.FetchLegacy<M602_Other, dynamic>(sql, new { Emp = emp, Prdx = Prdx(year, month) }, conn) ?? [];
        var ex = await Exempt(emp, clNumber, opay, conn);
        return r.Where(x => !ex.Contains(x.AcctNumber)).ToList();
    }

    public async Task<List<M602_Line>> _02Earnings(string emp, int year, int month, int pd, string opay, string conn)
    {
        string trn = (year % 100).ToString("00") + month.ToString("00") + pd.ToString("00");
        string sql = $@"select trim(ifnull(c.AcctName, t.acctNumber)) AcctName, t.amount Amount
                        from {opay}.tmptbltran t left join {opay}.chartofacct c on trim(c.AcctNumber) = convert(trim(t.acctNumber) using latin1)
                        where trim(t.empNumber) = @Emp and left(t.TRN, 6) = @Trn and c.AcctType = 'E' order by c.AcctName";
        return await _sql.FetchLegacy<M602_Line, dynamic>(sql, new { Emp = emp, Trn = trn }, conn) ?? [];
    }

    public async Task<List<M602_Line>> _02Deductions(string emp, string clNumber, int year, int month, int pd, string opay, string conn)
    {
        string f = Fld(pd);
        string prdx = Prdx(year, month);
        var lines = new List<M602_Line>();

        foreach (var o in await _02Others(emp, clNumber, year, month, pd, opay, conn))
            lines.Add(new M602_Line { AcctNumber = o.AcctNumber, AcctName = o.AcctName, Amount = o.Amount, Source = "Other", Kind = "Other" });

        string loans = $@"select trim(l.DEDNCODE) AcctNumber, trim(ifnull(c.AcctName, l.DEDNDESC)) AcctName, l.AMORT Amount
                          from {opay}.loans l left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(l.DEDNCODE)
                          where trim(l.EMPNUMBER) = @Emp and l.STATUS = 'A' and l.BALANCE > 0 and l.{f} = 1
                            and if(year(l.payRes) > 1, concat(year(l.payRes), lpad(month(l.payRes), 2, '0')), concat(year(l.payStart), lpad(month(l.payStart), 2, '0'))) <= @Prdx";
        foreach (var l in await _sql.FetchLegacy<M602_Line, dynamic>(loans, new { Emp = emp, Prdx = prdx }, conn) ?? [])
        { l.Source = "Loan"; l.Kind = "Loan"; lines.Add(l); }

        string mand = $@"select trim(d.acctNumber) AcctNumber, trim(ifnull(c.AcctName, d.acctNumber)) AcctName, d.ContAmt Amount
                         from {opay}.dedmandatory d left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(d.acctNumber)
                         where d.status = 'A' and d.{f} = 1
                           and d.idDedMandatory not in (select IdDedMandatory from {opay}.dedmandatoryexemption where trim(ClNumber) = @Cl)";
        foreach (var m in await _sql.FetchLegacy<M602_Line, dynamic>(mand, new { Cl = Cl(clNumber) }, conn) ?? [])
        { m.Source = "Mandatory"; m.Kind = "Mand"; lines.Add(m); }

        var ex = await Exempt(emp, clNumber, opay, conn);
        return lines.Where(x => !ex.Contains(x.AcctNumber)).ToList();
    }

    public async Task<string?> _01Save(string emp, int year, int month, int pd, string acct, decimal amount, List<M602_Other> shown, string opay, string conn)
    {
        if (string.IsNullOrWhiteSpace(acct)) return "Please select an account.";
        if (amount == 0) return "Amount should not be zero!";
        if (shown.Any(x => x.AcctNumber == acct)) return "Unable to save existing account, use update instead.";

        var p = new { Year = year, Month = month, Pd = pd.ToString(), Emp = emp, Acct = acct, Amount = amount,
                      P1 = pd == 1 ? 1 : 0, P2 = pd == 2 ? 1 : 0, P3 = pd == 3 ? 1 : 0, P4 = pd == 4 ? 1 : 0, P5 = pd == 5 ? 1 : 0 };
        var dup = await _sql.FetchLegacy<long, dynamic>($@"select count(*) from {opay}.tblotherdeductions
                        where year = @Year and month = @Month and periodID = @Pd and empNumber = @Emp and acctNumber = @Acct", p, conn);
        if ((dup?.FirstOrDefault() ?? 0) > 0) return "Unable to save existing account, use update instead.";

        await _sql.ExecuteLegacyCmd($@"insert into {opay}.tblotherdeductions (year, month, periodID, empNumber, acctNumber, amount, Processed, Remarks, contproc, p1, p2, p3, p4, p5)
                        values (@Year, @Month, @Pd, @Emp, @Acct, @Amount, 0, '', 0, @P1, @P2, @P3, @P4, @P5)", p, conn);
        return null;
    }

    public async Task _03Update(string emp, M602_Other row, decimal amount, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($@"update {opay}.tblotherdeductions set amount = @Amount
                        where year = @Year and month = @Month and periodID = @PeriodId and empNumber = @Emp and acctNumber = @Acct",
                        new { Amount = amount, row.Year, row.Month, row.PeriodId, Emp = emp, Acct = row.AcctNumber }, conn);
    }

    public async Task _04Delete(string emp, List<M602_Other> rows, string opay, string conn)
    {
        foreach (var r in rows)
            await _sql.ExecuteLegacyCmd($@"delete from {opay}.tblotherdeductions
                        where year = @Year and month = @Month and periodID = @PeriodId and empNumber = @Emp and acctNumber = @Acct",
                        new { r.Year, r.Month, r.PeriodId, Emp = emp, Acct = r.AcctNumber }, conn);
    }
}

public interface IDA_602O
{
    Task<List<M602_Emp>> _02Employees(string opis, string conn);
    Task<List<M602_Acct>> _02Accounts(string opay, string conn);
    Task<List<M602_Other>> _02Others(string emp, string clNumber, int year, int month, int pd, string opay, string conn);
    Task<List<M602_Line>> _02Earnings(string emp, int year, int month, int pd, string opay, string conn);
    Task<List<M602_Line>> _02Deductions(string emp, string clNumber, int year, int month, int pd, string opay, string conn);
    Task<string?> _01Save(string emp, int year, int month, int pd, string acct, decimal amount, List<M602_Other> shown, string opay, string conn);
    Task _03Update(string emp, M602_Other row, decimal amount, string opay, string conn);
    Task _04Delete(string emp, List<M602_Other> rows, string opay, string conn);
}

public class M602_Emp { public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string ClNumber { get; set; } = ""; }
public class M602_Acct { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; }

public class M602_Other
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string PeriodId { get; set; } = "";
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public decimal Amount { get; set; }
}

public class M602_Line
{
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public decimal Amount { get; set; }
    public string Source { get; set; } = "";
    public string Kind { get; set; } = "";
}
