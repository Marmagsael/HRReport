using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class PositionDataAccess : IPositionDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PositionDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PositionModel?> _01(PositionModel position, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Position (CODE, NAME, ISGUARD, sort) values (@CODE, @NAME, @ISGUARD, @sort)";
        await _sql.ExecuteCmd<dynamic>(sql, position, conn);

        sql = $@"SELECT * FROM {schema}.Position WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<PositionModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<PositionModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, CODE, NAME, ISGUARD, sort from {schema}.Position where Id = @Id";
        var data = await _sql.FetchData<PositionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<PositionModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  Id, CODE, NAME, ISGUARD, sort from {schema}.Position order by Name";
        var data = await _sql.FetchData<PositionModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<PositionModel?> _03(int id, PositionModel position, string schema, string conn)
    {
        string sql = $@"Update {schema}.Position set CODE = @CODE, NAME = @NAME, ISGUARD = @ISGUARD, sort = @sort where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, position, conn);

        sql = $@" select  * from {schema}.Position x where x.Id = @Id ;";
        var data = await _sql.FetchData<PositionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<PositionModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Position where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Position x where x.Id = @Id ;";
        var data = await _sql.FetchData<PositionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}