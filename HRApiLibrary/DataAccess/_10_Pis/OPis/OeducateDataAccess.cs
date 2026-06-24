
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OEducateDataAccess : IOEducateDataAccess
    {
        private readonly I_90_001_MySqlDataAccess _sql;

        public OEducateDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OEducateModel?> _01(OEducateModel educate, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Educate (EMPNUMBER, CODE, SCHOOL, FROM_, TO_, COURSE, LEVEL) values (@EMPNUMBER, @CODE, @SCHOOL, @FROM_, @TO_, @COURSE, @LEVEL)";
            await _sql.ExecuteCmd<dynamic>(sql, educate, conn);

            sql = $@"SELECT * FROM {schema}.Educate WHERE EMPNUMBER = @EMPNUMBER";

            var res = await _sql.FetchData<OEducateModel?, dynamic>(sql, new { educate.EmpNumber }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OEducateModel?>?> _02(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"select  EMPNUMBER, CODE, SCHOOL, FROM_, TO_, COURSE, LEVEL from {schema}.Educate where Empnumber = @Empnumber";
            var data = await _sql.FetchData<OEducateModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data;
        }

        public async Task<List<OEducateModel?>?> _02ByEmpnumberAndCode(string? empnumber, string code, string? schema, string? conn)
        {
            string? sql = $@"select  EMPNUMBER, CODE, SCHOOL, FROM_, TO_, COURSE, LEVEL from {schema}.Educate where Empnumber = @Empnumber and Code = @Code";
            var data = await _sql.FetchData<OEducateModel?, dynamic>(sql, new { Empnumber = empnumber, Code = code }, conn);
            return data;
        }

        


        public async Task<OEducateModel?> _03(string? empnumber, OEducateModel educate, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Educate set EMPNUMBER = @EMPNUMBER, CODE = @CODE, SCHOOL = @SCHOOL, FROM_ = @FROM_, TO_ = @TO_, COURSE = @COURSE, LEVEL = @LEVEL where Empnumber = @Empnumber and Code = @Code;";
            await _sql.ExecuteCmd<dynamic>(sql, educate, conn);

            sql = $@" select  * from {schema}.Educate x where x.Empnumber = @Empnumber;";
            var data = await _sql.FetchData<OEducateModel?, dynamic>(sql, new { Empnumber = empnumber, educate.Code }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OEducateModel?> _04(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Educate where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber }, conn);

            sql = $@" select  * from {schema}.Educate x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OEducateModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOEducateDataAccess
{
    Task<OEducateModel?> _01(OEducateModel educate, string? schema, string? conn);
    Task<List<OEducateModel?>?> _02(string? empnumber, string? schema, string? conn);
    Task<List<OEducateModel?>?> _02ByEmpnumberAndCode(string? empnumber, string code, string? schema, string? conn);
    Task<OEducateModel?> _03(string? empnumber, OEducateModel educate, string? schema, string? conn);
    Task<OEducateModel?> _04(string? empnumber, string? schema, string? conn);
}
