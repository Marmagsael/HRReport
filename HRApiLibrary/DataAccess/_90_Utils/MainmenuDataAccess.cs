using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._90_Utils;
using System.Data;

namespace HRApiLibrary.DataAccess._90_Utils;

public class MainmenuDataAccess : IMainmenuDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;
    public MainmenuDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<MainmenuModel?>?> _02(string? schema, string? conn)
    {
        var sql = $@"select  Id, Type, IdParent, Indent, Icon, DispText, Action, Odr, IsCommercialUse, CoTypeCode from {schema}.Menu order by Odr, Id";
        var data = await _sql.FetchData<MainmenuModel?, dynamic>(sql, new { }, conn);
        return data;
    }

    // -- CoTypeCode 'ALL' = for every company; otherwise only for the companies of that type (UsersCompany.CoTypeCode) --
    public async Task<List<MainmenuModel?>?> _02(string? schema, string? coTypeCode, string? conn)
    {
        var sql = $@"select  Id, Type, IdParent, Indent, Icon, DispText, Action, Odr, IsCommercialUse, CoTypeCode from {schema}.Menu where CoTypeCode = 'ALL' or CoTypeCode = @CoTypeCode order by Odr, Id";
        var data = await _sql.FetchData<MainmenuModel?, dynamic>(sql, new { CoTypeCode = coTypeCode ?? "" }, conn);
        return data;
    }
}


public interface IMainmenuDataAccess
{
    Task<List<MainmenuModel?>?> _02(string? schema, string? conn);
    Task<List<MainmenuModel?>?> _02(string? schema, string? coTypeCode, string? conn);
}