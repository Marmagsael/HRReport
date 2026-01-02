using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class DeviationDataAccess : IDeviationDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public DeviationDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<DeviationModel?> _01(DeviationModel deviation, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Deviation (Control_No, Prep_ById, Prep_Dt, CoId, EmpNumber, Dev_NO, Occur_Dt, Freq_No, Penalty_No, Appr_BYId, Appr_DT, DevStart, DevEnd) values (@Control_No, @Prep_ById, @Prep_Dt, @CoId, @EmpNumber, @Dev_NO, @Occur_Dt, @Freq_No, @Penalty_No, @Appr_BYId, @Appr_DT, @DevStart, @DevEnd)";
        await _sql.ExecuteCmd<dynamic>(sql, deviation, conn);

        sql = $@"SELECT * FROM {schema}.Deviation WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<DeviationModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<DeviationModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  d.* from {schema}.Deviation d where d.Id = @Id";
        var data = await _sql.FetchData<DeviationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<DeviationModel>?> _02ByEmpNumber(string empno, string schema, string conn)
    {
        string sql = $@" SELECT d.*, dd.dev_name devName, p.desc_ penalty FROM {schema}.Deviation d
                        LEFT JOIN secpis.devdata dd on d.Dev_No = dd.Dev_No
                        LEFT JOIN secpis.penalty p on p.penalty_no = d.penalty_no
                        WHERE EmpNumber = @EmpNumber Order By d.Occur_dt ";

        var data = await _sql.FetchData<DeviationModel, dynamic>(sql, new { EmpNumber = empno }, conn);
        
        var data2 = (data==null) ? [] : data.ToList();

        return data2;
    }



    public async Task<DeviationModel?> _02ByControlNo(string controlNo, string schema, string conn)
    {
        string sql = $@" SELECT * FROM {schema}.Deviation WHERE Control_No = @Control_No";

        var data = await _sql.FetchData<DeviationModel, dynamic>(sql, new { Control_No = controlNo }, conn);
        return data?.FirstOrDefault();
    }




    public async Task<DeviationModel?> _03(int id, DeviationModel deviation, string schema, string conn)
    {
        string sql = $@"Update {schema}.Deviation set Control_No = @Control_No, Prep_ById = @Prep_ById, Prep_Dt = @Prep_Dt, CoId = @CoId, EmpNumber = @EmpNumber, Dev_NO = @Dev_NO, Occur_Dt = @Occur_Dt, Freq_No = @Freq_No, Penalty_No = @Penalty_No, Appr_BYId = @Appr_BYId, Appr_DT = @Appr_DT, DevStart = @DevStart, DevEnd = @DevEnd where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, deviation, conn);

        sql = $@" select  * from {schema}.Deviation x where x.Id = @Id ;";
        var data = await _sql.FetchData<DeviationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<DeviationModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Deviation where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Deviation x where x.Id = @Id ;";
        var data = await _sql.FetchData<DeviationModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
