using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OLicverstatusDataAccess : IOLicverstatusDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OLicverstatusDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OLicverstatusModel?> _01(OLicverstatusModel licverstatus, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Licverstatus (licverStatus) values (@licverStatus)";
            await _sql.ExecuteCmd<dynamic>(sql, licverstatus, conn);

            sql = $@"SELECT * FROM {schema}.Licverstatus WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OLicverstatusModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<OLicverstatusModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  licverStatus from {schema}.Licverstatus where Id = @Id";
            var data = await _sql.FetchData<OLicverstatusModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<List<OLicverstatusModel?>?> _02s(string schema, string conn)
        {
            string sql = $@"select  licverStatus from {schema}.Licverstatus ";
            var data = await _sql.FetchData<OLicverstatusModel?, dynamic>(sql, new { }, conn);
            return data;
        }


        public async Task<OLicverstatusModel?> _03(int id, OLicverstatusModel licverstatus, string schema, string conn)
        {
            string sql = $@"Update {schema}.Licverstatus set licverStatus = @licverStatus where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, licverstatus, conn);

            sql = $@" select  * from {schema}.Licverstatus x where x.Id = @Id ;";
            var data = await _sql.FetchData<OLicverstatusModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OLicverstatusModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Licverstatus where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Licverstatus x where x.Id = @Id ;";
            var data = await _sql.FetchData<OLicverstatusModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IOLicverstatusDataAccess
    {
        Task<OLicverstatusModel?> _01(OLicverstatusModel licverstatus, string schema, string conn);
        Task<OLicverstatusModel?> _02(int id, string schema, string conn);
        Task<List<OLicverstatusModel?>?> _02s(string schema, string conn);
        Task<OLicverstatusModel?> _03(int id, OLicverstatusModel licverstatus, string schema, string conn);
        Task<OLicverstatusModel?> _04(int id, string schema, string conn);
    }
}
