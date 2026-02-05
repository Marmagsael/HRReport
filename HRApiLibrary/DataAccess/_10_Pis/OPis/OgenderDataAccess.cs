using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OgenderDataAccess : IOgenderDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OgenderDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }




    public async Task<List<OgenderModel?>?> _02(string schema, string conn)
    {
        var sql = $@"select  * from {schema}.Sex ";
        var data = await _sql.FetchData<OgenderModel?, dynamic>(sql, new { }, conn);
        return data;
    }


}


public interface IOgenderDataAccess
{
    Task<List<OgenderModel?>?> _02(string schema, string conn);
}