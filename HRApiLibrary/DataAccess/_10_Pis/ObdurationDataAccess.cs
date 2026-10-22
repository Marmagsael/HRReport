using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class ObdurationDataAccess : IObdurationDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public ObdurationDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // --- Day Type dropdown ( replaces the hard-coded dayTypes list in _12_102_OBRequest )
    public async Task<List<ObdurationModel?>?> _02s(string? schema, string? conn)
    {
        string? sql = $@"select  Id, Name from {schema}.ObDuration order by Id";
        var data = await _sql.FetchData<ObdurationModel?, dynamic>(sql, new { }, conn);
        return data;
    }

}


public interface IObdurationDataAccess
{
    Task<List<ObdurationModel?>?>   _02s(string? schema, string? conn);
}
