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

            sql = $@"SELECT * FROM {schema}.Procode WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OProcodeModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OProcodeModel?>?> _02( string? schema, string? conn)
        {
            string? sql = $@"select  CODE, NAME from {schema}.Procode ";
            var data = await _sql.FetchData<OProcodeModel?, dynamic>(sql, new {  }, conn);
            return data;
        }


        public async Task<OProcodeModel?> _03(int? id, OProcodeModel procode, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Procode set CODE = @CODE, NAME = @NAME where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, procode, conn);

            sql = $@" select  * from {schema}.Procode x where x.Id = @Id ;";
            var data = await _sql.FetchData<OProcodeModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OProcodeModel?> _04(int? id, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Procode where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Procode x where x.Id = @Id ;";
            var data = await _sql.FetchData<OProcodeModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }
}

    public interface IOProcodeDataAccess
    {
        Task<OProcodeModel?> _01(OProcodeModel procode, string? schema, string? conn);
        Task<List<OProcodeModel?>?> _02( string? schema, string? conn);
        Task<OProcodeModel?> _03(int? id, OProcodeModel procode, string? schema, string? conn);
        Task<OProcodeModel?> _04(int? id, string? schema, string? conn);
    }