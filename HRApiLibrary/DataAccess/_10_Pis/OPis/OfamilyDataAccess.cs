using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OfamilyDataAccess : IOfamilyDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OfamilyDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }




    public async Task<List<OfamilyModel?>?> _02(string schema, string conn)
    {
        var sql = $@"select EMPNUMBER,NAME, NULLIF(BIRTH, '0000-00-00') AS BIRTH,RELATION from {schema}.family ";
        var data = await _sql.FetchData<OfamilyModel?, dynamic>(sql, new { }, conn);
        return data;
    }


}


public interface IOfamilyDataAccess
{
    Task<List<OfamilyModel?>?> _02(string schema, string conn);
}