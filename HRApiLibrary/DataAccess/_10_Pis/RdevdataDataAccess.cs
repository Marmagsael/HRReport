using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._00_Main;

public class RdevdataDataAccess : IDevdataDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RdevdataDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RdevdataModel?> _01(RdevdataModel devdata, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rdevdata (DEV_NO, DEV_NAME, DEV_TYPE, DEV_LEVEL, DEV_PARENT) values (@DEV_NO, @DEV_NAME, @DEV_TYPE, @DEV_LEVEL, @DEV_PARENT)";
        await _sql.ExecuteCmd<dynamic>(sql, devdata, conn);

        sql = $@"SELECT * FROM {schema}.Rdevdata WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<RdevdataModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<RdevdataModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  DEV_NO, DEV_NAME, DEV_TYPE, DEV_LEVEL, DEV_PARENT from {schema}.Rdevdata where Id = @Id";
        var data = await _sql.FetchData<RdevdataModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<RdevdataModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  DEV_NO, DEV_NAME, DEV_TYPE, DEV_LEVEL, DEV_PARENT from {schema}.Rdevdata";
        var data = await _sql.FetchData<RdevdataModel?, dynamic>(sql, new {  }, conn);
        return data;
    }


    public async Task<RdevdataModel?> _03(int id, RdevdataModel devdata, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rdevdata set DEV_NO = @DEV_NO, DEV_NAME = @DEV_NAME, DEV_TYPE = @DEV_TYPE, DEV_LEVEL = @DEV_LEVEL, DEV_PARENT = @DEV_PARENT where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, devdata, conn);

        sql = $@" select  * from {schema}.Rdevdata x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdevdataModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<RdevdataModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rdevdata where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Rdevdata x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdevdataModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}