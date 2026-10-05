using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._11_AMS;

namespace HRApiLibrary.DataAccess._11_AMS;

public class AttschedweeklyhdrDataAccess : IAttschedweeklyhdrDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public AttschedweeklyhdrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // Insert
    public async Task<AttschedweeklyhdrModel?> _01(AttschedweeklyhdrModel hdr, string schema, string conn)
    {
        string sql = $@"INSERT INTO {schema}.AttSchedWeeklyHdr (Code, Description) 
                        VALUES (@Code, @Description)";
        await _sql.ExecuteCmd<dynamic>(sql, hdr, conn);

        sql = $@"SELECT * FROM {schema}.AttSchedWeeklyHdr WHERE Id = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<AttschedweeklyhdrModel?, dynamic>(sql, new { }, conn);
        return res?.FirstOrDefault();
    }

    // Select single
    public async Task<AttschedweeklyhdrModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"SELECT Id, Code, Description 
                        FROM {schema}.AttSchedWeeklyHdr 
                        WHERE Id = @Id";
        var data = await _sql.FetchData<AttschedweeklyhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // Select all
    public async Task<List<AttschedweeklyhdrModel?>?> _02s(string schema, string conn)
    {
        string sql = $@"SELECT Id, Code, Description 
                        FROM {schema}.AttSchedWeeklyHdr 
                        ORDER BY Code";
        var data = await _sql.FetchData<AttschedweeklyhdrModel?, dynamic>(sql, new { }, conn);
        return data;
    }

    // Update
    public async Task<AttschedweeklyhdrModel?> _03(int id, AttschedweeklyhdrModel hdr, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.AttSchedWeeklyHdr 
                        SET Code = @Code, Description = @Description 
                        WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, hdr, conn);

        sql = $@"SELECT * FROM {schema}.AttSchedWeeklyHdr WHERE Id = @Id";
        var data = await _sql.FetchData<AttschedweeklyhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // Delete
    public async Task<AttschedweeklyhdrModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"DELETE FROM {schema}.AttSchedWeeklyHdr WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@"SELECT * FROM {schema}.AttSchedWeeklyHdr WHERE Id = @Id";
        var data = await _sql.FetchData<AttschedweeklyhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IAttschedweeklyhdrDataAccess
{
    Task<AttschedweeklyhdrModel?>              _01(AttschedweeklyhdrModel hdr, string schema, string conn);
    Task<AttschedweeklyhdrModel?>              _02(int id, string schema, string conn);
    Task<List<AttschedweeklyhdrModel?>?>       _02s(string schema, string conn);
    Task<AttschedweeklyhdrModel?>              _03(int id, AttschedweeklyhdrModel hdr, string schema, string conn);
    Task<AttschedweeklyhdrModel?>              _04(int id, string schema, string conn);
}
