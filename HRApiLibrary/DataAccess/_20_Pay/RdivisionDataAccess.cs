using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._20_Pay;

public class RdivisionDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RdivisionDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RdivisionModel?> _01(RdivisionModel rdivision, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rdivision (SName, Name, SupervisorId) values (@SName, @Name, @SupervisorId)";
        await _sql.ExecuteCmd<dynamic>(sql, rdivision, conn);

        sql = $@"SELECT * FROM {schema}.Rdivision WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<RdivisionModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<RdivisionModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, SName, Name, SupervisorId from {schema}.Rdivision where Id = @Id";
        var data = await _sql.FetchData<RdivisionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<RdivisionModel?> _03(int id, RdivisionModel rdivision, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rdivision set SName = @SName, Name = @Name, SupervisorId = @SupervisorId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, rdivision, conn);

        sql = $@" select  * from {schema}.Rdivision x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdivisionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<RdivisionModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rdivision where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Rdivision x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdivisionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
