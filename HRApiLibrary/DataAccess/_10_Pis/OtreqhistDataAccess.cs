using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class OtreqhistDataAccess : IOtreqhistDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public OtreqhistDataAccess(I_90_001_MySqlDataAccess sql) { _sql = sql; }
    public async Task<OtreqhistModel?> _01(OtreqhistModel otreqhist, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Otreqhist 
							(OtReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
							(@OtReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks)";
        await _sql.ExecuteCmd<dynamic>(sql, otreqhist, conn);
        sql = $@"SELECT * FROM {schema}.Otreqhist WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<OtreqhistModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<OtreqhistModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, OtReqHdrId, DActionTaken, SetStatusTo, Empnumber_Approver, Remarks 
						 from {schema}.Otreqhist where Id = @Id";
        var data = await _sql.FetchData<OtreqhistModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<OtreqhistModel?>?> _02ByOTReqHdrId(int otReqHdrId, string schema, string conn)
    {
        string sql = $@"select  Id, OtReqHdrId, DActionTaken, SetStatusTo, Empnumber_Approver, Remarks 
						from {schema}.Otreqhist where Id = @Id";
        var data = await _sql.FetchData<OtreqhistModel?, dynamic>(sql, new { OtReqHdrId = otReqHdrId }, conn);
        return data;
    }


    public async Task<OtreqhistModel?> _03(int id, OtreqhistModel otreqhist, string schema, string conn)
    {
        string sql = $@"Update {schema}.Otreqhist set OtReqHdrId = @OtReqHdrId, DActionTaken = @DActionTaken, 
						SetStatusTo = @SetStatusTo, Empnumber_Approver = @Empnumber_Approver, Remarks = @Remarks 
						where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, otreqhist, conn);

        sql = $@" select  * from {schema}.Otreqhist x where x.Id = @Id ;";
        var data = await _sql.FetchData<OtreqhistModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Otreqhist where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

    }

    public async Task _04ByOtReqHdrId(int otReqHdrId, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Otreqhist where OtReqHdrId = @OtReqHdrId;";
        await _sql.ExecuteCmd<dynamic>(sql, new { OtReqHdrId = otReqHdrId }, conn);

    }

}

public interface IOtreqhistDataAccess
{
    Task<OtreqhistModel?> 	_01(OtreqhistModel otreqhist, string schema, string conn);
    Task<OtreqhistModel?> 	_02(int id, string schema, string conn);
    Task<List<OtreqhistModel?>?> _02ByOTReqHdrId(int otReqHdrId, string schema, string conn);
    Task<OtreqhistModel?> 	_03(int id, OtreqhistModel otreqhist, string schema, string conn);
    Task 					_04(int id, string schema, string conn);
    Task 					_04ByOtReqHdrId(int otReqHdrId, string schema, string conn);
}
