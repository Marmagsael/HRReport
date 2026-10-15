using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._11_AMS;

namespace HRApiLibrary.DataAccess._11_AMS;

public class AttscheddailyDataAccess : IAttscheddailyDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttscheddailyDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttscheddailyModel?> _01(AttscheddailyModel attscheddaily, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Attscheddaily (Name, DutyType, PIn, Duration, POut, Nd) values (@Name, @DutyType, @PIn, @Duration, @POut, @Nd)";
        await _sql.ExecuteCmd<dynamic>(sql, attscheddaily, conn);
        sql = $@"SELECT * FROM {schema}.Attscheddaily WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<AttscheddailyModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<AttscheddailyModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Name, DutyType, PIn, Duration, POut, Nd from {schema}.Attscheddaily where Id = @Id";
        var data = await _sql.FetchData<AttscheddailyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<AttscheddailyModel?>?> _02s(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Attscheddaily order by Name";
        var data = await _sql.FetchData<AttscheddailyModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<AttscheddailyModel?> _03(int id, AttscheddailyModel attscheddaily, string schema, string conn)
    {
        string sql = $@"Update {schema}.Attscheddaily set Name = @Name, DutyType = @DutyType, PIn = @PIn, Duration = @Duration, POut = @POut, Nd = @Nd where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, attscheddaily, conn);

        sql = $@" select  * from {schema}.Attscheddaily x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttscheddailyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<AttscheddailyModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Attscheddaily where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Attscheddaily x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttscheddailyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}


public interface IAttscheddailyDataAccess
{
    Task<AttscheddailyModel?> _01(AttscheddailyModel attscheddaily, string schema, string conn);
    Task<AttscheddailyModel?> _02(int id, string schema, string conn);
    Task<List<AttscheddailyModel?>?> _02s(string schema, string conn);
    Task<AttscheddailyModel?> _03(int id, AttscheddailyModel attscheddaily, string schema, string conn);
    Task<AttscheddailyModel?> _04(int id, string schema, string conn);
}
