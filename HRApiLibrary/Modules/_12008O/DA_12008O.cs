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
                        group by f.EmpmasId, e.EmpNumber, e.SystemId, o.EmpLastNm, o.EmpFirstNm, o.EmpMidNm
                        order by o.EmpLastNm, o.EmpFirstNm";
        return await _sql.FetchLegacy<M12008_Enrolled, dynamic>(sql, new { }, conn) ?? [];
    }

    // -- All the samples, for the Punch Capturing page --
    public async Task<List<M12008_Template>> _02Templates(string pisdb, string conn)
    {
        var r = await _sql.FetchData<M12008_Template, dynamic>($"select EmpmasId, Descriptor from {pisdb}.FaceTemplate order by EmpmasId, Id", new { }, conn);
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
        var r = await _sql.FetchData<long, dynamic>($"select count(*) from {pisdb}.FaceTemplate where EmpmasId = @Id", new { Id = empmasId }, conn);
        return (int)r.FirstOrDefault();
    }

    // -- Time in when the employee has no open punch, Time out when there is one (same records as the self service punch) --
    public async Task<M12008_PunchResult> _01Punch(int empmasId, string pisdb, string opisdb, string conn, int? timeZoneId, string? ip, string? mac, M12008_MobileInfo? mob = null)
    {
        var emp = (await _sql.FetchData<M12008_PisEmp, dynamic>(
            $"select Id, ifnull(SystemId, 0) SystemId, trim(EmpNumber) EmpNumber from {pisdb}.Empmas where Id = @Id", new { Id = empmasId }, conn)).FirstOrDefault();
        if (emp == null) return new() { Ok = false, Msg = "Employee not found." };

        string name = await _02EmpName(empmasId, pisdb, opisdb, conn);
        if (emp.SystemId <= 0)
            return new() { Ok = false, EmpName = name, Msg = "This employee has no system id yet. Open the employee in Employee Entry and save it." };

        int systemId = emp.SystemId;
        var zone = await CompanyZone(pisdb, conn);
        var now = mob?.AtUtc != null ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(mob.AtUtc.Value, DateTimeKind.Utc), zone)
                                     : TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);

        if (mob != null)
        {
            var dup = await _sql.FetchData<long, dynamic>($"select count(*) from {pisdb}.Attpunches1 where EmpmasId = @Id and (PunchInDate = @At or PunchOutDate = @At)", new { Id = systemId, At = now }, conn);
            if (dup.FirstOrDefault() > 0) return new() { Ok = true, Type = "DUP", EmpName = name, At = now, AtUtc = ToUtc(now, zone), Msg = "Already recorded." };
        }

        Attpunches1Model? open;
        if (mob != null)
        {
            // -- From the phone : when the table already has a later record (IN or OUT), this punch is no longer valid (void).
            //    Otherwise it is an OUT when an earlier IN is still open, and an IN when not. OB / Leave rows are not counted. --
            var latest = (await _sql.FetchData<DateTime?, dynamic>($@"select greatest(max(PunchInDate), ifnull(max(PunchOutDate), max(PunchInDate)))
                                from {pisdb}.Attpunches1 where EmpmasId = @Id and Status not like 'OB%' and Status not like 'LV%'", new { Id = systemId }, conn)).FirstOrDefault();
            if (latest != null && latest > now)
                return new() { Ok = false, Code = "EXISTS", EmpName = name, At = now, AtUtc = ToUtc(now, zone),
                    Msg = $"Not uploaded: there is already a later record ({Fmt(latest)}), so this punch can no longer be added." };

            open = (await _sql.FetchData<Attpunches1Model, dynamic>($@"select * from {pisdb}.Attpunches1
                            where EmpmasId = @Id and PunchOutDate is null and PunchInDate <= @At
                            order by PunchInDate desc limit 1", new { Id = systemId, At = now }, conn)).FirstOrDefault();
        }
        else open = (await _punches._02NoPunchOut(systemId, pisdb, conn))?.FirstOrDefault(x => x != null);

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

            if (mob != null)
                await _sql.ExecuteCmd($@"update {pisdb}.Attpunches1 set LatIn = @Lat, LngIn = @Lng, AccIn = @Acc, FaceDistIn = @Fd, Source = 'M', IsOffline = @Off, ForReview = @Rev, SyncedAt = now()
                                         where EmpmasId = @Id and PunchInDate = @At",
                    new { Id = systemId, At = now, Lat = mob.Lat, Lng = mob.Lng, Acc = mob.Acc, Fd = mob.FaceDist, Off = mob.Offline ? 1 : 0, Rev = mob.ForReview ? 1 : 0 }, conn);

            return new() { Ok = true, Type = "IN", EmpName = name, At = now, AtUtc = ToUtc(now, zone), Msg = "Punch IN" };
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

            if (mob != null)
                await _sql.ExecuteCmd($@"update {pisdb}.Attpunches1 set LatOut = @Lat, LngOut = @Lng, AccOut = @Acc, FaceDistOut = @Fd, Source = 'M',
                                                IsOffline = if(IsOffline = 1, 1, @Off), ForReview = if(ForReview = 1, 1, @Rev), SyncedAt = now()
                                         where EmpmasId = @Id and PunchInDate = @InAt",
                    new { Id = systemId, InAt = inDate, Lat = mob.Lat, Lng = mob.Lng, Acc = mob.Acc, Fd = mob.FaceDist, Off = mob.Offline ? 1 : 0, Rev = mob.ForReview ? 1 : 0 }, conn);

            return new() { Ok = true, Type = "OUT", EmpName = name, At = now, AtUtc = ToUtc(now, zone), Msg = "Punch OUT" };
        }
    }

    // -- Face samples (128 numbers each) of one employee, for the mobile app --
    public async Task<List<string>> _02OwnSamples(int empmasId, string pisdb, string conn)
    {
        var r = await _sql.FetchData<string, dynamic>($"select Descriptor from {pisdb}.FaceTemplate where EmpmasId = @Id order by Id", new { Id = empmasId }, conn);
        return r ?? [];
    }

    // -- Next action (IN when there is no open punch, OUT when there is one) and the last punch of the employee --
    public async Task<M12008_MobileStatus> _02MobileStatus(int empmasId, string pisdb, string opisdb, string conn)
    {
        var emp = (await _sql.FetchData<M12008_PisEmp, dynamic>(
            $"select Id, ifnull(SystemId, 0) SystemId, trim(EmpNumber) EmpNumber from {pisdb}.Empmas where Id = @Id", new { Id = empmasId }, conn)).FirstOrDefault();
        var st = new M12008_MobileStatus { EmpName = await _02EmpName(empmasId, pisdb, opisdb, conn), EmpNumber = emp?.EmpNumber, NextAction = "IN" };
        if (emp == null || emp.SystemId <= 0) return st;
        var zone = await CompanyZone(pisdb, conn);

        var open = (await _punches._02NoPunchOut(emp.SystemId, pisdb, conn))?.FirstOrDefault(x => x != null);
        if (open != null) { st.NextAction = "OUT"; st.LastAt = open.PunchInDate; st.LastType = "IN"; st.LastAtUtc = open.PunchInDate == null ? null : ToUtc(open.PunchInDate.Value, zone); return st; }

        var last = (await _punches._02LastPunches(emp.SystemId, 1, pisdb, conn))?.FirstOrDefault(x => x != null);
        if (last != null)
        {
            st.LastAt = last.PunchOutDate ?? last.PunchInDate; st.LastType = last.PunchOutDate != null ? "OUT" : "IN";
            st.LastAtUtc = st.LastAt == null ? null : ToUtc(st.LastAt.Value, zone);
        }
        return st;
    }

    // -- Time of the punch follows the company time zone (Main.UsersCompany.TimeZone, default Asia/Manila), not the server clock --
    public Task<TimeZoneInfo> _02CompanyZone(string pisdb, string conn) => CompanyZone(pisdb, conn);

    private async Task<TimeZoneInfo> CompanyZone(string pisdb, string conn)
    {
        string? id = null;
        try
        {
            var r = await _sql.FetchData<string, dynamic>("select TimeZone from Main.UsersCompany where PISSchema = @Pis limit 1", new { Pis = pisdb }, conn);
            id = r.FirstOrDefault();
        }
        catch { }
        foreach (var name in new[] { id, "Asia/Manila", "Singapore Standard Time" })
        {
            if (string.IsNullOrWhiteSpace(name)) continue;
            try { return TimeZoneInfo.FindSystemTimeZoneById(name); } catch { }
        }
        return TimeZoneInfo.CreateCustomTimeZone("PH", TimeSpan.FromHours(8), "PH", "PH");
    }

    private static string Fmt(DateTime? d) => d?.ToString("MMM d, h:mm tt") ?? "";

    private static DateTime ToUtc(DateTime companyTime, TimeZoneInfo zone) =>
        TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(companyTime, DateTimeKind.Unspecified), zone);

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
    Task<M12008_PunchResult> _01Punch(int empmasId, string pisdb, string opisdb, string conn, int? timeZoneId, string? ip, string? mac, M12008_MobileInfo? mob = null);
    Task<List<string>> _02OwnSamples(int empmasId, string pisdb, string conn);
    Task<M12008_MobileStatus> _02MobileStatus(int empmasId, string pisdb, string opisdb, string conn);
    Task<TimeZoneInfo> _02CompanyZone(string pisdb, string conn);
}

public class M12008_MobileInfo
{
    public DateTime? AtUtc { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public double? Acc { get; set; }
    public double? FaceDist { get; set; }
    public bool Offline { get; set; }
    public bool ForReview { get; set; }
}

public class M12008_MobileStatus
{
    public string? EmpName { get; set; }
    public string? EmpNumber { get; set; }
    public string NextAction { get; set; } = "IN";
    public DateTime? LastAt { get; set; }
    public DateTime? LastAtUtc { get; set; }
    public string? LastType { get; set; }
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
    public DateTime? AtUtc { get; set; }
    public string? Code { get; set; }
    public string? Msg { get; set; }
}
