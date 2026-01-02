
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class TrandeploymentapprovalhistoryDataAccess : ITrandeploymentapprovalhistoryDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TrandeploymentapprovalhistoryDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TrandeploymentapprovalhistoryModel?> _01(TrandeploymentapprovalhistoryModel Trandeploymentapprovalhistory, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Trandeploymentapprovalhistory (TranNumber, Date, UserId, Status, ApproverId, ApproverRemarks) values (@TranNumber, @Date, @UserId, @Status, @ApproverId, @ApproverRemarks)";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeploymentapprovalhistory, conn);

        sql = $@"SELECT * FROM {schema}.Trandeploymentapprovalhistory WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TrandeploymentapprovalhistoryModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TrandeploymentapprovalhistoryModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, TranNumber, Date, UserId, Status, ApproverId, ApproverRemarks from {schema}.Trandeploymentapprovalhistory where Id = @Id";
        var data = await _sql.FetchData<TrandeploymentapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TrandeploymentapprovalhistoryModel?> _02(string trannumber, string schema, string conn)
    {
        string sql = $@"select  Id, TranNumber, Date, UserId, Status, ApproverId, ApproverRemarks from {schema}.Trandeploymentapprovalhistory where Trannumber = @Trannumber";
        var data = await _sql.FetchData<TrandeploymentapprovalhistoryModel?, dynamic>(sql, new { Trannumber = trannumber }, conn);
        return data?.FirstOrDefault();
    }



    public async Task<TrandeploymentapprovalhistoryModel?> _03(int id, TrandeploymentapprovalhistoryModel Trandeploymentapprovalhistory, string schema, string conn)
    {
        string sql = $@"Update {schema}.Trandeploymentapprovalhistory set TranNumber = @TranNumber, Date = @Date, UserId = @UserId, Status = @Status, ApproverId = @ApproverId, ApproverRemarks = @ApproverRemarks where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeploymentapprovalhistory, conn);

        sql = $@" select  * from {schema}.Trandeploymentapprovalhistory x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandeploymentapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

   

    public async Task<TrandeploymentapprovalhistoryModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Trandeploymentapprovalhistory where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Trandeploymentapprovalhistory x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandeploymentapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}