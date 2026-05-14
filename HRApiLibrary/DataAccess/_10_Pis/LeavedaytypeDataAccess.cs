using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class LeavedaytypeDataAccess : ILeavedaytypeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LeavedaytypeDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<LeavedaytypeModel?> _01(LeavedaytypeModel leavedaytype, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Leavedaytype (Name) values (@Name)";
        await _sql.ExecuteCmd<dynamic>(sql, leavedaytype, conn);

        sql = $@"SELECT * FROM {schema}.Leavedaytype WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<LeavedaytypeModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<LeavedaytypeModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Name from {schema}.Leavedaytype where Id = @Id";
        var data = await _sql.FetchData<LeavedaytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<LeavedaytypeModel?> _03(int id, LeavedaytypeModel leavedaytype, string schema, string conn)
    {
        string sql = $@"Update {schema}.Leavedaytype set Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, leavedaytype, conn);

        sql = $@" select  * from {schema}.Leavedaytype x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavedaytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<LeavedaytypeModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Leavedaytype where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Leavedaytype x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavedaytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface ILeavedaytypeDataAccess
{
    Task<LeavedaytypeModel?> _01(LeavedaytypeModel leavedaytype, string schema, string conn);
    Task<LeavedaytypeModel?> _02(int id, string schema, string conn);
    Task<LeavedaytypeModel?> _03(int id, LeavedaytypeModel leavedaytype, string schema, string conn);
    Task<LeavedaytypeModel?> _04(int id, string schema, string conn);
}
