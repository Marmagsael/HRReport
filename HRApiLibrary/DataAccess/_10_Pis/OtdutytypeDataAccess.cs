using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class OtdutytypeDataAccess : IOtdutytypeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;
    public OtdutytypeDataAccess(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<OtdutytypeModel?> _01(OtdutytypeModel otdutytype, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Otdutytype (Code, Name) values (@Code, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, otdutytype, conn);
        sql = $@"SELECT * FROM {schema}.Otdutytype WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<OtdutytypeModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }

    public async Task<OtdutytypeModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name from {schema}.Otdutytype where Id = @Id";
        var data = await _sql.FetchData<OtdutytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<OtdutytypeModel?>?> _02s(string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name from {schema}.Otdutytype ";
        var data = await _sql.FetchData<OtdutytypeModel?, dynamic>(sql, new { }, conn);
        return data;
    }

    public async Task<OtdutytypeModel?> _03(int id, OtdutytypeModel otdutytype, string schema, string conn)
    {
        string sql = $@"Update {schema}.Otdutytype set Code = @Code, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, otdutytype, conn);

        sql = $@" select  * from {schema}.Otdutytype x where x.Id = @Id ;";
        var data = await _sql.FetchData<OtdutytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OtdutytypeModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Otdutytype where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Otdutytype x where x.Id = @Id ;";
        var data = await _sql.FetchData<OtdutytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOtdutytypeDataAccess
{
    Task<OtdutytypeModel?> 			_01(OtdutytypeModel otdutytype, string schema, string conn);
    Task<OtdutytypeModel?> 			_02(int id, string schema, string conn);
    Task<List<OtdutytypeModel?>?> 	_02s(string schema, string conn);
    Task<OtdutytypeModel?> 			_03(int id, OtdutytypeModel otdutytype, string schema, string conn);
    Task<OtdutytypeModel?> 			_04(int id, string schema, string conn);
}
