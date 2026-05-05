using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class LeaveapplicationdtlDataAccess : ILeaveapplicationdtlDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LeaveapplicationdtlDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01(LeaveapplicationdtlModel leaveapplicationdtl, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Leaveapplicationdtl 
                            (LeaveApplicationId,  EmpmasId,  EmpNumber,  Start,  DutyType,  TimeStart,  TimeDuration,  End,  CreditedHrs,  IsPayable) values 
                            (@LeaveApplicationId, @EmpmasId, @EmpNumber, @Start, @DutyType, @TimeStart, @TimeDuration, @End, @CreditedHrs, @IsPayable) 
                            on duplicate key update 
                            Start=@Start,  DutyType=@DutyType,  TimeStart=@TimeStart,  TimeDuration=@TimeDuration,  End=@End,  CreditedHrs=@CreditedHrs,  
                            IsPayable=@IsPayable; ";
        await _sql.ExecuteCmd<dynamic>(sql, leaveapplicationdtl, conn);
    }


    public async Task<LeaveapplicationdtlModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, LeaveApplicationId, EmpmasId, EmpNumber, Start, DutyType, TimeStart, TimeDuration, End, CreditedHrs, IsPayable 
                                from {schema}.Leaveapplicationdtl where Id = @Id";
        var data = await _sql.FetchData<LeaveapplicationdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<LeaveapplicationdtlModel?>?> _02ByLvApplicationId(int leaveApplicationId, string schema, string conn)
    {
        string sql = $@"select  *  
                        from {schema}.Leaveapplicationdtl 
                        where LeaveApplicationId = @LeaveApplicationId 
                        order by Start ";
        var data = await _sql.FetchData<LeaveapplicationdtlModel?, dynamic>(sql, new { LeaveApplicationId = leaveApplicationId }, conn);
        return data;
    }



    public async Task<LeaveapplicationdtlModel?> _03(int id, LeaveapplicationdtlModel leaveapplicationdtl, string schema, string conn)
    {
        string sql = $@"Update {schema}.Leaveapplicationdtl set 
                            LeaveApplicationId  = @LeaveApplicationId, 
                            EmpmasId            = @EmpmasId, 
                            EmpNumber           = @EmpNumber, 
                            Start               = @Start, 
                            DutyType            = @DutyType, 
                            TimeStart           = @TimeStart, 
                            TimeDuration        = @TimeDuration, 
                            End                 = @End, 
                            CreditedHrs         = @CreditedHrs, 
                            IsPayable           = @IsPayable 
                        where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, leaveapplicationdtl, conn);

        sql = $@" select  * from {schema}.Leaveapplicationdtl x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeaveapplicationdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<LeaveapplicationdtlModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Leaveapplicationdtl where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Leaveapplicationdtl x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeaveapplicationdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task _04OutRange(int lvaId, DateTime dstart, DateTime dEnd, string schema, string conn)
    {
        string sql = $@"DELETE FROM {schema}.Leaveapplicationdtl
                        WHERE leaveApplicationId = @LvaId and  (Start < @Start
                              OR Start >= DATE_ADD(@End, INTERVAL 1 DAY));";
        await _sql.ExecuteCmd<dynamic>(sql, new { LvaId = lvaId, Start = dstart.Date, End = dEnd.Date }, conn);
    }



}


public interface ILeaveapplicationdtlDataAccess
{
    Task                                    _01(LeaveapplicationdtlModel leaveapplicationdtl, string schema, string conn);
    Task<LeaveapplicationdtlModel?>         _02(int id, string schema, string conn);
    Task<List<LeaveapplicationdtlModel?>?>  _02ByLvApplicationId(int leaveApplicationId, string schema, string conn);
    Task<LeaveapplicationdtlModel?>         _03(int id, LeaveapplicationdtlModel leaveapplicationdtl, string schema, string conn);
    Task<LeaveapplicationdtlModel?>         _04(int id, string schema, string conn);
    Task                                    _04OutRange(int lvaId, DateTime dstart, DateTime dEnd, string schema, string conn); 
}
