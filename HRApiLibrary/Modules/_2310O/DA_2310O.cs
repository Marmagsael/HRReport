using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._2310O;

// -- OB Entry : HR encodes the official business of an employee. It is saved in the old PIS (the source of truth) : {OPis}.ob1 --
// -- and in the new PIS as an approved application (ObApplication, keyed by Empmas.Id) so it shows in the portal of the employee --
public class DA_2310O : IDA_2310O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_2310O(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    private const string Cols = @"o.OBNUMBER ObNumber, o.OBDATE ObDate, trim(o.EMPNUMBER) EmpNumber,
                               concat(trim(ifnull(e.EmpLastNm, '')), ', ', trim(ifnull(e.EmpFirstNm, '')), ' ', trim(ifnull(e.EmpMidNm, ''))) EmpName,
                               o.`START` ObStart, o.`END` ObEnd, o.NO_DAYS NoDays, ifnull(o.DAYS_WITHPAY, 0) DaysWithPay,
                               o.DESTINATION Destination, o.PURPOSE Purpose, o.CONTACTPERSON ContactPerson, o.TELNO TelNo, o.REMARKS Remarks";

    // -- OB transactions that overlap the dates --
    public async Task<List<M2310_Transaction>> _02Transactions(DateTime from, DateTime to, string opisdb, string conn)
    {
        string sql = $@"select {Cols}
                        from   {opisdb}.ob1 o left join {opisdb}.Empmas e on e.EmpNumber = o.EMPNUMBER
                        where  o.`START` <= @To and o.`END` >= @From
                        order by o.`START` desc, o.OBNUMBER desc";
        return await _sql.FetchLegacy<M2310_Transaction, dynamic>(sql, new { From = from.Date, To = to.Date }, conn) ?? [];
    }

    // -- OB history of one employee, the latest first --
    public async Task<List<M2310_Transaction>> _02History(string empNumber, string opisdb, string conn)
    {
        string sql = $@"select {Cols}
                        from   {opisdb}.ob1 o left join {opisdb}.Empmas e on e.EmpNumber = o.EMPNUMBER
                        where  o.EMPNUMBER = @EmpNumber
                        order by o.`START` desc, o.OBNUMBER desc";
        return await _sql.FetchLegacy<M2310_Transaction, dynamic>(sql, new { EmpNumber = empNumber }, conn) ?? [];
    }

    // -- The employee already has an OB on those dates --
    public async Task<int> _02Overlapping(string empNumber, DateTime start, DateTime end, string? exceptObNumber, string opisdb, string conn)
    {
        string sql = $"select count(*) from {opisdb}.ob1 where EMPNUMBER = @EmpNumber and `START` <= @End and `END` >= @Start and OBNUMBER <> @Except";
        var r = await _sql.FetchLegacy<long, dynamic>(sql, new { EmpNumber = empNumber, Start = start.Date, End = end.Date, Except = exceptObNumber ?? "" }, conn);
        return (int)r.FirstOrDefault();
    }

    // -- OBNUMBER = yy-MM of the date filed + the running number of that month --
    public async Task<string> _02NextNumber(DateTime obDate, string opisdb, string conn)
    {
        string prefix = obDate.ToString("yy-MM");
        var maxes = await _sql.FetchLegacy<long, dynamic>(
            $"select ifnull(max(cast(right(OBNUMBER, 3) as unsigned)), 0) from {opisdb}.ob1 where OBNUMBER like @Like",
            new { Like = prefix + "%" }, conn);
        return prefix + (maxes.FirstOrDefault() + 1).ToString("000");
    }

    public async Task<string> _01Save(M2310_Save s, string? encodeBy, string opisdb, string conn)
    {
        string obNumber = await _02NextNumber(s.ObDate, opisdb, conn);
        string sql = $@"insert into {opisdb}.ob1
                            (OBNUMBER, OBDATE, EMPNUMBER, `START`, `END`, NO_DAYS, DAYS_WITHPAY, DESTINATION, PURPOSE, CONTACTPERSON, TELNO, REMARKS, ENCODEBY, ENCODEDT, `mode`)
                        values
                            (@ObNumber, @ObDate, @EmpNumber, @ObStart, @ObEnd, @NoDays, @DaysWithPay, @Destination, @Purpose, @ContactPerson, @TelNo, @Remarks, @EncodeBy, @EncodeDt, 'O')";
        await _sql.ExecuteLegacyCmd<dynamic>(sql, new
        {
            ObNumber = obNumber, ObDate = s.ObDate.Date, s.EmpNumber, ObStart = s.ObStart.Date, ObEnd = s.ObEnd.Date, s.NoDays, s.DaysWithPay,
            s.Destination, s.Purpose, s.ContactPerson, s.TelNo, s.Remarks, EncodeBy = encodeBy?.Trim(), EncodeDt = DateTime.Now.ToString("yyyy-MM-dd")
        }, conn);
        return obNumber;
    }

    public async Task _03Update(string obNumber, M2310_Save s, string? changeBy, string opisdb, string conn)
    {
        string sql = $@"update {opisdb}.ob1 set OBDATE = @ObDate, EMPNUMBER = @EmpNumber, `START` = @ObStart, `END` = @ObEnd, NO_DAYS = @NoDays, DAYS_WITHPAY = @DaysWithPay,
                               DESTINATION = @Destination, PURPOSE = @Purpose, CONTACTPERSON = @ContactPerson, TELNO = @TelNo, REMARKS = @Remarks,
                               CHANGEBY = @ChangeBy, CHANGEDT = @ChangeDt
                        where OBNUMBER = @ObNumber";
        await _sql.ExecuteLegacyCmd<dynamic>(sql, new
        {
            ObNumber = obNumber, ObDate = s.ObDate.Date, s.EmpNumber, ObStart = s.ObStart.Date, ObEnd = s.ObEnd.Date, s.NoDays, s.DaysWithPay,
            s.Destination, s.Purpose, s.ContactPerson, s.TelNo, s.Remarks, ChangeBy = changeBy?.Trim(), ChangeDt = DateTime.Now.ToString("yyyy-MM-dd")
        }, conn);
    }

    public async Task _04Delete(string obNumber, string opisdb, string conn)
    {
        await _sql.ExecuteLegacyCmd<dynamic>($"delete from {opisdb}.ob1 where OBNUMBER = @ObNumber", new { ObNumber = obNumber }, conn);
    }

    // =========== PIS side ===========

    // -- Approved at once (HR encoded it) : header, days and history. Returns the application Id. --
    public async Task<int> _01SavePis(M2310_PisSave s, int hrEmpmasId, string pisdb, string conn)
    {
        string sql = $@"insert into {pisdb}.ObApplication
                            (Yr, EmpmasId, DateApplied, ObStart, ObEnd, Destination, Purpose, ContactPerson, TelNo, Remarks, DaysCnt, DaysWithPay,
                             Approver1Id, Approver2Id, ApproverLevel, DateApprove2, Status)
                        values
                            (@Yr, @EmpmasId, @DateApplied, @ObStart, @ObEnd, @Destination, @Purpose, @ContactPerson, @TelNo, @Remarks, @DaysCnt, @DaysWithPay,
                             0, @HrEmpmasId, 2, now(), 'A');
                        select last_insert_id();";
        var ids = await _sql.FetchData<long, dynamic>(sql, new
        {
            s.Yr, s.EmpmasId, s.DateApplied, s.ObStart, s.ObEnd, s.Destination, s.Purpose, s.ContactPerson, s.TelNo, s.Remarks, s.DaysCnt, s.DaysWithPay,
            HrEmpmasId = hrEmpmasId
        }, conn);
        var id = (int)ids.First();

        foreach (var d in s.Days)
        {
            await _sql.ExecuteCmd($@"insert into {pisdb}.ObApplicationDtl (ObApplicationId, EmpmasId, Date, DutyType, ObDurationId, IsPayable)
                                     values (@Id, @EmpmasId, @Date, @DutyType, @ObDurationId, @IsPayable)",
                                  new { Id = id, s.EmpmasId, d.Date, d.DutyType, d.ObDurationId, d.IsPayable }, conn);
        }

        await _sql.ExecuteCmd($"insert into {pisdb}.ObApplicationHist (ObaId, EmpmasId, Date, Action) values (@Id, @HrEmpmasId, now(), 'Encoded by HR - Approved')",
                              new { Id = id, HrEmpmasId = hrEmpmasId }, conn);
        return id;
    }

    // -- The approved application of that OB (same employee and dates) is Cancelled; its OB punches are taken out --
    public async Task _03CancelPis(int empmasId, int systemId, DateTime start, DateTime end, int hrEmpmasId, string pisdb, string conn)
    {
        var ids = await _sql.FetchData<int, dynamic>(
            $"select Id from {pisdb}.ObApplication where EmpmasId = @EmpmasId and ObStart = @Start and ObEnd = @End and Status = 'A'",
            new { EmpmasId = empmasId, Start = start.Date, End = end.Date }, conn);

        foreach (var id in ids)
        {
            await _sql.ExecuteCmd($"update {pisdb}.ObApplication set Status = 'C' where Id = @Id", new { Id = id }, conn);

            if (systemId > 0)
                await _sql.ExecuteCmd($@"delete p from {pisdb}.Attpunches1 p
                                         join {pisdb}.ObApplicationDtl d on d.ObApplicationId = @Id and date(p.PunchInDate) = date(d.Date)
                                         where p.EmpmasId = @SystemId and p.Status like 'OB%'",
                                      new { Id = id, SystemId = systemId }, conn);

            await _sql.ExecuteCmd($"insert into {pisdb}.ObApplicationHist (ObaId, EmpmasId, Date, Action) values (@Id, @HrEmpmasId, now(), 'Cancelled by HR')",
                                  new { Id = id, HrEmpmasId = hrEmpmasId }, conn);
        }
    }
}

public interface IDA_2310O
{
    Task<List<M2310_Transaction>>  _02Transactions(DateTime from, DateTime to, string opisdb, string conn);
    Task<List<M2310_Transaction>>  _02History(string empNumber, string opisdb, string conn);
    Task<int>                      _02Overlapping(string empNumber, DateTime start, DateTime end, string? exceptObNumber, string opisdb, string conn);
    Task<string>                   _02NextNumber(DateTime obDate, string opisdb, string conn);
    Task<string>                   _01Save(M2310_Save s, string? encodeBy, string opisdb, string conn);
    Task                           _03Update(string obNumber, M2310_Save s, string? changeBy, string opisdb, string conn);
    Task                           _04Delete(string obNumber, string opisdb, string conn);
    Task<int>                      _01SavePis(M2310_PisSave s, int hrEmpmasId, string pisdb, string conn);
    Task                           _03CancelPis(int empmasId, int systemId, DateTime start, DateTime end, int hrEmpmasId, string pisdb, string conn);
}

public class M2310_Transaction
{
    public string?   ObNumber      { get; set; }
    public DateTime? ObDate        { get; set; }
    public string?   EmpNumber     { get; set; }
    public string?   EmpName       { get; set; }
    public DateTime? ObStart       { get; set; }
    public DateTime? ObEnd         { get; set; }
    public double?   NoDays        { get; set; }
    public double?   DaysWithPay   { get; set; }
    public string?   Destination   { get; set; }
    public string?   Purpose       { get; set; }
    public string?   ContactPerson { get; set; }
    public string?   TelNo         { get; set; }
    public string?   Remarks       { get; set; }
}

public class M2310_Save
{
    public DateTime  ObDate        { get; set; } = DateTime.Today;
    public string    EmpNumber     { get; set; } = "";
    public DateTime  ObStart       { get; set; }
    public DateTime  ObEnd         { get; set; }
    public double    NoDays        { get; set; }
    public double    DaysWithPay   { get; set; }
    public string?   Destination   { get; set; }
    public string?   Purpose       { get; set; }
    public string?   ContactPerson { get; set; }
    public string?   TelNo         { get; set; }
    public string?   Remarks       { get; set; }
}

public class M2310_PisDay
{
    public DateTime Date         { get; set; }
    public string   DutyType     { get; set; } = "R";
    public int      IsPayable    { get; set; }
    public int      ObDurationId { get; set; } = 1;     // 1 whole day, 2 first half, 3 second half
}

public class M2310_PisSave
{
    public int       Yr            { get; set; }
    public int       EmpmasId      { get; set; }
    public DateTime  DateApplied   { get; set; }
    public DateTime  ObStart       { get; set; }
    public DateTime  ObEnd         { get; set; }
    public string?   Destination   { get; set; }
    public string?   Purpose       { get; set; }
    public string?   ContactPerson { get; set; }
    public string?   TelNo         { get; set; }
    public string?   Remarks       { get; set; }
    public double    DaysCnt       { get; set; }
    public double    DaysWithPay   { get; set; }
    public List<M2310_PisDay> Days { get; set; } = [];
}
