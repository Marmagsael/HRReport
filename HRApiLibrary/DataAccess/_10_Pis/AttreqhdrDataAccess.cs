using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class AttreqhdrDataAccess : IAttreqhdrDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttreqhdrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttreqhdrModel?> _01(AttreqhdrModel attreqhdr, string? schema, string? conn )
	{
		string sql = $@"Insert into {schema}.Attreqhdr 
                            (UserId,  EmpNumber,  DateRequested,  CovStart,  CovEnd,  AttReqTypeId,  Remarks,  Status,  EmpNumber_Approver,  TotHrs) values 
                            (@UserId, @EmpNumber, @DateRequested, @CovStart, @CovEnd, @AttReqTypeId, @Remarks, @Status, @EmpNumber_Approver, @TotHrs)" ; 
		await _sql.ExecuteCmd<dynamic>(sql, attreqhdr, conn);
		sql = $@"SELECT * FROM {schema}.Attreqhdr WHERE ID = (SELECT @@IDENTITY)"; 
		var res = await _sql.FetchData<AttreqhdrModel?,dynamic>(sql,new { },conn);

		return res.FirstOrDefault();
	}


    public async Task<List<AttreqhdrModel?>> _02s(int? id, string? schema, string? conn)
    {
        string? sql = $@"select  Id, UserId, EmpNumber, DateRequested, CovStart, CovEnd, 
                                AttReqTypeId, Remarks, Status, EmpNumber_Approver, TotHrs 
                        from {schema}.Attreqhdr where Id = @Id";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic> (sql, new { Id = id }, conn);
        return data ?? [];
    }
    
    public async Task<List<AttreqhdrModel?>> _02ByUserId_ByTypeId_ByStatus
    (int? userid, int? typeId, List<string> status, string? pisdb, string? opisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS ApproverName, h.*
                        from      {pisdb}.Attreqhdr h
                        left join {opisdb}.empmas e on e.Empnumber = h.EmpNumber_Approver    
                        where h.UserId = @UserId and h.AttReqTypeId = @TypeId and h.Status in @Status ";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, 
                        new { UserId=userid, TypeId=typeId, Status=status }, conn);
        return data ?? [];
    }
    
    public async Task<List<AttreqhdrModel?>> _02ForApproval_PerApprover(string? approver_empnumber, string? pisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS RequestorName, h.*
                        from      {pisdb}.Attreqhdr h
                        left join {pisdb}.empmas e on e.Id = h.UserId    
                        where h.Empnumber_Approver = @EmpNumber_Approver and Status in ('F', 'FA') ";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, 
                        new { EmpNumber_Approver =approver_empnumber }, conn);
        return data ?? [];
    }

    public async Task<AttreqhdrModel?> _03(int? id,AttreqhdrModel attreqhdr, string? schema, string? conn)
	{
		string sql = $@"Update {schema}.Attreqhdr set 
                            DateRequested       = @DateRequested, 
                            CovStart            = @CovStart, 
                            CovEnd              = @CovEnd, 
                            Remarks             = @Remarks, 
                            EmpNumber_Approver  = @EmpNumber_Approver
                        where Id = @Id;"; 
		await _sql.ExecuteCmd<dynamic>(sql, attreqhdr, conn);
		
		sql = $@" select  * from {schema}.Attreqhdr x where x.Id = @Id ;";
		var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
		return data?.FirstOrDefault();
	}
    
    public async Task _03Return(AttreqhdrModel arh, string? empNumber, string? schema, string? conn)
	{
		string sql = $@"Update {schema}.Attreqhdr set ApprRemarks = @ApprRemarks, Status = 'R' where Id = @Id;"; 
		await _sql.ExecuteCmd<dynamic>(sql, new { Id = arh.Id, AppRemarks=arh.ApprRemarks }, conn);

        AttreqhistModel h = new()
        { AttReqHdrId = arh.Id,  DActionTaken = DateTime.Now,  SetStatusTo="R",  Empnumber_Approver = empNumber,  Remarks = "Return Request" }; 
        
        sql = $@"Insert into {schema}.attreqhist 
                    (AttReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@AttReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }
    
    public async Task _03PartiallyApprove(AttreqhdrModel arh, string? empNumber, string? schema, string? conn)
	{
		string sql = $@"Update {schema}.Attreqhdr set EmpNumber_Approver  = @EmpNumber_Approver where Id = @Id;"; 
		await _sql.ExecuteCmd<dynamic>(sql, new { Id = arh.Id, EmpNumber_Approver = empNumber }, conn);

        AttreqhistModel h = new()
        { 
            AttReqHdrId = arh.Id, 
            DActionTaken = DateTime.Now, 
            SetStatusTo = "F", 
            Empnumber_Approver = arh.EmpNumber_Approver, 
            Remarks = $"Partially Aprove [{empNumber??""}]" 
        };

        sql = $@"Insert into {schema}.attreqhist 
                    (AttReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@AttReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }

    
    public async Task<AttreqhdrModel?> _03SendForApproval(AttreqhdrModel attreqhdr, string? schema, string? conn)
	{
        string? sql = $@"Update {schema}.Attreqhdr set 
                            DateRequested       = @DateRequested, 
                            CovStart            = @CovStart, 
                            CovEnd              = @CovEnd, 
                            Remarks             = @Remarks, 
                            EmpNumber_Approver  = @EmpNumber_Approver, 
                            Status              = 'F' 
                        where Id = @Id;
                        select  * from {schema}.Attreqhdr x where x.Id = @Id ;"; 
		var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, attreqhdr, conn);

        // *****************************************************************************************************
        AttreqhistModel attreqhist = new() { AttReqHdrId = attreqhdr.Id, DActionTaken = DateTime.Now, Empnumber_Approver = attreqhdr.EmpNumber_Approver, 
                                             Remarks = attreqhdr.Remarks??"For Approval", SetStatusTo = "F" }; 

        var sql1 =  $@"Insert into {schema}.Attreqhist 
                            (AttReqHdrId,  DActionTaken,  SetStatusTo,  Remarks,             Empnumber_Approver) values 
                            (@AttReqHdrId, @DActionTaken, @SetStatusTo, 'Send For Approval', @Empnumber_Approver)" ; 
		await _sql.ExecuteCmd<dynamic>(sql1, attreqhist, conn);
        // *****************************************************************************************************


		return data?.FirstOrDefault();
        
	}
    
    public async Task<AttreqhdrModel?> _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Attreqhdr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Attreqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IAttreqhdrDataAccess
{
    Task<AttreqhdrModel?>       _01(AttreqhdrModel attreqhdr, string? schema, string? conn);
    Task<List<AttreqhdrModel?>> _02s(int? id, string? schema, string? conn);
    Task<List<AttreqhdrModel?>> _02ByUserId_ByTypeId_ByStatus(int? userid, int? typeId, List<string> status, string? pisdb, string? opisdb, string? conn);
    Task<List<AttreqhdrModel?>> _02ForApproval_PerApprover(string? approver_empnumber, string? pisdb, string? conn); 
    Task<AttreqhdrModel?>       _03(int? id, AttreqhdrModel attreqhdr, string? schema, string? conn);
    Task<AttreqhdrModel?>       _03SendForApproval(AttreqhdrModel attreqhdr, string? schema, string? conn);
    Task                        _03Return(AttreqhdrModel arh, string? empNumber, string? schema, string? conn);
    Task                        _03PartiallyApprove(AttreqhdrModel arh, string? empNumber, string? schema, string? conn); 
    Task<AttreqhdrModel?>       _04(int? id, string? schema, string? conn);
}
