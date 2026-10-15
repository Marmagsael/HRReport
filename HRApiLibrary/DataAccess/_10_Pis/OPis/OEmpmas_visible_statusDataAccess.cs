using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OEmpmas_visible_statusDataAccess : IOEmpmas_visible_statusDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OEmpmas_visible_statusDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OEmpmas_visible_statusModel?> _01(OEmpmas_visible_statusModel empmas_visible_status, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Empmas_visible_status (empstat_) values (@empstat_)";
            await _sql.ExecuteCmd<dynamic>(sql, empmas_visible_status, conn);

            sql = $@"SELECT * FROM {schema}.Empmas_visible_status WHERE Empstat_ = @Empstat_";

            var res = await _sql.FetchData<OEmpmas_visible_statusModel?, dynamic>(sql, empmas_visible_status, conn);

            return res.FirstOrDefault();
        }


        public async Task<OEmpmas_visible_statusModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  empstat_ from {schema}.Empmas_visible_status where Id = @Id";
            var data = await _sql.FetchData<OEmpmas_visible_statusModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<List<OEmpmas_visible_statusModel?>?> _02s(string schema, string conn)
        {
            string sql = $@"select  empstat_ from {schema}.Empmas_visible_status ";
            var data = await _sql.FetchData<OEmpmas_visible_statusModel?, dynamic>(sql, new { }, conn);
            return data;
        }

        public async Task<OEmpmas_visible_statusModel?> _03(int id, OEmpmas_visible_statusModel empmas_visible_status, string schema, string conn)
        {
            string sql = $@"Update {schema}.Empmas_visible_status set empstat_ = @empstat_ where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, empmas_visible_status, conn);

            sql = $@" select  * from {schema}.Empmas_visible_status x where x.Id = @Id ;";
            var data = await _sql.FetchData<OEmpmas_visible_statusModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OEmpmas_visible_statusModel?> _04(OEmpmas_visible_statusModel empstat, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Empmas_visible_status where Empstat_ = @Empstat_;";
            await _sql.ExecuteCmd<dynamic>(sql, empstat, conn);

            sql = $@" select  * from {schema}.Empmas_visible_status x where Empstat_ = @Empstat_ ;";
            var data = await _sql.FetchData<OEmpmas_visible_statusModel?, dynamic>(sql, empstat, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IOEmpmas_visible_statusDataAccess
    {
        Task<OEmpmas_visible_statusModel?> _01(OEmpmas_visible_statusModel empmas_visible_status, string schema, string conn);
        Task<OEmpmas_visible_statusModel?> _02(int id, string schema, string conn);
        Task<List<OEmpmas_visible_statusModel?>?> _02s(string schema, string conn);
        Task<OEmpmas_visible_statusModel?> _03(int id, OEmpmas_visible_statusModel empmas_visible_status, string schema, string conn);
        Task<OEmpmas_visible_statusModel?> _04(OEmpmas_visible_statusModel empstat,  string schema, string conn);
    }
}
