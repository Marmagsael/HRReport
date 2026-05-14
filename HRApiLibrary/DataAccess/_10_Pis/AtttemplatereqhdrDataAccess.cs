using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class AtttemplatereqhdrDataAccess : IAtttemplatereqhdrDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public AtttemplatereqhdrDataAccess(I_90_001_MySqlDataAccess sql)     { _sql = sql; }

    public async Task _01(AtttemplatereqhdrModel atttemplatereqhdr, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Atttemplatereqhdr 
							(UserId,  EmpNumber,  DateRequested,  Effectivity,  Remarks,  Status,  EmpNumber_Approver) values 
							(@UserId, @EmpNumber, @DateRequested, @Effectivity, @Remarks, @Status, @EmpNumber_Approver)";
        await _sql.ExecuteCmd<dynamic>(sql, atttemplatereqhdr, conn);
    }
    
    public async Task<AtttemplatereqhdrModel?> _01_02(AtttemplatereqhdrModel atttemplatereqhdr, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Atttemplatereqhdr 
							(UserId,  EmpNumber,  DateRequested,  Effectivity,  Remarks,  Status,  EmpNumber_Approver) values 
							(@UserId, @EmpNumber, @DateRequested, @Effectivity, @Remarks, @Status, @EmpNumber_Approver); 
                        select * from {schema}.Atttemplatereqhdr where Id = (SELECT @@IDENTITY); ";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, atttemplatereqhdr, conn);   
        return data.FirstOrDefault();
    }


    public async Task<List<AtttemplatereqhdrModel?>?> _02s(int id, string schema, string conn)
    {
        string sql = $@"select  Id, UserId, EmpNumber, DateRequested, Effectivity, Remarks, Status, EmpNumber_Approver 
                            from {schema}.Atttemplatereqhdr where Id = @Id";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }
    
    public async Task<List<AtttemplatereqhdrModel?>?> _02ByUserIds(int userId, string pisdb, string opisdb, string conn)
    {
        string sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS ApproverName, h.*
                        from {pisdb}.Atttemplatereqhdr h 
                        left join {opisdb}.Empmas e on e.empnumber = h.empnumber 
                        where h.UserId = @UserId 
                        order by h.DateRequested ";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, new { UserId = userId }, conn);
        return data;
    }



    public async Task<AtttemplatereqhdrModel?> _03(AtttemplatereqhdrModel atttemplatereqhdr, string schema, string conn)
    {
        string sql = $@"Update {schema}.Atttemplatereqhdr set 
							DateRequested 		= @DateRequested, 
							Effectivity 		= @Effectivity, 
							Remarks 			= @Remarks, 
							Status 				= @Status, 
							EmpNumber_Approver 	= @EmpNumber_Approver where Id = @Id;
						select  * from {schema}.Atttemplatereqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, atttemplatereqhdr, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<AtttemplatereqhdrModel?> _03SendForApproval(AtttemplatereqhdrModel atttemplatereqhdr, string schema, string conn)
    {
        string sql = $@"Update {schema}.Atttemplatereqhdr set 
							DateRequested 		= @DateRequested, 
							Effectivity 		= @Effectivity, 
							Remarks 			= @Remarks, 
							Status 				= 'F', 
							EmpNumber_Approver 	= @EmpNumber_Approver where Id = @Id;
						select  * from {schema}.Atttemplatereqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, atttemplatereqhdr, conn);

        // *************************************************************************************************
        var h = atttemplatereqhdr; 
        AtttemplatereqhistModel hist = new()
        {
            AtttemplateReqHdrId = h.Id, 
            DActionTaken        = DateTime.Now, 
            Empnumber_Approver  = h.EmpNumber_Approver, 
            Remarks             = h.Remarks??"For Approval", 
            SetStatusTo         = "F" 
        }; 




        return data?.FirstOrDefault();
    }


    public async Task _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Atttemplatereqhdr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);
    }
}

public interface IAtttemplatereqhdrDataAccess
{
    Task                                    _01(AtttemplatereqhdrModel atttemplatereqhdr, string schema, string conn);
    Task<AtttemplatereqhdrModel?>           _01_02(AtttemplatereqhdrModel atttemplatereqhdr, string schema, string conn); 
    Task<List<AtttemplatereqhdrModel?>?>    _02s(int id, string schema, string conn);
    Task<List<AtttemplatereqhdrModel?>?>    _02ByUserIds(int userId, string pisdb, string opisdb, string conn); 
    Task<AtttemplatereqhdrModel?>           _03(AtttemplatereqhdrModel atttemplatereqhdr, string schema, string conn);
    Task<AtttemplatereqhdrModel?>           _03SendForApproval(AtttemplatereqhdrModel atttemplatereqhdr, string schema, string conn); 
    Task                                    _04(int id, string schema, string conn);
}
