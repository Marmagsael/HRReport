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

            sql = $@"SELECT * FROM {schema}.Area  WHERE TRIM(UPPER(AreaCode)) = @AreaCode";

            var res = await _sql.FetchData<OAreaModel?, dynamic>(sql, new { AreaCode = area.AreaCode?.Trim().ToUpper() }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OAreaModel?>?> _02( string schema, string conn)
        {
            string sql = $@"select  AREACODE, AREANAME from {schema}.Area Order By AREANAME";
            var data = await _sql.FetchData<OAreaModel?, dynamic>(sql, new {  }, conn);
            return data;
        }

        public async Task<List<OAreaModel?>?> _02(string code, string name, string schema, string conn)
        {
            string sql = $@"SELECT * FROM {schema}.Area  WHERE TRIM(UPPER(AREACODE)) = @Code  OR TRIM(UPPER(AREANAME)) = @Name";

            var data = await _sql.FetchData<OAreaModel?, dynamic>(sql, new { Code = code, Name = name }, conn);
            return data;
        }

        public async Task<OAreaModel?> _03(string code, OAreaModel ac, string schema, string conn)
        {
            var parameters = new
            {
                oldCode = code,
                ac.AreaCode,
                ac.AreaName,
            };

            string sql = $@"UPDATE {schema}.Area  SET AreaCode = @AreaCode, AreaName = @AreaName WHERE TRIM(UPPER(AreaCode)) = @oldCode;";

            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);

            sql = $@"SELECT * FROM {schema}.Area x WHERE TRIM(UPPER(x.AreaCode)) = @AreaCode;";

            var data = await _sql.FetchData<OAreaModel?, dynamic>(sql, new { AreaCode = ac.AreaCode?.Trim().ToUpper() }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<OAreaModel?> _04(string code, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Area x WHERE TRIM(UPPER(x.AreaCode)) = @AreaCode;";
            await _sql.ExecuteCmd<dynamic>(sql, new { AreaCode = code?.Trim().ToUpper() }, conn);

            sql = $@" select  * from {schema}.Area x WHERE TRIM(UPPER(x.AreaCode)) = @AreaCode ;";
            var data = await _sql.FetchData<OAreaModel?, dynamic>(sql, new { AreaCode = code?.Trim().ToUpper() }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IOAreaDataAccess
    {
        Task<OAreaModel?> _01(OAreaModel area, string schema, string conn);
        Task<List<OAreaModel?>?> _02(string schema, string conn);
        Task<List<OAreaModel?>?> _02(string code, string name, string schema, string conn);
        Task<OAreaModel?> _03(string code, OAreaModel ac, string schema, string conn);
        Task<OAreaModel?> _04(string code, string schema, string conn);
    }
}
