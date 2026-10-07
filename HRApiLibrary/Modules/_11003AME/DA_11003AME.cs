using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.Modules._11003AME;

public class DA_11003AME : IDA_11003AME
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public DA_11003AME(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<M11003AME_Empmas>?> _02AME_EmplistPerPayrollgrp(int payrollgrpId, string pisdb, string opisdb, string conn)
    {
        string sql = $@"SELECT COALESCE(e1.systemid, 0) AS Systemid, COALESCE(e1.systemid, 0) AS Empmasid,
                            d.Empnumber,
                            d.PayrollgrpId,
                            d.IdDeployment,
                            CONCAT(TRIM(COALESCE(e.EmpLastNm, '')), ', ',
                            TRIM(COALESCE(e.EmpFirstNm, '')), ' ',
                            TRIM(COALESCE(e.EmpMidNm, ''))) AS EmpName
                            FROM {opisdb}.Deprec d
                            LEFT JOIN {opisdb}.Empmas  e  ON e.Empnumber  = d.Empnumber
                            LEFT JOIN {pisdb}.empmas   e1 ON e1.EmpNumber = e.empnumber
                            WHERE d.PayrollgrpId = @PayrollgrpId
                            ORDER BY e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm";

        var data = await _sql.FetchData<M11003AME_Empmas, dynamic>(sql, new { PayrollgrpId = payrollgrpId }, conn);
        return data ?? [];
    }

    public async Task _01AME_SavePunch(Attpunches1Model punch, string pisdb, string conn)
    {
        string sql = $@"INSERT INTO {pisdb}.Attpunches1
                            (EmpmasId, DayNo, PunchInDate, PunchOutDate, PunchT, SchedDuration, DutyTypeId, Status)
                        VALUES
                            (@EmpmasId, @DayNo, @PunchInDate, @PunchOutDate, @PunchT, @SchedDuration, @DutyTypeId, @Status)
                        ON DUPLICATE KEY UPDATE
                            PunchOutDate    = @PunchOutDate,
                            PunchT          = @PunchT,
                            SchedDuration   = @SchedDuration,
                            DutyTypeId      = @DutyTypeId,
                            Status          = @Status;";

        await _sql.ExecuteCmd<dynamic>(sql, punch, conn);
    }

    public async Task _04AME_DeletePunch(int empmasId, DateTime punchInDate, string pisdb, string conn)
    {
        string sql = $@"DELETE FROM {pisdb}.Attpunches1 WHERE EmpmasId = @EmpmasId AND PunchInDate = @PunchInDate;";
        await _sql.ExecuteCmd<dynamic>(sql, new { EmpmasId = empmasId, PunchInDate = punchInDate }, conn);
    }
}

public interface IDA_11003AME
{
    Task<List<M11003AME_Empmas>?>   _02AME_EmplistPerPayrollgrp(int payrollgrpId, string pisdb, string opisdb, string conn);
    Task                            _01AME_SavePunch(Attpunches1Model punch, string pisdb, string conn);
    Task                            _04AME_DeletePunch(int empmasId, DateTime punchInDate, string pisdb, string conn);
}
