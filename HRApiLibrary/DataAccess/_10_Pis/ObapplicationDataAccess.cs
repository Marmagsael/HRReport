using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class ObapplicationDataAccess : IObapplicationDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public ObapplicationDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<ObapplicationModel?> _01(ObapplicationModel obapplication, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.ObApplication
            (Yr,        EmpmasId,   DateApplied,  ObStart,   ObEnd,    Destination,  Purpose,   ContactPerson,
             TelNo,     Remarks,    DaysCnt,      DaysWithPay, Approver1Id, Approver2Id, Status) values
            (@Yr,       @EmpmasId,  @DateApplied, @ObStart,  @ObEnd,   @Destination, @Purpose,  @ContactPerson,
             @TelNo,    @Remarks,   @DaysCnt,     @DaysWithPay, @Approver1Id, @Approver2Id, @Status);

            SELECT * FROM {schema}.ObApplication WHERE Id = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<ObapplicationModel?, dynamic>(sql, obapplication, conn);
        return res.FirstOrDefault();
    }

    public async Task<ObapplicationModel?> _02(int? id, string? schema, string? conn)
    {
        string? sql  = $@"select  * from {schema}.ObApplication where Id = @Id";
        var data    = await _sql.FetchData<ObapplicationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // --- Show My Request : Saved / For Approval
    public async Task<List<ObapplicationModel?>?> _02ByRequest(int? empmasId, string? schema, string? conn)
    {
        string? sql  = $@"select  h.*,
                                CONCAT_WS(' ', TRIM(e1.EmpFirstNm), trim(e1.EmpMidNm), TRIM(e1.EmpLastNm)) AS Approver1Name,
                                CONCAT_WS(' ', TRIM(e2.EmpFirstNm), trim(e2.EmpMidNm), TRIM(e2.EmpLastNm)) AS Approver2Name
                          from {schema}.ObApplication h
                              left join {schema}.Empmas e1 on e1.Id = h.Approver1Id
                              left join {schema}.Empmas e2 on e2.Id = h.Approver2Id
                          where h.EmpmasId = @EmpmasId and h.Status in ('S','FA','R') order by h.ObStart desc ";
        var data    = await _sql.FetchData<ObapplicationModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }

    public async Task<List<ObapplicationModel?>?> _02ForApproval_PerApprover(int? approverId, string? schema, string? conn)
    {
        string? sql = $@"select  h.*,
                                CONCAT_WS(' ', TRIM(e.EmpFirstNm),  trim(e.EmpMidNm),  TRIM(e.EmpLastNm))  AS RequestorName,
                                CONCAT_WS(' ', TRIM(e1.EmpFirstNm), trim(e1.EmpMidNm), TRIM(e1.EmpLastNm)) AS Approver1Name,
                                CONCAT_WS(' ', TRIM(e2.EmpFirstNm), trim(e2.EmpMidNm), TRIM(e2.EmpLastNm)) AS Approver2Name
                        from {schema}.ObApplication h
                            left join {schema}.Empmas e  on e.Id  = h.EmpmasId
                            left join {schema}.Empmas e1 on e1.Id = h.Approver1Id
                            left join {schema}.Empmas e2 on e2.Id = h.Approver2Id
                        where ( (h.Approver1Id = @ApproverId and h.ApproverLevel = 1) or
                                (h.Approver2Id = @ApproverId and h.ApproverLevel = 2)     ) and
                              h.Status = 'FA' ";
        var data = await _sql.FetchData<ObapplicationModel?, dynamic>(sql, new { ApproverId = approverId }, conn);
        return data ?? [];
    }

    // --- dates overlapping another OB that is For Approval / Approved
    public async Task<List<ObapplicationModel?>?> _02Overlapping(int? empmasId, int? obapplicationId, DateTime startDate, DateTime endDate, string? schema, string? conn)
    {
        string? sql  = $@"select  * from {schema}.ObApplication
                          where EmpmasId = @EmpmasId and Status in ('FA','A') and ObStart <= @EndDate and ObEnd >= @StartDate and Id != @ObapplicationId ";
        var data    = await _sql.FetchData<ObapplicationModel?, dynamic>(sql,
                        new { EmpmasId = empmasId, StartDate = startDate.Date, EndDate = endDate.Date, ObapplicationId = obapplicationId }, conn);
        return data;
    }

    public async Task<ObapplicationModel?> _03(int? id, ObapplicationModel obapplication, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.ObApplication set
                            Yr              = @Yr,
                            DateApplied     = @DateApplied,
                            ObStart         = @ObStart,
                            ObEnd           = @ObEnd,
                            Destination     = @Destination,
                            Purpose         = @Purpose,
                            ContactPerson   = @ContactPerson,
                            TelNo           = @TelNo,
                            Remarks         = @Remarks,
                            DaysCnt         = @DaysCnt,
                            DaysWithPay     = @DaysWithPay,
                            Approver1Id     = @Approver1Id,
                            Approver2Id     = @Approver2Id,
                            Status          = @Status
                        where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, obapplication, conn);

        sql = $@" select  * from {schema}.ObApplication x where x.Id = @Id ;";
        var data = await _sql.FetchData<ObapplicationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // --- S -> FA ; no first approver assigned -> go straight to the final approver ( level 2 )
    public async Task<ObapplicationModel?> _03SendForApproval(ObapplicationModel oba, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.ObApplication set Status        = 'FA',
                                                         ApproverLevel = case when ifnull(Approver1Id,0) > 0 then 1 else 2 end
                         where Id = @Id;

                         select  * from {schema}.ObApplication x where x.Id = @Id ; ";

        var data = await _sql.FetchData<ObapplicationModel?, dynamic>(sql, new { oba.Id }, conn);
        return data?.FirstOrDefault();
    }

    // --- level 1 -> moves to level 2 ; level 2 -> Status A
    public async Task _03Approve(ObapplicationModel oba, string? schema, string? conn)
    {
        string? sql = @$"update {schema}.ObApplication set ApproverLevel = 2, DateApprove1 = now() where Id = @Id; ";

        if (oba.ApproverLevel == 2)
            sql = @$"update {schema}.ObApplication set Status = 'A', DateApprove2 = now() where Id = @Id; ";

        await _sql.ExecuteCmd<dynamic>(sql, new { oba.Id }, conn);
    }

    // --- FA -> R ; back to level 1 so a re-send starts the approval again
    public async Task _03Return(int? id, string? apprRemarks, string? schema, string? conn)
    {
        string? sql = @$"update {schema}.ObApplication set Status = 'R', ApprRemarks = @ApprRemarks, ApproverLevel = 1 where Id = @Id; ";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id, ApprRemarks = apprRemarks ?? "" }, conn);
    }

    // --- S -> C
    public async Task _03Cancel(int? id, string? schema, string? conn)
    {
        string? sql = @$"update {schema}.ObApplication set Status = 'C' where Id = @Id; ";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);
    }

    public async Task<ObapplicationModel?> _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.ObApplicationDtl where ObApplicationId = @Id;
                         Delete from {schema}.ObApplication    where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.ObApplication x where x.Id = @Id ;";
        var data = await _sql.FetchData<ObapplicationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

}


public interface IObapplicationDataAccess
{
    Task<ObapplicationModel?>           _01(ObapplicationModel obapplication, string? schema, string? conn);
    Task<ObapplicationModel?>           _02(int? id, string? schema, string? conn);
    Task<List<ObapplicationModel?>?>    _02ByRequest(int? empmasId, string? schema, string? conn);
    Task<List<ObapplicationModel?>?>    _02ForApproval_PerApprover(int? approverId, string? schema, string? conn);
    Task<List<ObapplicationModel?>?>    _02Overlapping(int? empmasId, int? obapplicationId, DateTime startDate, DateTime endDate, string? schema, string? conn);
    Task<ObapplicationModel?>           _03(int? id, ObapplicationModel obapplication, string? schema, string? conn);
    Task<ObapplicationModel?>           _03SendForApproval(ObapplicationModel oba, string? schema, string? conn);
    Task                                _03Approve(ObapplicationModel oba, string? schema, string? conn);
    Task                                _03Return(int? id, string? apprRemarks, string? schema, string? conn);
    Task                                _03Cancel(int? id, string? schema, string? conn);
    Task<ObapplicationModel?>           _04(int? id, string? schema, string? conn);
}
