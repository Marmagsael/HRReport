using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class EmpblockpostDataAccess : IEmpblockpostDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EmpblockpostDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<EmpblockpostModel?> _01(EmpblockpostModel empblockpost, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empblockpost (EmpmasId, DeploymentId) values (@EmpmasId, @DeploymentId); 
                        SELECT * FROM {schema}.Empblockpost WHERE EmpmasId = @EmpmasId and DeploymentId = @DeploymentId";
        var res = await _sql.FetchData<EmpblockpostModel?, dynamic>(sql, empblockpost, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpblockpostModel?> _02(int EmpmasId, int DeploymentId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empblockpost where EmpmasId = @EmpmasId and DeploymentId = @DeploymentId";
        var data = await _sql.FetchData<EmpblockpostModel?, dynamic>(sql, new { EmpmasId, DeploymentId  }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<EmpblockpostModel?>?> _02(int EmpmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empblockpost where EmpmasId = @EmpmasId ";
        var data = await _sql.FetchData<EmpblockpostModel?, dynamic>(sql, new { EmpmasId }, conn);
        return data;
    }

    public async Task<List<EmpblockpostModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empblockpost ";
        var data = await _sql.FetchData<EmpblockpostModel?, dynamic>(sql, new { }, conn);
        return data;
    }



    public async Task<EmpblockpostModel?> _04(EmpblockpostModel rec, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empblockpost where EmpmasId = @EmpmasId and DeploymentId = @DeploymentId;";
        await _sql.ExecuteCmd<dynamic>(sql, new { rec.EmpmasId, rec.DeploymentId }, conn);

        sql = $@" select  * from {schema}.Empblockpost x where EmpmasId = @EmpmasId and DeploymentId = @DeploymentId ;";
        var data = await _sql.FetchData<EmpblockpostModel?, dynamic>(sql, new { rec.EmpmasId,  rec.DeploymentId }, conn);
        return data?.FirstOrDefault();
    }
}