using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class LeavecreditDataAccess : ILeavecreditDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LeavecreditDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<LeavecreditModel?> _01(LeavecreditModel leavecredit, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Leavecredit 
                            (Year,  EmpmasId,  LeaveTypeId,  AnnivStart,  AnnivEnd,  Credit,  Consumed) values 
                            (@Year, @EmpmasId, @LeaveTypeId, @AnnivStart, @AnnivEnd, @Credit, @Consumed)";
        await _sql.ExecuteCmd<dynamic>(sql, leavecredit, conn);

        sql = $@"SELECT * FROM {schema}.Leavecredit WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<LeavecreditModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<LeavecreditModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Year, EmpmasId, LeaveTypeId, AnnivStart, AnnivEnd, Credit, Consumed 
                        from {schema}.Leavecredit where Id = @Id";
        var data = await _sql.FetchData<LeavecreditModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<LeavecreditModel?> _03(int id, LeavecreditModel leavecredit, string schema, string conn)
    {
        string sql = $@"Update {schema}.Leavecredit set 
                                Year        = @Year, 
                                EmpmasId    = @EmpmasId, 
                                LeaveTypeId = @LeaveTypeId, 
                                AnnivStart  = @AnnivStart, 
                                AnnivEnd    = @AnnivEnd, 
                                Credit      = @Credit, 
                                Consumed    = @Consumed where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, leavecredit, conn);

        sql = $@" select  * from {schema}.Leavecredit x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavecreditModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<LeavecreditModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Leavecredit where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Leavecredit x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavecreditModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
