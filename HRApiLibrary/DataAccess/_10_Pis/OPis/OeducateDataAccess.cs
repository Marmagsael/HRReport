using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OeducateDataAccess : IOeducateDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OeducateDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }
    public async Task<OeducateModel?> _01(OeducateModel educate, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Educate 
    					(EMPNUMBER, CODE, SCHOOL, FROM_, TO_, COURSE, LEVEL) values 
    					(@EMPNUMBER, @CODE, @SCHOOL, @FROM_, @TO_, @COURSE, @LEVEL)";
        await _sql.ExecuteCmd<dynamic>(sql, educate, conn);

        sql = $@"SELECT * FROM {schema}.Empmas WHERE EMPNUMBER = @EMPNUMBER";

        var res = await _sql.FetchData<OeducateModel?, dynamic>(sql, new { Empnumber = educate.EMPNUMBER }, conn);

        return res.FirstOrDefault();
    }


    public async Task<List<OeducateModel?>?> _02(string empnumber, string schema, string conn)
    {
        var sql = $@"select  * from {schema}.Educate where Empnumber = @Empnumber";
        var data = await _sql.FetchData<OeducateModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data;
    }


    public async Task<OeducateModel?> _03(int id, OeducateModel empmas, string schema, string conn)
    {
        string sql = $@"Update {schema}.Educate set 
                                CODE = @CODE, SCHOOL = @SCHOOL, FROM_ = @FROM, TO_ = @_TO, COURSE = @COURSE, LEVEL = @LEVEL where EMPNUMBER = @EMPNUMBER;";
        await _sql.ExecuteCmd<dynamic>(sql, empmas, conn);

        sql = $@" select  * from {schema}.Educate x where x.EMPNUMBER = @EMPNUMBER ;";
        var data = await _sql.FetchData<OeducateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OeducateModel?> _04(int empnumber, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Educate where EMPNUMBER = @EMPNUMBER;";
        // await _sql.ExecuteCmd<dynamic>(sql, new {Id=id},conn);

        sql = $@" select  * from {schema}.Educate x where x.EMPNUMBER = @EMPNUMBER ;";
        var data = await _sql.FetchData<OeducateModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
        return data?.FirstOrDefault();
    }


}

public interface IOeducateDataAccess
{
    Task<OeducateModel?> _01(OeducateModel educate, string schema, string conn);
    Task<List<OeducateModel?>?> _02(string empnumber, string schema, string conn);
    Task<OeducateModel?> _03(int id, OeducateModel empmas, string schema, string conn);
    Task<OeducateModel?> _04(int empnumber, string schema, string conn);
}