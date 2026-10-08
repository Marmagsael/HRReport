
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OCivstatDataAccess : IOCivstatDataAccess
    {
        private readonly I_90_001_MySqlDataAccess _sql;

        public OCivstatDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OCivstatModel?> _01(OCivstatModel civstat, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Civstat (CODE, NAME) values (@CODE, @NAME)";
            await _sql.ExecuteCmd<dynamic>(sql, civstat, conn);

            sql = $@"SELECT * FROM {schema}.Civstat WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@Code))";

            var res = await _sql.FetchData<OCivstatModel?, dynamic>(sql, new { Code = civstat.Code?.Trim().ToUpper() }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OCivstatModel?>?> _02(string? schema, string? conn)
        {
            string? sql = $@"select  CODE, NAME from {schema}.Civstat order by NAME";
            var data = await _sql.FetchData<OCivstatModel?, dynamic>(sql, new { }, conn);
            return data;
        }

        public async Task<List<OCivstatModel?>?> _02ByCodeorName(string code, string name, string schema, string conn)
        {
            string sql = $@"SELECT * FROM {schema}.Civstat  WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@Code))  OR TRIM(UPPER(Name)) = TRIM(UPPER(@Name)) ";

            var data = await _sql.FetchData<OCivstatModel?, dynamic>(sql, new { Code = code, Name = name }, conn);
            return data;
        }

        public async Task<OCivstatModel?> _03(string code, OCivstatModel civstat, string schema, string conn)
        {
            var parameters = new
            {
                oldCode = code,
                civstat.Code,
                civstat.Name,
            };

            string sql = $@"UPDATE {schema}.Civstat  SET Code = @Code, Name = @Name WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@oldCode));";

            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);

            sql = $@"SELECT * FROM {schema}.Civstat x WHERE TRIM(UPPER(x.Code)) = TRIM(UPPER(@Code));";

            var data = await _sql.FetchData<OCivstatModel?, dynamic>(sql, new { Code = civstat.Code }, conn);
            return data?.FirstOrDefault();
        }




        public async Task<OCivstatModel?> _04(string? code, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Civstat WHERE TRIM(UPPER(Code)) = TRIM(UPPER(@Code));";
            await _sql.ExecuteCmd<dynamic>(sql, new { Code = code}, conn);

            sql = $@" select  * from {schema}.Procode x WHERE TRIM(UPPER(x.Code)) = TRIM(UPPER(@Code)) ;";
            var data = await _sql.FetchData<OCivstatModel?, dynamic>(sql, new { Code = code }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOCivstatDataAccess
{
    Task<OCivstatModel?> _01(OCivstatModel civstat, string? schema, string? conn);
    Task<List<OCivstatModel?>?> _02(string? schema, string? conn);
    Task<List<OCivstatModel?>?> _02ByCodeorName(string code, string name, string schema, string conn);
    Task<OCivstatModel?> _03(string code, OCivstatModel civstat, string schema, string conn);
    Task<OCivstatModel?> _04(string? code, string? schema, string? conn);
}