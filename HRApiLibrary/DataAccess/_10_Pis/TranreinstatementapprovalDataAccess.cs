using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class TranreinstatementapprovalDataAccess : ITranreinstatementapprovalDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public TranreinstatementapprovalDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TranreinstatementapprovalModel?> _01(TranreinstatementapprovalModel tranmovement, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Tranreinstatementapproval (TranNumber, IdEmpmas, PrepDate, DepStart, DepEnd, DateApproved,  Mode, IdEmploymentType, IdDivision, IdSection, IdDepartment, IdPosition, IdDesignation, IdPayrollGrp,  IdDeployment, IdApprover, MarkApprove) values (@TranNumber, @IdEmpmas, @PrepDate, @DepStart, @DepEnd, @DateApproved,  @Mode, @IdEmploymentType, @IdDivision, @IdSection, @IdDepartment, @IdPosition, @IdDesignation, @IdPayrollGrp, @IdDeployment, @IdApprover, @MarkApprove)";
        await _sql.ExecuteCmd<dynamic>(sql, tranmovement, conn);
        sql = $@"SELECT * FROM {schema}.Tranreinstatementapproval WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TranreinstatementapprovalModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TranreinstatementapprovalModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  *  from {schema}.Tranreinstatementapproval where Id = @Id";
        var data = await _sql.FetchData<TranreinstatementapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TranreinstatementapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Tranreinstatementapproval where IdEmpmas = @IdEmpmas and PrepDate = (select max(PrepDate) from {schema}.Tranreinstatementapproval where IdEmpmas = @IdEmpmas);";
        var data = await _sql.FetchData<TranreinstatementapprovalModel?, dynamic>(sql, new { IdEmpmas = empmasId }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TranreinstatementapprovalModel?> _03(int id, TranreinstatementapprovalModel tranmovement, string schema, string conn)
    {
        string sql = $@"Update {schema}.Tranreinstatementapproval set IdEmpmas = @IdEmpmas, TranNumber = @TranNumber, PrepDate = @PrepDate, DepStart = @DepStart, DepEnd = @DepEnd, DateApproved = @DateApproved,  Mode = @Mode, IdEmploymentType = @IdEmploymentType, IdDivision = @IdDivision, IdSection = @IdSection, IdDepartment = @IdDepartment, IdPosition = @IdPosition, IdDesignation = @IdDesignation, IdPayrollGrp = @IdPayrollGrp, IdDeployment = @IdDeployment, IdApprover = @IdApprover, MarkApprove = @MarkApprove where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, tranmovement, conn);

        sql = $@" select  * from {schema}.Tranreinstatementapproval x where x.Id = @Id ;";
        var data = await _sql.FetchData<TranreinstatementapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TranreinstatementapprovalModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Tranreinstatementapproval where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Tranreinstatementapproval x where x.Id = @Id ;";
        var data = await _sql.FetchData<TranreinstatementapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
