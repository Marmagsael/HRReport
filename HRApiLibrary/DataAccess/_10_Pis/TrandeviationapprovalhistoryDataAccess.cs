
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class TrandeviationapprovalhistoryDataAccess : ITrandeviationapprovalhistoryDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TrandeviationapprovalhistoryDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TrandeviationapprovalhistoryModel?> _01(TrandeviationapprovalhistoryModel Trandeviationapprovalhistory, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Trandeviationapprovalhistory (TranNumber, Date, UserId, Status, ApproverId, ApproverRemarks) values (@TranNumber, @Date, @UserId, @Status, @ApproverId, @ApproverRemarks)";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeviationapprovalhistory, conn);

        sql = $@"SELECT * FROM {schema}.Trandeviationapprovalhistory WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TrandeviationapprovalhistoryModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TrandeviationapprovalhistoryModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, TranNumber, Date, UserId, Status, ApproverId, ApproverRemarks from {schema}.Trandeviationapprovalhistory where Id = @Id";
        var data = await _sql.FetchData<TrandeviationapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TrandeviationapprovalhistoryModel?> _02ByTrn(string trannumber, string schema, string conn)
    {
        string sql = $@"SELECT   h.*,  e.id,  CONCAT(
                        COALESCE(TRIM(e.emplastnm), ''), ', ',
                        COALESCE(TRIM(e.empfirstnm), ''), ' ',
                        COALESCE(TRIM(e.empmidnm), '')
                    ) AS PreparedBy
                FROM {schema}.Trandeviationapprovalhistory h
                LEFT JOIN mainpis.empmas e ON e.id = h.userId where Trannumber = @Trannumber";
        var data = await _sql.FetchData<TrandeviationapprovalhistoryModel?, dynamic>(sql, new { Trannumber = trannumber }, conn);

        return data?.FirstOrDefault();
    }



    public async Task<TrandeviationapprovalhistoryModel?> _03(int id, TrandeviationapprovalhistoryModel Trandeviationapprovalhistory, string schema, string conn)
    {
        string sql = $@"Update {schema}.Trandeviationapprovalhistory set TranNumber = @TranNumber, Date = @Date, UserId = @UserId, Status = @Status, ApproverId = @ApproverId, ApproverRemarks = @ApproverRemarks where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeviationapprovalhistory, conn);

        sql = $@" select  * from {schema}.Trandeviationapprovalhistory x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandeviationapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }



    public async Task<TrandeviationapprovalhistoryModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Trandeviationapprovalhistory where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Trandeviationapprovalhistory x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandeviationapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}