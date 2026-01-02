using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._20_Pay;

public class RsectionDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RsectionDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RsectionModel?> _01(RsectionModel rsection, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rsection (Departmentid, SName, Name, Level) values (@Departmentid, @SName, @Name, @Level)";
        await _sql.ExecuteCmd<dynamic>(sql, rsection, conn);

        sql = $@"SELECT * FROM {schema}.Rsection WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<RsectionModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<RsectionModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Departmentid, SName, Name, Level from {schema}.Rsection where Id = @Id";
        var data = await _sql.FetchData<RsectionModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<RsectionModel?> _03(int id, RsectionModel rsection, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rsection set Departmentid = @Departmentid, SName = @SName, Name = @Name, Level = @Level where Id = @Id;";
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
