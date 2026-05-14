using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class RsectionDataAccess : IRsectionDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RsectionDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RsectionModel?> _01(RsectionModel rsection, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rsection 
                            (Departmentid,  SName,  Name ) values 
                            (@Departmentid, @SName, @Name ); 
                        SELECT * FROM {schema}.Rsection WHERE ID = (SELECT @@IDENTITY) ";
        var res = await _sql.FetchData<RsectionModel?, dynamic>(sql, rsection, conn);
        return res.FirstOrDefault();
    }


    public async Task<RsectionModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Rsection where Id = @Id";
        var data = await _sql.FetchData<RsectionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<RsectionModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Rsection order by Name";
        var data = await _sql.FetchData<RsectionModel?, dynamic>(sql, new { }, conn);
        return data;
    }

    public async Task<List<RsectionModel?>?> _02ByDepartmentId(int departmentId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Rsection where DepartmentId = @DepartmentId order by Name";
        var data = await _sql.FetchData<RsectionModel?, dynamic>(sql, new { DepartmentId = departmentId }, conn);
        return data;
    }

    public async Task<RsectionModel?> _03(int id, RsectionModel rsection, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rsection set 
                            Departmentid    = @Departmentid, 
                            SName           = @SName, 
                            Name            = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, rsection, conn);

        sql = $@" select  * from {schema}.Rsection x where x.Id = @Id ;";
        var data = await _sql.FetchData<RsectionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<RsectionModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rsection where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Rsection x where x.Id = @Id ;";
        var data = await _sql.FetchData<RsectionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}