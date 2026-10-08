using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OPenaltyDataAccess : IOPenaltyDataAccess
    {
        private readonly I_90_001_MySqlDataAccess _sql;

        public OPenaltyDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OPenaltyModel?> _01(OPenaltyModel penalty, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Penalty (DEV_NO, FREQ, PENALTY_NO, DESC_, resetregref, isterminated, days) values (@DEV_NO, @FREQ, @PENALTY_NO, @DESC_, @resetregref, @isterminated, @days)";
            await _sql.ExecuteCmd<dynamic>(sql, penalty, conn);

            sql = $@"SELECT * FROM {schema}.Penalty WHERE TRIM(UPPER(PENALTY_NO)) =  TRIM(UPPER(@Penalty_No)) AND TRIM(UPPER(DESC_)) =  TRIM(UPPER(@Desc_))";

            var res = await _sql.FetchData<OPenaltyModel?, dynamic>(sql, new {  penalty.Penalty_No,  penalty.Desc_ }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OPenaltyModel?>?> _02( string schema, string conn)
        {
            string sql = $@"select  DEV_NO, FREQ, PENALTY_NO, DESC_, resetregref, isterminated, days from {schema}.Penalty order by Desc_";
            var data = await _sql.FetchData<OPenaltyModel?, dynamic>(sql, new { }, conn);
            return data;
        }

        public async Task<List<OPenaltyModel?>?> _02ByPenaltyNoByDescNo(string penaltyno, string desc, string schema, string conn)
        {
            string sql = $@" SELECT * FROM {schema}.Penalty WHERE TRIM(UPPER(PENALTY_NO)) = TRIM(UPPER(@Penalty_No)) AND TRIM(UPPER(DESC_)) = TRIM(UPPER(@Desc_))";

            var data = await _sql.FetchData<OPenaltyModel?, dynamic>(sql, new { Penalty_No = penaltyno, Desc_ = desc }, conn);
            return data;
        }

        public async Task<OPenaltyModel?> _03(int id, OPenaltyModel penalty, string schema, string conn)
        {
            string sql = $@"Update {schema}.Penalty set DEV_NO = @DEV_NO, FREQ = @FREQ, PENALTY_NO = @PENALTY_NO, DESC_ = @DESC_, resetregref = @resetregref, isterminated = @isterminated, days = @days where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, penalty, conn);

            sql = $@" select  * from {schema}.Penalty x where x.Id = @Id ;";
            var data = await _sql.FetchData<OPenaltyModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<OPenaltyModel?> _03FromPenaltyEntryModule(string penaltyNo, string desc, OPenaltyModel penalty, string schema, string conn)
        {
            var parameters = new
            {
                oldPenaltyNo = penaltyNo,
                oldDesc = desc,
                penalty.Penalty_No,
                penalty.Desc_,
                penalty.ResetRegRef,
                penalty.IsTerminated,
                penalty.Days,
            };

            string sql = $@"UPDATE {schema}.Penalty  Penalty set  PENALTY_NO = @PENALTY_NO, DESC_ = @DESC_, resetregref = @resetregref, isterminated = @isterminated, days = @days 
                        WHERE TRIM(UPPER(PENALTY_NO)) =  TRIM(UPPER(@oldPenaltyNo))  AND TRIM(UPPER(DESC_)) = TRIM(UPPER(@oldDesc)) ;";

            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);

            sql = $@"SELECT * FROM {schema}.Penalty x WHERE TRIM(UPPER(PENALTY_NO)) = TRIM(UPPER(@Penalty_No))  AND TRIM(UPPER(DESC_)) = TRIM(UPPER(@Desc_));";

            var data = await _sql.FetchData<OPenaltyModel?, dynamic>(sql, new {  penalty.Penalty_No,  penalty.Desc_ }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<OPenaltyModel?> _04(string penaltyNo, string desc, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Penalty x WHERE TRIM(UPPER(PENALTY_NO)) = TRIM(UPPER(@Penalty_No))  AND TRIM(UPPER(DESC_)) = TRIM(UPPER(@Desc));";
            await _sql.ExecuteCmd<dynamic>(sql, new { Penalty_No = penaltyNo, Desc = desc }, conn);

            sql = $@" select  * from {schema}.Penalty x WHERE TRIM(UPPER(PENALTY_NO)) = TRIM(UPPER(@Penalty_No))  AND TRIM(UPPER(DESC_)) = TRIM(UPPER(@Desc));";
            var data = await _sql.FetchData<OPenaltyModel?, dynamic>(sql, new { Penalty_No = penaltyNo, Desc = desc }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IOPenaltyDataAccess
    {
        Task<OPenaltyModel?> _01(OPenaltyModel penalty, string schema, string conn);
        Task<List<OPenaltyModel?>?> _02( string schema, string conn);
        Task<List<OPenaltyModel?>?> _02ByPenaltyNoByDescNo(string penaltyno, string desc, string schema, string conn);
        Task<OPenaltyModel?> _03(int id, OPenaltyModel penalty, string schema, string conn);
        Task<OPenaltyModel?> _03FromPenaltyEntryModule(string penaltyNo, string desc, OPenaltyModel penalty, string schema, string conn);
        Task<OPenaltyModel?> _04(string penaltyNo, string desc, string schema, string conn);
    }

}