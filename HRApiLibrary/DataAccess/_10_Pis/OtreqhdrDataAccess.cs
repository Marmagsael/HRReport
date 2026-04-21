using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class OtreqhdrDataAccess : IOtreqhdrDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public OtreqhdrDataAccess(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<OtreqhdrModel?> _01(OtreqhdrModel otreqhdr, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Otreqhdr 
							(UserId,  EmpNumber,  DateRequested,  CovStart,  CovEnd,  AttReqTypeId,  Remarks,  Status,  EmpNumber_Approver) values 
							(@UserId, @EmpNumber, @DateRequested, @CovStart, @CovEnd, @AttReqTypeId, @Remarks, @Status, @EmpNumber_Approver)";
        await _sql.ExecuteCmd<dynamic>(sql, otreqhdr, conn);
        sql = $@"SELECT * FROM {schema}.Otreqhdr WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<OtreqhdrModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<OtreqhdrModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, UserId, EmpNumber, DateRequested, CovStart, CovEnd, AttReqTypeId, Remarks, Status, EmpNumber_Approver, TotHrs, PayYear, PayMo, PayPP 
						from {schema}.Otreqhdr where Id = @Id";
        var data = await _sql.FetchData<OtreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<OtreqhdrModel?>?> _02ForApproval_PerApprover(string approver_empnumber, string pisdb, string conn)
    {
        string sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS RequestorName, h.*
                        from      {pisdb}.Otreqhdr h
                        left join {pisdb}.empmas e on e.Id = h.UserId    
                        where h.Empnumber_Approver = @EmpNumber_Approver and Status in ('F', 'FA') ";
        var data = await _sql.FetchData<OtreqhdrModel?, dynamic>(sql,
                        new { EmpNumber_Approver = approver_empnumber }, conn);
        return data ?? [];
    }


    public async Task<List<OtreqhdrModel?>?> _02ByUserId(int userId, List<string?> status,  string pisdb, string opisdb, string conn)
    {
        string sql = $@"select CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS ApproverName,   
                            h.*  
						from {pisdb}.Otreqhdr h 
                        left join {opisdb}.Empmas e on h.EmpNumber_Approver = e.EmpNumber
                        where UserId = @UserId and Status IN @Status";
        var data = await _sql.FetchData<OtreqhdrModel?, dynamic>(sql, new { UserId = userId, Status = status }, conn);
        return data;
    }


    public async Task<OtreqhdrModel?> _03(int id, OtreqhdrModel otreqhdr, string schema, string conn)
    {
        string sql = $@"Update {schema}.Otreqhdr set 
								DateRequested 	    = @DateRequested, 
								CovStart 		    = @CovStart, 
								CovEnd 			    = @CovEnd, 
								Remarks 		    = @Remarks, 
								EmpNumber_Approver  = @EmpNumber_Approver, 
								TotHrs 			    = @TotHrs 
						where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, otreqhdr, conn);

        sql = $@" select  * from {schema}.Otreqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<OtreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task _03SubmitForApproval(int id,string approver_empnumber,  string schema, string conn)
    {
        string sql = $@"Update {schema}.Otreqhdr set Status = 'F' where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" insert into {schema}.otreqhist 
                        (OtReqHdrId,  DActionTaken, SetStatusTo, Empnumber_Approver,  Remarks) values 
                        (@OtReqHdrId, now(),        'F',         @EmpNumber_Approver, 'Submit for Approval');";
        await _sql.ExecuteCmd<dynamic>(sql, new { OtReqHdrId = id, EmpNumber_Approver = approver_empnumber }, conn);



    }

    public async Task _03PartiallyApprove(OtreqhdrModel oth, string empNumber, string schema, string conn)
    {
        string sql = $@"Update {schema}.Otreqhdr set EmpNumber_Approver  = @EmpNumber_Approver where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = oth.Id, EmpNumber_Approver = empNumber }, conn);

        OtreqhistModel h = new()
        {   
            OtReqHdrId = oth.Id, 
            DActionTaken = DateTime.Now, 
            SetStatusTo = "F", 
            Empnumber_Approver = empNumber, 
            Remarks = "Partially Aprove" 
        };

        sql = $@"Insert into {schema}.otreqhist 
                    (OtReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@OtReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }
    
    public async Task _03Approve(OtreqhdrModel oth, string empNumber, string schema, string conn)
    {
        string sql = $@"Update {schema}.Otreqhdr set Status  = 'A' where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = oth.Id  }, conn);

        OtreqhistModel h = new()
        {   
            OtReqHdrId          = oth.Id, 
            DActionTaken        = DateTime.Now, 
            SetStatusTo         = "F", 
            Empnumber_Approver  = empNumber, 
            Remarks             = "Aproved" 
        };

        sql = $@"Insert into {schema}.otreqhist 
                    (OtReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@OtReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }


    public async Task _03Return(OtreqhdrModel oth, string empNumber, string schema, string conn)
    {
        string sql = $@"Update {schema}.Otreqhdr set ApprRemarks = @AppRemarks, Status = 'R' where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = oth.Id, AppRemarks = oth.ApprRemarks }, conn);

        OtreqhistModel  h       = new()
        { OtReqHdrId            = oth.Id, 
            DActionTaken        = DateTime.Now, 
            SetStatusTo         = "R", 
            Empnumber_Approver  = empNumber, 
            Remarks             = "Return Request" 
        };

        sql = $@"Insert into {schema}.otreqhist 
                    (OtReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@OtReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }


    public async Task _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Otreqhdr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

    }
}

public interface IOtreqhdrDataAccess
{
    Task<OtreqhdrModel?> 	    _01(OtreqhdrModel otreqhdr, string schema, string conn);
    Task<OtreqhdrModel?> 	    _02(int id, string schema, string conn);
    Task<List<OtreqhdrModel?>?> _02ByUserId(int userId, List<string?> status,  string pisdb, string opisdb, string conn);
    Task<List<OtreqhdrModel?>?> _02ForApproval_PerApprover(string approver_empnumber, string pisdb, string conn); 
    Task<OtreqhdrModel?> 	    _03(int id, OtreqhdrModel otreqhdr, string schema, string conn);
    Task                        _03SubmitForApproval(int id, string approver_empnumber,  string schema, string conn);
    Task                        _03PartiallyApprove(OtreqhdrModel oth, string empNumber, string schema, string conn);
    Task                        _03Approve(OtreqhdrModel oth, string empNumber, string schema, string conn);
    Task                        _03Return(OtreqhdrModel oth, string empNumber, string schema, string conn); 
    Task 					    _04(int id, string schema, string conn);
}
