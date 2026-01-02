using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class DesignationDataAccess : IDesignationDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public DesignationDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<DesignationModel?> _01(DesignationModel designation, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Designation (CODE, NAME, sort) values (@CODE, @NAME, @sort)";
        await _sql.ExecuteCmd<dynamic>(sql, designation, conn);

        sql = $@"SELECT * FROM {schema}.Designation WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<DesignationModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<DesignationModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name, Sort from {schema}.Designation where Id = @Id";
        var data = await _sql.FetchData<DesignationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<DesignationModel?>?> _02( string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name, Sort from {schema}.Designation ";
        var data = await _sql.FetchData<DesignationModel?, dynamic>(sql, new {  }, conn);
        return data;
    }



    public async Task<DesignationModel?> _03(int id, DesignationModel designation, string schema, string conn)
    {
        string sql = $@"Update {schema}.Designation set CODE = @CODE, NAME = @NAME, sort = @sort where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, designation, conn);

        sql = $@" select  * from {schema}.Designation x where x.Id = @Id ;";
        var data = await _sql.FetchData<DesignationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<DesignationModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Designation where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Designation x where x.Id = @Id ;";
        var data = await _sql.FetchData<DesignationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
