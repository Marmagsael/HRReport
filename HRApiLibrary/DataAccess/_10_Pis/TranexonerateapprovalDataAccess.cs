using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class TranexonerateapprovalDataAccess : ITranexonerateapprovalDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TranexonerateapprovalDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TranexonerateapprovalModel?> _01(TranexonerateapprovalModel Tranexonerateapproval, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Tranexonerateapproval (IdEmpmas, TranNumber, PrepDate, Prep_ById, Mode, EmpStatusId, IdApprover, MarkApprove) values (@IdEmpmas, @TranNumber, @PrepDate, @Prep_ById, @Mode, @EmpStatusId, @IdApprover, @MarkApprove);
                        Insert into {schema}.TranexonerateOther ( TranNumber, Remarks) values (@TranNumber, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, Tranexonerateapproval, conn);

        sql = $@"SELECT * FROM {schema}.Tranexonerateapproval tx left join {schema}.TranexonerateOther txo on txo.tranNumber = tx.tranNumber WHERE tx.ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TranexonerateapprovalModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TranexonerateapprovalModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Tranexonerateapproval where Id = @Id ";
        var data = await _sql.FetchData<TranexonerateapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TranexonerateapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  t.*, t2.remarks from {schema}.Tranexonerateapproval t
                        left join {schema}.Tranexonerateother t2 on t.TranNumber = t2.TranNumber
                        where t.IdEmpmas = @IdEmpmas and 
                        PrepDate = (select max(PrepDate) from {schema}.Tranexonerateapproval  where IdEmpmas = @IdEmpmas);
                    ";
        var data = await _sql.FetchData<TranexonerateapprovalModel?, dynamic>(sql, new { IdEmpmas = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TranexonerateapprovalModel?> _03(int id, TranexonerateapprovalModel Tranexonerateapproval, string schema, string conn)
    {
        string sql = $@"Update {schema}.Tranexonerateapproval set IdEmpmas = @IdEmpmas, TranNumber = @TranNumber, PrepDate = @PrepDate, Prep_ById = @Prep_ById, Mode = @Mode, EmpStatusId = @EmpStatusId, IdApprover = @IdApprover, MarkApprove = @MarkApprove where Id = @Id;
                        Update {schema}.Tranexonerateother set Remarks = @Remarks where TranNumber = @TranNumber";
        await _sql.ExecuteCmd<dynamic>(sql, Tranexonerateapproval, conn);

        sql = $@" select  * from {schema}.Tranexonerateapproval x where x.Id = @Id ;";
        var data = await _sql.FetchData<TranexonerateapprovalModel?, dynamic>(sql, new { Id = id , Tranexonerateapproval.TranNumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TranexonerateapprovalModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Tranexonerateapproval where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Tranexonerateapproval x where x.Id = @Id ;";
        var data = await _sql.FetchData<TranexonerateapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}