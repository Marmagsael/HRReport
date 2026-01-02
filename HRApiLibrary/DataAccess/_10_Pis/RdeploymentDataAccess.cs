using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class RdeploymentDataAccess : IRdeploymentDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RdeploymentDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RdeploymentModel?> _01(RdeploymentModel Rdeployment, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rdeployment (SName, Name) values (@SName, @Name); 
                        SELECT * FROM {schema}.Rdeployment WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<RdeploymentModel?, dynamic>(sql, Rdeployment, conn);
        return res.FirstOrDefault();
    }


    public async Task<RdeploymentModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Rdeployment where Id = @Id";
        var data = await _sql.FetchData<RdeploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<RdeploymentModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Rdeployment order by Name";
        var data = await _sql.FetchData<RdeploymentModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<RdeploymentModel?> _03(int id, RdeploymentModel Rdeployment, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rdeployment set SName = @SName, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Rdeployment, conn);

        sql = $@" select  * from {schema}.Rdeployment x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdeploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<RdeploymentModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rdeployment where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Rdeployment x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdeploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}