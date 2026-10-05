using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._11_AMS;

public class BioDailyPunchesDataAccess : IBioDailyPunchesDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public BioDailyPunchesDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // Insert
    public async Task _01(BioDailyPunchesModel model, string schema, string conn)
    {
        string sql = $@"INSERT INTO {schema}.BioDailyPunches 
                    (EmpNumber, Date, PayrollGrpId, DutyType, DayType, InSchedule, OutSchedule,
                     PunchIn, PunchOut, DayWork, Overtime, DutyStatus, ND, Late, Undertime, Absent)
                    VALUES 
                    (@EmpNumber, @Date, @PayrollGrpId, @DutyType, @DayType, @InSchedule, @OutSchedule,
                     @PunchIn, @PunchOut, @DayWork, @Overtime, @DutyStatus, @ND, @Late, @Undertime, @Absent)
                    ON DUPLICATE KEY UPDATE
                    DutyType    = VALUES(DutyType),
                    DayType     = VALUES(DayType),
                    InSchedule  = VALUES(InSchedule),
                    OutSchedule = VALUES(OutSchedule),
                    PunchIn     = VALUES(PunchIn),
                    PunchOut    = VALUES(PunchOut),
                    DayWork     = VALUES(DayWork),
                    Overtime    = VALUES(Overtime),
                    DutyStatus  = VALUES(DutyStatus),
                    ND          = VALUES(ND),
                    Late        = VALUES(Late),
                    Undertime   = VALUES(Undertime),
                    Absent      = VALUES(Absent)";
        await _sql.ExecuteCmd<dynamic>(sql, model, conn);
    }

    // Get by EmpNumber + Date
    public async Task<BioDailyPunchesModel?> _02(string empNumber, DateOnly date, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.BioDailyPunches 
                        WHERE EmpNumber = @EmpNumber AND Date = @Date";
        var data = await _sql.FetchData<BioDailyPunchesModel?, dynamic>(sql, new { EmpNumber = empNumber, Date = date }, conn);
        return data?.FirstOrDefault();
    }

    // Get list by EmpNumber + date range
    public async Task<List<BioDailyPunchesModel?>?> _02sByEmpDateRange(string empNumber, DateOnly startDate, DateOnly endDate, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.BioDailyPunches 
                        WHERE EmpNumber = @EmpNumber 
                        AND Date >= @StartDate 
                        AND Date <= @EndDate
                        ORDER BY Date";
        var data = await _sql.FetchData<BioDailyPunchesModel?, dynamic>(sql, new { EmpNumber = empNumber, StartDate = startDate, EndDate = endDate }, conn);
        return data;
    }

    // Get list by PayrollGrpId + date range
    public async Task<List<BioDailyPunchesModel?>?> _02sByPayrollGrpDateRange(int payrollGrpId, DateOnly startDate, DateOnly endDate, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.BioDailyPunches 
                        WHERE PayrollGrpId = @PayrollGrpId
                        AND Date >= @StartDate 
                        AND Date <= @EndDate
                        ORDER BY EmpNumber, Date";
        var data = await _sql.FetchData<BioDailyPunchesModel?, dynamic>(sql, new { PayrollGrpId = payrollGrpId, StartDate = startDate, EndDate = endDate }, conn);
        return data;
    }

    // Get list by date range (all employees)
    public async Task<List<BioDailyPunchesModel?>?> _02sByDateRange(DateOnly startDate, DateOnly endDate, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.BioDailyPunches 
                        WHERE Date >= @StartDate 
                        AND Date <= @EndDate
                        ORDER BY EmpNumber, Date";
        var data = await _sql.FetchData<BioDailyPunchesModel?, dynamic>(sql, new { StartDate = startDate, EndDate = endDate }, conn);
        return data;
    }

    // Update
    public async Task<BioDailyPunchesModel?> _03(BioDailyPunchesModel model, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.BioDailyPunches SET
                        PayrollGrpId = @PayrollGrpId,
                        DutyType     = @DutyType,
                        DayType      = @DayType,
                        InSchedule   = @InSchedule,
                        OutSchedule  = @OutSchedule,
                        PunchIn      = @PunchIn,
                        PunchOut     = @PunchOut,
                        DayWork      = @DayWork,
                        Overtime     = @Overtime,
                        DutyStatus   = @DutyStatus,
                        ND           = @ND,
                        Late         = @Late,
                        Undertime    = @Undertime,
                        Absent       = @Absent
                        WHERE EmpNumber = @EmpNumber AND Date = @Date";
        await _sql.ExecuteCmd<dynamic>(sql, model, conn);

        sql = $@"SELECT * FROM {schema}.BioDailyPunches 
                 WHERE EmpNumber = @EmpNumber AND Date = @Date";
        var data = await _sql.FetchData<BioDailyPunchesModel?, dynamic>(sql, new { model.EmpNumber, model.Date }, conn);
        return data?.FirstOrDefault();
    }

    // Delete
    public async Task<BioDailyPunchesModel?> _04(string empNumber, DateOnly date, string schema, string conn)
    {
        string sql = $@"DELETE FROM {schema}.BioDailyPunches 
                        WHERE EmpNumber = @EmpNumber AND Date = @Date";
        await _sql.ExecuteCmd<dynamic>(sql, new { EmpNumber = empNumber, Date = date }, conn);

        sql = $@"SELECT * FROM {schema}.BioDailyPunches 
                 WHERE EmpNumber = @EmpNumber AND Date = @Date";
        var data = await _sql.FetchData<BioDailyPunchesModel?, dynamic>(sql, new { EmpNumber = empNumber, Date = date }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IBioDailyPunchesDataAccess
{
    Task                                _01(BioDailyPunchesModel model, string schema, string conn);
    Task<BioDailyPunchesModel?>         _02(string empNumber, DateOnly date, string schema, string conn);
    Task<List<BioDailyPunchesModel?>?> _02sByEmpDateRange(string empNumber, DateOnly startDate, DateOnly endDate, string schema, string conn);
    Task<List<BioDailyPunchesModel?>?> _02sByPayrollGrpDateRange(int payrollGrpId, DateOnly startDate, DateOnly endDate, string schema, string conn);
    Task<List<BioDailyPunchesModel?>?> _02sByDateRange(DateOnly startDate, DateOnly endDate, string schema, string conn);
    Task<BioDailyPunchesModel?>         _03(BioDailyPunchesModel model, string schema, string conn);
    Task<BioDailyPunchesModel?>         _04(string empNumber, DateOnly date, string schema, string conn);
}
