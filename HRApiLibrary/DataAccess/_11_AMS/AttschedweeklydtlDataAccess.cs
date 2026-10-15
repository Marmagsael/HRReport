using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._11_AMS;

namespace HRApiLibrary.DataAccess._11_AMS;

public class AttschedweeklydtlDataAccess : IAttschedweeklydtlDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public AttschedweeklydtlDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // Insert
    public async Task<AttschedweeklydtlModel?> _01(AttschedweeklydtlModel dtl, string schema, string conn)
    {
        string sql = $@"INSERT INTO {schema}.AttSchedWeeklyDtl 
                        (AttSchedWeeklyHdrId,
                         D1_DutyType, D1_In, D1_HrsLength, D1_Out,
                         D2_DutyType, D2_In, D2_HrsLength, D2_Out,
                         D3_DutyType, D3_In, D3_HrsLength, D3_Out,
                         D4_DutyType, D4_In, D4_HrsLength, D4_Out,
                         D5_DutyType, D5_In, D5_HrsLength, D5_Out,
                         D6_DutyType, D6_In, D6_HrsLength, D6_Out,
                         D7_DutyType, D7_In, D7_HrsLength, D7_Out)
                        VALUES
                        (@AttschedweeklyhdrId,
                         @D1_dutytype, @D1_in, @D1_hrslength, @D1_out,
                         @D2_dutytype, @D2_in, @D2_hrslength, @D2_out,
                         @D3_dutytype, @D3_in, @D3_hrslength, @D3_out,
                         @D4_dutytype, @D4_in, @D4_hrslength, @D4_out,
                         @D5_dutytype, @D5_in, @D5_hrslength, @D5_out,
                         @D6_dutytype, @D6_in, @D6_hrslength, @D6_out,
                         @D7_dutytype, @D7_in, @D7_hrslength, @D7_out)";
        await _sql.ExecuteCmd<dynamic>(sql, dtl, conn);

        sql = $@"SELECT * FROM {schema}.AttSchedWeeklyDtl WHERE Id = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<AttschedweeklydtlModel?, dynamic>(sql, new { }, conn);
        return res?.FirstOrDefault();
    }

    // Select single by Id
    public async Task<AttschedweeklydtlModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.AttSchedWeeklyDtl WHERE Id = @Id";
        var data = await _sql.FetchData<AttschedweeklydtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // Select by HdrId
    public async Task<AttschedweeklydtlModel?> _02h(int hdrId, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.AttSchedWeeklyDtl 
                        WHERE AttSchedWeeklyHdrId = @HdrId";
        var data = await _sql.FetchData<AttschedweeklydtlModel?, dynamic>(sql, new { HdrId = hdrId }, conn);
        return data?.FirstOrDefault();
    }

    // Update
    public async Task<AttschedweeklydtlModel?> _03(int id, AttschedweeklydtlModel dtl, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.AttSchedWeeklyDtl SET
                         D1_DutyType = @D1_dutytype, D1_In = @D1_in, D1_HrsLength = @D1_hrslength, D1_Out = @D1_out,
                         D2_DutyType = @D2_dutytype, D2_In = @D2_in, D2_HrsLength = @D2_hrslength, D2_Out = @D2_out,
                         D3_DutyType = @D3_dutytype, D3_In = @D3_in, D3_HrsLength = @D3_hrslength, D3_Out = @D3_out,
                         D4_DutyType = @D4_dutytype, D4_In = @D4_in, D4_HrsLength = @D4_hrslength, D4_Out = @D4_out,
                         D5_DutyType = @D5_dutytype, D5_In = @D5_in, D5_HrsLength = @D5_hrslength, D5_Out = @D5_out,
                         D6_DutyType = @D6_dutytype, D6_In = @D6_in, D6_HrsLength = @D6_hrslength, D6_Out = @D6_out,
                         D7_DutyType = @D7_dutytype, D7_In = @D7_in, D7_HrsLength = @D7_hrslength, D7_Out = @D7_out
                        WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, dtl, conn);

        sql = $@"SELECT * FROM {schema}.AttSchedWeeklyDtl WHERE Id = @Id";
        var data = await _sql.FetchData<AttschedweeklydtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // Delete by Id
    public async Task<AttschedweeklydtlModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"DELETE FROM {schema}.AttSchedWeeklyDtl WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@"SELECT * FROM {schema}.AttSchedWeeklyDtl WHERE Id = @Id";
        var data = await _sql.FetchData<AttschedweeklydtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // Delete by HdrId (cascade delete)
    public async Task _04h(int hdrId, string schema, string conn)
    {
        string sql = $@"DELETE FROM {schema}.AttSchedWeeklyDtl WHERE AttSchedWeeklyHdrId = @HdrId";
        await _sql.ExecuteCmd<dynamic>(sql, new { HdrId = hdrId }, conn);
    }
}

public interface IAttschedweeklydtlDataAccess
{
    Task<AttschedweeklydtlModel?>   _01(AttschedweeklydtlModel dtl, string schema, string conn);
    Task<AttschedweeklydtlModel?>   _02(int id, string schema, string conn);
    Task<AttschedweeklydtlModel?>   _02h(int hdrId, string schema, string conn);
    Task<AttschedweeklydtlModel?>   _03(int id, AttschedweeklydtlModel dtl, string schema, string conn);
    Task<AttschedweeklydtlModel?>   _04(int id, string schema, string conn);
    Task                            _04h(int hdrId, string schema, string conn);
}
