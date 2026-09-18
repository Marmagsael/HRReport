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
            string sql = $@"Insert into {schema}.Levtbl (LVCODE, LVNAME, EARNCODE, NO_INCDATE) values (@LvCode, @LvName, @EarnCode, @No_Indicate)";
            await _sql.ExecuteCmd<dynamic>(sql, levtbl, conn);

            sql = $@"SELECT * FROM {schema}.Levtbl WHERE LVCODE = @LvCode";

            var res = await _sql.FetchData<OLevtblModel?, dynamic>(sql, new { levtbl.LvCode }, conn);

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
            string sql = $@"select  LVCODE, LVNAME, EARNCODE, NO_INCDATE from {schema}.Levtbl ORDER BY LVNAME";
            var data = await _sql.FetchData<OLevtblModel?, dynamic>(sql, new { }, conn);
            return data;
        }


        public async Task<List<OLevtblModel?>?> _02(string code, string name, string schema, string conn)
        {
            string sql = $@"SELECT * FROM {schema}.Levtbl  WHERE TRIM(UPPER(LVCODE)) = @Code  OR TRIM(UPPER(LVNAME)) = @Name";

            var data = await _sql.FetchData<OLevtblModel?, dynamic>(sql, new { Code = code, Name = name }, conn);
            return data;
        }


        public async Task<OLevtblModel?> _03(string code, OLevtblModel levtbl, string schema, string conn)
        {
            var parameters = new
            {
                oldCode = code,
                levtbl.LvCode,
                levtbl.LvName,
            };

            string sql = $@"UPDATE {schema}.Levtbl  SET LVCODE = @Code, LVNAME = @Name  WHERE TRIM(UPPER(LVCODE)) = @oldCode;";

            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);

            sql = $@"SELECT * FROM {schema}.Levtbl x WHERE TRIM(UPPER(x.LVCODE)) = @Code;";

            var data = await _sql.FetchData<OLevtblModel?, dynamic>( sql, new { Code = levtbl.LvCode?.Trim().ToUpper() },conn);
            return data?.FirstOrDefault();
        }

        public async Task<OLevtblModel?> _04(string code, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Levtbl  WHERE TRIM(UPPER(x.LVCODE)) = @Code;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Code = code }, conn);

            sql = $@" select  * from {schema}.Levtbl x  WHERE TRIM(UPPER(x.LVCODE)) = @Code ;";
            var data = await _sql.FetchData<OLevtblModel?, dynamic>(sql, new { Code = code?.Trim().ToUpper() }, conn);
            return data?.FirstOrDefault();
        }
    }


    public interface IOLevtblDataAccess
    {
        Task<OLevtblModel?> _01(OLevtblModel levtbl, string schema, string conn);
        Task<OLevtblModel?> _02(int id, string schema, string conn);
        Task<List<OLevtblModel?>?> _02(string schema, string conn);
        Task<List<OLevtblModel?>?> _02(string code, string name, string schema, string conn);
        Task<OLevtblModel?> _03(string code, OLevtblModel levtbl, string schema, string conn);
        Task<OLevtblModel?> _04(string code, string schema, string conn);
    }
}
