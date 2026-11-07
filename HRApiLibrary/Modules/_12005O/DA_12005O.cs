using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._12005O;

public class DA_12005O : IDA_12005O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_12005O(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // -- Daily schedule templates (AttSchedDaily) --
    public async Task<List<M12005_Daily>> _02Dailys(string pisdb, string conn)
    {
        string sql = $@"select Id, Name, DutyType, PIn, Duration, POut from {pisdb}.AttSchedDaily order by Name";
        return await _sql.FetchData<M12005_Daily, dynamic>(sql, new { }, conn) ?? [];
    }

    // -- Weekly schedule assigned to the people (AttTemplate), one row per employee per weekday (1 = Sunday ... 7 = Saturday) --
    public async Task<List<M12005_DaySched>> _02Templates(List<int> systemIds, string pisdb, string conn)
    {
        if (systemIds.Count == 0) return [];
        var parts = Enumerable.Range(1, 7).Select(n =>
            $"select EmpmasId, {n} DayNo, D{n}_DutyType DutyType, D{n}_In PIn, D{n}_HrsLength Hrs, null EffFrom, null EffTo from {pisdb}.Atttemplate where EmpmasId in @Ids");
        string sql = string.Join(" union all ", parts);
        return await _sql.FetchData<M12005_DaySched, dynamic>(sql, new { Ids = systemIds }, conn) ?? [];
    }

    // -- Weekly schedule that takes effect on a date (AttTemplate.AttschedweeklyhdrIdAdv + ChangeSchedEffectivity) --
    public async Task<List<M12005_DaySched>> _02WeeklyAdvance(List<int> systemIds, string pisdb, string conn)
    {
        if (systemIds.Count == 0) return [];
        var parts = Enumerable.Range(1, 7).Select(n =>
            $@"select t.EmpmasId, {n} DayNo, w.D{n}_DutyType DutyType, w.D{n}_In PIn, w.D{n}_HrsLength Hrs, t.ChangeSchedEffectivity EffFrom, t.ChangeSchedEnd EffTo
               from {pisdb}.Atttemplate t join {pisdb}.Attschedweeklydtl w on w.AttSchedWeeklyHdrId = t.AttschedweeklyhdrIdAdv
               where t.EmpmasId in @Ids and ifnull(t.AttschedweeklyhdrIdAdv, 0) > 0 and t.ChangeSchedEffectivity is not null");
        string sql = string.Join(" union all ", parts);
        return await _sql.FetchData<M12005_DaySched, dynamic>(sql, new { Ids = systemIds }, conn) ?? [];
    }

    // -- Default weekly schedule (AttSchedDefault) for people who have none --
    public async Task<List<M12005_DaySched>> _02DefaultSchedule(string pisdb, string conn)
    {
        var parts = Enumerable.Range(1, 7).Select(n =>
            $"select 0 EmpmasId, {n} DayNo, D{n}_DutyType DutyType, D{n}_In PIn, D{n}_HrsLength Hrs, null EffFrom, null EffTo from {pisdb}.Attscheddefault where Id = 1");
        string sql = string.Join(" union all ", parts);
        return await _sql.FetchData<M12005_DaySched, dynamic>(sql, new { }, conn) ?? [];
    }

    // -- Schedule of one date (AttAdvanceSchedule) : it wins over the weekly schedule --
    public async Task<List<M12005_Advance>> _02Advances(List<string> empNumbers, DateTime from, DateTime to, string pisdb, string conn)
    {
        if (empNumbers.Count == 0) return [];
        string sql = $@"select Empnumber EmpNumber, Date, DutyType, PIn, Duration, Pout POut
                        from {pisdb}.Attadvanceschedule
                        where Empnumber in @Nos and Date >= @From and Date <= @To";
        return await _sql.FetchData<M12005_Advance, dynamic>(sql, new { Nos = empNumbers, From = from.Date, To = to.Date }, conn) ?? [];
    }

    // -- Punches of the people on the dates (AttPunches1) --
    public async Task<List<M12005_Punch>> _02Punches(List<int> systemIds, DateTime from, DateTime to, string pisdb, string conn)
    {
        if (systemIds.Count == 0) return [];
        string sql = $@"select p.EmpmasId, p.PunchInDate, p.PunchOutDate, p.SchedDuration, p.Status, d.Code DutyCode
                        from {pisdb}.Attpunches1 p left join {pisdb}.AttDutyType d on d.Id = p.DutyTypeId
                        where p.EmpmasId in @Ids and p.PunchInDate >= @From and p.PunchInDate < @To
                        order by p.PunchInDate";
        return await _sql.FetchData<M12005_Punch, dynamic>(sql, new { Ids = systemIds, From = from.Date, To = to.Date.AddDays(1) }, conn) ?? [];
    }

    // -- Save : an existing punch (OrigPunchInDate) is updated, a new one is added. The status is always Locked. --
    public async Task _01Save(List<M12005_Save> rows, int userId, string pisdb, string conn)
    {
        foreach (var r in rows)
        {
            await _sql.ExecuteCmd($@"insert into {pisdb}.AttDutyType (Code, Name)
                                     select @Code, @Name from dual where not exists (select 1 from {pisdb}.AttDutyType where Code = @Code)",
                                   new { Code = r.DutyCode, Name = DutyName(r.DutyCode) }, conn);

            var p = new
            {
                r.EmpmasId, r.OrigPunchInDate, r.PunchInDate, r.PunchOutDate, r.DayNo, r.PunchT, r.SchedDuration, r.DutyCode, UserId = userId
            };

            if (r.OrigPunchInDate != null)
            {
                string upd = $@"update {pisdb}.Attpunches1 set
                                    PunchInDate = @PunchInDate, PunchOutDate = @PunchOutDate, DayNo = @DayNo, PunchT = @PunchT, SchedDuration = @SchedDuration,
                                    DutyTypeId = (select Id from {pisdb}.AttDutyType where Code = @DutyCode limit 1),
                                    UserIdOut = if(@PunchOutDate is null, UserIdOut, @UserId), Status = 'L'
                                where EmpmasId = @EmpmasId and PunchInDate = @OrigPunchInDate";
                await _sql.ExecuteCmd(upd, p, conn);
            }
            else
            {
                string ins = $@"insert into {pisdb}.Attpunches1
                                    (EmpmasId, DayNo, PunchInDate, PunchT, SchedDuration, DutyTypeId, TimeZoneIdIn, UserIdIn, PunchOutDate, TimeZoneIdOut, UserIdOut, Status)
                                values
                                    (@EmpmasId, @DayNo, @PunchInDate, @PunchT, @SchedDuration,
                                     (select Id from {pisdb}.AttDutyType where Code = @DutyCode limit 1),
                                     0, @UserId, @PunchOutDate, 0, if(@PunchOutDate is null, null, @UserId), 'L')
                                on duplicate key update
                                    PunchOutDate = @PunchOutDate, DayNo = @DayNo, PunchT = @PunchT, SchedDuration = @SchedDuration,
                                    DutyTypeId = (select Id from {pisdb}.AttDutyType where Code = @DutyCode limit 1),
                                    UserIdOut = if(@PunchOutDate is null, UserIdOut, @UserId), Status = 'L'";
                await _sql.ExecuteCmd(ins, p, conn);
            }
        }
    }

    private static string DutyName(string code) => code switch
    {
        "RD" => "Restday",
        "RN" => "Regular Night",
        _    => "Regulary Day"
    };
}

public interface IDA_12005O
{
    Task<List<M12005_Daily>>    _02Dailys(string pisdb, string conn);
    Task<List<M12005_DaySched>> _02Templates(List<int> systemIds, string pisdb, string conn);
    Task<List<M12005_DaySched>> _02WeeklyAdvance(List<int> systemIds, string pisdb, string conn);
    Task<List<M12005_DaySched>> _02DefaultSchedule(string pisdb, string conn);
    Task<List<M12005_Advance>>  _02Advances(List<string> empNumbers, DateTime from, DateTime to, string pisdb, string conn);
    Task<List<M12005_Punch>>    _02Punches(List<int> systemIds, DateTime from, DateTime to, string pisdb, string conn);
    Task                        _01Save(List<M12005_Save> rows, int userId, string pisdb, string conn);
}

public class M12005_Daily
{
    public int      Id        { get; set; }
    public string?  Name      { get; set; }
    public string?  DutyType  { get; set; }
    public int      PIn       { get; set; }
    public int      Duration  { get; set; }
    public int      POut      { get; set; }
}

// -- One weekday of a weekly schedule : DayNo 1 = Sunday ... 7 = Saturday, PIn as HHmm, Hrs as hours x 100 --
public class M12005_DaySched
{
    public int       EmpmasId  { get; set; }
    public int       DayNo     { get; set; }
    public string?   DutyType  { get; set; }
    public int       PIn       { get; set; }
    public int       Hrs       { get; set; }
    public DateTime? EffFrom   { get; set; }
    public DateTime? EffTo     { get; set; }
}

public class M12005_Advance
{
    public string?   EmpNumber { get; set; }
    public DateTime  Date      { get; set; }
    public string?   DutyType  { get; set; }
    public int       PIn       { get; set; }
    public int       Duration  { get; set; }
    public int       POut      { get; set; }
}

public class M12005_Punch
{
    public int       EmpmasId      { get; set; }
    public DateTime  PunchInDate   { get; set; }
    public DateTime? PunchOutDate  { get; set; }
    public int?      SchedDuration { get; set; }
    public string?   Status        { get; set; }
    public string?   DutyCode      { get; set; }
}

public class M12005_Save
{
    public int       EmpmasId        { get; set; }
    public DateTime? OrigPunchInDate { get; set; }
    public DateTime  PunchInDate     { get; set; }
    public DateTime? PunchOutDate    { get; set; }
    public int       DayNo           { get; set; }
    public int       PunchT          { get; set; }
    public int       SchedDuration   { get; set; }
    public string    DutyCode        { get; set; } = "R";
}
