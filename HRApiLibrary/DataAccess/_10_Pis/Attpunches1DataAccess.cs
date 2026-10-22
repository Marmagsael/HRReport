using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class Attpunches1DataAccess : IAttpunches1DataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public Attpunches1DataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<Attpunches1Model?> _01(Attpunches1Model attpunches1, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Attpunches1 
							(EmpmasId,  DayNo,  PunchInDate,  PunchT,  SchedDuration,  DutyTypeId,  TimeZoneIdIn,  IpAddressIn,  MacAddressIn,  UserIdIn,  Status  ) values 
							(@EmpmasId, @DayNo, @PunchInDate, @PunchT, @SchedDuration, @DutyTypeId, @TimeZoneIdIn, @IpAddressIn, @MacAddressIn, @UserIdIn, @Status );";
        await _sql.ExecuteCmd<dynamic>(sql, attpunches1, conn);

        sql = $@"SELECT * FROM {schema}.Attpunches1 WHERE EmpmasId=@EmpmasId and PunchInDate=@PunchInDate";
        var res = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, attpunches1, conn);
        return res.FirstOrDefault();
    }

    public async Task<List<Attpunches1Model?>?> _02LastPunches(int? empmasId, int? reccount, string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.Attpunches1 where EmpmasId=@EmpmasId order by PunchInDate desc limit @Reccount "; ;
        var data = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, new { EmpmasId = empmasId, Reccount = reccount }, conn);
        return data;
    }
    
    public async Task<List<Attpunches1Model?>?> _02ByIdByRange( int? empmasId, DateOnly dstart, DateOnly dend, string? schema, string? conn) 
    {
        string? sql = $@"select * from {schema}.Attpunches1 
                        where EmpmasId = @EmpmasId and PunchInDate >= @DStart and PunchInDate <  @DEnd
                        order by PunchInDate";

        DateTime start = dstart.ToDateTime(TimeOnly.MinValue);
        DateTime end   = dend.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var data = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, new { EmpmasId = empmasId, DStart = start, DEnd = end }, conn);
        return data;

    }

    public async Task<List<Attpunches1Model?>?> _02NoPunchOut(int? empmasId, string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.Attpunches1 where EmpmasId=@EmpmasId and status != 'L' order by PunchInDate desc  "; ;
        var data = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }


    public async Task<List<Attpunches1Model?>?> _03s(Attpunches1Model attpunches1, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Attpunches1 set 
							PunchOutDate 	= @PunchOutDate, 
							TimeZoneIdOut 	= @TimeZoneIdOut, 
							IpAddressOut 	= @IpAddressOut, 
							MacAddressOut 	= @MacAddressOut, 
							UserIdOut 		= @UserIdOut, 
                            Status          = @Status 
						where EmpmasId = @EmpmasId and PunchInDate = @PunchInDate;";
        await _sql.ExecuteCmd<dynamic>(sql, attpunches1, conn);

        sql = $@" select  * from {schema}.Attpunches1 where EmpmasId = @EmpmasId and PunchInDate = @PunchInDate;";
        var data = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, attpunches1, conn);
        return data;
    }

    // --- Final approval of OB / Leave : one Attpunches1 row per payable day
    //     dayTypeId : 1 Whole day   = sched in  -> sched out
    //                 2 First-Half  = sched in  -> sched in + 4h
    //                 3 Second-Half = sched out - 4h -> sched out
    //     status    : AttPunchStatus.OB(...) / AttPunchStatus.LV(...)
    public async Task _01Approved(int? empmasId, IEnumerable<(DateTime Date, int? DayTypeId, string Status)> days, int? approverUserId,
                                  int? timeZone, string? ipAdd, string? macAddress, string? schema, string? conn)
    {
        string? sql = $@"select * from {schema}.Atttemplate where EmpmasId = @EmpmasId";
        var at = (await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn)).FirstOrDefault()
                 ?? new AtttemplateModel { EmpmasId = empmasId };     // model defaults : Mon-Fri 08:00, 9h

        // --- timeZone / ipAdd / macAddress : client terminal of the final approver ( ClientTerminalService )

        sql = $@"insert into {schema}.Attpunches1
                    (EmpmasId,  DayNo,  PunchInDate,  PunchT,  SchedDuration,  DutyTypeId,  TimeZoneIdIn,  IpAddressIn,  MacAddressIn,  UserIdIn,
                     PunchOutDate,  TimeZoneIdOut,  IpAddressOut,  MacAddressOut,  UserIdOut,  Status) values
                    (@EmpmasId, @DayNo, @PunchInDate, @PunchT, @SchedDuration, @DutyTypeId, @TimeZoneIdIn, @IpAddressIn, @MacAddressIn, @UserIdIn,
                     @PunchOutDate, @TimeZoneIdOut, @IpAddressOut, @MacAddressOut, @UserIdOut, @Status)
                 on duplicate key update
                    PunchT          = @PunchT,
                    SchedDuration   = @SchedDuration,
                    DutyTypeId      = @DutyTypeId,
                    TimeZoneIdIn    = @TimeZoneIdIn,
                    IpAddressIn     = @IpAddressIn,
                    MacAddressIn    = @MacAddressIn,
                    UserIdIn        = @UserIdIn,
                    PunchOutDate    = @PunchOutDate,
                    TimeZoneIdOut   = @TimeZoneIdOut,
                    IpAddressOut    = @IpAddressOut,
                    MacAddressOut   = @MacAddressOut,
                    UserIdOut       = @UserIdOut,
                    Status          = @Status ;";

        foreach (var d in days)
        {
            var dayNo = MsdsDataAccess.GetDayNo(d.Date.DayOfWeek.ToString());
            var (inHHmm, lenHHmm, dutyType) = dayNo switch {
                1 => (at.D1_in ?? 0, at.D1_hrslength ?? 0, at.D1_dutytype ?? "RD"),
                2 => (at.D2_in ?? 0, at.D2_hrslength ?? 0, at.D2_dutytype ?? "RD"),
                3 => (at.D3_in ?? 0, at.D3_hrslength ?? 0, at.D3_dutytype ?? "RD"),
                4 => (at.D4_in ?? 0, at.D4_hrslength ?? 0, at.D4_dutytype ?? "RD"),
                5 => (at.D5_in ?? 0, at.D5_hrslength ?? 0, at.D5_dutytype ?? "RD"),
                6 => (at.D6_in ?? 0, at.D6_hrslength ?? 0, at.D6_dutytype ?? "RD"),
                _ => (at.D7_in ?? 0, at.D7_hrslength ?? 0, at.D7_dutytype ?? "RD"),
            };

            // --- schedule values are HHmm ints ( 800 = 08:00, 900 = 9h00m )
            DateTime schedIn  = d.Date.Date + new TimeSpan(inHHmm / 100, inHHmm % 100, 0);
            DateTime schedOut = schedIn     + new TimeSpan(lenHHmm / 100, lenHHmm % 100, 0);

            var (pin, pout) = d.DayTypeId switch {
                2 => (schedIn,               schedIn.AddHours(4)),
                3 => (schedOut.AddHours(-4), schedOut),
                _ => (schedIn,               schedOut)
            };

            var ap = new Attpunches1Model {
                EmpmasId        = empmasId,
                DayNo           = dayNo,
                PunchInDate     = pin,
                PunchT          = inHHmm,
                SchedDuration   = lenHHmm,
                DutyTypeId      = dutyType == "R" ? 1 : 2,
                TimeZoneIdIn    = timeZone,
                IpAddressIn     = ipAdd,
                MacAddressIn    = macAddress,
                UserIdIn        = approverUserId,
                PunchOutDate    = pout,
                TimeZoneIdOut   = timeZone,
                IpAddressOut    = ipAdd,
                MacAddressOut   = macAddress,
                UserIdOut       = approverUserId,
                Status          = d.Status
            };
            await _sql.ExecuteCmd<dynamic>(sql, ap, conn);
        }
    }

    public async Task _04(int? empmasId, DateTime punchInDate, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Attpunches1 where EmpmasId = @EmpmasId and PunchInDate = @PunchInDate;";
        await _sql.ExecuteCmd<dynamic>(sql, new { EmpmasId = empmasId, PunchInDate = punchInDate }, conn);
    }
}

public interface IAttpunches1DataAccess
{
    Task<Attpunches1Model?>             _01(Attpunches1Model attpunches1, string? schema, string? conn);
    Task                                _01Approved(int? empmasId, IEnumerable<(DateTime Date, int? DayTypeId, string Status)> days, int? approverUserId,
                                                int? timeZone, string? ipAdd, string? macAddress, string? schema, string? conn);
    Task<List<Attpunches1Model?>?>      _02LastPunches(int? empmasId, int? reccount, string? schema, string? conn);
    Task<List<Attpunches1Model?>?>      _02NoPunchOut(int? empmasId, string? schema, string? conn);
    Task<List<Attpunches1Model?>?>      _02ByIdByRange( int? empmasId, DateOnly dstart, DateOnly dend, string? schema, string? conn) ; 
    Task<List<Attpunches1Model?>?>      _03s(Attpunches1Model attpunches1, string? schema, string? conn);
    Task                                _04(int? empmasId, DateTime punchInDate, string? schema, string? conn);
}