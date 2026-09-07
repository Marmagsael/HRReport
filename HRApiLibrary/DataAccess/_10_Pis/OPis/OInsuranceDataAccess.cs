using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

    public class OInsuranceDataAccess : IOInsuranceDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OInsuranceDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OInsuranceModel?> _01(OInsuranceModel insurance, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Insurance (Name, PolicyNo, InsuranceType, FaceValue, Premiums, InsExpire) values (@Name, @PolicyNo, @InsuranceType, @FaceValue, @Premiums, @InsExpire)";
            await _sql.ExecuteCmd<dynamic>(sql, insurance, conn);

            sql = $@"SELECT * FROM {schema}.Insurance WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OInsuranceModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<OInsuranceModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  Id, Name, PolicyNo, InsuranceType, FaceValue, Premiums, InsExpire from {schema}.Insurance where Id = @Id";
            var data = await _sql.FetchData<OInsuranceModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<List<OInsuranceModel?>?> _02(string schema, string conn)
        {
            string sql = $@"select  Id, Name, PolicyNo, InsuranceType, FaceValue, Premiums, InsExpire from {schema}.Insurance where ORDER BY Name";
            var data = await _sql.FetchData<OInsuranceModel?, dynamic>(sql, new { }, conn);
            return data;
        }


        public async Task<OInsuranceModel?> _03(int id, OInsuranceModel insurance, string schema, string conn)
        {
            string sql = $@"Update {schema}.Insurance set Name = @Name, PolicyNo = @PolicyNo, InsuranceType = @InsuranceType, FaceValue = @FaceValue, Premiums = @Premiums, InsExpire = @InsExpire where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, insurance, conn);

            sql = $@" select  * from {schema}.Insurance x where x.Id = @Id ;";
            var data = await _sql.FetchData<OInsuranceModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OInsuranceModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Insurance where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Insurance x where x.Id = @Id ;";
            var data = await _sql.FetchData<OInsuranceModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }



public interface IOInsuranceDataAccess
{
    Task<OInsuranceModel?> _01(OInsuranceModel insurance, string schema, string conn);
    Task<OInsuranceModel?> _02(int id, string schema, string conn);
    Task<List<OInsuranceModel?>?> _02(string schema, string conn);
    Task<OInsuranceModel?> _03(int id, OInsuranceModel insurance, string schema, string conn);
    Task<OInsuranceModel?> _04(int id, string schema, string conn);
}