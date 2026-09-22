using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;
using System.Xml.Linq;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OPositionDataAccess : IOPositionDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OPositionDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<OPositionModel?> _01(OPositionModel position, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Position (CODE, NAME, ISGUARD, sort) values (@Code, @Name, @IsGuard, @Sort)";
        await _sql.ExecuteCmd<dynamic>(sql, position, conn);

        sql = $@"SELECT * FROM {schema}.Position WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@Code))";

        var data = await _sql.FetchData<OPositionModel?, dynamic>(sql, new { position.Code}, conn);

        return data?.FirstOrDefault();
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

    public async Task<List<OPositionModel?>?> _02ByCodeOrByName(string code, string name, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.Position  WHERE TRIM(UPPER(CODE)) = @Code  OR TRIM(UPPER(Name)) = @Name ORDER BY Name";

        var data = await _sql.FetchData<OPositionModel?, dynamic>(sql, new { Code = code, Name = name}, conn);
        return data;
    }

    public async Task<OPositionModel?> _03(string code, OPositionModel position, string schema, string conn)
    {
        var parameters = new
        {
            oldCode = code,
            position.Code,
            position.Name,
            position.Isguard,
            position.Sort
        };

        string sql = $@"UPDATE {schema}.Position SET CODE = @Code, NAME = @Name, ISGUARD = @Isguard, sort = @Sort   WHERE TRIM(UPPER(CODE))  = @oldCode;";

        await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);

        sql = $@" SELECT * FROM {schema}.Position x  WHERE TRIM(UPPER(x.Code)) = TRIM(UPPER(@Code);";

        var data = await _sql.FetchData<OPositionModel?, dynamic>( sql, new { Code = position.Code }, conn);
        return data?.FirstOrDefault();

    }

    public async Task<OPositionModel?> _04(string code, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Position WHERE TRIM(UPPER(x.Code)) = TRIM(UPPER(@Code);";
        await _sql.ExecuteCmd<dynamic>(sql, new { Code = code }, conn);

        sql = $@" select  * from {schema}.Position x WHERE TRIM(UPPER(x.Code)) = TRIM(UPPER(@Code);";
        var data = await _sql.FetchData<OPositionModel?, dynamic>(sql, new { Code = code }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOPositionDataAccess
{
    Task<OPositionModel?> _01(OPositionModel position, string schema, string conn);
    Task<OPositionModel?> _02(int id, string schema, string conn);
    Task<List<OPositionModel?>?> _02ByCodeOrByName(string code, string name, string schema, string conn);
    Task<List<OPositionModel?>?> _02(string schema, string conn);
    Task<OPositionModel?> _03(string code, OPositionModel position, string schema, string conn);
    Task<OPositionModel?> _04(string code, string schema, string conn);
}