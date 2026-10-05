using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class ORegionsDataAccess : IORegionsDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public ORegionsDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<ORegionsModel?> _01(ORegionsModel regions, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Regions (code, name, odr) values (@code, @name, @odr)";
            await _sql.ExecuteCmd<dynamic>(sql, regions, conn);

            sql = $@"SELECT * FROM {schema}.Regions WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<ORegionsModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<ORegionsModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  code, name, odr from {schema}.Regions where Id = @Id";
            var data = await _sql.FetchData<ORegionsModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<List<ORegionsModel?>?> _02(string schema, string conn)
        {
            string sql = $@"select  code, name, odr from {schema}.Regions  ORDER BY Name";
            var data = await _sql.FetchData<ORegionsModel?, dynamic>(sql, new { }, conn);
            return data;
        }



        public async Task<ORegionsModel?> _03(int id, ORegionsModel regions, string schema, string conn)
        {
            string sql = $@"Update {schema}.Regions set code = @code, name = @name, odr = @odr where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, regions, conn);

            sql = $@" select  * from {schema}.Regions x where x.Id = @Id ;";
            var data = await _sql.FetchData<ORegionsModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<ORegionsModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Regions where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Regions x where x.Id = @Id ;";
            var data = await _sql.FetchData<ORegionsModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IORegionsDataAccess
    {
        Task<ORegionsModel?> _01(ORegionsModel regions, string schema, string conn);
        Task<ORegionsModel?> _02(int id, string schema, string conn);
        Task<List<ORegionsModel?>?> _02(string schema, string conn);
        Task<ORegionsModel?> _03(int id, ORegionsModel regions, string schema, string conn);
        Task<ORegionsModel?> _04(int id, string schema, string conn);
    }
}
