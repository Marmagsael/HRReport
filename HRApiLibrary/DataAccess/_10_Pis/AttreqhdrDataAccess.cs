using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class AttreqhdrDataAccess : IAttreqhdrDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttreqhdrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttreqhdrModel?> _01(AttreqhdrModel attreqhdr, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Attreqhdr (UserId, EmpNumber, DateRequested, Remarks, Status, UserId_FApprover) values (@UserId, @EmpNumber, @DateRequested, @Remarks, @Status, @UserId_FApprover)";
        await _sql.ExecuteCmd<dynamic>(sql, attreqhdr, conn);

        sql = $@"SELECT * FROM {schema}.Attreqhdr WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<List<AttreqhdrModel?>> _02s(int id, string schema, string conn)
    {
        string sql = $@"select  Id, UserId, EmpNumber, DateRequested, Remarks, Status, UserId_FApprover from {schema}.Attreqhdr where Id = @Id";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data ?? [];
    }


    public async Task<AttreqhdrModel?> _03(int id, AttreqhdrModel attreqhdr, string schema, string conn)
    {
        string sql = $@"Update {schema}.Attreqhdr set UserId = @UserId, EmpNumber = @EmpNumber, DateRequested = @DateRequested, Remarks = @Remarks, Status = @Status, UserId_FApprover = @UserId_FApprover where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, attreqhdr, conn);

        sql = $@" select  * from {schema}.Attreqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<AttreqhdrModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Attreqhdr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Attreqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IAttreqhdrDataAccess
{
    Task<AttreqhdrModel?> _01(AttreqhdrModel attreqhdr, string schema, string conn);
    Task<List<AttreqhdrModel?>> _02s(int id, string schema, string conn);
    Task<AttreqhdrModel?> _03(int id, AttreqhdrModel attreqhdr, string schema, string conn);
    Task<AttreqhdrModel?> _04(int id, string schema, string conn);
}
