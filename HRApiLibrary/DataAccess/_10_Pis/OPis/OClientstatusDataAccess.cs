using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class ClientstatusDataAccess : IClientstatusDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public ClientstatusDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OClientstatusModel?> _01(OClientstatusModel clientstatus, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Clientstatus (ClStatus) values (@ClStatus)";
            await _sql.ExecuteCmd<dynamic>(sql, clientstatus, conn);

            sql = $@"SELECT * FROM {schema}.Clientstatus WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OClientstatusModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OClientstatusModel?>?> _02(string schema, string conn)
        {
            string sql = $@"select  ClStatus from {schema}.Clientstatus";
            var data = await _sql.FetchData<OClientstatusModel?, dynamic>(sql, new { }, conn);
            return data;
        }


        public async Task<OClientstatusModel?> _03(int id, OClientstatusModel clientstatus, string schema, string conn)
        {
            string sql = $@"Update {schema}.Clientstatus set ClStatus = @ClStatus where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, clientstatus, conn);

            sql = $@" select  * from {schema}.Clientstatus x where x.Id = @Id ;";
            var data = await _sql.FetchData<OClientstatusModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OClientstatusModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Clientstatus where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Clientstatus x where x.Id = @Id ;";
            var data = await _sql.FetchData<OClientstatusModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IClientstatusDataAccess
    {
        Task<OClientstatusModel?> _01(OClientstatusModel clientstatus, string schema, string conn);
        Task<List<OClientstatusModel?>?> _02(string schema, string conn);
        Task<OClientstatusModel?> _03(int id, OClientstatusModel clientstatus, string schema, string conn);
        Task<OClientstatusModel?> _04(int id, string schema, string conn);
    }
}
