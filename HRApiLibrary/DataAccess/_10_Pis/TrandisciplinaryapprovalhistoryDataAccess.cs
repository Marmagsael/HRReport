
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class TrandisciplinaryapprovalhistoryDataAccess : ITrandisciplinaryapprovalhistoryDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TrandisciplinaryapprovalhistoryDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TrandisciplinaryapprovalhistoryModel?> _01(TrandisciplinaryapprovalhistoryModel Trandeviationapprovalhistory, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Trandisciplinaryapprovalhistory (TranNumber, Date, UserId, Status, ApproverId, ApproverRemarks) values (@TranNumber, @Date, @UserId, @Status, @ApproverId, @ApproverRemarks)";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeviationapprovalhistory, conn);

        sql = $@"SELECT * FROM {schema}.Trandisciplinaryapprovalhistory WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TrandisciplinaryapprovalhistoryModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TrandisciplinaryapprovalhistoryModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, TranNumber, Date, UserId, Status, ApproverId, ApproverRemarks from {schema}.Trandisciplinaryapprovalhistory where Id = @Id";
        var data = await _sql.FetchData<TrandisciplinaryapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TrandisciplinaryapprovalhistoryModel?> _02ByTrn(string trannumber, string schema, string conn)
    {
        string sql = $@"SELECT   h.*,  e.id,  CONCAT(
                        COALESCE(TRIM(e.emplastnm), ''), ', ',
                        COALESCE(TRIM(e.empfirstnm), ''), ' ',
                        COALESCE(TRIM(e.empmidnm), '')
                    ) AS PreparedBy
                FROM {schema}.Trandisciplinaryapprovalhistory h
                LEFT JOIN mainpis.empmas e ON e.id = h.userId where Trannumber = @Trannumber";
        var data = await _sql.FetchData<TrandisciplinaryapprovalhistoryModel?, dynamic>(sql, new { Trannumber = trannumber }, conn);

        return data?.FirstOrDefault();
    }



    public async Task<TrandisciplinaryapprovalhistoryModel?> _03(int id, TrandisciplinaryapprovalhistoryModel Trandeviationapprovalhistory, string schema, string conn)
    {
        string sql = $@"Update {schema}.Trandisciplinaryapprovalhistory set TranNumber = @TranNumber, Date = @Date, UserId = @UserId, Status = @Status, ApproverId = @ApproverId, ApproverRemarks = @ApproverRemarks where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeviationapprovalhistory, conn);

        sql = $@" select  * from {schema}.Trandisciplinaryapprovalhistory x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandisciplinaryapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }



    public async Task<TrandisciplinaryapprovalhistoryModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Trandisciplinaryapprovalhistory where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Trandisciplinaryapprovalhistory x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandisciplinaryapprovalhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}