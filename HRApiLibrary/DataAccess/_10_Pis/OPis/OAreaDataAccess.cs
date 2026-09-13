using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OAreaDataAccess : IOAreaDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OAreaDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OAreaModel?> _01(OAreaModel area, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Area (AREACODE, AREANAME) values (@AREACODE, @AREANAME)";
            await _sql.ExecuteCmd<dynamic>(sql, area, conn);

            sql = $@"SELECT * FROM {schema}.Area WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OAreaModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OAreaModel?>?> _02( string schema, string conn)
        {
            string sql = $@"select  AREACODE, AREANAME from {schema}.Area";
            var data = await _sql.FetchData<OAreaModel?, dynamic>(sql, new {  }, conn);
            return data;
        }


        public async Task<OAreaModel?> _03(int id, OAreaModel area, string schema, string conn)
        {
            string sql = $@"Update {schema}.Area set AREACODE = @AREACODE, AREANAME = @AREANAME where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, area, conn);

            sql = $@" select  * from {schema}.Area x where x.Id = @Id ;";
            var data = await _sql.FetchData<OAreaModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OAreaModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Area where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Area x where x.Id = @Id ;";
            var data = await _sql.FetchData<OAreaModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IOAreaDataAccess
    {
        Task<OAreaModel?> _01(OAreaModel area, string schema, string conn);
        Task<List<OAreaModel?>?> _02(string schema, string conn);
        Task<OAreaModel?> _03(int id, OAreaModel area, string schema, string conn);
        Task<OAreaModel?> _04(int id, string schema, string conn);
    }
}
