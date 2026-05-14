using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OGenderDataAccess : IOGenderDataAccess
    {
        private readonly I_90_001_MySqlDataAccess _sql;

        public OGenderDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OGenderModel?> _01(OGenderModel sex, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Sex (CODE, NAME) values (@CODE, @NAME)";
            await _sql.ExecuteCmd<dynamic>(sql, sex, conn);

            sql = $@"SELECT * FROM {schema}.Sex WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OGenderModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OGenderModel?>?> _02( string schema, string conn)
        {
            string sql = $@"select  CODE, NAME from {schema}.Sex ";
            var data = await _sql.FetchData<OGenderModel?, dynamic>(sql, new { }, conn);
            return data;
        }


        public async Task<OGenderModel?> _03(int id, OGenderModel sex, string schema, string conn)
        {
            string sql = $@"Update {schema}.Sex set CODE = @CODE, NAME = @NAME where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, sex, conn);

            sql = $@" select  * from {schema}.Sex x where x.Id = @Id ;";
            var data = await _sql.FetchData<OGenderModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OGenderModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Sex where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Sex x where x.Id = @Id ;";
            var data = await _sql.FetchData<OGenderModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }
}
public interface IOGenderDataAccess
{
    Task<OGenderModel?> _01(OGenderModel sex, string schema, string conn);
    Task<List<OGenderModel?>?> _02( string schema, string conn);
    Task<OGenderModel?> _03(int id, OGenderModel sex, string schema, string conn);
    Task<OGenderModel?> _04(int id, string schema, string conn);
}
