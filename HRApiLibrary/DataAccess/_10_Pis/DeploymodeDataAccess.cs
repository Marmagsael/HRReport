using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class DeploymodeDataAccess : IDeploymodeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public DeploymodeDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<DeploymodeModel?> _01(DeploymodeModel deploymode, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Deploymode (Name) values (@Name); 
                        SELECT * FROM {schema}.Deploymode WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<DeploymodeModel?, dynamic>(sql, deploymode, conn);
        return res.FirstOrDefault();
    }


    public async Task<DeploymodeModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Name from {schema}.Deploymode where Id = @Id";
        var data = await _sql.FetchData<DeploymodeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<DeploymodeModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Deploymode ";
        var data = await _sql.FetchData<DeploymodeModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<DeploymodeModel?> _03(int id, DeploymodeModel deploymode, string schema, string conn)
    {
        string sql = $@"Update {schema}.Deploymode set Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, deploymode, conn);

        sql = $@" select  * from {schema}.Deploymode x where x.Id = @Id ;";
        var data = await _sql.FetchData<DeploymodeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<DeploymodeModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Deploymode where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Deploymode x where x.Id = @Id ;";
        var data = await _sql.FetchData<DeploymodeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
