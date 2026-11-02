using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OSecliclogsDataAccess : IOSecliclogsDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OSecliclogsDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OSecliclogsModel?> _01(OSecliclogsModel secliclogs, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Secliclogs (empnumber, sbrno, opno, validated, vfee, revalidated, valStatus, user, dateModified, licDateIssued) values (@empnumber, @sbrno, @opno, @validated, @vfee, @revalidated, @valStatus, @user, @dateModified, @licDateIssued)";
            await _sql.ExecuteCmd<dynamic>(sql, secliclogs, conn);

            sql = $@"SELECT * FROM {schema}.Secliclogs WHERE EmpNumber = @EmpNumber ORDER BY datemodified  DESC Limit 1 ";

            var res = await _sql.FetchData<OSecliclogsModel?, dynamic>(sql, secliclogs, conn);

            return res.FirstOrDefault();
        }


        public async Task<OSecliclogsModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  empnumber, sbrno, opno, validated, vfee, revalidated, valStatus, user, dateModified, licDateIssued from {schema}.Secliclogs where Id = @Id";
            var data = await _sql.FetchData<OSecliclogsModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<List<OSecliclogsModel?>?> _02s(string empnumber, string schema, string conn)
        {
            string sql = $@" SELECT s.Empnumber, s.Sbrno, s.Opno, s.Vfee, s.ValStatus, s.User,
                                NULLIF(s.Validated, 0000-00-00) AS Validated,
                                NULLIF(s.Revalidated, 0000-00-00) AS Revalidated,
                                NULLIF(s.DateModified, 0000-00-00) AS DateModified,
                                NULLIF(s.LicDateIssued, 0000-00-00) AS LicDateIssued,
                                CONCAT_WS(' ',
                                    CONCAT(TRIM(e.emplastnm), ','),
                                    TRIM(e.empfirstnm),
                                    TRIM(e.empmidnm)
                                ) AS UserName
                            FROM {schema}.Secliclogs s
                            LEFT JOIN {schema}.empmas e
                                ON e.EMPNUMBER = s.User
                            WHERE s.Empnumber = @Empnumber ORDER BY s.dateModified DESC"  ;
            var data = await _sql.FetchData<OSecliclogsModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data;
        }

        public async Task<OSecliclogsModel?> _03(int id, OSecliclogsModel secliclogs, string schema, string conn)
        {
            string sql = $@"Update {schema}.Secliclogs set empnumber = @empnumber, sbrno = @sbrno, opno = @opno, validated = @validated, vfee = @vfee, revalidated = @revalidated, valStatus = @valStatus, user = @user, dateModified = @dateModified, licDateIssued = @licDateIssued where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, secliclogs, conn);

            sql = $@" select  * from {schema}.Secliclogs x where x.Id = @Id ;";
            var data = await _sql.FetchData<OSecliclogsModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OSecliclogsModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Secliclogs where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Secliclogs x where x.Id = @Id ;";
            var data = await _sql.FetchData<OSecliclogsModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IOSecliclogsDataAccess
    {
        Task<OSecliclogsModel?> _01(OSecliclogsModel secliclogs, string schema, string conn);
        Task<OSecliclogsModel?> _02(int id, string schema, string conn);
        Task<List<OSecliclogsModel?>?> _02s(string empnumber, string schema, string conn);
        Task<OSecliclogsModel?> _03(int id, OSecliclogsModel secliclogs, string schema, string conn);
        Task<OSecliclogsModel?> _04(int id, string schema, string conn);
    }
}
