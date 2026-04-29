using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis.Attendance;

public class Attpunches1DataAccess : IAttpunches1DataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public Attpunches1DataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<Attpunches1Model?> _01In(Attpunches1Model attpunches, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Attpunches 
                            (
                                EmpmasId, DayNo, PunchInDate, PunchT,
                                SchedDuration, DutyTypeId,
                                TimeZoneIdIn, IpAddressIn, MacAddressIn, UserIdIn,
                                PunchOutDate, TimeZoneIdOut, IpAddressOut, MacAddressOut, UserIdOut,
                                Status
                            )
                                VALUES
                                (
                                    @EmpmasId, @DayNo, @PunchInDate, @PunchT,
                                    @SchedDuration, @DutyTypeId,
                                    @TimeZoneIdIn, @IpAddressIn, @MacAddressIn, @UserIdIn,
                                    @PunchOutDate, @TimeZoneIdOut, @IpAddressOut, @MacAddressOut, @UserIdOut,
                                    @Status
                                );
                        SELECT * FROM {schema}.Attpunches1 WHERE EmpmasId = @Empmasid and PunchDate = @Punchdate;";

        var res = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, attpunches, conn);

        return res.FirstOrDefault();

    }


    public async Task<Attpunches1Model?> _02(int empmasid, DateTime punchDate, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Attpunches1 where EmpmasId = @Empmasid and PunchDate = Date(@Punchdate) ";
        var data = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, new { Empmasid = empmasid, PunchDate = punchDate }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<Attpunches1Model?>?> _02s(int empmasid, DateTime punchDate, string schema, string conn)
    {
        string sql = $@"select  a.*, d.Code DutyType from {schema}.Attpunches1 a
                    LEFT JOIN {schema}.attdutytype d on d.id =  a.dutytypeid  
                    where EmpmasId = @Empmasid and PunchDate = Date(@Punchdate) ";
        var data = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, new { Empmasid = empmasid, PunchDate = punchDate }, conn);
        return data;
    }

    public async Task<List<Attpunches1Model?>?> _02ByMonthAndYear(int empmasid, int month, int year, string schema,string conn)
    {
        DateTime dStart = new DateTime(year, month, 1);
        DateTime dEnd = dStart.AddMonths(1);

        string sql = $@" SELECT a.*, d.Code DutyType from {schema}.Attpunches1 a
                    LEFT JOIN {schema}.attdutytype d on d.id =  a.dutytypeid  
                    WHERE EmpmasId = @EmpmasId AND PunchInDate >= @DStart AND PunchInDate < @DEnd";

        var data = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, new { EmpmasId = empmasid, DStart = dStart, DEnd = dEnd},conn);
        return data;
    }



    public async Task<Attpunches1Model?> _03(Attpunches1Model attpunches, string schema, string conn)
    {
        string sql = $@"Update {schema}.Attpunches1 set 
                            EmpmasId        = @EmpmasId,
                            DayNo           = @DayNo,
                            PunchInDate     = @PunchInDate,
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

                            Status          = @Status 
                            where Empmasid = @Empmasid and PunchDate = Date(@Punchdate) ;
                        select  * from {schema}.Attpunches where Empmasid = @Empmasid and PunchDate = Date(@Punchdate)";
        var data = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, attpunches, conn);
        return data?.FirstOrDefault();
    }

    public async Task<Attpunches1Model?> _04(int empmasid, DateTime punchDate, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Attpunches1 where Empmasid = @Empmasid and PunchDate = Date(@Punchdate); 
                        select  * from {schema}.Attpunches where Empmasid = @Empmasid and PunchDate = Date(@Punchdate)";
        var data = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, new { Empmasid = empmasid, PunchDate = punchDate }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IAttpunches1DataAccess
{
    Task<Attpunches1Model?> _01In(Attpunches1Model attpunches, string schema, string conn);
    Task<Attpunches1Model?> _02(int empmasid, DateTime punchDate, string schema, string conn);
    Task<List<Attpunches1Model?>?> _02s(int empmasid, DateTime punchDate, string schema, string conn);
    Task<List<Attpunches1Model?>?> _02ByMonthAndYear(int empmasid, int month, int year, string schema, string conn);
    Task<Attpunches1Model?> _03(Attpunches1Model attpunches, string schema, string conn);
    Task<Attpunches1Model?> _04(int empmasid, DateTime punchDate, string schema, string conn);
}