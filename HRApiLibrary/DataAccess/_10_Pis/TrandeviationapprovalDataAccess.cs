using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

public class TrandeviationapprovalDataAccess : ITrandeviationapprovalDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TrandeviationapprovalDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TrandeviationapprovalModel?> _01(TrandeviationapprovalModel Trandeviationapproval, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Trandeviationapproval (IdEmpmas, TranNumber, PrepDate, Mode, ReportDate, OccurDate, Allegation, Freq_No, EmpStatusId, IdApprover, MarkApprove) values (@IdEmpmas, @TranNumber, @PrepDate, @Mode, @ReportDate, @OccurDate, @Allegation,@Freq_No, @EmpStatusId, @IdApprover, @MarkApprove);
                        Insert into {schema}.Trandeviationother    (Remarks, Link, TranNumber) values (@Remarks, @Link, @TranNumber)
                        ";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeviationapproval, conn);

        sql = $@"SELECT t.*, tdo.* FROM {schema}.Trandeviationapproval  t
                LEFT JOIN {schema}.Trandeviationother tdo on t.TranNumber = tdo.TranNumber 
                WHERE t.ID = (SELECT @@IDENTITY);";

        var res = await _sql.FetchData<TrandeviationapprovalModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }

    public async Task<List<TrandeviationapprovalModel?>> _02(string schema, string conn)
    {
        string sql = $"select * from {schema}.Trandeviationapproval";
        var data = await _sql.FetchData<TrandeviationapprovalModel?, dynamic>(sql, new { }, conn);
        return data ?? new List<TrandeviationapprovalModel?>();
    }


    public async Task<TrandeviationapprovalModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Trandeviationapproval where Id = @Id ";
        var data = await _sql.FetchData<TrandeviationapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data.FirstOrDefault();
    }

    public async Task<List<TrandeviationapprovalModel?>> _02DistinctId(string schema, string conn)
    {
        string sql = $"select Distinct(IdEmpmas) from {schema}.Trandeviationapproval";
        var data = await _sql.FetchData<TrandeviationapprovalModel?, dynamic>(sql, new { }, conn);
        return data ?? new List<TrandeviationapprovalModel?>();
    }

    public async Task<TrandeviationapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select t.*, tdo.*  from {schema}.Trandeviationapproval t
                      left join {schema}.Trandeviationother tdo on t.trannumber = tdo.trannumber
                    where t.IdEmpmas = @IdEmpmas and PrepDate = (select max(PrepDate) from {schema}.Trandeviationapproval where IdEmpmas = @IdEmpmas);";
        var data = await _sql.FetchData<TrandeviationapprovalModel?, dynamic>(sql, new { IdEmpmas = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandeviationapprovalModel?> _03(int id, TrandeviationapprovalModel Trandeviationapproval, string schema, string conn)
    {
        string sql = $@"Update {schema}.Trandeviationapproval set IdEmpmas = @IdEmpmas, TranNumber = @TranNumber, PrepDate = @PrepDate, Mode = @Mode, ReportDate = @ReportDate,  OccurDate= @OccurDate, Allegation = @Allegation, Freq_No = @Freq_No, EmpStatusId = @EmpStatusId, IdApprover = @IdApprover , MarkApprove = @MarkApprove where Id = @Id;
                        Update {schema}.Trandeviationother set Remarks = @Remarks, Link = @Link where TranNumber = @TranNumber";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeviationapproval, conn);

        sql = $@" select  * from {schema}.Trandeviationapproval  x left join {schema}.Trandeviationother tdo on x.trannumber = tdo.trannumber where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandeviationapprovalModel?, dynamic>(sql, new { Id = id, Trandeviationapproval.TranNumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandeviationapprovalModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Trandeviationapproval where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Trandeviationapproval x where x.Id = @Id ;";
        var data = await _sql.FetchData<TrandeviationapprovalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}