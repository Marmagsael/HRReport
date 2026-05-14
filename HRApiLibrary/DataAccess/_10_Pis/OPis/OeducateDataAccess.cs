
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

        public async Task<OEducateModel?> _01(OEducateModel educate, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Educate (EMPNUMBER, CODE, SCHOOL, FROM_, TO_, COURSE, LEVEL) values (@EMPNUMBER, @CODE, @SCHOOL, @FROM_, @TO_, @COURSE, @LEVEL)";
            await _sql.ExecuteCmd<dynamic>(sql, educate, conn);

            sql = $@"SELECT * FROM {schema}.Educate WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OEducateModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OEducateModel?>?> _02(string empnumber, string schema, string conn)
        {
            string sql = $@"select  EMPNUMBER, CODE, SCHOOL, FROM_, TO_, COURSE, LEVEL from {schema}.Educate where Empnumber = @Empnumber";
            var data = await _sql.FetchData<OEducateModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data;
        }


        public async Task<OEducateModel?> _03(int id, OEducateModel educate, string schema, string conn)
        {
            string sql = $@"Update {schema}.Educate set EMPNUMBER = @EMPNUMBER, CODE = @CODE, SCHOOL = @SCHOOL, FROM_ = @FROM_, TO_ = @TO_, COURSE = @COURSE, LEVEL = @LEVEL where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, educate, conn);

            sql = $@" select  * from {schema}.Educate x where x.Id = @Id ;";
            var data = await _sql.FetchData<OEducateModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OEducateModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Educate where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Educate x where x.Id = @Id ;";
            var data = await _sql.FetchData<OEducateModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOEducateDataAccess
{
    Task<OEducateModel?> _01(OEducateModel educate, string schema, string conn);
    Task<List<OEducateModel?>?> _02(string empnumber, string schema, string conn);
    Task<OEducateModel?> _03(int id, OEducateModel educate, string schema, string conn);
    Task<OEducateModel?> _04(int id, string schema, string conn);
}
