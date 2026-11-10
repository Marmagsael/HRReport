using HRApiLibrary.DataAccess._10_Pis;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.Modules._12008O;

public class DA_12008O : IDA_12008O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    private readonly IAtttemplateDataAccess _tpl;
    private readonly IAttdailyDataAccess _daily;
    private readonly IAttpunches1DataAccess _punches;

    public DA_12008O(I_90_001_MySqlDataAccess sql, IAtttemplateDataAccess tpl, IAttdailyDataAccess daily, IAttpunches1DataAccess punches)
    {
        _sql = sql; _tpl = tpl; _daily = daily; _punches = punches;
    }

    private const string NameSql = "concat(trim(ifnull(o.EmpLastNm, '')), ', ', trim(ifnull(o.EmpFirstNm, '')), ' ', trim(ifnull(o.EmpMidNm, '')))";

    // -- Employees that have a face print, with the number of samples --
    public async Task<List<M12008_Enrolled>> _02Enrolled(string pisdb, string opisdb, string conn)
    {
        string sql = $@"select f.EmpmasId, trim(e.EmpNumber) EmpNumber, {NameSql} EmpName, ifnull(e.SystemId, 0) SystemId,
                               count(*) Samples, max(f.CreatedAt) EnrolledAt
                        from   {pisdb}.FaceTemplate f
                               join {pisdb}.Empmas e on e.Id = f.EmpmasId
                               left join {opisdb}.Empmas o on trim(o.EmpNumber) = trim(e.EmpNumber)
                        where  length(f.Descriptor) > 8000
                        group by f.EmpmasId, e.EmpNumber, e.SystemId, o.EmpLastNm, o.EmpFirstNm, o.EmpMidNm
                        order by o.EmpLastNm, o.EmpFirstNm";
        return await _sql.FetchLegacy<M12008_Enrolled, dynamic>(sql, new { }, conn) ?? [];
    }

    // -- All the samples, for the Punch Capturing page --
    public async Task<List<M12008_Template>> _02Templates(string pisdb, string conn)
    {
        var r = await _sql.FetchData<M12008_Template, dynamic>($"select EmpmasId, Descriptor from {pisdb}.FaceTemplate where length(Descriptor) > 8000 order by EmpmasId, Id", new { }, conn);
        return r ?? [];
    }

    public async Task<string> _02EmpName(int empmasId, string pisdb, string opisdb, string conn)
    {
        string sql = $@"select {NameSql} from {pisdb}.Empmas e left join {opisdb}.Empmas o on trim(o.EmpNumber) = trim(e.EmpNumber) where e.Id = @Id";
        var r = await _sql.FetchLegacy<string, dynamic>(sql, new { Id = empmasId }, conn);
        return (r.FirstOrDefault() ?? "").Trim();
    }

    public async Task<int> _02EmpmasIdByNumber(string empNumber, string pisdb, string conn)
    {
        var r = await _sql.FetchData<int, dynamic>($"select Id from {pisdb}.Empmas where trim(EmpNumber) = @EmpNumber order by Id limit 1", new { EmpNumber = (empNumber ?? "").Trim() }, conn);
        return r.FirstOrDefault();
    }

    public async Task<string> _02EmpNumber(int empmasId, string pisdb, string conn)
    {
        var r = await _sql.FetchData<string, dynamic>($"select trim(EmpNumber) from {pisdb}.Empmas where Id = @Id", new { Id = empmasId }, conn);
        return (r.FirstOrDefault() ?? "").Trim();
    }

    public async Task _01Sample(int empmasId, string descriptor, int userId, string pisdb, string conn)
    {
        await _sql.ExecuteCmd($"insert into {pisdb}.FaceTemplate (EmpmasId, Descriptor, CreatedById) values (@EmpmasId, @Descriptor, @UserId)",
            new { EmpmasId = empmasId, Descriptor = descriptor, UserId = userId }, conn);
    }

    public async Task _04Delete(int empmasId, string pisdb, string conn)
    {
        await _sql.ExecuteCmd($"delete from {pisdb}.FaceTemplate where EmpmasId = @Id", new { Id = empmasId }, conn);
    }

    public async Task<int> _02Samples(int empmasId, string pisdb, string conn)
    {
        var r = await _sql.FetchData<long, dynamic>($"select count(*) from {pisdb}.FaceTemplate where EmpmasId = @Id and length(Descriptor) > 8000", new { Id = empmasId }, conn);
        return (int)r.FirstOrDefault();
    }

    // -- Time in when the employee has no open punch, Time out when there is one (same records as the self service punch) --
    public async Task<M12008_PunchResult> _01Punch(int empmasId, string pisdb, string opisdb, string conn, int? timeZoneId, string? ip, string? mac)
    {
        var emp = (await _sql.FetchData<M12008_PisEmp, dynamic>(
            $"select Id, ifnull(SystemId, 0) SystemId, trim(EmpNumber) EmpNumber from {pisdb}.Empmas where Id = @Id", new { Id = empmasId }, conn)).FirstOrDefault();
        if (emp == null) return new() { Ok = false, Msg = "Employee not found." };

        string name = await _02EmpName(empmasId, pisdb, opisdb, conn);
        if (emp.SystemId <= 0)
            return new() { Ok = false, EmpName = name, Msg = "This employee has no system id yet. Open the employee in Employee Entry and save it." };

        int systemId = emp.SystemId;
        var now = DateTime.Now;
        var open = (await _punches._02NoPunchOut(systemId, pisdb, conn))?.FirstOrDefault(x => x != null);

        if (open == null)
        {
            var at = (await _tpl._02s(systemId, pisdb, conn))?.FirstOrDefault(x => x != null) ?? (await _tpl._02NoSchedule(systemId, pisdb, conn))!;
            int? dayNo = MsdsDataAccess.GetDayNo(now.DayOfWeek.ToString());
            var s = Sched(dayNo, at);
            int dutyId = await DutyId(s.DutyType, pisdb, conn);

            await _daily._01PunchIn(new AttdailyModel
            {
                EmpmasId = systemId, Empnumber = emp.EmpNumber, Punchdate = now.Date, Dayno = dayNo, Timein = now,
                Timeint = s.HrsLength, Dutytypeid = dutyId, Inbyid = systemId
            }, pisdb, conn);

            await _punches._01(new Attpunches1Model
            {
                EmpmasId = systemId, DayNo = dayNo, PunchInDate = now, PunchT = s.InTime, SchedDuration = s.HrsLength,
                DutyTypeId = dutyId, TimeZoneIdIn = timeZoneId, IpAddressIn = ip, MacAddressIn = mac, UserIdIn = systemId, Status = "N"
            }, pisdb, conn);

            return new() { Ok = true, Type = "IN", EmpName = name, At = now, Msg = "Punch IN" };
        }
        else
        {
            var at = (await _tpl._02s(systemId, pisdb, conn))?.FirstOrDefault(x => x != null) ?? (await _tpl._02NoSchedule(systemId, pisdb, conn))!;
            var inDate = open.PunchInDate ?? now;
            int? dayNo = MsdsDataAccess.GetDayNo(inDate.DayOfWeek.ToString());
            var s = Sched(dayNo, at);

            var schedIn = inDate.Date.AddHours((s.InTime ?? 0) / 100).AddMinutes((s.InTime ?? 0) % 100);
            var schedOut = schedIn.AddHours((s.HrsLength ?? 0) / 100).AddMinutes((s.HrsLength ?? 0) % 100);
            int outSched = schedOut.Hour * 100 + schedOut.Minute;
            int dutyId = await DutyId(s.DutyType, pisdb, conn);

            await _daily._01PunchOut(new AttdailyModel
            {
                EmpmasId = systemId, Empnumber = emp.EmpNumber, Punchdate = now.Date, Dayno = dayNo, Timein = inDate,
                Timeint = open.PunchT, Timeoutt = outSched, Dutytypeid = dutyId, Inbyid = systemId, Outbyid = systemId
            }, pisdb, conn);

            await _punches._03s(new Attpunches1Model
            {
                EmpmasId = systemId, DayNo = dayNo, PunchInDate = open.PunchInDate, PunchOutDate = now,
                TimeZoneIdOut = timeZoneId, IpAddressOut = ip, MacAddressOut = mac, UserIdOut = systemId, Status = "L"
            }, pisdb, conn);

            return new() { Ok = true, Type = "OUT", EmpName = name, At = now, Msg = "Punch OUT" };
        }
    }

    private async Task<int> DutyId(string? code, string pisdb, string conn)
    {
        var r = await _sql.FetchData<int, dynamic>($"select Id from {pisdb}.AttDutyType where Code = @Code limit 1", new { Code = code ?? "RD" }, conn);
        var id = r.FirstOrDefault();
        return id > 0 ? id : (code == "R" ? 1 : 2);
    }

    private record Sch(int? InTime, int? HrsLength, string? DutyType);

    private static Sch Sched(int? dayNo, AtttemplateModel at) => dayNo switch
    {
        1 => new(at.D1_in, at.D1_hrslength, at.D1_dutytype ?? "RD"),
        2 => new(at.D2_in, at.D2_hrslength, at.D2_dutytype ?? "RD"),
        3 => new(at.D3_in, at.D3_hrslength, at.D3_dutytype ?? "RD"),
        4 => new(at.D4_in, at.D4_hrslength, at.D4_dutytype ?? "RD"),
        5 => new(at.D5_in, at.D5_hrslength, at.D5_dutytype ?? "RD"),
        6 => new(at.D6_in, at.D6_hrslength, at.D6_dutytype ?? "RD"),
        7 => new(at.D7_in, at.D7_hrslength, at.D7_dutytype ?? "RD"),
        _ => new(0, 0, "RD")
    };
}

public interface IDA_12008O
{
    Task<List<M12008_Enrolled>> _02Enrolled(string pisdb, string opisdb, string conn);
    Task<List<M12008_Template>> _02Templates(string pisdb, string conn);
    Task<string> _02EmpName(int empmasId, string pisdb, string opisdb, string conn);
    Task<string> _02EmpNumber(int empmasId, string pisdb, string conn);
    Task<int> _02EmpmasIdByNumber(string empNumber, string pisdb, string conn);
    Task _01Sample(int empmasId, string descriptor, int userId, string pisdb, string conn);
    Task _04Delete(int empmasId, string pisdb, string conn);
    Task<int> _02Samples(int empmasId, string pisdb, string conn);
    Task<M12008_PunchResult> _01Punch(int empmasId, string pisdb, string opisdb, string conn, int? timeZoneId, string? ip, string? mac);
}

public class M12008_Enrolled
{
    public int EmpmasId { get; set; }
    public string? EmpNumber { get; set; }
    public string? EmpName { get; set; }
    public int SystemId { get; set; }
    public int Samples { get; set; }
    public DateTime? EnrolledAt { get; set; }
}

public class M12008_Template
{
    public int EmpmasId { get; set; }
    public string? Descriptor { get; set; }
}

public class M12008_PisEmp
{
    public int Id { get; set; }
    public int SystemId { get; set; }
    public string? EmpNumber { get; set; }
}

public class M12008_PunchResult
{
    public bool Ok { get; set; }
    public string? Type { get; set; }
    public string? EmpName { get; set; }
    public DateTime At { get; set; }
    public string? Msg { get; set; }
}
