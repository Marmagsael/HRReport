using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._11_AMS;

public class BioManHourHdrDataAccess : IBioManHourHdrDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public BioManHourHdrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // Insert with ON DUPLICATE KEY UPDATE
    public async Task _01(BioManHourHdrModel model, string schema, string conn)
    {
        string sql = $@"INSERT INTO {schema}.BioManHourHdr
                        (PayrollGrpId, CoverageStart, CoverageEnd, Remarks)
                        VALUES
                        (@PayrollGrpId, @CoverageStart, @CoverageEnd, @Remarks)
                        ON DUPLICATE KEY UPDATE
                        CoverageEnd = VALUES(CoverageEnd),
                        Remarks     = VALUES(Remarks)";
        await _sql.ExecuteCmd<dynamic>(sql, model, conn);
    }

    // Get by Id
    public async Task<BioManHourHdrModel?> _02(long id, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.BioManHourHdr WHERE Id = @Id";
        var data = await _sql.FetchData<BioManHourHdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // Get list by PayrollGrpId
    public async Task<List<BioManHourHdrModel?>?> _02sByPayrollGrp(int payrollGrpId, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.BioManHourHdr
                        WHERE PayrollGrpId = @PayrollGrpId
                        ORDER BY CoverageStart DESC";
        var data = await _sql.FetchData<BioManHourHdrModel?, dynamic>(sql, new { PayrollGrpId = payrollGrpId }, conn);
        return data;
    }

    // Update
    public async Task _03(BioManHourHdrModel model, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.BioManHourHdr SET
                        CoverageEnd = @CoverageEnd,
                        Remarks     = @Remarks
                        WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, model, conn);
    }

    // Delete
    public async Task _04(long id, string schema, string conn)
    {
        string sql = $@"DELETE FROM {schema}.BioManHourHdr WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);
    }
}

public interface IBioManHourHdrDataAccess
{
    Task _01(BioManHourHdrModel model, string schema, string conn);
    Task<BioManHourHdrModel?> _02(long id, string schema, string conn);
    Task<List<BioManHourHdrModel?>?> _02sByPayrollGrp(int payrollGrpId, string schema, string conn);
    Task _03(BioManHourHdrModel model, string schema, string conn);
    Task _04(long id, string schema, string conn);
}
