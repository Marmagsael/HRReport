using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;


namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OPisDomainaccessDataAccess : IOPisDomainaccessDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OPisDomainaccessDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<OPisDomainaccessModel?> _01(OPisDomainaccessModel domainaccess, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Domainaccess ( username, idsysmenu, module) values ( @username, @idsysmenu, @module)";
        await _sql.ExecuteCmd<dynamic>(sql, domainaccess, conn);

        sql = $@"SELECT * FROM {schema}.Domainaccess WHERE IduserAccess = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<OPisDomainaccessModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<OPisDomainaccessModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  iduserAccess, username, idsysmenu, module from {schema}.Domainaccess where Id = @Id";
        var data = await _sql.FetchData<OPisDomainaccessModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<List<OPisDomainaccessModel?>?> _02ByUserName_Module(string username, string module, string schema, string conn)
    {
        string sql = $@"select  iduserAccess, username, idsysmenu, module from {schema}.Domainaccess where LOWER(TRIM(Username)) = @Username AND LOWER(TRIM(Module)) = @Module";
        var data = await _sql.FetchData<OPisDomainaccessModel?, dynamic>(sql, new { UserName = username, Module = module }, conn);
        return data;
    }

    public async Task<OPisDomainaccessModel?> _03(int id, OPisDomainaccessModel domainaccess, string schema, string conn)
    {
        string sql = $@"Update {schema}.Domainaccess set iduserAccess = @iduserAccess, username = @username, idsysmenu = @idsysmenu, module = @module where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, domainaccess, conn);

        sql = $@" select  * from {schema}.Domainaccess x where x.Id = @Id ;";
        var data = await _sql.FetchData<OPisDomainaccessModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OPisDomainaccessModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Domainaccess where IduserAccess = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Domainaccess x where x.iduserAccess = @Id ;";
        var data = await _sql.FetchData<OPisDomainaccessModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<OPisDomainaccessModel?> _04ByUserName_Idsysmenu_Module(OPisDomainaccessModel domainaccess, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Domainaccess where  Username = @username AND Idsysmenu = @idsysmenu AND Module = @module;";
        await _sql.ExecuteCmd<dynamic>(sql, domainaccess, conn);

        sql = $@" select  * from {schema}.Domainaccess x where x.iduserAccess = @Id ;";
        var data = await _sql.FetchData<OPisDomainaccessModel?, dynamic>(sql, new { Id = domainaccess.IduserAccess }, conn);
        return data?.FirstOrDefault();
    }
}
    public interface IOPisDomainaccessDataAccess
{
        Task<OPisDomainaccessModel?> _01(OPisDomainaccessModel domainaccess, string schema, string conn);
        Task<OPisDomainaccessModel?> _02(int id, string schema, string conn);
        Task<List<OPisDomainaccessModel?>?> _02ByUserName_Module(string username, string module, string schema, string conn);
        Task<OPisDomainaccessModel?> _03(int id, OPisDomainaccessModel domainaccess, string schema, string conn);
        Task<OPisDomainaccessModel?> _04(int id, string schema, string conn);
        Task<OPisDomainaccessModel?> _04ByUserName_Idsysmenu_Module(OPisDomainaccessModel domainaccess, string schema, string conn);
    }