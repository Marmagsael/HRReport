using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class TranreinstatementDataAccess : ITranreinstatementDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public TranreinstatementDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TranreinstatementModel?> _01(TranreinstatementModel Tranreinstatement, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Tranreinstatement (TranNumber, IdEmpmas, PrepDate, DepStart, DepEnd, DateApproved,  Mode, IdEmploymentType, IdDivision, IdSection, IdDepartment, IdPosition, IdDesignation, IdPayrollGrp, IdDeployment, IdApprover, MarkApprove) values (@TranNumber, @IdEmpmas, @PrepDate, @DepStart, @DepEnd, @DateApproved,  @Mode, @IdEmploymentType, @IdDivision, @IdSection, @IdDepartment, @IdPosition, @IdDesignation, @IdPayrollGrp, @IdDeployment, @IdApprover, @MarkApprove)";
        await _sql.ExecuteCmd<dynamic>(sql, Tranreinstatement, conn);
        sql = $@"SELECT * FROM {schema}.Tranreinstatement WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TranreinstatementModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TranreinstatementModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Tranreinstatement where Id = @Id";
        var data = await _sql.FetchData<TranreinstatementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TranreinstatementModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Tranreinstatement where IdEmpmas = @IdEmpmas;";
        var data = await _sql.FetchData<TranreinstatementModel?, dynamic>(sql, new { IdEmpmas = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TranreinstatementModel?> _02ByTrnNumber(string trnNumber, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Tranreinstatement where TranNumber = @TranNumber";
        var data = await _sql.FetchData<TranreinstatementModel?, dynamic>(sql, new { TranNumber = trnNumber }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TranreinstatementModel?> _03(int id, TranreinstatementModel Tranreinstatement, string schema, string conn)
    {
        string sql = $@"Update {schema}.Tranreinstatement set IdEmpmas = @IdEmpmas, TranNumber = @TranNumber, PrepDate = @PrepDate, DepStart = @DepStart, DepEnd = @DepEnd, DateApproved = @DateApproved,  Mode = @Mode, IdEmploymentType = @IdEmploymentType, IdDivision = @IdDivision, IdSection = @IdSection, IdDepartment = @IdDepartment, IdPosition = @IdPosition, IdDesignation = @IdDesignation, IdPayrollGrp = @IdPayrollGrp, IdDeployment = @IdDeployment, IdApprover = @IdApprover, MarkApprove = @MarkApprove where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Tranreinstatement, conn);

        sql = $@" select  * from {schema}.Tranreinstatement x where x.Id = @Id ;";
        var data = await _sql.FetchData<TranreinstatementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TranreinstatementModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Tranreinstatement where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Tranreinstatement x where x.Id = @Id ;";
        var data = await _sql.FetchData<TranreinstatementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
