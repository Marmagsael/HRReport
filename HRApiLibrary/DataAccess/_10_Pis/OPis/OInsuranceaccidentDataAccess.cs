
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
            await _sql.ExecuteCmd<dynamic>(sql, insurance_accident, conn);

            sql = $@"SELECT * FROM {schema}.Insurance_accident WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OInsuranceaccidentModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<OInsuranceaccidentModel?> _02(int empnumber, string schema, string conn)
        {
            string sql = $@"select  EMPNUMBER, INSURANCE, POLICYNO, FACEVALUE, PREMIUM, INSEXPIRE from {schema}.Insurance_accident where Id = @Id";
            var data = await _sql.FetchData<OInsuranceaccidentModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data?.FirstOrDefault();
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
            await _sql.ExecuteCmd<dynamic>(sql, insurance_accident, conn);

            sql = $@" select  * from {schema}.Insurance_accident x where x.EMPNUMBER = @EMPNUMBER ;";
            var data = await _sql.FetchData<OInsuranceaccidentModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<OInsuranceaccidentModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Insurance_accident where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Insurance_accident x where x.Id = @Id ;";
            var data = await _sql.FetchData<OInsuranceaccidentModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOInsuranceaccidentDataAccess
{
    Task<OInsuranceaccidentModel?> _01(OInsuranceaccidentModel insurance_accident, string schema, string conn);
    Task<OInsuranceaccidentModel?> _02(int id, string schema, string conn);
    Task<OInsuranceaccidentModel?> _03(string empnumber, OInsuranceaccidentModel insurance_accident, string schema, string conn);
    Task<OInsuranceaccidentModel?> _04(int id, string schema, string conn);
}