using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class EmploytypeDataAccess : IEmploytypeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EmploytypeDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<EmploytypeModel?> _01(EmploytypeModel employtype, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Employtype (Name) values (@Name); 
                        SELECT * FROM {schema}.Employtype WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmploytypeModel?, dynamic>(sql, employtype, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmploytypeModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Name from {schema}.Employtype where Id = @Id";
        var data = await _sql.FetchData<EmploytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<EmploytypeModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  Id, Name from {schema}.Employtype ";
        var data = await _sql.FetchData<EmploytypeModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<EmploytypeModel?> _03(int id, EmploytypeModel employtype, string schema, string conn)
    {
        string sql = $@"Update {schema}.Employtype set Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, employtype, conn);

        sql = $@" select  * from {schema}.Employtype x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmploytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmploytypeModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Employtype where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Employtype x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmploytypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}