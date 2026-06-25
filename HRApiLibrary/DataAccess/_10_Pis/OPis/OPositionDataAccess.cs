using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;

public class OPositionDataAccess : IOPositionDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OPositionDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<OPositionModel?> _01(OPositionModel position, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Position (CODE, NAME, ISGUARD, sort) values (@CODE, @NAME, @ISGUARD, @sort)";
        await _sql.ExecuteCmd<dynamic>(sql, position, conn);

        sql = $@"SELECT * FROM {schema}.Position WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<OPositionModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<OPositionModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  CODE, NAME, ISGUARD, sort from {schema}.Position where Id = @Id";
        var data = await _sql.FetchData<OPositionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<OPositionModel?>?> _02( string schema, string conn)
    {
        string sql = $@"select  CODE, NAME, ISGUARD, sort from {schema}.Position order by Name";
        var data = await _sql.FetchData<OPositionModel?, dynamic>(sql, new {  }, conn);
        return data;
    }


    public async Task<OPositionModel?> _03(int id, OPositionModel position, string schema, string conn)
    {
        string sql = $@"Update {schema}.Position set CODE = @CODE, NAME = @NAME, ISGUARD = @ISGUARD, sort = @sort where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, position, conn);

        sql = $@" select  * from {schema}.Position x where x.Id = @Id ;";
        var data = await _sql.FetchData<OPositionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OPositionModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Position where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Position x where x.Id = @Id ;";
        var data = await _sql.FetchData<OPositionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOPositionDataAccess
{
    Task<OPositionModel?> _01(OPositionModel position, string schema, string conn);
    Task<OPositionModel?> _02(int id, string schema, string conn);
    Task<List<OPositionModel?>?> _02(string schema, string conn);
    Task<OPositionModel?> _03(int id, OPositionModel position, string schema, string conn);
    Task<OPositionModel?> _04(int id, string schema, string conn);
}