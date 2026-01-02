using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class TrandeviationDataAccess : ITrandeviationDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TrandeviationDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TrandeviationModel?> _01(TrandeviationModel Trandeviation, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Trandeviation (IdEmpmas, TranNumber, PrepDate, Mode, ReportDate, OccurDate, Allegation,  EmpStatusId, IdApprover, MarkApprove) values (@IdEmpmas, @TranNumber, @PrepDate, @Mode, @ReportDate, @OccurDate, @Allegation,  @EmpStatusId, @IdApprover, @MarkApprove);
                        Update {schema}.Trandeviationother set Remarks = @Remarks, Link = @Link where TranNumber = @TranNumber";
                    
                await _sql.ExecuteCmd<dynamic>(sql, Trandeviation, conn);

        sql = $@"SELECT t.*, tdo.* FROM {schema}.Trandeviation  t
                LEFT JOIN {schema}.Trandeviationother tdo on t.TranNumber = tdo.TranNumber 
                WHERE t.ID = (SELECT @@IDENTITY);";

        var res = await _sql.FetchData<TrandeviationModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<TrandeviationModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Trandeviation where Id = @Id ";
        var data = await _sql.FetchData<TrandeviationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandeviationModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"SELECT t.* FROM {schema}.Trandeviation t
                        WHERE t.IdEmpmas = @IdEmpmas AND t.PrepDate = (SELECT MAX(PrepDate) FROM {schema}.Trandeviation WHERE IdEmpmas = @IdEmpmas);";

        var data = await _sql.FetchData<TrandeviationModel?, dynamic>(sql, new { IdEmpmas = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandeviationModel?> _02ByTrn(string trn, string schema, string conn)
    {
        string sql = $@"SELECT t.*, tro.*, th.*, th.UserId PrepId, TRIM(CONCAT(
                        TRIM(IFNULL(e.EmpLastNm, '')), ', ',
                        TRIM(IFNULL(e.EmpFirstNm, '')), ' ',
                        TRIM(IFNULL(e.EmpMidNm, ''))
                      )) AS PreparedBy
                    FROM {schema}.Trandeviation t
                        LEFT JOIN {schema}.Trandeviationother tro ON tro.TranNumber = t.TranNumber
                        LEFT JOIN {schema}.trandeviationapprovalhistory th ON th.TranNumber   = t.TranNumber
                        left join mainpis.Empmas e         on e.Id     = th.UserId
                        WHERE t.TranNumber = @TranNumber;";

        var data = await _sql.FetchData<TrandeviationModel?, dynamic>(sql, new { TranNumber = trn }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandeviationModel?> _03(int id, TrandeviationModel trandeviation, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Trandeviation SET IdEmpmas = @IdEmpmas,TranNumber = @TranNumber,
                            PrepDate = @PrepDate, Mode = @Mode,ReportDate = @ReportDate, DateReported = @DateReported, Allegation = @Allegation,
                            OccurDate = @OccurDate, EmpStatusId = @EmpStatusId, IdApprover = @IdApprover, MarkApprove = @MarkApprove
                        WHERE Id = @Id;

                        UPDATE {schema}.trandeviationother SET Remarks = @Remarks, Link = @Link WHERE TranNumber = @TranNumber;";

        await _sql.ExecuteCmd<dynamic>(sql, trandeviation, conn);

        var data = await _02ByEmpmasId(trandeviation.IdEmpmas, schema, conn);
        return data;
    }


    public async Task<TrandeviationModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Trandeviation where Id = @Id;
                        Delete from  {schema}.trandeviationother where Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Trandeviation x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandeviationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}