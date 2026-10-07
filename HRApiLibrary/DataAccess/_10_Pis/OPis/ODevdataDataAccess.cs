using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class ODevdataDataAccess : IODevdataDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public ODevdataDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<ODevdataModel?> _01(ODevdataModel devdata, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Devdata (DEV_NO, DEV_NAME, DEV_TYPE, DEV_LEVEL, DEV_PARENT) values (@DEV_NO, @DEV_NAME, @DEV_TYPE, @DEV_LEVEL, @DEV_PARENT)";
            await _sql.ExecuteCmd<dynamic>(sql, devdata, conn);

            sql = $@"SELECT * FROM {schema}.Devdata WHERE TRIM(UPPER(DEV_NO)) = TRIM(UPPER(@Dev_No))";
            var res = await _sql.FetchData<ODevdataModel?, dynamic>(sql, new { devdata.Dev_No }, conn);
            return res.FirstOrDefault();
        }

        public async Task<int?> _02ByMaxDevNo(string? schema, string? conn)
        {

            var sql = $@"select  LPAD(MAX(CAST(DEV_NO AS UNSIGNED)) + 1, 5, 0) from {schema}.Devdata ";
            var clnumber = await _sql.FetchData<int?, dynamic>(sql, new { }, conn);
            return clnumber.FirstOrDefault();
        }

        public async Task<ODevdataModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  DEV_NO, DEV_NAME, DEV_TYPE, DEV_LEVEL, DEV_PARENT from {schema}.Devdata where Id = @Id";
            var data = await _sql.FetchData<ODevdataModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<List<ODevdataModel?>?> _02(string schema, string conn)
        {
            string sql = $@"select  * from {schema}.Devdata";
            var data = await _sql.FetchData<ODevdataModel?, dynamic>(sql, new { }, conn);
            return data;
        }

        public async Task<List<ODevdataModel?>?> _02ByDevName(string devname, string schema, string conn)
        {
            string sql = $@"SELECT * FROM {schema}.Devdata  WHERE TRIM(UPPER(DEV_NAME)) = TRIM(UPPER(@DEV_NAME))  ";

            var data = await _sql.FetchData<ODevdataModel?, dynamic>(sql, new { DEV_NAME = devname }, conn);
            return data;
        }

        public async Task<ODevdataModel?> _03FromDeviationEntryModule( ODevdataModel dev, string schema, string conn)
        {

            string sql = $@"UPDATE {schema}.Devdata SET Dev_No = @Dev_No, Dev_Name = @Dev_Name WHERE TRIM(UPPER(Dev_No)) =  TRIM(UPPER(@Dev_No));";

            await _sql.ExecuteCmd<dynamic>(sql, dev, conn);

            sql = $@"SELECT * FROM {schema}.Devdata x WHERE TRIM(UPPER(x.Dev_No)) = TRIM(UPPER(@Dev_No)) ;";

            var data = await _sql.FetchData<ODevdataModel?, dynamic>( sql,new {  dev.Dev_No}, conn);

            return data?.FirstOrDefault();
        }

        public async Task<ODevdataModel?> _04(string dev_no, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Devdata WHERE TRIM(UPPER(Dev_No)) = TRIM(UPPER(@Dev_No)) ;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Dev_No = dev_no }, conn);

            sql = $@" select  * from {schema}.Devdata x WHERE TRIM(UPPER(Dev_No)) = TRIM(UPPER(@Dev_No)) ;";
            var data = await _sql.FetchData<ODevdataModel?, dynamic>(sql, new { Dev_No = dev_no}, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IODevdataDataAccess
    {
        Task<ODevdataModel?> _01(ODevdataModel devdata, string schema, string conn);
        Task<int?> _02ByMaxDevNo(string? schema, string? conn);
        Task<ODevdataModel?> _02(int id, string schema, string conn);
        Task<List<ODevdataModel?>?> _02(string schema, string conn);
        Task<List<ODevdataModel?>?> _02ByDevName(string devname, string schema, string conn);
        Task<ODevdataModel?> _03FromDeviationEntryModule( ODevdataModel dev, string schema, string conn);
        Task<ODevdataModel?> _04(string dev_no, string schema, string conn);
    }
}
