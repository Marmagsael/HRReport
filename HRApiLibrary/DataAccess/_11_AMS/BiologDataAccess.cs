using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._11_AMS;

public class BiologDataAccess : IBiologDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public BiologDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // Single insert
    public async Task<BiologModel?> _01(BiologModel biolog, string schema, string conn)
    {
        string sql = $@"INSERT IGNORE INTO {schema}.Biolog 
                        (DeviceNo, BiometricEmpNo, LogDatetime, PunchDate, VerifyMode, AttState, WorkCode, ImportBatch) 
                        VALUES 
                        (@DeviceNo, @BiometricEmpNo, @LogDatetime, @PunchDate, @VerifyMode, @AttState, @WorkCode, @ImportBatch)";
        await _sql.ExecuteCmd<dynamic>(sql, biolog, conn);

        sql = $@"SELECT * FROM {schema}.Biolog WHERE Id = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<BiologModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }

    // Batch insert
    public async Task _01Batch(List<BiologModel> biologs, string schema, string conn)
    {
        string sql = $@"INSERT IGNORE INTO {schema}.Biolog 
                        (DeviceNo, BiometricEmpNo, LogDatetime, PunchDate, VerifyMode, AttState, WorkCode, ImportBatch) 
                        VALUES 
                        (@DeviceNo, @BiometricEmpNo, @LogDatetime, @PunchDate, @VerifyMode, @AttState, @WorkCode, @ImportBatch)";
        foreach (var biolog in biologs)
            await _sql.ExecuteCmd<dynamic>(sql, biolog, conn);
    }

    // Get by Id
    public async Task<BiologModel?> _02(long id, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.Biolog WHERE Id = @Id";
        var data = await _sql.FetchData<BiologModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // Get by BiometricEmpNo + date range
    public async Task<List<BiologModel?>?> _02sByEmpDate(string biometricEmpNo, DateTime startDate, DateTime endDate, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.Biolog 
                        WHERE BiometricEmpNo = @BiometricEmpNo 
                        AND LogDatetime >= @StartDate 
                        AND LogDatetime < @EndDate";
        var data = await _sql.FetchData<BiologModel?, dynamic>(sql, new
        {
            BiometricEmpNo = biometricEmpNo,
            StartDate = startDate.Date,
            EndDate = endDate.Date.AddDays(1)
        }, conn);
        return data;
    }

    // Get unprocessed records
    public async Task<List<BiologModel?>?> _02sPending(string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.Biolog WHERE IsProcessed = 0 ORDER BY LogDatetime";
        var data = await _sql.FetchData<BiologModel?, dynamic>(sql, new { }, conn);
        return data;
    }

    // Update IsProcessed flag
    public async Task<BiologModel?> _03(long id, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Biolog SET IsProcessed = 1 WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@"SELECT * FROM {schema}.Biolog WHERE Id = @Id";
        var data = await _sql.FetchData<BiologModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // Delete
    public async Task<BiologModel?> _04(long id, string schema, string conn)
    {
        string sql = $@"DELETE FROM {schema}.Biolog WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@"SELECT * FROM {schema}.Biolog WHERE Id = @Id";
        var data = await _sql.FetchData<BiologModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IBiologDataAccess
{
    Task<BiologModel?> _01(BiologModel biolog, string schema, string conn);
    Task _01Batch(List<BiologModel> biologs, string schema, string conn);
    Task<BiologModel?> _02(long id, string schema, string conn);
    Task<List<BiologModel?>?> _02sByEmpDate(string biometricEmpNo, DateTime startDate, DateTime endDate, string schema, string conn);
    Task<List<BiologModel?>?> _02sPending(string schema, string conn);
    Task<BiologModel?> _03(long id, string schema, string conn);
    Task<BiologModel?> _04(long id, string schema, string conn);
}