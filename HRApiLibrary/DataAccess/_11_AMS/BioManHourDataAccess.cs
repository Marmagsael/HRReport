using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._11_AMS;

public class BioManHourDataAccess : IBioManHourDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public BioManHourDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // Insert with ON DUPLICATE KEY UPDATE
    public async Task _01(BioManHourModel model, string schema, string conn)
    {
        string sql = $@"INSERT INTO {schema}.BioManHour
                        (BioManHourHdrId, EmpNumber,
                         RdDays, RdRd, RdTardiness, RdOt,
                         LhDays, LhRd, LhTardiness, LhOt,
                         ShDays, ShRd, ShTardiness, ShOt,
                         DhDays, DhRd, DhTardiness, DhOt)
                        VALUES
                        (@BioManHourHdrId, @EmpNumber,
                         @RdDays, @RdRd, @RdTardiness, @RdOt,
                         @LhDays, @LhRd, @LhTardiness, @LhOt,
                         @ShDays, @ShRd, @ShTardiness, @ShOt,
                         @DhDays, @DhRd, @DhTardiness, @DhOt)
                        ON DUPLICATE KEY UPDATE
                        RdDays        = VALUES(RdDays),
                        RdRd          = VALUES(RdRd),
                        RdTardiness   = VALUES(RdTardiness),
                        RdOt          = VALUES(RdOt),
                        LhDays        = VALUES(LhDays),
                        LhRd          = VALUES(LhRd),
                        LhTardiness   = VALUES(LhTardiness),
                        LhOt          = VALUES(LhOt),
                        ShDays        = VALUES(ShDays),
                        ShRd          = VALUES(ShRd),
                        ShTardiness   = VALUES(ShTardiness),
                        ShOt          = VALUES(ShOt),
                        DhDays        = VALUES(DhDays),
                        DhRd          = VALUES(DhRd),
                        DhTardiness   = VALUES(DhTardiness),
                        DhOt          = VALUES(DhOt)";
        await _sql.ExecuteCmd<dynamic>(sql, model, conn);
    }

    // Get by Id
    public async Task<BioManHourModel?> _02(long id, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.BioManHour WHERE Id = @Id";
        var data = await _sql.FetchData<BioManHourModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // Get list by BioManHourHdrId with EmpName
    public async Task<List<BioManHourModel?>?> _02sByHdr(long bioManHourHdrId, string schema, string opisdb, string conn)
    {
        string sql = $@"SELECT bm.*,
                               CONCAT(TRIM(COALESCE(e.EmpLastNm, '')), ', ',
                                      TRIM(COALESCE(e.EmpFirstNm, '')), ' ',
                                      TRIM(COALESCE(e.EmpMidNm, ''))) AS EmpName
                        FROM {schema}.BioManHour bm
                        LEFT JOIN {opisdb}.Empmas e ON e.EmpNumber = bm.EmpNumber
                        WHERE bm.BioManHourHdrId = @BioManHourHdrId
                        ORDER BY e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm";
        var data = await _sql.FetchData<BioManHourModel?, dynamic>(sql, new { BioManHourHdrId = bioManHourHdrId }, conn);
        return data;
    }

    // Update
    public async Task _03(BioManHourModel model, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.BioManHour SET
                        RdDays        = @RdDays,
                        RdRd          = @RdRd,
                        RdTardiness   = @RdTardiness,
                        RdOt          = @RdOt,
                        LhDays        = @LhDays,
                        LhRd          = @LhRd,
                        LhTardiness   = @LhTardiness,
                        LhOt          = @LhOt,
                        ShDays        = @ShDays,
                        ShRd          = @ShRd,
                        ShTardiness   = @ShTardiness,
                        ShOt          = @ShOt,
                        DhDays        = @DhDays,
                        DhRd          = @DhRd,
                        DhTardiness   = @DhTardiness,
                        DhOt          = @DhOt
                        WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, model, conn);
    }

    // Delete by Id
    public async Task _04(long id, string schema, string conn)
    {
        string sql = $@"DELETE FROM {schema}.BioManHour WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);
    }

    // Delete all by BioManHourHdrId
    public async Task _04ByHdr(long bioManHourHdrId, string schema, string conn)
    {
        string sql = $@"DELETE FROM {schema}.BioManHour WHERE BioManHourHdrId = @BioManHourHdrId";
        await _sql.ExecuteCmd<dynamic>(sql, new { BioManHourHdrId = bioManHourHdrId }, conn);
    }
}

public interface IBioManHourDataAccess
{
    Task _01(BioManHourModel model, string schema, string conn);
    Task<BioManHourModel?> _02(long id, string schema, string conn);
    Task<List<BioManHourModel?>?> _02sByHdr(long bioManHourHdrId, string schema, string opisdb, string conn);
    Task _03(BioManHourModel model, string schema, string conn);
    Task _04(long id, string schema, string conn);
    Task _04ByHdr(long bioManHourHdrId, string schema, string conn);
}