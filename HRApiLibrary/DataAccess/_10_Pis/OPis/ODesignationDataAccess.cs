using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class ODesignationModel
{
    public string? Code { get; set; }
    public string? Name { get; set; }
}

public class ODesignationDataAccess : IODesignationDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public ODesignationDataAccess(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<ODesignationModel>> _02(string? schema, string? conn)
    {
        var data = await _sql.FetchLegacy<ODesignationModel, dynamic>($"select trim(Code) Code, Name from {schema}.designation order by Name", new { }, conn);
        return data ?? new List<ODesignationModel>();
    }

    public async Task<bool> _02Exists(string? code, string? schema, string? conn)
    {
        var data = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {schema}.designation where trim(upper(Code)) = trim(upper(@Code))", new { Code = code }, conn);
        return data.FirstOrDefault() > 0;
    }

    public Task _01(ODesignationModel d, string? schema, string? conn) =>
        _sql.ExecuteLegacyCmd<dynamic>($"insert into {schema}.designation (Code, Name) values (trim(@Code), trim(@Name))", d, conn);

    public Task _03(ODesignationModel d, string? schema, string? conn) =>
        _sql.ExecuteLegacyCmd<dynamic>($"update {schema}.designation set Name = trim(@Name) where trim(upper(Code)) = trim(upper(@Code))", d, conn);

    public Task _04(string? code, string? schema, string? conn) =>
        _sql.ExecuteLegacyCmd<dynamic>($"delete from {schema}.designation where trim(upper(Code)) = trim(upper(@Code))", new { Code = code }, conn);
}

public interface IODesignationDataAccess
{
    Task<List<ODesignationModel>> _02(string? schema, string? conn);
    Task<bool> _02Exists(string? code, string? schema, string? conn);
    Task _01(ODesignationModel d, string? schema, string? conn);
    Task _03(ODesignationModel d, string? schema, string? conn);
    Task _04(string? code, string? schema, string? conn);
}
