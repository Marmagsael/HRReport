using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class OtreqdtlDataAccess : IOtreqdtlDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public OtreqdtlDataAccess(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task _01(OtreqdtlModel otreqdtl, string schema, string conn)
    {
        var totHrs = otreqdtl.TotHrs;
        string sql = $@"Delete from  {schema}.Otreqdtl where EmpmasId = @EmpmasId and PunchIn = @PunchIn and OtReqHdrId = @OtReqHdrId; ";    

        if (totHrs > 0)
        {
            sql = $@"Insert into {schema}.Otreqdtl 
                            (OtReqHdrId,  EmpmasId,  PunchIn,  TotHrs,  DutyTypeId,  DayTypeId) values 
                            (@OtReqHdrId, @EmpmasId, @PunchIn, @TotHrs, @DutyTypeId, @DayTypeId) 
                        ON DUPLICATE KEY UPDATE TotHrs = @TotHrs, DutyTypeId = @DutyTypeId; ";    
        }

        await _sql.ExecuteCmd<dynamic>(sql, otreqdtl, conn);

    }

    public async Task<List<OtreqdtlModel?>?> _02ByOtReqHdrId(int otReqHdrId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Otreqdtl where OtReqHdrId = @OtReqHdrId ;";
        var data = await _sql.FetchData<OtreqdtlModel?, dynamic>(sql, new { OtReqHdrId = otReqHdrId }, conn);
        return data;
    }
    
    public async Task<List<OtreqdtlModel?>?> _02ByEmpmasId_ByPunchInRange(int empmasId, DateTime dStart, DateTime dEnd, string schema, string conn)
    {
        var start = dStart.Date; // 00:00:00
        var end = dEnd.Date.AddDays(1).AddTicks(-1); // 23:59:59.9999999

        string sql = $@"select  * from {schema}.Otreqdtl where EmpmasId = @EmpmasId AND PunchIn >= @Start AND PunchIn <= @End;";
        var data = await _sql.FetchData<OtreqdtlModel?, dynamic>(sql, new { EmpmasId = empmasId, Start = start, End = end }, conn);
        return data;
    }

    public async Task<List<OtreqdtlModel?>?> _02RangeByPunchIn(DateTime dStart, DateTime dEnd, string schema, string conn)
    {
        var start = dStart.Date; // 00:00:00
        var end = dEnd.Date.AddDays(1).AddTicks(-1); // 23:59:59.9999999

        string sql = $@"select  * from {schema}.Otreqdtl WHERE PunchIn >= @Start AND PunchIn <= @End;";
        var data = await _sql.FetchData<OtreqdtlModel?, dynamic>(sql, new { Start = start, End = end }, conn);
        return data;
    }

    public async Task<List<OtreqdtlModel?>?> _02ByEmpmasId_RangeByPunchIn(int empmasId, DateTime dStart, DateTime dEnd, string schema, string conn)
    {
        var start = dStart.Date; // 00:00:00
        var end = dEnd.Date.AddDays(1).AddTicks(-1); // 23:59:59.9999999

        string sql = $@"select  * from {schema}.Otreqdtl WHERE EmpmasId = @EmpmasId AND PunchIn >= @Start AND PunchIn <= @End;";
        var data = await _sql.FetchData<OtreqdtlModel?, dynamic>(sql, new { EmpmasId = empmasId, Start = start, End = end }, conn);
        return data;
    }


    public async Task _03(int empmasId, DateTime punchIn, string schema, string conn)
    {
        string sql = $@"Update {schema}.Otreqdtl set 
                            TotHrs          = @TotHrs, 
                            DutyTypeId      = @DutyTypeId
                        where EmpmasId = @EmpmasId and PunchIn = @PunchIn;";
        await _sql.ExecuteCmd<dynamic>(sql, new { EmpmasId = empmasId, PunchIn = punchIn }, conn);

    }

    public async Task _04ByEmpmasId_ByPunchIn(int empmasId, DateTime punchIn, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Otreqdtl where EmpmasId = @EmpmasId and PunchIn = @PunchIn;";
        await _sql.ExecuteCmd<dynamic>(sql, new { EmpmasId = empmasId, PunchIn = punchIn }, conn);
    }
    public async Task _04ByOtReqHdrId(int otReqHdrId, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Otreqdtl where OtReqHdrId = @OtReqHdrId;";
        await _sql.ExecuteCmd<dynamic>(sql, new { OtReqHdrId = otReqHdrId }, conn);
    }

}


public interface IOtreqdtlDataAccess
{
    Task _01(OtreqdtlModel otreqdtl, string schema, string conn);
    Task<List<OtreqdtlModel?>?>     _02ByEmpmasId_RangeByPunchIn(int empmasId, DateTime dStart, DateTime dEnd, string schema, string conn);
    Task<List<OtreqdtlModel?>?>     _02ByOtReqHdrId(int otReqHdrId, string schema, string conn);
    Task<List<OtreqdtlModel?>?>     _02ByEmpmasId_ByPunchInRange(int empmasId, DateTime dStart, DateTime dEnd, string schema, string conn);
    Task<List<OtreqdtlModel?>?>     _02RangeByPunchIn(DateTime dStart, DateTime dEnd, string schema, string conn);
    Task                            _03(int empmasId, DateTime punchIn, string schema, string conn);
    Task                            _04ByEmpmasId_ByPunchIn(int empmasId, DateTime punchIn, string schema, string conn);
    Task                            _04ByOtReqHdrId(int otReqHdrId, string schema, string conn);
    
}
