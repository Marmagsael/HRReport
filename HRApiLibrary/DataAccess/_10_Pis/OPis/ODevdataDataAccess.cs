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

            sql = $@"SELECT * FROM {schema}.Devdata WHERE Dev_No = @Dev_No";

            var res = await _sql.FetchData<ODevdataModel?, dynamic>(sql, new { devdata.Dev_No }, conn);

            return res.FirstOrDefault();
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

        public async Task<List<ODevdataModel?>?> _02(string code, string name, string schema, string conn)
        {
            string sql = $@"SELECT * FROM {schema}.Devdata  WHERE TRIM(UPPER(Dev_No)) = @Dev_No  OR TRIM(UPPER(Dev_Name)) = @Dev_Name ";

            var data = await _sql.FetchData<ODevdataModel?, dynamic>(sql, new { Dev_No = code, Dev_Name = name }, conn);
            return data;
        }

        public async Task<ODevdataModel?> _03(string dev_no, ODevdataModel dev, string schema, string conn)
        {
            var parameters = new
            {
                oldDev_no = dev_no,
                dev.Dev_No,
                dev.Dev_Name,
            };

            string sql = $@"UPDATE {schema}.Devdata SET Dev_No = @Dev_No, Dev_Name = @Dev_Name WHERE TRIM(UPPER(Dev_No)) = @oldDev_no;";

            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);

            sql = $@"SELECT * FROM {schema}.Devdata x WHERE TRIM(UPPER(x.Dev_No)) = @Dev_No;";

            var data = await _sql.FetchData<ODevdataModel?, dynamic>( sql,new { Dev_No = dev.Dev_No?.Trim().ToUpper() }, conn);

            return data?.FirstOrDefault();
        }

        public async Task<ODevdataModel?> _04(string dev_no, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Devdata WHERE TRIM(UPPER(x.Dev_No)) = @dev_no;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Dev_No = dev_no }, conn);

            sql = $@" select  * from {schema}.Devdata x WHERE TRIM(UPPER(x.Dev_No)) = @dev_no ;";
            var data = await _sql.FetchData<ODevdataModel?, dynamic>(sql, new { Dev_No = dev_no?.Trim().ToUpper() }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IODevdataDataAccess
    {
        Task<ODevdataModel?> _01(ODevdataModel devdata, string schema, string conn);
        Task<ODevdataModel?> _02(int id, string schema, string conn);
        Task<List<ODevdataModel?>?> _02(string schema, string conn);
        Task<List<ODevdataModel?>?> _02(string code, string name, string schema, string conn);
        Task<ODevdataModel?> _03(string dev_no, ODevdataModel dev, string schema, string conn);
        Task<ODevdataModel?> _04(string dev_no, string schema, string conn);
    }
}
