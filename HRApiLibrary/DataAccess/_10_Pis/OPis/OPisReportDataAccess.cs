using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OPisReportDataAccess : IOPisReportDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OPisReportDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }


    public async Task<List<OClientModel>> _02Client(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Client order by ClName ";
        var data = await _sql.FetchData<OClientModel, dynamic>(sql, new { }, conn);
        return data ?? [];
    }

    public async Task<List<OClientModel>> _02ClientByStatus(string status, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Client where Status = @Status order by ClName ";
        var data = await _sql.FetchData<OClientModel, dynamic>(sql, new { Status = status }, conn);
        return data ?? [];
    }

    public async Task<List<OEmpstatModel>> _02Empstats(string status, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empstat order by Name ";
        var data = await _sql.FetchData<OEmpstatModel, dynamic>(sql, new {  }, conn);
        return data ?? [];
    }



}

public interface IOPisReportDataAccess
{
    Task<List<OClientModel>> _02Client(string schema, string conn);
    Task<List<OClientModel>> _02ClientByStatus(string status, string schema, string conn);
    Task<List<OEmpstatModel>> _02Empstats(string status, string schema, string conn); 
}
