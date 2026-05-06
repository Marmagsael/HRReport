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

    public async Task<AttpunchesModel?> _01In(AttpunchesModel attpunches, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Attpunches 
                            (EmpmasId,  PunchDate,  DayNo, Action,  PunchT,  DutyTypeId,  TimeZoneId,  IpAddress,  MacAddress,  UserId) values 
                            (@EmpmasId, @Punchdate, @Dayno, @Action, @Puncht, @Dutytypeid, @Timezoneid, @Ipaddress, @Macaddress, @Userid);
                        SELECT * FROM {schema}.Attpunches WHERE EmpmasId = @EmpmasId and PunchDate = @Punchdate;";

        var res = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, attpunches, conn);

        return res.FirstOrDefault();

    }

    
    public async Task<AttpunchesModel?> _02(int? empmasid, DateTime punchDate, string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.Attpunches where EmpmasId = @EmpmasId and PunchDate = Date(@Punchdate) ";
        var data = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, new { EmpmasId = empmasid, PunchDate = punchDate }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<AttpunchesModel?> _02LastPunches(int? empmasid, int? reccount, string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.Attpunches where EmpmasId = @EmpmasId order by PunchDate desc limit @Reccount ";
        var data = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, new { EmpmasId = empmasid, Reccount = reccount }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<AttpunchesModel?>?> _02s(int? empmasid, DateTime punchDate, string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.Attpunches where EmpmasId = @EmpmasId and PunchDate = Date(@Punchdate) ";
        var data = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, new { EmpmasId = empmasid, PunchDate = punchDate }, conn);
        return data;
    }


    public async Task<AttpunchesModel?> _03(AttpunchesModel attpunches, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Attpunches set 
                            DayNo       = @DayNo,  
                            EmpmasId    = @EmpmasId,  
                            PunchDate   = @Punchdate,  
                            Action      = @Action,  
                            PunchT      = @Puncht,  
                            DutyTypeId  = @Dutytypeid,  
                            TimeZoneId  = @Timezoneid,  
                            IpAddress   = @Ipaddress,  
                            MacAddress  = @Macaddress where EmpmasId = @EmpmasId and PunchDate = Date(@Punchdate) ;
                        select  * from {schema}.Attpunches where EmpmasId = @EmpmasId and PunchDate = Date(@Punchdate)";
        var data = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, attpunches, conn);
        return data?.FirstOrDefault();
    }

    public async Task<AttpunchesModel?> _04(int? empmasid, DateTime punchDate, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Attpunches where EmpmasId = @EmpmasId and PunchDate = Date(@Punchdate); 
                        select  * from {schema}.Attpunches where EmpmasId = @EmpmasId and PunchDate = Date(@Punchdate)";
        var data = await _sql.FetchData<AttpunchesModel?, dynamic>(sql, new { EmpmasId = empmasid, PunchDate = punchDate }, conn);
        return data?.FirstOrDefault();
    }
}
