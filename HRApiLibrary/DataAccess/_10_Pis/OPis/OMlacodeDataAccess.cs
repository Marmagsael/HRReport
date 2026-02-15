
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

        public async Task<OMlacodeModel?> _01(OMlacodeModel mlacode, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Mlacode (CODE, NAME) values (@CODE, @NAME)";
            await _sql.ExecuteCmd<dynamic>(sql, mlacode, conn);

            sql = $@"SELECT * FROM {schema}.Mlacode WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OMlacodeModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OMlacodeModel?>?> _02(string schema, string conn)
        {
            string sql = $@"select  CODE, NAME from {schema}.Mlacode ";
            var data = await _sql.FetchData<OMlacodeModel?, dynamic>(sql, new {}, conn);
            return data;
        }


        public async Task<OMlacodeModel?> _03(int id, OMlacodeModel mlacode, string schema, string conn)
        {
            string sql = $@"Update {schema}.Mlacode set CODE = @CODE, NAME = @NAME where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, mlacode, conn);

            sql = $@" select  * from {schema}.Mlacode x where x.Id = @Id ;";
            var data = await _sql.FetchData<OMlacodeModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OMlacodeModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Mlacode where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Mlacode x where x.Id = @Id ;";
            var data = await _sql.FetchData<OMlacodeModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOMlacodeDataAccess
{
    Task<OMlacodeModel?> _01(OMlacodeModel mlacode, string schema, string conn);
    Task<List<OMlacodeModel?>?> _02( string schema, string conn);
    Task<OMlacodeModel?> _03(int id, OMlacodeModel mlacode, string schema, string conn);
    Task<OMlacodeModel?> _04(int id, string schema, string conn);
}