using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class AttpunchesDataAccess : IAttpunchesDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttpunchesDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttpunchesModel?> _01In(AttpunchesModel attpunches, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Attpunches 
                            (EmpmasId,  PunchDate,  DayNo, Action,  PunchT,  DutyTypeId,  TimeZoneId,  IpAddress,  MacAddress,  UserId) values 
                            (@Empmasid, @Punchdate, @Dayno, @Action, @Puncht, @Dutytypeid, @Timezoneid, @Ipaddress, @Macaddress, @Userid);
                        SELECT * FROM {schema}.Attpunches WHERE EmpmasId = @Empmasid and PunchDate = @Punchdate;";

        var res = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, attpunches, conn);

        return res.FirstOrDefault();

    }


    public async Task<AttpunchesModel?> _02(int empmasid, DateTime punchDate, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Attpunches where EmpmasId = @Empmasid and PunchDate = Date(@Punchdate) ";
        var data = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, new { Empmasid = empmasid, PunchDate = punchDate }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<AttpunchesModel?>?> _02s(int empmasid, DateTime punchDate, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Attpunches where EmpmasId = @Empmasid and PunchDate = Date(@Punchdate) ";
        var data = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, new { Empmasid = empmasid, PunchDate = punchDate }, conn);
        return data;
    }


    public async Task<AttpunchesModel?> _03(AttpunchesModel attpunches, string schema, string conn)
    {
        string sql = $@"Update {schema}.Attpunches set 
                            DayNo       = @DayNo,  
                            EmpmasId    = @Empmasid,  
                            PunchDate   = @Punchdate,  
                            Action      = @Action,  
                            PunchT      = @Puncht,  
                            DutyTypeId  = @Dutytypeid,  
                            TimeZoneId  = @Timezoneid,  
                            IpAddress   = @Ipaddress,  
                            MacAddress  = @Macaddress where Empmasid = @Empmasid and PunchDate = Date(@Punchdate) ;
                        select  * from {schema}.Attpunches where Empmasid = @Empmasid and PunchDate = Date(@Punchdate)";
        var data = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, attpunches, conn);
        return data?.FirstOrDefault();
    }

    public async Task<AttpunchesModel?> _04(int empmasid, DateTime punchDate, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Attpunches where Empmasid = @Empmasid and PunchDate = Date(@Punchdate); 
                        select  * from {schema}.Attpunches where Empmasid = @Empmasid and PunchDate = Date(@Punchdate)";
        var data = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, new { Empmasid = empmasid, PunchDate = punchDate }, conn);
        return data?.FirstOrDefault();
    }
}