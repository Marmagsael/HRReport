
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

        public async Task<OCivstatModel?> _01(OCivstatModel civstat, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Civstat (CODE, NAME) values (@CODE, @NAME)";
            await _sql.ExecuteCmd<dynamic>(sql, civstat, conn);

            sql = $@"SELECT * FROM {schema}.Civstat WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OCivstatModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OCivstatModel?>?> _02(string schema, string conn)
        {
            string sql = $@"select  CODE, NAME from {schema}.Civstat ";
            var data = await _sql.FetchData<OCivstatModel?, dynamic>(sql, new { }, conn);
            return data;
        }


        public async Task<OCivstatModel?> _03(int id, OCivstatModel civstat, string schema, string conn)
        {
            string sql = $@"Update {schema}.Civstat set CODE = @CODE, NAME = @NAME where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, civstat, conn);

            sql = $@" select  * from {schema}.Civstat x where x.Id = @Id ;";
            var data = await _sql.FetchData<OCivstatModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OCivstatModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Civstat where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Civstat x where x.Id = @Id ;";
            var data = await _sql.FetchData<OCivstatModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOCivstatDataAccess
{
    Task<OCivstatModel?> _01(OCivstatModel civstat, string schema, string conn);
    Task<List<OCivstatModel?>?> _02(string schema, string conn);
    Task<OCivstatModel?> _03(int id, OCivstatModel civstat, string schema, string conn);
    Task<OCivstatModel?> _04(int id, string schema, string conn);
}