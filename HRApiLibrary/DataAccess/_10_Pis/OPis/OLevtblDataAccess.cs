using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OLevtblDataAccess : IOLevtblDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OLevtblDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OLevtblModel?> _01(OLevtblModel levtbl, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Levtbl (LVCODE, LVNAME, EARNCODE, NO_INCDATE) values (@LVCODE, @LVNAME, @EARNCODE, @NO_INCDATE)";
            await _sql.ExecuteCmd<dynamic>(sql, levtbl, conn);

            sql = $@"SELECT * FROM {schema}.Levtbl WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OLevtblModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<OLevtblModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  LVCODE, LVNAME, EARNCODE, NO_INCDATE from {schema}.Levtbl where Id = @Id";
            var data = await _sql.FetchData<OLevtblModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<List<OLevtblModel?>?> _02(string schema, string conn)
        {
            string sql = $@"select  LVCODE, LVNAME, EARNCODE, NO_INCDATE from {schema}.Levtbl";
            var data = await _sql.FetchData<OLevtblModel?, dynamic>(sql, new { }, conn);
            return data;
        }



        public async Task<OLevtblModel?> _03(int id, OLevtblModel levtbl, string schema, string conn)
        {
            string sql = $@"Update {schema}.Levtbl set LVCODE = @LVCODE, LVNAME = @LVNAME, EARNCODE = @EARNCODE, NO_INCDATE = @NO_INCDATE where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, levtbl, conn);

            sql = $@" select  * from {schema}.Levtbl x where x.Id = @Id ;";
            var data = await _sql.FetchData<OLevtblModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OLevtblModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Levtbl where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Levtbl x where x.Id = @Id ;";
            var data = await _sql.FetchData<OLevtblModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }


    public interface IOLevtblDataAccess
    {
        Task<OLevtblModel?> _01(OLevtblModel levtbl, string schema, string conn);
        Task<OLevtblModel?> _02(int id, string schema, string conn);
        Task<List<OLevtblModel?>?> _02(string schema, string conn);
        Task<OLevtblModel?> _03(int id, OLevtblModel levtbl, string schema, string conn);
        Task<OLevtblModel?> _04(int id, string schema, string conn);
    }
}
