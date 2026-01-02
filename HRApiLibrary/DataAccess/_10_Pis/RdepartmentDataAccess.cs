using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class RdepartmentDataAccess : IRdepartmentDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RdepartmentDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RdepartmentModel?> _01(RdepartmentModel rdepartment, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rdepartment (SName, Name, SupervisorId) values (@SName, @Name, @SupervisorId); 
                        SELECT * FROM {schema}.Rdepartment WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<RdepartmentModel?, dynamic>(sql, rdepartment, conn);
        return res.FirstOrDefault();
    }


    public async Task<RdepartmentModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, SName, Name, SupervisorId from {schema}.Rdepartment where Id = @Id";
        var data = await _sql.FetchData<RdepartmentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<RdepartmentModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  Id, SName, Name, SupervisorId from {schema}.Rdepartment order by Name";
        var data = await _sql.FetchData<RdepartmentModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<RdepartmentModel?> _03(int id, RdepartmentModel rdepartment, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rdepartment set SName = @SName, Name = @Name, SupervisorId = @SupervisorId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, rdepartment, conn);

        sql = $@" select  * from {schema}.Rdepartment x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdepartmentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<RdepartmentModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rdepartment where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Rdepartment x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdepartmentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}