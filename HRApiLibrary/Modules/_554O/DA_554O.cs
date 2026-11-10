using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._554O;

public class DA_554O : IDA_554O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_554O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M554_Emp>> _02Employees(string opis, string conn)
    {
        string sql = $@"select trim(a.EmpNumber) EmpNumber,
                               concat(trim(ifnull(a.EmpLastNm, '')), ', ', trim(ifnull(a.EmpFirstNm, '')), ' ', left(trim(ifnull(a.EmpMidNm, '')), 1), '.') EmpName
                        from {opis}.empmas a
                        where a.EmpStat_ in (select Code from {opis}.EmpStat where isResigned = 0)
                        order by a.EmpLastNm, a.EmpFirstNm";
        return await _sql.FetchLegacy<M554_Emp, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M554_Acct>> _02Accounts(string opay, string conn)
    {
        string sql = $"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName from {opay}.chartofacct where acctType = 'E' and hasRateOverBasic = 0 order by acctname";
        return await _sql.FetchLegacy<M554_Acct, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M554_Row>> _02Rows(string emp, string opay, string conn)
    {
        string sql = $@"select fe.idfixed_earnings Id, trim(fe.acctNumber) AcctNumber, trim(ifnull(c.acctname, fe.acctNumber)) AcctName, fe.dStart DStart,
                               ifnull(fe.amount, 0) Amount, ifnull(fe.dayspara, 0) DaysPara,
                               ifnull(fe.p1, 0) P1, ifnull(fe.p2, 0) P2, ifnull(fe.p3, 0) P3, ifnull(fe.p4, 0) P4, ifnull(fe.p5, 0) P5
                        from {opay}.fixed_earnings fe left join {opay}.chartofacct c on c.acctnumber = fe.acctnumber
                        where (fe.dEnd is null or year(fe.dEnd) < 2 or fe.dEnd > now()) and trim(fe.empnumber) = @Emp
                        order by c.acctname";
        var r = await _sql.FetchLegacy<M554_Row, dynamic>(sql, new { Emp = emp }, conn) ?? [];
        foreach (var x in r) if (x.DStart is { } d && d.Year < 2) x.DStart = null;
        return r;
    }

    public async Task<string?> _01Save(string emp, M554_Row f, string user, string opay, string conn)
    {
        if (string.IsNullOrWhiteSpace(f.AcctNumber)) return "Please select account from the list.";
        if (f.DStart == null) return "Starting date is invalid.";
        if (f.Amount == 0) return "Amount is invalid.";
        if (!(f.P1 == 1 || f.P2 == 1 || f.P3 == 1 || f.P4 == 1 || f.P5 == 1)) return "Schedule is invalid.";

        var active = await _02Rows(emp, opay, conn);
        if (active.Any(x => x.AcctNumber == f.AcctNumber.Trim())) return "Account already in used.";

        await _sql.ExecuteLegacyCmd($@"insert into {opay}.fixed_earnings (empnumber, dStart, acctNumber, amount, createdby, dayspara, p1, p2, p3, p4, p5)
                        values (@Emp, @DStart, @Acct, @Amount, @User, @DaysPara, @P1, @P2, @P3, @P4, @P5)",
                        new { Emp = emp, f.DStart, Acct = f.AcctNumber.Trim(), f.Amount, User = (user ?? "").Trim(), f.DaysPara, f.P1, f.P2, f.P3, f.P4, f.P5 }, conn);
        return null;
    }

    public async Task _03Terminate(int id, string user, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"update {opay}.fixed_earnings set dEnd = now(), terminatedby = @User where idfixed_earnings = @Id",
                                    new { Id = id, User = (user ?? "").Trim() }, conn);
    }
}

public interface IDA_554O
{
    Task<List<M554_Emp>> _02Employees(string opis, string conn);
    Task<List<M554_Acct>> _02Accounts(string opay, string conn);
    Task<List<M554_Row>> _02Rows(string emp, string opay, string conn);
    Task<string?> _01Save(string emp, M554_Row f, string user, string opay, string conn);
    Task _03Terminate(int id, string user, string opay, string conn);
}

public class M554_Emp { public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; }
public class M554_Acct { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; }

public class M554_Row
{
    public int Id { get; set; }
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public DateTime? DStart { get; set; }
    public decimal Amount { get; set; }
    public decimal DaysPara { get; set; } = 0;
    public int P1 { get; set; }
    public int P2 { get; set; }
    public int P3 { get; set; }
    public int P4 { get; set; }
    public int P5 { get; set; }
}
