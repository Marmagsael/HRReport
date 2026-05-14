using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class TrandisciplinaryDataAccess : ITrandisciplinaryDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TrandisciplinaryDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TrandisciplinaryModel?> _01(TrandisciplinaryModel Trandisciplinary, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Trandisciplinary (IdEmpmas, TranNumber, PrepDate, Mode, Penalty_No, StartDate, EndDate, NoOfDays, EmpStatusId) values (@IdEmpmas, @TranNumber, @PrepDate, @Mode,@Penalty_No, @StartDate, @EndDate, @NoOfDays, @EmpStatusId)";
        await _sql.ExecuteCmd<dynamic>(sql, Trandisciplinary, conn);

        sql = $@"SELECT * FROM {schema}.Trandisciplinary WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TrandisciplinaryModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TrandisciplinaryModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Trandisciplinary where Id = @Id ";
        var data = await _sql.FetchData<TrandisciplinaryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandisciplinaryModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Trandisciplinary where IdEmpmas = @IdEmpmas and PrepDate = (select max(PrepDate) from {schema}.Trandisciplinary where IdEmpmas = @IdEmpmas);";
        var data = await _sql.FetchData<TrandisciplinaryModel?, dynamic>(sql, new { IdEmpmas = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandisciplinaryModel?> _03(int id, TrandisciplinaryModel Trandisciplinary, string schema, string conn)
    {
        string sql = $@"Update {schema}.Trandisciplinary set  IdEmpmas = @IdEmpmas, TranNumber = @TranNumber, PrepDate = @PrepDate, Mode =@Mode, Penalty_No =@Penalty_No, StartDate =@StartDate, EndDate =@EndDate, NoOfDays =@NoOfDays, EmpStatusId =@EmpStatusId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Trandisciplinary, conn);

        sql = $@" select  * from {schema}.Trandisciplinary x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandisciplinaryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandisciplinaryModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Trandisciplinary where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Trandisciplinary x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandisciplinaryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}