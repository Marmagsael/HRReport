
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OInsuranceaccidentDataAccess : IOInsuranceaccidentDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OInsuranceaccidentDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OInsuranceaccidentModel?> _01(OInsuranceaccidentModel insurance_accident, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Insurance_accident (EMPNUMBER, INSURANCE, POLICYNO, FACEVALUE, PREMIUM, INSEXPIRE) values (@EMPNUMBER, @INSURANCE, @POLICYNO, @FACEVALUE, @PREMIUM, @INSEXPIRE)";
            await _sql.ExecuteLegacyCmd<dynamic>(sql, insurance_accident, conn);

            sql = $@"SELECT * FROM {schema}.Insurance_accident WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchLegacy<OInsuranceaccidentModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<OInsuranceaccidentModel?> _02(string empnumber, string schema, string conn)
        {
            string sql = $@"select  EMPNUMBER, INSURANCE, POLICYNO, FACEVALUE, PREMIUM, INSEXPIRE from {schema}.Insurance_accident where EMPNUMBER = @EMPNUMBER";
            var data = await _sql.FetchLegacy<OInsuranceaccidentModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<List<OInsuranceaccidentModel?>?> _02List(string empnumber, string schema, string conn)
        {
            string sql = $@"select  EMPNUMBER, INSURANCE, POLICYNO, FACEVALUE, PREMIUM, INSEXPIRE from {schema}.Insurance_accident where EMPNUMBER = @EMPNUMBER";
            var data = await _sql.FetchLegacy<OInsuranceaccidentModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data;
        }

        // Employees assigned to the given accident insurance (all insurance fields must match).
        // Same columns/output as OEmpmasDataAccess._02ByInsurance so both tabs fill the same employee grid.
        public async Task<List<OEmpmasModel?>?> _02ByInsurance(OInsuranceModel insurance, string? schema, string? conn)
        {
            string sql = $@"SELECT e.EmpNumber, e.Email,
                               CONCAT_WS(', ', NULLIF(TRIM(e.EmpLastNm), ''),
                                               CONCAT_WS(' ', NULLIF(TRIM(e.EmpFirstNm), ''), NULLIF(TRIM(e.EmpMidNm), ''))) AS Fullname,
                               s.Name AS EmpStatus,
                               c.ClName
                        FROM {schema}.Insurance_accident a
                        INNER JOIN {schema}.Empmas  e ON e.EmpNumber = a.EmpNumber
                        LEFT JOIN  {schema}.EmpStat s ON s.Code      = e.EmpStat_
                        LEFT JOIN  {schema}.Client  c ON c.ClNumber  = e.Client_
                        WHERE TRIM(a.Insurance)     = TRIM(@Insurance)
                          AND TRIM(a.PolicyNo)      = TRIM(@PolicyNo)
                          AND ROUND(a.FaceValue, 2) = ROUND(@FaceValue, 2)
                          AND ROUND(a.Premium, 2)   = ROUND(@Premium, 2)
                          AND DATE(a.InsExpire)     = DATE(@InsExpire)
                        ORDER BY e.EmpLastNm, e.EmpFirstNm;";

            // OInsuranceModel uses Name / Premiums, Insurance_accident uses Insurance / Premium
            var param = new
            {
                Insurance = insurance.Name,
                insurance.PolicyNo,
                insurance.FaceValue,
                Premium   = insurance.Premiums,
                insurance.InsExpire
            };

            var data = await _sql.FetchLegacy<OEmpmasModel?, dynamic>(sql, param, conn);
            return data;
        }

        // Assigns an Accident insurance to the given employees (one row per employee; EMPNUMBER is the primary key)
        public async Task _03AssignInsurance(List<string> empnumbers, OInsuranceModel insurance, string? schema, string? conn)
        {
            if (empnumbers == null || empnumbers.Count == 0) return;

            string sql = $@"INSERT INTO {schema}.Insurance_accident
                                (EMPNUMBER, INSURANCE, POLICYNO, FACEVALUE, PREMIUM, INSEXPIRE)
                            VALUES
                                (@EmpNumber, @Insurance, @PolicyNo, @FaceValue, @Premium, @InsExpire)
                            ON DUPLICATE KEY UPDATE
                                INSURANCE = VALUES(INSURANCE),
                                POLICYNO  = VALUES(POLICYNO),
                                FACEVALUE = VALUES(FACEVALUE),
                                PREMIUM   = VALUES(PREMIUM),
                                INSEXPIRE = VALUES(INSEXPIRE);";

            // Dapper runs the statement once per item in the list
            var rows = empnumbers.Select(n => new
            {
                EmpNumber = n,
                Insurance = insurance.Name?.Trim(),
                PolicyNo  = insurance.PolicyNo?.Trim(),
                FaceValue = insurance.FaceValue ?? 0,
                Premium   = insurance.Premiums ?? 0,
                insurance.InsExpire
            }).ToList();

            await _sql.ExecuteLegacyCmd<dynamic>(sql, rows, conn);
        }

        // Removes the Accident insurance from the given employees
        public async Task _04ByEmpnumbers(List<string> empnumbers, string? schema, string? conn)
        {
            if (empnumbers == null || empnumbers.Count == 0) return;

            string sql = $@"DELETE FROM {schema}.Insurance_accident WHERE EMPNUMBER IN @EmpNumbers;";
            await _sql.ExecuteLegacyCmd<dynamic>(sql, new { EmpNumbers = empnumbers }, conn);
        }

        public async Task<OInsuranceaccidentModel?> _03(string empnumber, OInsuranceaccidentModel insurance_accident, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Insurance_accident 
                        (EMPNUMBER, INSURANCE, POLICYNO, FACEVALUE, PREMIUM, INSEXPIRE) 
                    values 
                        (@EMPNUMBER, @INSURANCE, @POLICYNO, @FACEVALUE, @PREMIUM, @INSEXPIRE)
                    ON DUPLICATE KEY UPDATE
                        INSURANCE = VALUES(INSURANCE),
                        POLICYNO = VALUES(POLICYNO),
                        FACEVALUE = VALUES(FACEVALUE),
                        PREMIUM = VALUES(PREMIUM),
                        INSEXPIRE = VALUES(INSEXPIRE);";
            await _sql.ExecuteLegacyCmd<dynamic>(sql, insurance_accident, conn);

            sql = $@" select  * from {schema}.Insurance_accident x where x.EMPNUMBER = @EMPNUMBER ;";
            var data = await _sql.FetchLegacy<OInsuranceaccidentModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<OInsuranceaccidentModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Insurance_accident where Id = @Id;";
            await _sql.ExecuteLegacyCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Insurance_accident x where x.Id = @Id ;";
            var data = await _sql.FetchLegacy<OInsuranceaccidentModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOInsuranceaccidentDataAccess
{
    Task<OInsuranceaccidentModel?> _01(OInsuranceaccidentModel insurance_accident, string schema, string conn);
    Task<OInsuranceaccidentModel?> _02(string empnumber, string schema, string conn);
    Task<List<OInsuranceaccidentModel?>?> _02List(string empnumber, string schema, string conn);
    Task<List<OEmpmasModel?>?> _02ByInsurance(OInsuranceModel insurance, string? schema, string? conn);
    Task _03AssignInsurance(List<string> empnumbers, OInsuranceModel insurance, string? schema, string? conn);
    Task _04ByEmpnumbers(List<string> empnumbers, string? schema, string? conn);
    Task<OInsuranceaccidentModel?> _03(string empnumber, OInsuranceaccidentModel insurance_accident, string schema, string conn);
    Task<OInsuranceaccidentModel?> _04(int id, string schema, string conn);
}