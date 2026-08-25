using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay.OPay;


namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class ODomainaccessDataAccess : IODomainaccessDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public ODomainaccessDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<ODomainaccessModel?> _01(ODomainaccessModel domainaccess, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Domainaccess (iduserAccess, username, idsysmenu, module) values (@iduserAccess, @username, @idsysmenu, @module)";
        await _sql.ExecuteCmd<dynamic>(sql, domainaccess, conn);

        sql = $@"SELECT * FROM {schema}.Domainaccess WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<ODomainaccessModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<ODomainaccessModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  iduserAccess, username, idsysmenu, module from {schema}.Domainaccess where Id = @Id";
        var data = await _sql.FetchData<ODomainaccessModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<ODomainaccessModel?> _03(int id, ODomainaccessModel domainaccess, string schema, string conn)
    {
        string sql = $@"Update {schema}.Domainaccess set iduserAccess = @iduserAccess, username = @username, idsysmenu = @idsysmenu, module = @module where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, domainaccess, conn);

        sql = $@" select  * from {schema}.Domainaccess x where x.Id = @Id ;";
        var data = await _sql.FetchData<ODomainaccessModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<ODomainaccessModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Domainaccess where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Domainaccess x where x.Id = @Id ;";
        var data = await _sql.FetchData<ODomainaccessModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

}
    public interface IODomainaccessDataAccess
    {
        Task<ODomainaccessModel?> _01(ODomainaccessModel domainaccess, string schema, string conn);
        Task<ODomainaccessModel?> _02(int id, string schema, string conn);
        Task<ODomainaccessModel?> _03(int id, ODomainaccessModel domainaccess, string schema, string conn);
        Task<ODomainaccessModel?> _04(int id, string schema, string conn);
    }