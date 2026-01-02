using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class AttdailyDataAccess : IAttdailyDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public AttdailyDataAccess(I_90_001_MySqlDataAccess sql)
    { _sql = sql; }
    public async Task<AttdailyModel?> _01(AttdailyModel attdaily, string schema, string conn)
    {
        var sql = $@"Insert into {schema}.Attdaily 
    					(EmpmasId,  EmpNumber,  PunchDate,  DayNo,  TimeIn,  TimeInT,  TimeOut,  TimeOutT,  DutyTypeId,  InById,  OutById) values 
    					(@EmpmasId, @EmpNumber, @PunchDate, @DayNo, @TimeIn, @TimeInT, @TimeOut, @TimeOutT, @DutyTypeId, @InById, @OutById)";
        await _sql.ExecuteCmd<dynamic>(sql, attdaily, conn);
        sql = $@"SELECT * FROM {schema}.Attdaily WHERE EmpmasId = @EmpmasId and PunchDate = @PunchDate";
        var res = await _sql.FetchData<AttdailyModel?, dynamic>(sql, attdaily, conn);
        return res.FirstOrDefault();
    }

    public async Task<AttdailyModel?> _01PunchIn(AttdailyModel attdaily, string schema, string conn)
    {
        var sql = $@"Insert into {schema}.Attdaily 
    					(EmpmasId,  EmpNumber,  PunchDate,          DayNo,  TimeIn,  TimeInT,  DutyTypeId,  InById) values 
    					(@EmpmasId, @Empnumber, date(@Punchdate),   @DayNo, now(),  @Timeint,  @Dutytypeid, @Inbyid)
    					on duplicate key update TimeInT = @Timeint, TimeIn = now(), InById = @Inbyid; 
					SELECT * FROM {schema}.Attdaily WHERE EmpmasId = @Empmasid and PunchDate = @Punchdate; ";
        var res = await _sql.FetchData<AttdailyModel?, dynamic>(sql, attdaily, conn);
        return res.FirstOrDefault();
    }

    public async Task<AttdailyModel?> _01PunchOut(AttdailyModel attdaily, string schema, string conn)
    {
        var sql = $@"Insert into {schema}.Attdaily 
    					(EmpmasId,  EmpNumber,  PunchDate,  DayNo,  TimeOut,  TimeOutT,  DutyTypeId,  OutById) values 
    					(@Empmasid, @Empnumber, @Punchdate, @Dayno, now(),    @Timeoutt, @Dutytypeid, @Outbyid) 
    					on duplicate key update TimeOut=now(), TimeOutT=@Timeoutt,    OutById = @Outbyid; 
					SELECT * FROM {schema}.Attdaily WHERE EmpmasId = @Empmasid and PunchDate = @Punchdate;";
        var res = await _sql.FetchData<AttdailyModel?, dynamic>(sql, attdaily, conn);
        return res.FirstOrDefault();
    }

    public async Task<AttdailyModel?> _02(int id, DateTime punchdate, string schema, string conn)
    {
        var sql = $@"select  * from {schema}.Attdaily where EmpasId = @EmpmasId and PunchDate = @PunchDate";
        var data = await _sql.FetchData<AttdailyModel?, dynamic>(sql, new { EmpmasId = id, PunchDate = punchdate }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<AttdailyModel?> _02CurrPunch(int empmasId, string schema, string conn)
    {
        var sql  = $@"select  * from {schema}.Attdaily where EmpmasId   = @EmpmasId and PunchDate = curdate() 
                     order by PunchDate desc limit 1; ";
        var data = await _sql.FetchData<AttdailyModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<AttdailyModel?> _02PrevPunch(int empmasId, string schema, string conn)
    {
        var sql = $@"select  * from {schema}.Attdaily 
                     where EmpmasId = @EmpmasId and
                           Date(PunchDate) = CURDATE() - INTERVAL 1 DAY
                     order by PunchDate desc limit 1; ";
        var data = await _sql.FetchData<AttdailyModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<AttdailyModel?>?> _02ByMonth(int empmasId, int yr, int month, string schema, string conn)
    {
        var sql = $@"select  * from {schema}.Attdaily 
                     where EmpmasId   = @EmpmasId and year(PunchDate) = @Myr and  month(PunchDate) = @Mmonth 
                     order by PunchDate ; ";
        var data = await _sql.FetchData<AttdailyModel?, dynamic>(sql, new { EmpmasId = empmasId, Myr = yr, Mmonth = month }, conn);
        return data;
    }

    public async Task<AttdailyModel?> _03(int id, AttdailyModel attdaily, string schema, string conn)
    {
        string sql = $@"Update {schema}.Attdaily set 
                               DayNo 		= @DayNo, 
                               TimeIn 		= @TimeIn, 
                               TimeInT 		= @TimeInT, 
                               TimeOut 		= @TimeOut, 
                               TimeOutT 	= @TimeOutT, 
                               DutyTypeId 	= @DutyTypeId, 
                               InById 		= @InById, 
                               OutById 		= @OutById, 
                               IpAddress 	= @IpAddress where EmpmasId = @EmpmasId and PunchDate = @PunchDate;";
        await _sql.ExecuteCmd<dynamic>(sql, attdaily, conn);

        sql = $@" select  * from {schema}.Attdaily where EmpmasId = @EmpmasId and PunchDate = @PunchDate;";
        var data = await _sql.FetchData<AttdailyModel?, dynamic>(sql, attdaily, conn);
        return data?.FirstOrDefault();
    }
    public async Task<AttdailyModel?> _04(int empmasId, DateTime punchDate, string schema, string conn)
    {
        var sql = $@"Delete from {schema}.Attdaily where EmpmasId = @EmpmasId and PunchDate = @PunchDate;
					 select  * from {schema}.Attdaily where EmpmasId = @EmpmasId and PunchDate = @PunchDate;";
        var data = await _sql.FetchData<AttdailyModel?, dynamic>(sql, new { EmpamsId = empmasId, PunchDate = punchDate }, conn);
        return data?.FirstOrDefault();
    }
}
