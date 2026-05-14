
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class TranreinstatementapprovalhistoryDataAccess : ITranreinstatementapprovalhistoryDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TranreinstatementapprovalhistoryDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TranreinstatementapprovalhistoryModel?> _01(TranreinstatementapprovalhistoryModel Tranreinstatementapprovalhistory, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Tranreinstatementapprovalhistory (TranNumber, Date, UserId, Status, ApproverId, ApproverRemarks) values (@TranNumber, @Date, @UserId, @Status, @ApproverId, @ApproverRemarks)";
        await _sql.ExecuteCmd<dynamic>(sql, Tranreinstatementapprovalhistory, conn);

        sql = $@"SELECT * FROM {schema}.Tranreinstatementapprovalhistory WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TranreinstatementapprovalhistoryModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TranreinstatementapprovalhistoryModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, TranNumber, Date, UserId, Status, ApproverId, ApproverRemarks from {schema}.Tranreinstatementapprovalhistory where Id = @Id";
        var data = await _sql.FetchData<TranreinstatementapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TranreinstatementapprovalhistoryModel?> _02(string trannumber, string schema, string conn)
    {
        string sql = $@"select  Id, TranNumber, Date, UserId, Status, ApproverId, ApproverRemarks from {schema}.Tranreinstatementapprovalhistory where Trannumber = @Trannumber";
        var data = await _sql.FetchData<TranreinstatementapprovalhistoryModel?, dynamic>(sql, new { Trannumber = trannumber }, conn);
        return data?.FirstOrDefault();
    }



    public async Task<TranreinstatementapprovalhistoryModel?> _03(int id, TranreinstatementapprovalhistoryModel Tranreinstatementapprovalhistory, string schema, string conn)
    {
        string sql = $@"Update {schema}.Tranreinstatementapprovalhistory set TranNumber = @TranNumber, Date = @Date, UserId = @UserId, Status = @Status, ApproverId = @ApproverId, ApproverRemarks = @ApproverRemarks where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Tranreinstatementapprovalhistory, conn);

        sql = $@" select  * from {schema}.Tranreinstatementapprovalhistory x where x.Id = @Id ;";
        var data = await _sql.FetchData<TranreinstatementapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }



    public async Task<TranreinstatementapprovalhistoryModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Tranreinstatementapprovalhistory where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Tranreinstatementapprovalhistory x where x.Id = @Id ;";
        var data = await _sql.FetchData<TranreinstatementapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}