using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Attendance
{
    public class AttendanceDataAccess : IAttendanceDataAccess
    {
        private readonly I_90_001_MySqlDataAccess _sql;

        public AttendanceDataAccess(I_90_001_MySqlDataAccess sql)
        { _sql = sql; }

        public async Task<List<AtttemplatereqdtlModel?>> _02TemplateByUserIdAndEffectivityDate(int? userId, int? month, int? year, string? schema, string? conn)
        {
            var startDate   = new DateTime(year ?? 0001, month ?? 1, 1);
            var endDate     = startDate.AddMonths(1); 

            string sql      = $@"SELECT h.Effectivity, h.End EffectivityEnd, t.Name AttendanceType, d.* FROM {schema}.atttemplatereqhdr h
                                  INNER JOIN {schema}.atttemplatereqdtl d on d.AtttemplateReqHdrId = h.Id
                                  LEFT JOIN {schema}.atttype t on t.Id = d.AttendanceTypeId 
                                  WHERE h.userId = @UserId
                                  AND h.status = 'A'
                                  AND h.effectivity < @EndDate 
                                  AND h.End  >=  @StartDate;";
            var data = await _sql.FetchData<AtttemplatereqdtlModel?, dynamic>(sql, new { UserId = userId, StartDate = startDate, EndDate = endDate }, conn);
            return data;
        }

        public async Task<List<Attpunches1Model?>> _02PunchesByEmpmasIdAndPunchInDate(int? empmasId, int? month, int? year, string? schema, string? conn)
        {
            var startDate = new DateTime(year ?? 0001, month ?? 1, 1);
            var endDate = startDate.AddMonths(1);

            string sql = $@"SELECT p.*, d.Code DutyTypeCode FROM {schema}.attpunches1 p
                                  LEFT JOIN {schema}.attdutytype d on d.Id = p.DutyTypeId
                                  WHERE p.empmasId = @EmpmasId
                                  AND  p.punchInDate >=  @StartDate
                                  AND p.punchInDate  <  @EndDate;";
            var data = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, new { EmpmasId = empmasId, StartDate = startDate, EndDate = endDate }, conn);
            return data;
        }

        public async Task<List<AttLeaveapplicationModel?>> _02LeaveByEmpmasIdAndLeaveDuration(int? empmasId, int? month, int? year, string? schema, string? conn)
        {
            var startDate = new DateTime(year ?? 0001, month ?? 1, 1);
            var endDate = startDate.AddMonths(1);

            string sql = $@"SELECT t.Code LeaveCode,  t.leavename, l.lvStart, l.lvEnd, l.Reason, d.dutyType, d.Start, d.End, d.TimeStart, d.TimeDuration,  d.CreditedHrs, d.LeaveDayTypeId, ld.Name LeaveDayType FROM {schema}.leaveapplication l
                            LEFT JOIN {schema}.leaveapplicationdtl d on d.leaveapplicationId = l.Id
                            LEFT JOIN {schema}.leavetype t on t.Id = leavetypeId
                            LEFT JOIN {schema}.leavedaytype ld on ld.Id = d.leavedaytypeId
                            WHERE l.empmasId =  @EmpmasId and l.Status = 'A' and lvStart >= @startDate and lvEnd <  @endDate;";
            var data = await _sql.FetchData<AttLeaveapplicationModel?, dynamic>(sql, new { EmpmasId = empmasId, StartDate = startDate, EndDate = endDate }, conn);
            return data;
        }

        public async Task<List<LeavetypeModel?>> _02LeaveTypes(string? schema, string? conn)
        {
            string sql = $@"SELECT * FROM {schema}.leavetype l;";
            var data = await _sql.FetchData<LeavetypeModel?, dynamic>(sql, new { }, conn);
            return data;
        }

        public async Task<List<AttdutytypeModel?>> _02DutyTypes(string? schema, string? conn)
        {
            string sql = $@"SELECT * FROM {schema}.attdutytype ";
            var data = await _sql.FetchData<AttdutytypeModel?, dynamic>(sql, new { }, conn);
            return data;
        }


  

    }

    public interface IAttendanceDataAccess
    {
        Task<List<AtttemplatereqdtlModel?>> _02TemplateByUserIdAndEffectivityDate(int? userId, int? month, int? year, string? schema, string? conn);
        Task<List<Attpunches1Model?>> _02PunchesByEmpmasIdAndPunchInDate(int? userId, int? month, int? year, string? schema, string? conn);
        Task<List<AttLeaveapplicationModel?>> _02LeaveByEmpmasIdAndLeaveDuration(int? userId, int? month, int? year, string? schema, string? conn);
        Task<List<LeavetypeModel?>> _02LeaveTypes(string? schema, string? conn);
        Task<List<AttdutytypeModel?>> _02DutyTypes(string? schema, string? conn);
    }
}
