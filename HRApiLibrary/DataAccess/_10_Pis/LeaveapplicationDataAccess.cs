using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class LeaveapplicationDataAccess : ILeaveapplicationDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LeaveapplicationDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<LeaveapplicationModel?> _01(LeaveapplicationModel leaveapplication, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Leaveapplication 
            (Yr,       EmpmasId,  DateApplied,  LeaveTypeId,  LvBalance,  DaysCnt,  LvTime,       DaysWithPay, 
            Urgency,   LvStart,   LvEnd,        Reason,       Address,    TelNo,    Approver1Id,  Approver2Id,  Approver3Id,  Status) values 
            (@Yr,      @EmpmasId, @DateApplied, @LeaveTypeId, @LvBalance, @DaysCnt, @LvTime,      @DaysWithPay, 
             @Urgency, @LvStart,  @LvEnd,       @Reason,      @Address,   @TelNo,   @Approver1Id, @Approver2Id, @Approver3Id, @Status)";
        await _sql.ExecuteCmd<dynamic>(sql, leaveapplication, conn);

        sql = $@"SELECT * FROM {schema}.Leaveapplication WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<LeaveapplicationModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();

    }

    public async Task<LeaveapplicationModel?> _02(int? id, string? schema, string? conn)
    {
        string? sql  = $@"select  * from {schema}.Leaveapplication where Id = @Id";
        var data    = await _sql.FetchData<LeaveapplicationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<LeaveapplicationModel?>?> _02ByRequest(int? empmasId, string? schema, string? conn)
    {
        string? sql  = $@"select  * from {schema}.Leaveapplication where EmpmasId = @EmpmasId and Status in ('S','F') ";
        var data    = await _sql.FetchData<LeaveapplicationModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;

    }
    
    public async Task<double> _02LvBalance(int? lvTypeId, int? empmasId, int? yr, string? schema, string? conn)
    {
        double lvBal =0; 
        string?  q   = @$"select sum(Credit) Credit from {schema}.LvCredit 
                            where EmpmasId = @EmpmasId and LeaveTypeId = @LvTypeId and Year = @Year ";
        var     r1  = await _sql.FetchData<LvcreditModel?, dynamic>(q, new { EmpmasId=empmasId, LvTypeId = lvTypeId, Year = yr }, conn);
        double lvTotal = r1.FirstOrDefault().Credit??0; 

        if (lvTotal < 1) return 0;

        string?  q2 = @$"select sum(DaysWithPay) DaysWithPay from {schema}.leaveapplication  
                            where EmpmasId = @EmpmasId and LeaveTypeId = @LvTypeId and Yr = @Year 
                                  and Status in ('A', 'FA')  ";
        var     r2 = await _sql.FetchData<LeaveapplicationModel?, 
                        dynamic>(q2, new { EmpmasId = empmasId, LvTypeId = lvTypeId, Year = yr }, conn);
        double  lvUsed = r2.FirstOrDefault().DaysWithPay ;

        lvBal = lvTotal - lvUsed;
        return lvBal;
    }

    public async Task<List<LeaveapplicationModel?>?> _02Chk_Entry_LvType(int? leaveTypeId, string? schema, string? conn)
    {
        string? sql  = $@"select  * from {schema}.Leaveapplication where LeaveTypeId = @LeaveTypeId limit 1 ";
        var data    = await _sql.FetchData<LeaveapplicationModel?, dynamic>(sql, new { LeaveTypeId = leaveTypeId }, conn);
        return data;
    }
    public async Task<List<LeaveapplicationModel?>?> _02ForApproval_PerApprover(int? approverId, string? pisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS RequestorName,
                                ApproverLevel,
                                h.*, lt.LeaveName as Leavetypename, 
                                CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS RequestorName,
                                    CONCAT_WS(' ', TRIM(e1.EmpFirstNm), trim(e1.EmpMidNm), TRIM(e1.EmpLastNm)) AS Approver1Name,
                                    CONCAT_WS(' ', TRIM(e2.EmpFirstNm), trim(e2.EmpMidNm), TRIM(e2.EmpLastNm)) AS Approver2Name
                        from {pisdb}.Leaveapplication h
                        left join {pisdb}.empmas    e   on e.Id     = h.EmpmasId
                        left join {pisdb}.leavetype lt  on lt.Id    = h.LeaveTypeId
                        left join {pisdb}.empmas    e1   on e1.Id     = h.Approver1Id
                        left join {pisdb}.empmas    e2   on e2.Id     = h.Approver2Id                        
                        where ( (h.Approver1Id = @ApproverId and h.ApproverLevel = 1) or 
                                (h.Approver2Id = @ApproverId and h.ApproverLevel = 2)     ) and 
                              Status in ('F', 'FA')";
        var data = await _sql.FetchData<LeaveapplicationModel?, dynamic>(sql,
                        new { ApproverId = approverId }, conn);
        return data ?? [];
    }


    public async Task<LeaveapplicationModel?> _03(int? id, LeaveapplicationModel leaveapplication, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Leaveapplication set 
                            Yr          = @Yr,  
                            EmpmasId    = @EmpmasId,  
                            LeaveTypeId = @LeaveTypeId,  
                            LvBalance   = @LvBalance,  
                            DaysCnt     = @DaysCnt,  
                            LvTime      = @LvTime,  
                            DaysWithPay = @DaysWithPay,  
                            Urgency     = @Urgency,  
                            LvStart     = @LvStart,  
                            LvEnd       = @LvEnd,  
                            Reason      = @Reason,  
                            Address     = @Address,  
                            TelNo       = @TelNo,  
                            Approver1Id = @Approver1Id,  
                            Approver2Id = @Approver2Id,  
                            Approver3Id = @Approver3Id,  
                            Status = @Status where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, leaveapplication, conn);

        sql = $@" select  * from {schema}.Leaveapplication x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeaveapplicationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task _03Return(LeaveapplicationModel lva, int? approverId, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.LeaveApplication set ApprRemarks = @AppRemarks, Status = 'R' where Id = @Id;
                        
                        Insert into {schema}.LeaveApplicationHist (LvaId, EmpasId,  Date,  Action) values  
                                                                  (@Id,   @ApproverId, now(), @Action) ;  ";

        int? id = lva.Id;
        string? apprRemarks = lva.ApprRemarks??"";
        string? action = "Returned"  ;
        
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id, AppRemarks = apprRemarks, ApproverId = approverId, Action = action }, conn);
    }

    public async Task _03Approve(LeaveapplicationModel lva, int? approverId, string? schema, string? conn)
    {
        string? msql = @$"update {schema}.Leaveapplication set ApproverLevel = 2, DateApprove1 = now() where Id = @Id;
                         Insert into {schema}.LeaveApplicationHist 
                                (LvaId, EmpasId,  Date,  Action) values  
                                (@Id,   @ApproverId, now(), @Action) ; ";

        if(lva.ApproverLevel == 2)
        {
            msql    = @$"update {schema}.Leaveapplication set Status = 'A', DateApprove2 = now() where Id = @Id;
                         Insert into {schema}.LeaveApplicationHist 
                                (LvaId, EmpasId,  Date,  Action) values  
                                (@Id,   @ApproverId, now(), @Action) ; ";

        }

        int? id = lva.Id;
        string? action = "Approved - Level " + lva.ApproverLevel.ToString();
        await _sql.ExecuteCmd<dynamic>(msql, new { Id = id, ApproverId = approverId, Action = action } , conn);

    }

    
    public async Task<LeaveapplicationModel?> _03SendForApproval(LeaveapplicationModel lva, string? schema, string? conn)
    {
        var     lvaId   = lva.Id; 
        var     empasId = lva.EmpmasId; 
        var     date    = DateTime.Now; 
        string?  action  = "Send for Approval";

        string?  sql     = $@"Update {schema}.Leaveapplication set Status      = 'F' where Id = @Id;
                             
                             Insert into {schema}.LeaveApplicationHist 
                                (LvaId, EmpasId,  Date,  Action) values  (@id,   @EmpasId, @Date, @Action); 
                             
                             select  * from {schema}.Leaveapplication x where x.Id = @Id ; ";

        var     data    = await _sql.FetchData<LeaveapplicationModel?, dynamic>
                          (sql, new { Id = lva.Id, LvaId = lvaId, EmpasId = empasId, Date = date, Action = action}, conn);

        return  data?.FirstOrDefault();

    }

    public async Task<LeaveapplicationModel?> _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Leaveapplication where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Leaveapplication x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeaveapplicationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

}
