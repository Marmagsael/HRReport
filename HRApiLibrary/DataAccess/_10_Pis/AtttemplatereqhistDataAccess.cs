using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class AtttemplatereqhistDataAccess : IAtttemplatereqhistDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public AtttemplatereqhistDataAccess(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<AtttemplatereqhistModel?> _01(AtttemplatereqhistModel atttemplatereqhist, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Atttemplatereqhist 
							(AtttemplateReqHdrId,  UserId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
							(@AtttemplateReqHdrId, @UserId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks)";
        await _sql.ExecuteCmd<dynamic>(sql, atttemplatereqhist, conn);
        sql = $@"SELECT * FROM {schema}.Atttemplatereqhist WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<AtttemplatereqhistModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();

    }


    public async Task<List<AtttemplatereqhistModel?>?> _02s(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Atttemplatereqhist where Id = @Id";
        var data = await _sql.FetchData<AtttemplatereqhistModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }

    public async Task<List<AtttemplatereqhistModel?>?> _02ByAtttemplateReqHdrIds(int atttemplateReqHdrId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Atttemplatereqhist where AtttemplateReqHdrId = @AtttemplateReqHdrId";
        var data = await _sql.FetchData<AtttemplatereqhistModel?, dynamic>(sql, new { AtttemplateReqHdrId = atttemplateReqHdrId }, conn);
        return data;
    }


    public async Task<AtttemplatereqhistModel?> _03(int id, AtttemplatereqhistModel atttemplatereqhist, string schema, string conn)
    {
        string sql = $@"Update {schema}.Atttemplatereqhist set 
							AtttemplateReqHdrId = @AtttemplateReqHdrId, 
							UserId 				= @UserId, 
							DActionTaken 		= @DActionTaken, 
							SetStatusTo 		= @SetStatusTo, 
							Empnumber_Approver 	= @Empnumber_Approver, 
							Remarks 			= @Remarks where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, atttemplatereqhist, conn);

        sql = $@" select  * from {schema}.Atttemplatereqhist x where x.Id = @Id ;";
        var data = await _sql.FetchData<AtttemplatereqhistModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Atttemplatereqhist where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

    }
}

public interface IAtttemplatereqhistDataAccess
{
    Task<AtttemplatereqhistModel?> 			_01(AtttemplatereqhistModel atttemplatereqhist, string schema, string conn);
    Task<List<AtttemplatereqhistModel?>?> 	_02ByAtttemplateReqHdrIds(int atttemplateReqHdrId, string schema, string conn);
    Task<List<AtttemplatereqhistModel?>?> 	_02s(int id, string schema, string conn);
    Task<AtttemplatereqhistModel?> 			_03(int id, AtttemplatereqhistModel atttemplatereqhist, string schema, string conn);
    Task 									_04(int id, string schema, string conn);
}
