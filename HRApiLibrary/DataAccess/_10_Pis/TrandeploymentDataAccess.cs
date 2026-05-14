using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class TrandeploymentDataAccess : ITrandeploymentDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public TrandeploymentDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TrandeploymentModel?> _01(TrandeploymentModel Trandeployment, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Trandeployment (TranNumber, IdEmpmas, PrepDate, DepStart, DepEnd, DateApproved,  Mode, IdEmploymentType, IdDivision, IdSection, IdDepartment, IdPosition, IdDesignation, IdPayrollGrp, IdDeployment, IdApprover, MarkApprove) values (@TranNumber, @IdEmpmas, @PrepDate, @DepStart, @DepEnd, @DateApproved,  @Mode, @IdEmploymentType, @IdDivision, @IdSection, @IdDepartment, @IdPosition, @IdDesignation, @IdPayrollGrp, @IdDeployment, @IdApprover, @MarkApprove)";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeployment, conn);
        sql = $@"SELECT * FROM {schema}.Trandeployment WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TrandeploymentModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TrandeploymentModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Trandeployment where Id = @Id";
        var data = await _sql.FetchData<TrandeploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TrandeploymentModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Trandeployment where IdEmpmas = @IdEmpmas;";
        var data = await _sql.FetchData<TrandeploymentModel?, dynamic>(sql, new { IdEmpmas = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandeploymentModel?> _02ByTrnNumber(string trnNumber, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Trandeployment where TranNumber = @TranNumber";
        var data = await _sql.FetchData<TrandeploymentModel?, dynamic>(sql, new { TranNumber = trnNumber }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TrandeploymentModel?> _03(int id, TrandeploymentModel Trandeployment, string schema, string conn)
    {
        string sql = $@"Update {schema}.Trandeployment set IdEmpmas = @IdEmpmas, TranNumber = @TranNumber, PrepDate = @PrepDate, DepStart = @DepStart, DepEnd = @DepEnd, DateApproved = @DateApproved,  Mode = @Mode, IdEmploymentType = @IdEmploymentType, IdDivision = @IdDivision, IdSection = @IdSection, IdDepartment = @IdDepartment, IdPosition = @IdPosition, IdDesignation = @IdDesignation, IdPayrollGrp = @IdPayrollGrp, IdDeployment = @IdDeployment, IdApprover = @IdApprover, MarkApprove = @MarkApprove where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeployment, conn);

        sql = $@" select  * from {schema}.Trandeployment x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandeploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandeploymentModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Trandeployment where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Trandeployment x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandeploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
