using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OcivstatDataAccess : IOcivstatDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OcivstatDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }




    public async Task<List<OcivstatModel?>?> _02( string schema, string conn)
    {
        var sql = $@"select  * from {schema}.Civstat ";
        var data = await _sql.FetchData<OcivstatModel?, dynamic>(sql, new {  }, conn);
        return data;
    }


}


public interface IOcivstatDataAccess
{
    Task<List<OcivstatModel?>?> _02( string schema, string conn);
}
