using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OProcodeDataAccess : IOProcodeDataAccess
    {
        private readonly I_90_001_MySqlDataAccess _sql;

        public OProcodeDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OProcodeModel?> _01(OProcodeModel procode, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Procode (CODE, NAME) values (@CODE, @NAME)";
            await _sql.ExecuteCmd<dynamic>(sql, procode, conn);

            sql = $@"SELECT * FROM {schema}.Procode  WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@Code))";

            var res = await _sql.FetchData<OProcodeModel?, dynamic>(sql, new { Code = procode.Code }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OProcodeModel?>?> _02( string? schema, string? conn)
        {
            string? sql = $@"select  CODE, NAME from {schema}.Procode ORDER BY Name ";
            var data = await _sql.FetchData<OProcodeModel?, dynamic>(sql, new {  }, conn);
            return data;
        }

        public async Task<List<OProcodeModel?>?> _02ByCodeOrByName(string code, string name, string schema, string conn)
        {
            string sql = $@"SELECT * FROM {schema}.Procode  WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@Code))  OR TRIM(UPPER(Name)) = TRIM(UPPER(@Name))";

            var data = await _sql.FetchData<OProcodeModel?, dynamic>(sql, new { Code = code, Name = name }, conn);
            return data;
        }


        public async Task<OProcodeModel?> _03(string code, OProcodeModel prov, string schema, string conn)
        {
            var parameters = new
            {
                oldCode = code,
                prov.Code,
                prov.Name,
            };

            string sql = $@"UPDATE {schema}.Procode  SET Code = @Code, Name = @Name WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@oldCode));";

            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);

            sql = $@"SELECT * FROM {schema}.Procode x WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@Code));";

            var data = await _sql.FetchData<OProcodeModel?, dynamic>(sql, new { Code = prov.Code?.Trim().ToUpper() }, conn);
            return data?.FirstOrDefault();
        }

       

        public async Task<OProcodeModel?> _04(string? code, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Procode WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@Code));";
            await _sql.ExecuteCmd<dynamic>(sql, new { Code = code?.Trim().ToUpper() }, conn);

            sql = $@" select  * from {schema}.Procode x WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@Code)) ;";
            var data = await _sql.FetchData<OProcodeModel?, dynamic>(sql, new { Code = code }, conn);
            return data?.FirstOrDefault();
        }
    }
}

    public interface IOProcodeDataAccess
    {
        Task<OProcodeModel?> _01(OProcodeModel procode, string? schema, string? conn);
        Task<List<OProcodeModel?>?> _02( string? schema, string? conn);
        Task<List<OProcodeModel?>?> _02ByCodeOrByName(string code, string name, string schema, string conn);
        Task<OProcodeModel?> _03(string code, OProcodeModel prov, string schema, string conn);
        Task<OProcodeModel?> _04(string? code, string? schema, string? conn);
    }