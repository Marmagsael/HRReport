using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class EmploymenttypeDataAccess : IEmploymenttypeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EmploymenttypeDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<EmploymenttypeModel?> _01(EmploymenttypeModel employmenttype, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Employmenttype (Name, IsVisible, ShowDeploymentEnd, CanbeDeleted) values (@Name, @IsVisible, @ShowDeploymentEnd, @CanbeDeleted); 
                        SELECT * FROM {schema}.Employmenttype WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmploymenttypeModel?, dynamic>(sql, employmenttype, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmploymenttypeModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select Id, Name, IsVisible, ShowDeploymentEnd, CanbeDeleted from {schema}.Employmenttype where Id = @Id";
        var data = await _sql.FetchData<EmploymenttypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<EmploymenttypeModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select Id, Name, IsVisible, ShowDeploymentEnd, CanbeDeleted from {schema}.Employmenttype ";
        var data = await _sql.FetchData<EmploymenttypeModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<EmploymenttypeModel?> _03(int id, EmploymenttypeModel employmenttype, string schema, string conn)
    {
        string sql = $@"Update {schema}.Employmenttype set Name = @Name , IsVisible = @IsVisible, ShowDeploymentEnd = @ShowDeploymentEnd, CanbeDeleted = @CanbeDeleted where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, employmenttype, conn);

        sql = $@" select  * from {schema}.Employmenttype x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmploymenttypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmploymenttypeModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Employmenttype where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Employmenttype x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmploymenttypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}


