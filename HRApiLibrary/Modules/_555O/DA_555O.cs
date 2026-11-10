using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._555O;

public class DA_555O : IDA_555O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_555O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M555_Client>> _02Clients(string opis, string conn)
    {
        string sql = $"select trim(clnumber) ClNumber, trim(clname) ClName from {opis}.client where trim(upper(status)) = 'ACTIVE' order by clname";
        return await _sql.FetchLegacy<M555_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M555_Acct>> _02Accounts(string opay, string conn)
    {
        string sql = $"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName from {opay}.chartofacct where acctType = 'E' and hasRateOverBasic = 0 order by acctname";
        return await _sql.FetchLegacy<M555_Acct, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M555_Desig>> _02Designations(string opis, string conn)
    {
        string sql = $"select trim(Code) Code, trim(Name) Name from {opis}.designation order by name";
        return await _sql.FetchLegacy<M555_Desig, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<string>> _02Periods(string opay, string conn)
    {
        string sql = $"select trim(PdNumber) from {opay}.period where PdNumber != '00' order by PdNumber";
        return await _sql.FetchLegacy<string, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M555_Row>> _02Rows(string clNumber, string opis, string opay, string conn)
    {
        string sql = $@"select fe.id Id, trim(fe.acctNumber) AcctNumber, trim(ifnull(c.acctname, fe.acctNumber)) AcctName, fe.dStart DStart,
                               ifnull(fe.amount, 0) Amount, ifnull(fe.dayspara, 0) DaysPara,
                               ifnull(fe.p1, 0) P1, ifnull(fe.p2, 0) P2, ifnull(fe.p3, 0) P3, ifnull(fe.p4, 0) P4, ifnull(fe.p5, 0) P5,
                               ifnull(fe.perdayearnings, 0) PerDay, trim(ifnull(fe.desig_, '')) DesigCode, trim(ifnull(d.name, '')) Designation
                        from {opay}.fixed_earnings_grp fe
                             left join {opay}.chartofacct c on c.acctnumber = fe.acctnumber
                             left join {opis}.designation d on d.code = fe.desig_
                        where trim(fe.clnumber) = @Cl
                        order by c.acctname";
        var r = await _sql.FetchLegacy<M555_Row, dynamic>(sql, new { Cl = clNumber }, conn) ?? [];
        foreach (var x in r) if (x.DStart is { } d && d.Year < 2) x.DStart = null;
        return r;
    }

    public async Task<List<M555_Emp>> _02Employees(string clNumber, string desig, string opis, string conn)
    {
        string sql = $@"select trim(e.empnumber) EmpNumber,
                               concat(trim(ifnull(e.emplastnm, '')), ', ', trim(ifnull(e.empfirstnm, '')), ' ', trim(ifnull(e.empmidnm, ''))) EmpName
                        from {opis}.empmas e
                        where trim(e.client_) = @Cl and trim(e.desig_) = @Desig
                        order by e.emplastnm, e.empfirstnm";
        return await _sql.FetchLegacy<M555_Emp, dynamic>(sql, new { Cl = clNumber, Desig = (desig ?? "").Trim() }, conn) ?? [];
    }

    public async Task<(string? Error, int Id)> _01Save(string clNumber, M555_Row f, bool isNew, string user, string opis, string opay, string conn)
    {
        if (string.IsNullOrWhiteSpace(f.AcctNumber)) return ("Please select account from the list.", 0);
        if (string.IsNullOrWhiteSpace(f.DesigCode)) return ("Please select designation from the list.", 0);
        if (f.DStart == null) return ("Starting date is invalid.", 0);
        if (f.Amount == 0) return ("Amount is invalid.", 0);
        if (!(f.P1 == 1 || f.P2 == 1 || f.P3 == 1 || f.P4 == 1 || f.P5 == 1)) return ("Schedule is invalid.", 0);

        var p = new
        {
            Cl = clNumber, f.DStart, Acct = f.AcctNumber.Trim(), f.Amount, User = (user ?? "").Trim(), f.DaysPara,
            f.P1, f.P2, f.P3, f.P4, f.P5, f.PerDay, Desig = f.DesigCode.Trim(), f.Id
        };

        if (isNew)
        {
            var exist = await _02Rows(clNumber, opis, opay, conn);
            if (exist.Any(x => x.AcctNumber == f.AcctNumber.Trim() && x.DesigCode == f.DesigCode.Trim())) return ("Account already in used.", 0);
            await _sql.ExecuteLegacyCmd($@"insert into {opay}.fixed_earnings_grp (clnumber, dStart, acctNumber, amount, createdby, dayspara, p1, p2, p3, p4, p5, perdayearnings, desig_)
                        values (@Cl, @DStart, @Acct, @Amount, @User, @DaysPara, @P1, @P2, @P3, @P4, @P5, @PerDay, @Desig)", p, conn);
            var ids = await _sql.FetchLegacy<int, dynamic>($"select max(id) from {opay}.fixed_earnings_grp where trim(clnumber) = @Cl", new { Cl = clNumber }, conn);
            return (null, ids?.FirstOrDefault() ?? 0);
        }

        await _sql.ExecuteLegacyCmd($@"update {opay}.fixed_earnings_grp set dStart = @DStart, acctNumber = @Acct, amount = @Amount, dayspara = @DaysPara,
                        perdayearnings = @PerDay, p1 = @P1, p2 = @P2, p3 = @P3, p4 = @P4, p5 = @P5, desig_ = @Desig where id = @Id and trim(clnumber) = @Cl", p, conn);
        return (null, f.Id);
    }

    public async Task _04Delete(int id, string clNumber, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"delete from {opay}.fixed_earnings_grp where id = @Id and trim(clnumber) = @Cl", new { Id = id, Cl = clNumber }, conn);
    }
}

public interface IDA_555O
{
    Task<List<M555_Client>> _02Clients(string opis, string conn);
    Task<List<M555_Acct>> _02Accounts(string opay, string conn);
    Task<List<M555_Desig>> _02Designations(string opis, string conn);
    Task<List<string>> _02Periods(string opay, string conn);
    Task<List<M555_Row>> _02Rows(string clNumber, string opis, string opay, string conn);
    Task<List<M555_Emp>> _02Employees(string clNumber, string desig, string opis, string conn);
    Task<(string? Error, int Id)> _01Save(string clNumber, M555_Row f, bool isNew, string user, string opis, string opay, string conn);
    Task _04Delete(int id, string clNumber, string opay, string conn);
}

public class M555_Client { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; }
public class M555_Acct { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; }
public class M555_Desig { public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public class M555_Emp { public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; }

public class M555_Row
{
    public int Id { get; set; }
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public DateTime? DStart { get; set; }
    public decimal Amount { get; set; }
    public decimal DaysPara { get; set; }
    public int P1 { get; set; }
    public int P2 { get; set; }
    public int P3 { get; set; }
    public int P4 { get; set; }
    public int P5 { get; set; }
    public int PerDay { get; set; }
    public string DesigCode { get; set; } = "";
    public string Designation { get; set; } = "";
}
