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

    public async Task _04(int? empmasId, DateTime punchInDate, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Attpunches1 where EmpmasId = @EmpmasId and PunchInDate = @PunchInDate;";
        await _sql.ExecuteCmd<dynamic>(sql, new { EmpmasId = empmasId, PunchInDate = punchInDate }, conn);
    }
}

public interface IAttpunches1DataAccess
{
    Task<Attpunches1Model?>             _01(Attpunches1Model attpunches1, string? schema, string? conn);
    Task<List<Attpunches1Model?>?>      _02LastPunches(int? empmasId, int? reccount, string? schema, string? conn);
    Task<List<Attpunches1Model?>?>      _02NoPunchOut(int? empmasId, string? schema, string? conn);
    Task<List<Attpunches1Model?>?>      _02ByIdByRange( int? empmasId, DateOnly dstart, DateOnly dend, string? schema, string? conn) ; 
    Task<List<Attpunches1Model?>?>      _03s(Attpunches1Model attpunches1, string? schema, string? conn);
    Task                                _04(int? empmasId, DateTime punchInDate, string? schema, string? conn);
}