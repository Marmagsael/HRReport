using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class OtdaytypeDataAccess : IOtdaytypeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OtdaytypeDataAccess(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<OtdaytypeModel?> _01(OtdaytypeModel otdaytype, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Otdaytype (Code, Name) values (@Code, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, otdaytype, conn);

        sql = $@"SELECT * FROM {schema}.Otdaytype WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<OtdaytypeModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<OtdaytypeModel?> _02(int? id, string? schema, string? conn)
    {
        string? sql = $@"select  Id, Code, Name from {schema}.Otdaytype where Id = @Id";
        var data = await _sql.FetchData<OtdaytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<OtdaytypeModel?>?> _02s(string? schema, string? conn)
    {
        string? sql = $@"select  Id, Code, Name from {schema}.Otdaytype ";
        var data = await _sql.FetchData<OtdaytypeModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<OtdaytypeModel?> _03(int? id, OtdaytypeModel otdaytype, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Otdaytype set Code = @Code, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, otdaytype, conn);

        sql = $@" select  * from {schema}.Otdaytype x where x.Id = @Id ;";
        var data = await _sql.FetchData<OtdaytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OtdaytypeModel?> _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Otdaytype where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Otdaytype x where x.Id = @Id ;";
        var data = await _sql.FetchData<OtdaytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOtdaytypeDataAccess
{
    Task<OtdaytypeModel?>           _01(OtdaytypeModel otdaytype, string? schema, string? conn);
    Task<OtdaytypeModel?>           _02(int? id, string? schema, string? conn);
    Task<List<OtdaytypeModel?>?>    _02s(string? schema, string? conn);
    Task<OtdaytypeModel?>           _03(int? id, OtdaytypeModel otdaytype, string? schema, string? conn);
    Task<OtdaytypeModel?>           _04(int? id, string? schema, string? conn);
}
