using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._2306O;

// -- 14/2306 Leave Transaction Entry : HR encodes the leave of an employee. It is saved in the old PIS (the source of truth) : {OPis}.leave1 --
public class DA_2306O : IDA_2306O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_2306O(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<M2306_Emp>> _02Employees(string opisdb, string conn)
    {
        string sql = $@"select trim(EmpNumber) EmpNumber,
                               concat(trim(ifnull(EmpLastNm, '')), ', ', trim(ifnull(EmpFirstNm, '')), ' ', trim(ifnull(EmpMidNm, ''))) EmpName
                        from   {opisdb}.Empmas
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        return await _sql.FetchLegacy<M2306_Emp, dynamic>(sql, new { }, conn) ?? [];
    }

    // -- One employee : name, current status and client --
    public async Task<M2306_EmpInfo?> _02Employee(string empNumber, string opisdb, string conn)
    {
        string sql = $@"select trim(e.EmpNumber) EmpNumber,
                               concat(trim(ifnull(e.EmpLastNm, '')), ', ', trim(ifnull(e.EmpFirstNm, '')), ' ', trim(ifnull(e.EmpMidNm, ''))) EmpName,
                               trim(s.NAME) Status, trim(c.CLNAME) Client
                        from   {opisdb}.Empmas e
                               left join {opisdb}.empstat s on s.CODE = e.EMPSTAT_
                               left join {opisdb}.client  c on c.CLNUMBER = e.CLIENT_
                        where  e.EmpNumber = @EmpNumber";
        var r = await _sql.FetchLegacy<M2306_EmpInfo, dynamic>(sql, new { EmpNumber = empNumber }, conn);
        return r.FirstOrDefault();
    }

    // -- Search Employee : by number or name --
    public async Task<List<M2306_EmpInfo>> _02EmployeeSearch(string keyword, string opisdb, string conn)
    {
        string sql = $@"select trim(e.EmpNumber) EmpNumber,
                               concat(trim(ifnull(e.EmpLastNm, '')), ', ', trim(ifnull(e.EmpFirstNm, '')), ' ', trim(ifnull(e.EmpMidNm, ''))) EmpName,
                               trim(s.NAME) Status, trim(c.CLNAME) Client
                        from   {opisdb}.Empmas e
                               left join {opisdb}.empstat s on s.CODE = e.EMPSTAT_
                               left join {opisdb}.client  c on c.CLNUMBER = e.CLIENT_
                        where  e.EmpNumber like @K or e.EmpLastNm like @K or e.EmpFirstNm like @K or e.EmpMidNm like @K
                        order by e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm limit 200";
        return await _sql.FetchLegacy<M2306_EmpInfo, dynamic>(sql, new { K = "%" + (keyword ?? "").Trim() + "%" }, conn) ?? [];
    }

    // -- The statuses that mean "on leave" (empstat.isonleaved) : the employee status can be set to one of them --
    public async Task<List<M2306_Status>> _02LeaveStatuses(string opisdb, string conn)
    {
        string sql = $"select trim(CODE) Code, trim(NAME) Name from {opisdb}.empstat where isonleaved = '1' order by NAME";
        return await _sql.FetchLegacy<M2306_Status, dynamic>(sql, new { }, conn) ?? [];
    }

    // -- The number the next leave of that month will get (shown as the Control Number before saving) --
    public async Task<string> _02NextNumber(DateTime lvDate, string opisdb, string conn)
    {
        string prefix = lvDate.ToString("yy-MM");
        var maxes = await _sql.FetchLegacy<long, dynamic>(
            $@"select ifnull(max(cast(right(LVNUMBER, 3) as unsigned)), 0) from {opisdb}.leave1 where LVNUMBER like @Like",
            new { Like = prefix + "%" }, conn);
        return prefix + (maxes.FirstOrDefault() + 1).ToString("000");
    }

    // -- Leave types of the old PIS (levtbl) --
    public async Task<List<M2306_LeaveType>> _02LeaveTypes(string opisdb, string conn)
    {
        string sql = $"select trim(LVCODE) Code, trim(LVNAME) Name from {opisdb}.levtbl order by LVNAME";
        return await _sql.FetchLegacy<M2306_LeaveType, dynamic>(sql, new { }, conn) ?? [];
    }

    // -- Leave transactions that overlap the dates, with the employee name --
    public async Task<List<M2306_Transaction>> _02Transactions(DateTime from, DateTime to, string opisdb, string conn)
    {
        string sql = $@"select l.LVNUMBER LvNumber, l.LVDATE LvDate, trim(l.EMPNUMBER) EmpNumber,
                               concat(trim(ifnull(e.EmpLastNm, '')), ', ', trim(ifnull(e.EmpFirstNm, '')), ' ', trim(ifnull(e.EmpMidNm, ''))) EmpName,
                               trim(l.LVCODE) LeaveCode, trim(t.LVNAME) LeaveName, l.`START` LvStart, l.`END` LvEnd, l.NO_DAYS NoDays, l.WITHPAY WithPay,
                               ifnull(l.DAYS_WITHPAY, if(l.WITHPAY = 'Y', l.NO_DAYS, 0)) DaysWithPay
                        from   {opisdb}.leave1 l
                               left join {opisdb}.Empmas e on e.EmpNumber = l.EMPNUMBER
                               left join {opisdb}.levtbl t on t.LVCODE = l.LVCODE
                        where  l.`START` <= @To and l.`END` >= @From
                        order by l.`START` desc, l.LVNUMBER desc";
        return await _sql.FetchLegacy<M2306_Transaction, dynamic>(sql, new { From = from.Date, To = to.Date }, conn) ?? [];
    }

    // -- Leave history of one employee : every leave he or she filed, the latest first --
    public async Task<List<M2306_Transaction>> _02History(string empNumber, string opisdb, string conn)
    {
        string sql = $@"select l.LVNUMBER LvNumber, l.LVDATE LvDate, trim(l.EMPNUMBER) EmpNumber,
                               concat(trim(ifnull(e.EmpLastNm, '')), ', ', trim(ifnull(e.EmpFirstNm, '')), ' ', trim(ifnull(e.EmpMidNm, ''))) EmpName,
                               trim(l.LVCODE) LeaveCode, trim(t.LVNAME) LeaveName, l.`START` LvStart, l.`END` LvEnd, l.NO_DAYS NoDays, l.WITHPAY WithPay,
                               ifnull(l.DAYS_WITHPAY, if(l.WITHPAY = 'Y', l.NO_DAYS, 0)) DaysWithPay
                        from   {opisdb}.leave1 l
                               left join {opisdb}.Empmas e on e.EmpNumber = l.EMPNUMBER
                               left join {opisdb}.levtbl t on t.LVCODE = l.LVCODE
                        where  l.EMPNUMBER = @EmpNumber
                        order by l.`START` desc, l.LVNUMBER desc";
        return await _sql.FetchLegacy<M2306_Transaction, dynamic>(sql, new { EmpNumber = empNumber }, conn) ?? [];
    }

    // -- The employee already has a leave on those dates --
    public async Task<int> _02Overlapping(string empNumber, DateTime start, DateTime end, string? exceptLvNumber, string opisdb, string conn)
    {
        string sql = $@"select count(*) from {opisdb}.leave1 where EMPNUMBER = @EmpNumber and `START` <= @End and `END` >= @Start and LVNUMBER <> @Except";
        var r = await _sql.FetchLegacy<long, dynamic>(sql, new { EmpNumber = empNumber, Start = start.Date, End = end.Date, Except = exceptLvNumber ?? "" }, conn);
        return (int)r.FirstOrDefault();
    }

    // -- Save : LVNUMBER = yy-MM of the leave date + the running number of that month. Returns the LVNUMBER. --
    public async Task<string> _01Save(M2306_Save s, string? encodeBy, string opisdb, string conn)
    {
        string lvNumber = await _02NextNumber(s.LvDate, opisdb, conn);

        string sql = $@"insert into {opisdb}.leave1 (LVNUMBER, LVDATE, EMPNUMBER, LVCODE, `START`, `END`, NO_DAYS, WITHPAY, DAYS_WITHPAY, ENCODEBY, ENCODEDT, `mode`)
                        values (@LvNumber, @LvDate, @EmpNumber, @LeaveCode, @LvStart, @LvEnd, @NoDays, @WithPay, @DaysWithPay, @EncodeBy, @EncodeDt, 'L')";
        await _sql.ExecuteLegacyCmd<dynamic>(sql, new
        {
            LvNumber = lvNumber, LvDate = s.LvDate.Date, s.EmpNumber, s.LeaveCode, LvStart = s.LvStart.Date, LvEnd = s.LvEnd.Date, s.NoDays,
            WithPay = s.DaysWithPay > 0 ? "Y" : "N", s.DaysWithPay, EncodeBy = encodeBy?.Trim(), EncodeDt = DateTime.Now.ToString("yyyy-MM-dd")
        }, conn);
        return lvNumber;
    }

    public async Task _03Update(string lvNumber, M2306_Save s, string? changeBy, string opisdb, string conn)
    {
        string sql = $@"update {opisdb}.leave1 set LVDATE = @LvDate, EMPNUMBER = @EmpNumber, LVCODE = @LeaveCode, `START` = @LvStart, `END` = @LvEnd,
                               NO_DAYS = @NoDays, WITHPAY = @WithPay, DAYS_WITHPAY = @DaysWithPay, CHANGEBY = @ChangeBy, CHANGEDT = @ChangeDt
                        where LVNUMBER = @LvNumber";
        await _sql.ExecuteLegacyCmd<dynamic>(sql, new
        {
            LvNumber = lvNumber, LvDate = s.LvDate.Date, s.EmpNumber, s.LeaveCode, LvStart = s.LvStart.Date, LvEnd = s.LvEnd.Date, s.NoDays,
            WithPay = s.DaysWithPay > 0 ? "Y" : "N", s.DaysWithPay, ChangeBy = changeBy?.Trim(), ChangeDt = DateTime.Now.ToString("yyyy-MM-dd")
        }, conn);
    }

    // -- Set Employee Status To : the employee status of the old PIS becomes the chosen "on leave" status --
    //    The change is recorded in empstatmovement (from / to / user / date), like the Employee Status module. --
    public async Task _03EmpStatus(string empNumber, string statusCode, DateTime statusDate, string? user, string opisdb, string conn)
    {
        var names = await _sql.FetchLegacy<string, dynamic>(
            $@"select ifnull((select trim(NAME) from {opisdb}.empstat where CODE = e.EMPSTAT_), '') from {opisdb}.Empmas e where e.EmpNumber = @EmpNumber
               union all select trim(NAME) from {opisdb}.empstat where CODE = @Code",
            new { EmpNumber = empNumber, Code = statusCode }, conn);
        string from = names.ElementAtOrDefault(0) ?? "", to = names.ElementAtOrDefault(1) ?? statusCode;

        await _sql.ExecuteLegacyCmd<dynamic>($"update {opisdb}.Empmas set EMPSTAT_ = @Code, STATUSDATE = @Date where EmpNumber = @EmpNumber",
                                             new { Code = statusCode, Date = statusDate.Date, EmpNumber = empNumber }, conn);

        // -- empstatmovement keeps the names cut to 15 characters --
        string Fit(string x) => x.Length > 15 ? x.Substring(0, 15) : x;
        await _sql.ExecuteLegacyCmd<dynamic>($@"insert into {opisdb}.empstatmovement (empnumber, user, stat_from, stat_to, datemove, module)
                                                values (@EmpNumber, @User, @From, @To, now(), 'Leave Transaction Entry')",
                                             new { EmpNumber = empNumber, User = user?.Trim(), From = Fit(from), To = Fit(to) }, conn);
    }

    // =========== PIS side : the leave is also recorded in the new PIS (keyed by Empmas.Id) so the leave balance can be monitored ===========

    // -- The employee in the new PIS (EmpNumber links the old and the new PIS). SystemId is 0 when the employee has no user account. --
    public async Task<M2306_PisEmp?> _02PisEmp(string empNumber, string pisdb, string conn)
    {
        var r = await _sql.FetchData<M2306_PisEmp, dynamic>(
            $"select Id EmpmasId, ifnull(SystemId, 0) SystemId from {pisdb}.Empmas where trim(EmpNumber) = @EmpNumber order by Id limit 1",
            new { EmpNumber = empNumber }, conn);
        return r.FirstOrDefault();
    }

    // -- The leave type of the old PIS (levtbl.LVCODE) and the new PIS (Leavetype.Code) are matched by code; 0 when the new PIS has none --
    public async Task<int> _02PisLeaveTypeId(string leaveCode, string pisdb, string conn)
    {
        var r = await _sql.FetchData<int, dynamic>($"select Id from {pisdb}.Leavetype where trim(Code) = @Code limit 1", new { Code = leaveCode.Trim() }, conn);
        return r.FirstOrDefault();
    }

    // -- The application is Approved at once (HR encoded it) : header, days and history. Returns the application Id. --
    public async Task<int> _01SavePis(M2306_PisSave s, int hrEmpmasId, string pisdb, string conn)
    {
        string sql = $@"insert into {pisdb}.Leaveapplication
                            (Yr, EmpmasId, DateApplied, LeaveTypeId, LvBalance, DaysCnt, DaysWithPay, Urgency, LvStart, LvEnd, Reason, Approver1Id, Approver2Id, Approver3Id, Status)
                        values
                            (@Yr, @EmpmasId, @DateApplied, @LeaveTypeId, @LvBalance, @DaysCnt, @DaysWithPay, 'Normal', @LvStart, @LvEnd, @Reason, 0, @HrEmpmasId, 0, 'A');
                        select last_insert_id();";
        var ids = await _sql.FetchData<long, dynamic>(sql, new
        {
            s.Yr, s.EmpmasId, s.DateApplied, s.LeaveTypeId, s.LvBalance, s.DaysCnt, s.DaysWithPay, s.LvStart, s.LvEnd, s.Reason, HrEmpmasId = hrEmpmasId
        }, conn);
        var id = (int)ids.First();

        foreach (var d in s.Days)
        {
            await _sql.ExecuteCmd($@"insert into {pisdb}.Leaveapplicationdtl (LeaveApplicationId, EmpmasId, EmpNumber, Date, DutyType, IsPayable, LeaveDayTypeId)
                                     values (@Id, @EmpmasId, @EmpNumber, @Date, @DutyType, @IsPayable, @LeaveDayTypeId)",
                                  new { Id = id, s.EmpmasId, s.EmpNumber, d.Date, d.DutyType, d.IsPayable, d.LeaveDayTypeId }, conn);
        }

        await _sql.ExecuteCmd($"insert into {pisdb}.LeaveApplicationHist (LvaId, EmpasId, Date, Action) values (@Id, @HrEmpmasId, now(), 'Encoded by HR - Approved')",
                              new { Id = id, HrEmpmasId = hrEmpmasId }, conn);
        return id;
    }

    // -- The approved application of that leave (same employee, type and dates) is Cancelled; its leave punches are taken out --
    public async Task _03CancelPis(int empmasId, int systemId, int leaveTypeId, DateTime start, DateTime end, int hrEmpmasId, string pisdb, string conn)
    {
        var ids = await _sql.FetchData<int, dynamic>(
            $"select Id from {pisdb}.Leaveapplication where EmpmasId = @EmpmasId and LeaveTypeId = @TypeId and LvStart = @Start and LvEnd = @End and Status = 'A'",
            new { EmpmasId = empmasId, TypeId = leaveTypeId, Start = start.Date, End = end.Date }, conn);

        foreach (var id in ids)
        {
            await _sql.ExecuteCmd($"update {pisdb}.Leaveapplication set Status = 'C' where Id = @Id", new { Id = id }, conn);

            if (systemId > 0)
                await _sql.ExecuteCmd($@"delete p from {pisdb}.Attpunches1 p
                                         join {pisdb}.Leaveapplicationdtl d on d.LeaveApplicationId = @Id and date(p.PunchInDate) = date(d.Date)
                                         where p.EmpmasId = @SystemId and p.Status like 'LV%'",
                                      new { Id = id, SystemId = systemId }, conn);

            await _sql.ExecuteCmd($"insert into {pisdb}.LeaveApplicationHist (LvaId, EmpasId, Date, Action) values (@Id, @HrEmpmasId, now(), 'Cancelled by HR')",
                                  new { Id = id, HrEmpmasId = hrEmpmasId }, conn);
        }
    }

    public async Task _04Delete(string lvNumber, string opisdb, string conn)
    {
        await _sql.ExecuteLegacyCmd<dynamic>($"delete from {opisdb}.leave1 where LVNUMBER = @LvNumber", new { LvNumber = lvNumber }, conn);
    }
}

public interface IDA_2306O
{
    Task<List<M2306_Emp>>          _02Employees(string opisdb, string conn);
    Task<List<M2306_LeaveType>>    _02LeaveTypes(string opisdb, string conn);
    Task<List<M2306_Transaction>>  _02History(string empNumber, string opisdb, string conn);
    Task<List<M2306_Transaction>>  _02Transactions(DateTime from, DateTime to, string opisdb, string conn);
    Task<int>                      _02Overlapping(string empNumber, DateTime start, DateTime end, string? exceptLvNumber, string opisdb, string conn);
    Task<M2306_EmpInfo?>           _02Employee(string empNumber, string opisdb, string conn);
    Task<List<M2306_EmpInfo>>      _02EmployeeSearch(string keyword, string opisdb, string conn);
    Task<List<M2306_Status>>       _02LeaveStatuses(string opisdb, string conn);
    Task<string>                   _02NextNumber(DateTime lvDate, string opisdb, string conn);
    Task                           _03Update(string lvNumber, M2306_Save s, string? changeBy, string opisdb, string conn);
    Task                           _03EmpStatus(string empNumber, string statusCode, DateTime statusDate, string? user, string opisdb, string conn);
    Task<string>                   _01Save(M2306_Save s, string? encodeBy, string opisdb, string conn);
    Task                           _04Delete(string lvNumber, string opisdb, string conn);
    Task<M2306_PisEmp?>            _02PisEmp(string empNumber, string pisdb, string conn);
    Task<int>                      _02PisLeaveTypeId(string leaveCode, string pisdb, string conn);
    Task<int>                      _01SavePis(M2306_PisSave s, int hrEmpmasId, string pisdb, string conn);
    Task                           _03CancelPis(int empmasId, int systemId, int leaveTypeId, DateTime start, DateTime end, int hrEmpmasId, string pisdb, string conn);
}

public class M2306_Emp
{
    public string?  EmpNumber { get; set; }
    public string?  EmpName   { get; set; }
}

public class M2306_PisEmp
{
    public int EmpmasId { get; set; }
    public int SystemId { get; set; }
}

public class M2306_PisDay
{
    public DateTime Date           { get; set; }
    public string   DutyType       { get; set; } = "R";
    public int      IsPayable      { get; set; }
    public int      LeaveDayTypeId { get; set; } = 1;     // 1 whole day, 2 first half, 3 second half
}

public class M2306_PisSave
{
    public int       Yr           { get; set; }
    public int       EmpmasId     { get; set; }
    public string    EmpNumber    { get; set; } = "";
    public DateTime  DateApplied  { get; set; }
    public int       LeaveTypeId  { get; set; }
    public double    LvBalance    { get; set; }
    public double    DaysCnt      { get; set; }
    public double    DaysWithPay  { get; set; }
    public DateTime  LvStart      { get; set; }
    public DateTime  LvEnd        { get; set; }
    public string?   Reason       { get; set; }
    public List<M2306_PisDay> Days { get; set; } = [];
}

public class M2306_EmpInfo
{
    public string?  EmpNumber { get; set; }
    public string?  EmpName   { get; set; }
    public string?  Status    { get; set; }
    public string?  Client    { get; set; }
}

public class M2306_Status
{
    public string?  Code { get; set; }
    public string?  Name { get; set; }
}

public class M2306_LeaveType
{
    public string?  Code { get; set; }
    public string?  Name { get; set; }
}

public class M2306_Transaction
{
    public string?   LvNumber  { get; set; }
    public DateTime? LvDate    { get; set; }
    public string?   EmpNumber { get; set; }
    public string?   EmpName   { get; set; }
    public string?   LeaveCode { get; set; }
    public string?   LeaveName { get; set; }
    public DateTime? LvStart   { get; set; }
    public DateTime? LvEnd     { get; set; }
    public double?   NoDays    { get; set; }
    public string?   WithPay   { get; set; }
    public double?   DaysWithPay { get; set; }
}

public class M2306_Save
{
    public DateTime  LvDate    { get; set; } = DateTime.Today;
    public string    EmpNumber { get; set; } = "";
    public string    LeaveCode { get; set; } = "";
    public DateTime  LvStart   { get; set; }
    public DateTime  LvEnd     { get; set; }
    public double    NoDays    { get; set; }
    public double    DaysWithPay { get; set; }
}
