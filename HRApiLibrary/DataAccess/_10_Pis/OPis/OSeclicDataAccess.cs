using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OSeclicDataAccess : IOSeclicDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OSeclicDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OSeclicModel?> _01(OSeclicModel seclic, string schema, string conn)
        {
            string sql = $@" INSERT INTO {schema}.Seclic( empnumber, sbrno, opno, validated, vfee, revalidated, valStatus, user, dateModified, licDateIssued)
                            VALUES ( @Empnumber, @Sbrno, @Opno, @Validated, @Vfee, @Revalidated, @ValStatus, @User, @DateModified, @LicDateIssued  )
                            ON DUPLICATE KEY UPDATE
                                sbrno = @Sbrno,
                                opno = @Opno,
                                validated = @Validated,
                                vfee = @Vfee,
                                revalidated = @Revalidated,
                                valStatus = @ValStatus,
                                user = @User,
                                dateModified = @DateModified,
                                licDateIssued = @LicDateIssued";
            await _sql.ExecuteCmd<dynamic>(sql, seclic, conn);
            sql = $@"SELECT * FROM {schema}.Seclic WHERE Empnumber = @Empnumber";

            var res = await _sql.FetchData<OSeclicModel?, dynamic>(sql, seclic, conn);

            return res.FirstOrDefault();
        }


        public async Task<OSeclicModel?> _02(string empnumber, string schema, string conn)
        {
            string sql = $@"select  empnumber, sbrno, opno, validated, vfee, revalidated, valStatus, user, dateModified, licDateIssued from {schema}.Seclic where Empnumber = @Empnumber";
            var data = await _sql.FetchData<OSeclicModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<OSeclicModel?> _03(string empnumber, OSeclicModel seclic, string schema, string conn)
        {
            string sql = $@"Update {schema}.Seclic set empnumber = @empnumber, sbrno = @sbrno, opno = @opno, validated = @validated, vfee = @vfee, revalidated = @revalidated, valStatus = @valStatus, user = @user, dateModified = @dateModified, licDateIssued = @licDateIssued where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, seclic, conn);

            sql = $@" select  * from {schema}.Seclic x where x.Id = @Id ;";
            var data = await _sql.FetchData<OSeclicModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OSeclicModel?> _04(string empnumber, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Seclic where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber }, conn);

            sql = $@" select  * from {schema}.Seclic x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OSeclicModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IOSeclicDataAccess
    {
        Task<OSeclicModel?> _01(OSeclicModel seclic, string schema, string conn);
        Task<OSeclicModel?> _02(string empnumber, string schema, string conn);
        Task<OSeclicModel?> _03(string empnumber, OSeclicModel seclic, string schema, string conn);
        Task<OSeclicModel?> _04(string empnumber, string schema, string conn);
    }
}
