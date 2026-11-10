using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._605O;

public class DA_605O : IDA_605O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_605O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M605_Acct>> _02Accounts(string opay, string conn)
    {
        string sql = $@"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName from {opay}.chartofacct
                        where acctType = 'D' and trim(AcctNumber) not in ('D001', 'D002', 'D003', 'D004', 'D005', 'D006') order by AcctName";
        return await _sql.FetchLegacy<M605_Acct, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M605_Row>> _02Rows(string opay, string conn)
    {
        string sql = $@"select d.idDedMandatory Id, trim(d.acctNumber) AcctNumber, trim(ifnull(c.AcctName, d.acctNumber)) AcctName,
                               ifnull(d.ContAmt, 0) ContAmt, ifnull(d.MaxAmt, 0) MaxAmt, ifnull(d.sched, 3) Sched,
                               ifnull(d.remarks, '') Remarks, ifnull(d.status, 'A') Status,
                               ifnull(d.p1, 0) P1, ifnull(d.p2, 0) P2, ifnull(d.p3, 0) P3, ifnull(d.p4, 0) P4, ifnull(d.p5, 0) P5
                        from {opay}.dedmandatory d left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(d.acctNumber)
                        where d.status = 'A' order by AcctName";
        return await _sql.FetchLegacy<M605_Row, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M605_Client>> _02Clients(string opis, string conn)
    {
        string sql = $@"select trim(clNumber) Number, concat(trim(clName), ' - ', trim(ifnull(status, ''))) Name from {opis}.client order by clName";
        return await _sql.FetchLegacy<M605_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<string>> _02Exempted(int id, string opay, string conn)
    {
        return await _sql.FetchLegacy<string, dynamic>($"select trim(ClNumber) from {opay}.dedmandatoryexemption where IdDedMandatory = @Id", new { Id = id }, conn) ?? [];
    }

    public async Task<(string? Error, int Id)> _01Save(M605_Row f, bool isNew, string opay, string conn)
    {
        if (string.IsNullOrWhiteSpace(f.AcctNumber)) return ("Please select an account name from the list.", 0);
        if (f.ContAmt <= 0) return ("Amount is required.", 0);
        if (f.MaxAmt < 0) return ("Max. Amount is invalid.", 0);

        var p = new { f.Id, f.AcctNumber, f.ContAmt, f.MaxAmt, f.Sched, f.Remarks, f.Status, f.P1, f.P2, f.P3, f.P4, f.P5 };
        if (isNew)
        {
            var dup = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {opay}.dedmandatory where trim(acctNumber) = @AcctNumber and status <> 'S'", p, conn);
            if ((dup?.FirstOrDefault() ?? 0) > 0) return ("This account already has a mandatory deduction.", 0);

            await _sql.ExecuteLegacyCmd($@"insert into {opay}.dedmandatory (acctNumber, ContAmt, MaxAmt, sched, remarks, status, p1, p2, p3, p4, p5)
                                          values (@AcctNumber, @ContAmt, @MaxAmt, @Sched, @Remarks, 'A', @P1, @P2, @P3, @P4, @P5)", p, conn);
            var id = await _sql.FetchLegacy<long, dynamic>($"select ifnull(max(idDedMandatory), 0) from {opay}.dedmandatory", new { }, conn);
            return (null, (int)(id?.FirstOrDefault() ?? 0));
        }

        await _sql.ExecuteLegacyCmd($@"update {opay}.dedmandatory set ContAmt = @ContAmt, MaxAmt = @MaxAmt, sched = @Sched, remarks = @Remarks,
                                      status = @Status, p1 = @P1, p2 = @P2, p3 = @P3, p4 = @P4, p5 = @P5 where idDedMandatory = @Id", p, conn);
        return (null, f.Id);
    }

    public async Task _04Stop(int id, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"update {opay}.dedmandatory set status = 'S' where idDedMandatory = @Id", new { Id = id }, conn);
    }

    public async Task _01Exempt(int id, List<string> clients, string opay, string conn)
    {
        foreach (var c in clients.Distinct())
            await _sql.ExecuteLegacyCmd($"insert ignore into {opay}.dedmandatoryexemption (IdDedMandatory, ClNumber) values (@Id, @Cl)", new { Id = id, Cl = c }, conn);
    }

    public async Task _04Exempt(int id, List<string> clients, string opay, string conn)
    {
        foreach (var c in clients.Distinct())
            await _sql.ExecuteLegacyCmd($"delete from {opay}.dedmandatoryexemption where IdDedMandatory = @Id and ClNumber = @Cl", new { Id = id, Cl = c }, conn);
    }
}

public interface IDA_605O
{
    Task<List<M605_Acct>> _02Accounts(string opay, string conn);
    Task<List<M605_Row>> _02Rows(string opay, string conn);
    Task<List<M605_Client>> _02Clients(string opis, string conn);
    Task<List<string>> _02Exempted(int id, string opay, string conn);
    Task<(string? Error, int Id)> _01Save(M605_Row f, bool isNew, string opay, string conn);
    Task _04Stop(int id, string opay, string conn);
    Task _01Exempt(int id, List<string> clients, string opay, string conn);
    Task _04Exempt(int id, List<string> clients, string opay, string conn);
}

public class M605_Acct { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; }
public class M605_Client { public string Number { get; set; } = ""; public string Name { get; set; } = ""; }

public class M605_Row
{
    public int Id { get; set; }
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public decimal ContAmt { get; set; }
    public decimal MaxAmt { get; set; }
    public int Sched { get; set; } = 3;
    public string Remarks { get; set; } = "";
    public string Status { get; set; } = "A";
    public bool P1 { get; set; }
    public bool P2 { get; set; }
    public bool P3 { get; set; }
    public bool P4 { get; set; }
    public bool P5 { get; set; }
    public M605_Row Copy() => (M605_Row)MemberwiseClone();
}
