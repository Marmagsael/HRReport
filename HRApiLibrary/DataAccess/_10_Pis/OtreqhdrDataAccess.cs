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
							(UserId,  EmpNumber,  DateRequested,  CovStart,  CovEnd,  AttReqTypeId,  Remarks,  Status,  EmpNumber_Approver,  TotHrs,  PayYear,  PayMo,  PayPP) values 
							(@UserId, @EmpNumber, @DateRequested, @CovStart, @CovEnd, @AttReqTypeId, @Remarks, @Status, @EmpNumber_Approver, @TotHrs, @PayYear, @PayMo, @PayPP)";
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


    public async Task<OtreqhdrModel?> _03(int id, OtreqhdrModel otreqhdr, string schema, string conn)
    {
        string sql = $@"Update {schema}.Otreqhdr set 
								UserId 			= @UserId, 
								EmpNumber 		= @EmpNumber, 
								DateRequested 	= @DateRequested, 
								CovStart 		= @CovStart, 
								CovEnd 			= @CovEnd, 
								AttReqTypeId 	= @AttReqTypeId, 
								Remarks 		= @Remarks, 
								Status 			= @Status, 
								EmpNumber_Approver = @EmpNumber_Approver, 
								TotHrs 			= @TotHrs, 
								PayYear 		= @PayYear, 
								PayMo 			= @PayMo, 
								PayPP 			= @PayPP where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, otreqhdr, conn);

        sql = $@" select  * from {schema}.Otreqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<OtreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Otreqhdr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

    }
}

public interface IOtreqhdrDataAccess
{
    Task<OtreqhdrModel?> _01(OtreqhdrModel otreqhdr, string schema, string conn);
    Task<OtreqhdrModel?> _02(int id, string schema, string conn);
    Task<OtreqhdrModel?> _03(int id, OtreqhdrModel otreqhdr, string schema, string conn);
    Task _04(int id, string schema, string conn);
}
