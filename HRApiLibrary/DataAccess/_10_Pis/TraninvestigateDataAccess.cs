using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class TraninvestigateDataAccess : ITraninvestigateDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TraninvestigateDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TraninvestigateModel?> _01(TraninvestigateModel Traninvestigate, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Traninvestigate (IdEmpmas, TranNumber, PrepDate, Prep_ById, Mode, StartDate, EndDate, Remarks, EmpStatusId, IdApprover, MarkApprove) values (@IdEmpmas, @TranNumber, @PrepDate, @Prep_ById, @Mode, @StartDate, @EndDate, @Remarks, @EmpStatusId, @IdApprover, @MarkApprove)";
        await _sql.ExecuteCmd<dynamic>(sql, Traninvestigate, conn);

        sql = $@"SELECT * FROM {schema}.Traninvestigate WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<TraninvestigateModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TraninvestigateModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Traninvestigate where Id = @Id ";
        var data = await _sql.FetchData<TraninvestigateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TraninvestigateModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Traninvestigate where IdEmpmas = @EmpmasId and 
                        PrepDate = (select max(PrepDate) from {schema}.Traninvestigate  where IdEmpmas = @EmpmasId) ";
        var data = await _sql.FetchData<TraninvestigateModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<TraninvestigateModel?> _03(int id, TraninvestigateModel Traninvestigate, string schema, string conn)
    {
        string sql = $@"Update {schema}.Traninvestigate set IdEmpmas = @IdEmpmas, TranNumber = @TranNumber, PrepDate = @PrepDate, Prep_ById =@Prep_ById, Mode = @Mode, StartDate = @StartDate, EndDate = @EndDate, Remarks = @Remarks, EmpStatusId = @EmpStatusId, IdApprover = @IdApprover, MarkApprove = @MarkApprove where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Traninvestigate, conn);

        sql = $@" select  * from {schema}.Traninvestigate x where x.Id = @Id ;";
        var data = await _sql.FetchData<TraninvestigateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TraninvestigateModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Traninvestigate where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Traninvestigate x where x.Id = @Id ;";
        var data = await _sql.FetchData<TraninvestigateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}