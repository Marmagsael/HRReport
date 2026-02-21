using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class AttreqhdrDataAccess : IAttreqhdrDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttreqhdrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttreqhdrModel?> _01(AttreqhdrModel attreqhdr, string schema, string conn )
	{
		string sql = $@"Insert into {schema}.Attreqhdr 
                            (UserId,  EmpNumber,  DateRequested,  CovStart,  CovEnd,  AttReqTypeId,  Remarks,  Status,  EmpNumber_Approver,  TotHrs) values 
                            (@UserId, @EmpNumber, @DateRequested, @CovStart, @CovEnd, @AttReqTypeId, @Remarks, @Status, @EmpNumber_Approver, @TotHrs)" ; 
		await _sql.ExecuteCmd<dynamic>(sql, attreqhdr, conn);
		sql = $@"SELECT * FROM {schema}.Attreqhdr WHERE ID = (SELECT @@IDENTITY)"; 
		var res = await _sql.FetchData<AttreqhdrModel?,dynamic>(sql,new { },conn);

		return res.FirstOrDefault();
	}


    public async Task<List<AttreqhdrModel?>> _02s(int id, string schema, string conn)
    {
        string sql = $@"select  Id, UserId, EmpNumber, DateRequested, Remarks, Status, EmpNumber_Approver from {schema}.Attreqhdr where Id = @Id";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data ?? [];
    }


    public async Task<AttreqhdrModel?> _03(int id,AttreqhdrModel attreqhdr, string schema, string conn)
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
    
    public async Task<AttreqhdrModel?> _03SendForApproval(AttreqhdrModel attreqhdr, string schema, string conn)
	{
        string sql = $@"Update {schema}.Attreqhdr set 
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
        AttreqhistModel attreqhist = new() { AttReqHdrId = attreqhdr.Id, DActionTaken = DateTime.Now, 
                                             Remarks = attreqhdr.Remarks??"For Approval", SetStatusTo = "F" }; 

        var sql1 =  $@"Insert into {schema}.Attreqhist 
                            (AttReqHdrId,  DActionTaken,  SetStatusTo,  Remarks) values 
                            (@AttReqHdrId, @DActionTaken, @SetStatusTo, @Remarks)" ; 
		await _sql.ExecuteCmd<dynamic>(sql1, attreqhist, conn);
        // *****************************************************************************************************



		return data?.FirstOrDefault();
	}

    public async Task<AttreqhdrModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Attreqhdr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Attreqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IAttreqhdrDataAccess
{
    Task<AttreqhdrModel?> _01(AttreqhdrModel attreqhdr, string schema, string conn);
    Task<List<AttreqhdrModel?>> _02s(int id, string schema, string conn);
    Task<AttreqhdrModel?> _03(int id, AttreqhdrModel attreqhdr, string schema, string conn);
    Task<AttreqhdrModel?> _03SendForApproval(AttreqhdrModel attreqhdr, string schema, string conn); 
    Task<AttreqhdrModel?> _04(int id, string schema, string conn);
}
