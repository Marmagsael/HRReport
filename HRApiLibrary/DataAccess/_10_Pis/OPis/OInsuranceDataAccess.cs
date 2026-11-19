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
            await _sql.ExecuteLegacyCmd<dynamic>(sql, insurance, conn);

            sql = $@"SELECT * FROM {schema}.Insurance WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchLegacy<OInsuranceModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<OInsuranceModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  Id, Name, PolicyNo, InsuranceType, FaceValue, Premiums, InsExpire from {schema}.Insurance where Id = @Id";
            var data = await _sql.FetchLegacy<OInsuranceModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<List<OInsuranceModel?>?> _02(string schema, string conn)
        {
            string sql = $@"select  Id, Name, PolicyNo, InsuranceType, FaceValue, Premiums, InsExpire from {schema}.Insurance ORDER BY Name";
            var data = await _sql.FetchLegacy<OInsuranceModel?, dynamic>(sql, new { }, conn);
            return data;
        }

    public async Task<List<OInsuranceModel?>?> _02ByAllFields( OInsuranceModel insurance, string schema, string conn)
    {
        string sql = $@" SELECT *
                            FROM {schema}.Insurance
                            WHERE TRIM(UPPER(Name)) = TRIM(UPPER(@Name))
                              AND TRIM(UPPER(PolicyNo)) = TRIM(UPPER(@PolicyNo))
                              AND TRIM(UPPER(InsuranceType)) = TRIM(UPPER(@InsuranceType))
                              AND FaceValue = @FaceValue
                              AND Premiums = @Premiums
                              AND DATE(InsExpire) = DATE(@InsExpire)
                            ORDER BY Name";

        var data = await _sql.FetchLegacy<OInsuranceModel?, dynamic>(
            sql,
            new
            {
                insurance.Name,
                insurance.PolicyNo,
                insurance.InsuranceType,
                insurance.FaceValue,
                insurance.Premiums,
                insurance.InsExpire
            },
            conn);

        return data;
    }


    // Insurance Assignment report: every employee assigned to an insurance, for both types.
    // Life is stored on Empmas, Accident in Insurance_accident; both are matched to the insurance
    // by ALL its fields (same rule as the assignment screen).
    public async Task<List<OInsuranceAssignmentModel?>?> _02AssignmentReport(string? schema, string? conn)
    {
        string sql = $@"SELECT i.InsuranceType, i.Name AS InsuranceName, i.PolicyNo, i.FaceValue, i.Premiums, i.InsExpire,
                               e.EmpNumber,
                               CONCAT_WS(', ', NULLIF(TRIM(e.EmpLastNm), ''),
                                               CONCAT_WS(' ', NULLIF(TRIM(e.EmpFirstNm), ''), NULLIF(TRIM(e.EmpMidNm), ''))) AS EmpName,
                               c.ClName,
                               s.Name AS EmpStatus
                        FROM {schema}.Insurance i
                        INNER JOIN {schema}.Empmas e
                                ON TRIM(UPPER(e.Insurance)) = TRIM(UPPER(i.Name))
                               AND TRIM(UPPER(e.PolicyNo))  = TRIM(UPPER(i.PolicyNo))
                               AND ROUND(e.FaceValue, 2)    = ROUND(i.FaceValue, 2)
                               AND ROUND(e.Premium, 2)      = ROUND(i.Premiums, 2)
                               AND DATE(e.InsExpire)        = DATE(i.InsExpire)
                        LEFT JOIN {schema}.EmpStat s ON s.Code     = e.EmpStat_
                        LEFT JOIN {schema}.Client  c ON c.ClNumber = e.Client_
                        WHERE i.InsuranceType = 'Life'

                        UNION ALL

                        SELECT i.InsuranceType, i.Name AS InsuranceName, i.PolicyNo, i.FaceValue, i.Premiums, i.InsExpire,
                               e.EmpNumber,
                               CONCAT_WS(', ', NULLIF(TRIM(e.EmpLastNm), ''),
                                               CONCAT_WS(' ', NULLIF(TRIM(e.EmpFirstNm), ''), NULLIF(TRIM(e.EmpMidNm), ''))) AS EmpName,
                               c.ClName,
                               s.Name AS EmpStatus
                        FROM {schema}.Insurance i
                        INNER JOIN {schema}.Insurance_accident a
                                ON TRIM(UPPER(a.Insurance)) = TRIM(UPPER(i.Name))
                               AND TRIM(UPPER(a.PolicyNo))  = TRIM(UPPER(i.PolicyNo))
                               AND ROUND(a.FaceValue, 2)    = ROUND(i.FaceValue, 2)
                               AND ROUND(a.Premium, 2)      = ROUND(i.Premiums, 2)
                               AND DATE(a.InsExpire)        = DATE(i.InsExpire)
                        INNER JOIN {schema}.Empmas  e ON e.EmpNumber = a.EmpNumber
                        LEFT JOIN  {schema}.EmpStat s ON s.Code      = e.EmpStat_
                        LEFT JOIN  {schema}.Client  c ON c.ClNumber  = e.Client_
                        WHERE i.InsuranceType = 'Accident'

                        ORDER BY InsuranceType, InsuranceName, InsExpire, EmpName;";

        var data = await _sql.FetchLegacy<OInsuranceAssignmentModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<OInsuranceModel?> _03(int? id, OInsuranceModel insurance, string schema, string conn)
        {
            string sql = $@"Update {schema}.Insurance set Name = @Name, PolicyNo = @PolicyNo, InsuranceType = @InsuranceType, FaceValue = @FaceValue, Premiums = @Premiums, InsExpire = @InsExpire where Id = @Id;";
            await _sql.ExecuteLegacyCmd<dynamic>(sql, insurance, conn);

            sql = $@" select  * from {schema}.Insurance x where x.Id = @Id ;";
            var data = await _sql.FetchLegacy<OInsuranceModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OInsuranceModel?> _04(int? id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Insurance where Id = @Id;";
            await _sql.ExecuteLegacyCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Insurance x where x.Id = @Id ;";
            var data = await _sql.FetchLegacy<OInsuranceModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }



public interface IOInsuranceDataAccess
{
    Task<OInsuranceModel?> _01(OInsuranceModel insurance, string schema, string conn);
    Task<OInsuranceModel?> _02(int id, string schema, string conn);
    Task<List<OInsuranceModel?>?> _02(string schema, string conn);
    Task<List<OInsuranceModel?>?> _02ByAllFields(OInsuranceModel insurance, string schema, string conn);
    Task<List<OInsuranceAssignmentModel?>?> _02AssignmentReport(string? schema, string? conn);
    Task<OInsuranceModel?> _03(int? id, OInsuranceModel insurance, string schema, string conn);
    Task<OInsuranceModel?> _04(int? id, string schema, string conn);
}