using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class TranexonerateDataAccess : ITranexonerateDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TranexonerateDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TranexonerateModel?> _01(TranexonerateModel Tranexonerate, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Tranexonerate (IdEmpmas, TranNumber, PrepDate, Prep_ById, Mode,EmpStatusId, IdApprover, MarkApprove) values (@IdEmpmas, @TranNumber, @PrepDate, @Prep_ById, @Mode,  @EmpStatusId, @IdApprover, @MarkApprove);
                        UPDATE  {schema}.Tranexonerateother set Remarks = @Remarks WHERE TranNumber = @TranNumber";
        await _sql.ExecuteCmd<dynamic>(sql, Tranexonerate, conn);

        sql = $@"SELECT * FROM {schema}.Tranexonerate WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TranexonerateModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TranexonerateModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Tranexonerate where Id = @Id ";
        var data = await _sql.FetchData<TranexonerateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TranexonerateModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  t.*, t2.remarks from {schema}.Tranexonerate t
                        left join {schema}.Tranexonerateother t2 on t.Id = t2.Id
                        where t.IdEmpmas = @IdEmpmas and 
                        PrepDate = (select max(PrepDate) from {schema}.Tranexonerate  where IdEmpmas = @IdEmpmas);
                    ";
        var data = await _sql.FetchData<TranexonerateModel?, dynamic>(sql, new { IdEmpmas = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TranexonerateModel?> _03(int id, TranexonerateModel Tranexonerate, string schema, string conn)
    {
        string sql = $@"Update {schema}.Tranexonerate set IdEmpmas = @IdEmpmas, TranNumber = @TranNumber, PrepDate = @PrepDate, Prep_ById = @Prep_ById, Mode = @Mode, Remarks = @Remarks, EmpStatusId = @EmpStatusId, IdApprover = @IdApprover, MarkApprove = @MarkApprove where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Tranexonerate, conn);

        sql = $@" select  * from {schema}.Tranexonerate x where x.Id = @Id ;";
        var data = await _sql.FetchData<TranexonerateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TranexonerateModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Tranexonerate where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Tranexonerate x where x.Id = @Id ;";
        var data = await _sql.FetchData<TranexonerateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}