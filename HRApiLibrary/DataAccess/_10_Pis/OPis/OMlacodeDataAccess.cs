
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OMlacodeDataAccess : IOMlacodeDataAccess
    {
        private readonly I_90_001_MySqlDataAccess _sql;

        public OMlacodeDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OMlacodeModel?> _01(OMlacodeModel mlacode, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Mlacode (CODE, NAME) values (@CODE, @NAME)";
            await _sql.ExecuteCmd<dynamic>(sql, mlacode, conn);

            sql = $@"SELECT * FROM {schema}.Mlacode WHERE TRIM(UPPER(Code)) = @Code";

            var res = await _sql.FetchData<OMlacodeModel?, dynamic>(sql, new { Code = mlacode.Code?.Trim().ToUpper() }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OMlacodeModel?>?> _02(string? schema, string? conn)
        {
            string? sql = $@"select  CODE, NAME from {schema}.Mlacode ORDER BY NAME";
            var data = await _sql.FetchData<OMlacodeModel?, dynamic>(sql, new {}, conn);
            return data;
        }


        public async Task<List<OMlacodeModel?>?> _02(string code, string name, string schema, string conn)
        {
            string sql = $@"SELECT * FROM {schema}.Mlacode  WHERE TRIM(UPPER(Code)) = @Code  OR TRIM(UPPER(Name)) = @Name";

            var data = await _sql.FetchData<OMlacodeModel?, dynamic>(sql, new { Code = code, Name = name }, conn);
            return data;
        }


        public async Task<OMlacodeModel?> _03(string code, OMlacodeModel mla, string schema, string conn)
        {
            var parameters = new
            {
                oldCode = code,
                mla.Code,
                mla.Name,
            };

            string sql = $@"UPDATE {schema}.Mlacode  SET Code = @Code, Name = @Name WHERE TRIM(UPPER(Code)) = @oldCode;";

            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);

            sql = $@"SELECT * FROM {schema}.Mlacode x WHERE TRIM(UPPER(x.Code)) = @Code;";

            var data = await _sql.FetchData<OMlacodeModel?, dynamic>(sql, new { Code = mla.Code?.Trim().ToUpper() }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OMlacodeModel?> _04(string? code, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Mlacode WHERE TRIM(UPPER(x.Code)) = @Code;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Code = code?.Trim().ToUpper() }, conn);

            sql = $@" select  * from {schema}.Mlacode x WHERE TRIM(UPPER(x.Code)) = @Code ;";
            var data = await _sql.FetchData<OMlacodeModel?, dynamic>(sql, new { Code = code?.Trim().ToUpper() }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOMlacodeDataAccess
{
    Task<OMlacodeModel?> _01(OMlacodeModel mlacode, string? schema, string? conn);
    Task<List<OMlacodeModel?>?> _02( string? schema, string? conn);
    Task<List<OMlacodeModel?>?> _02(string code, string name, string schema, string conn);
    Task<OMlacodeModel?> _03(string code, OMlacodeModel mla, string schema, string conn);
    Task<OMlacodeModel?> _04(string? code, string? schema, string? conn);
}