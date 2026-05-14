using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OParentDataAccess : IOParentDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OParentDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OParentModel?> _01(OParentModel parent, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Parent (EMPNUMBER, CODE, NAME, AGE, OCC, ADDR, dob) values (@EMPNUMBER, @CODE, @NAME, @AGE, @OCC, @ADDR, @dob)";
            await _sql.ExecuteCmd<dynamic>(sql, parent, conn);

            sql = $@"SELECT * FROM {schema}.Parent WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OParentModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OParentModel?>?> _02(string empnumber, string schema, string conn)
        {
            string sql = $@"select  EMPNUMBER, CODE, NAME, AGE, OCC, ADDR, IF(dob IN ('0000-00-00','0000-00-00 00:00:00'), NULL, dob) AS dob
                         from {schema}.Parent where  EMPNUMBER = @EMPNUMBER";
            var data = await _sql.FetchData<OParentModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data;
        }


        public async Task<OParentModel?> _03(int id, OParentModel parent, string schema, string conn)
        {
            string sql = $@"Update {schema}.Parent set EMPNUMBER = @EMPNUMBER, CODE = @CODE, NAME = @NAME, AGE = @AGE, OCC = @OCC, ADDR = @ADDR, dob = @dob where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, parent, conn);

            sql = $@" select  * from {schema}.Parent x where x.Id = @Id ;";
            var data = await _sql.FetchData<OParentModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OParentModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Parent where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Parent x where x.Id = @Id ;";
            var data = await _sql.FetchData<OParentModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }
}


public interface IOParentDataAccess
{
    Task<OParentModel?> _01(OParentModel parent, string schema, string conn);
    Task<List<OParentModel?>?> _02(string empnumber, string schema, string conn);
    Task<OParentModel?> _03(int id, OParentModel parent, string schema, string conn);
    Task<OParentModel?> _04(int id, string schema, string conn);
}