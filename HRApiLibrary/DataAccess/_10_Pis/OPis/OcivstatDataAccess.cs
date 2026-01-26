using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OemergencDataAccess : IOemergencDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OemergencDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }




    public async Task<List<OemergencModel?>?> _02( string schema, string conn)
    {
        var sql = $@"select  * from {schema}.emergenc ";
        var data = await _sql.FetchData<OemergencModel?, dynamic>(sql, new {  }, conn);
        return data;
    }


}


public interface IOemergencDataAccess
{
    Task<List<OemergencModel?>?> _02( string schema, string conn);
}
