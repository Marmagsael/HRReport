using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class TraninvestigateapprovalDataAccess : ITraninvestigateapprovalDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TraninvestigateapprovalDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TraninvestigateapprovalModel?> _01(TraninvestigateapprovalModel Traninvestigateapproval, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Traninvestigateapproval (IdEmpmas, TranNumber, PrepDate, Prep_ById, Mode, StartDate, EndDate, Remarks, EmpStatusId, IdApprover, MarkApprove) values (@IdEmpmas, @TranNumber, @PrepDate, @Prep_ById, @Mode, @StartDate, @EndDate, @Remarks, @EmpStatusId, @IdApprover, @MarkApprove)";
        await _sql.ExecuteCmd<dynamic>(sql, Traninvestigateapproval, conn);

        sql = $@"SELECT * FROM {schema}.Traninvestigateapproval WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TraninvestigateapprovalModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TraninvestigateapprovalModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Traninvestigateapproval where Id = @Id ";
        var data = await _sql.FetchData<TraninvestigateapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TraninvestigateapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Traninvestigateapproval  where IdEmpmas = @EmpmasId and 
                        PrepDate = (select max(PrepDate) from {schema}.Traninvestigateapproval  where IdEmpmas = @EmpmasId) ";
        var data = await _sql.FetchData<TraninvestigateapprovalModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<TraninvestigateapprovalModel?> _03(int id, TraninvestigateapprovalModel Traninvestigateapproval, string schema, string conn)
    {
        string sql = $@"Update {schema}.Traninvestigateapproval set IdEmpmas = @IdEmpmas, TranNumber = @TranNumber, PrepDate = @PrepDate, Prep_ById =@Prep_ById, Mode = @Mode, StartDate = @StartDate, EndDate = @EndDate, Remarks = @Remarks, EmpStatusId = @EmpStatusId, IdApprover = @IdApprover, MarkApprove = @MarkApprove where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Traninvestigateapproval, conn);

        sql = $@" select  * from {schema}.Traninvestigateapproval x where x.Id = @Id ;";
        var data = await _sql.FetchData<TraninvestigateapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TraninvestigateapprovalModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Traninvestigateapproval where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Traninvestigateapproval x where x.Id = @Id ;";
        var data = await _sql.FetchData<TraninvestigateapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}