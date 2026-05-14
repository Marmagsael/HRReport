using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class TrandisciplinaryapprovalDataAccess : ITrandisciplinaryapprovalDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TrandisciplinaryapprovalDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TrandisciplinaryapprovalModel?> _01(TrandisciplinaryapprovalModel Trandisciplinary, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Trandisciplinaryapproval (IdEmpmas, TranNumber, PrepDate, Mode, Penalty_No, StartDate, EndDate, NoOfDays, EmpStatusId) values (@IdEmpmas, @TranNumber, @PrepDate, @Mode,@Penalty_No, @StartDate, @EndDate, @NoOfDays, @EmpStatusId)";
        await _sql.ExecuteCmd<dynamic>(sql, Trandisciplinary, conn);

        sql = $@"SELECT * FROM {schema}.Trandisciplinaryapproval WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TrandisciplinaryapprovalModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TrandisciplinaryapprovalModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Trandisciplinaryapproval where Id = @Id ";
        var data = await _sql.FetchData<TrandisciplinaryapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandisciplinaryapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Trandisciplinaryapproval where IdEmpmas = @IdEmpmas and PrepDate = (select max(PrepDate) from {schema}.Trandisciplinaryapproval where IdEmpmas = @IdEmpmas);";
        var data = await _sql.FetchData<TrandisciplinaryapprovalModel?, dynamic>(sql, new { IdEmpmas = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandisciplinaryapprovalModel?> _03(int id, TrandisciplinaryapprovalModel Trandisciplinary, string schema, string conn)
    {
        string sql = $@"Update {schema}.Trandisciplinaryapproval set  IdEmpmas = @IdEmpmas, TranNumber = @TranNumber, PrepDate = @PrepDate, Mode =@Mode, Penalty_No =@Penalty_No, StartDate =@StartDate, EndDate =@EndDate, NoOfDays =@NoOfDays, EmpStatusId =@EmpStatusId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Trandisciplinary, conn);

        sql = $@" select  * from {schema}.Trandisciplinaryapproval x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandisciplinaryapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandisciplinaryapprovalModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Trandisciplinaryapproval where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Trandisciplinaryapproval x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandisciplinaryapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}