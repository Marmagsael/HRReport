using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class LvcreditDataAccess : ILvcreditDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public LvcreditDataAccess(I_90_001_MySqlDataAccess sql)
    {   _sql = sql; }

    public async Task _01(LvcreditModel lvcredit, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Lvcredit 
                                (Year,  EmpmasId,  LeaveTypeId,  CreditStart,  CreditEnd,  Credit) 
                        values  (@Year, @EmpmasId, @LeaveTypeId, @CreditStart, @CreditEnd, @Credit) 
                        on duplicate key update CreditEnd = @CreditEnd, Credit = @Credit ; ";

        var credit = lvcredit.Credit;
        if (credit == 0)
        {
            sql = $@"Delete from {schema}.Lvcredit  
                     where  Year=@Year 
                            and EmpmasId=@EmpmasId 
                            and LeaveTypeId=@LeaveTypeId 
                            and CreditStart=Date(@CreditStart);";

            var empmasId = lvcredit.EmpmasId; 
            var leaveTypeId = lvcredit.LeaveTypeId; 
            var creditStart = lvcredit.CreditStart; 

        }

        await _sql.ExecuteCmd<dynamic>(sql, lvcredit, conn);

    }


    public async Task<List<LvcreditModel?>?> _02ByLvType_ByCreditStart(int? LvTypeId, DateTime creditStart, string? schema, string? conn)
    {
        string? sql = $@"select  Year, EmpmasId, LeaveTypeId, CreditStart, CreditEnd, Credit 
                        from {schema}.Lvcredit 
                        where LeaveTypeId = @LeaveTypeId and CreditStart = @CreditStart ";
        var data = await _sql.FetchData<LvcreditModel?, dynamic>(sql, new { LeaveTypeId = LvTypeId, CreditStart = creditStart }, conn);
        return data;
    }
    
    public async Task<List<LvcreditModel?>?> _02AssignPerYr(int? LvTypeId, int? yr, string? schema, string? conn)
    {
        string? sql = $@"select EmpmasId, sum(Credit) Credit   
                        from {schema}.Lvcredit 
                        where LeaveTypeId = @LeaveTypeId and Year = @Year 
                        Group by EmpmasId  ";
        var data = await _sql.FetchData<LvcreditModel?, dynamic>(sql, new { LeaveTypeId = LvTypeId, Year = yr }, conn);
        return data;
    }



    public async Task _03(LvcreditModel lvcredit, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Lvcredit set 
                            Year = @Year, 
                            CreditStart = @CreditStart, 
                            CreditEnd = @CreditEnd, 
                            Credit = @Credit 
                        where EmpmasId = @EmpmasId and LeaveTypeId = @LeaveTypeId and CreditStart = @CreditStart;";
        await _sql.ExecuteCmd<dynamic>(sql, lvcredit, conn);
    }

    public async Task _04(int? empmasId, string? lvTypeId, DateTime creditStart, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Lvcredit where EmpmasId = @EmpmasId and LeaveTypeId = @LeaveTypeId and CreditStart = @CreditStart;";
        await _sql.ExecuteCmd<dynamic>(sql, new { EmpmasId = empmasId, LeaveTypeId = lvTypeId, CreditStart = creditStart }, conn);
    }
}

public interface ILvcreditDataAccess
{
    Task                        _01(LvcreditModel lvcredit, string? schema, string? conn);
    Task<List<LvcreditModel?>?> _02ByLvType_ByCreditStart(int? LvTypeId, DateTime creditStart, string? schema, string? conn);
    Task<List<LvcreditModel?>?> _02AssignPerYr(int? LvTypeId, int? yr, string? schema, string? conn); 
    Task                        _03(LvcreditModel lvcredit, string? schema, string? conn);
    Task                        _04(int? empmasId, string? lvTypeId, DateTime creditStart, string? schema, string? conn);
}
