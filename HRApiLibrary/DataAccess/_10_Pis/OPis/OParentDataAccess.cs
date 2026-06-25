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

        public async Task<OParentModel?> _01(OParentModel parent, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Parent (EMPNUMBER, CODE, NAME, AGE, OCC, ADDR, dob) values (@EMPNUMBER, @CODE, @NAME, @AGE, @OCC, @ADDR, @dob)";
            await _sql.ExecuteCmd<dynamic>(sql, parent, conn);

            sql = $@"SELECT * FROM {schema}.Parent WHERE EMPNUMBER = @EMPNUMBER";

            var res = await _sql.FetchData<OParentModel?, dynamic>(sql, new { parent.EmpNumber }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OParentModel?>?> _02(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"select  EMPNUMBER, CODE, NAME, AGE, OCC, ADDR, IF(dob IN ('0000-00-00','0000-00-00 00:00:00'), NULL, dob) AS dob
                         from {schema}.Parent where  EMPNUMBER = @EMPNUMBER";
            var data = await _sql.FetchData<OParentModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data;
        }


        public async Task<List<OParentModel?>?> _02CheckExisting(string? empnumber, string? name, string? code, string? schema, string? conn)
        {
            string? sql = $@"select  EMPNUMBER, CODE, NAME
                         from {schema}.Parent where  EMPNUMBER = @EMPNUMBER AND NAME = @NAME AND CODE=@CODE";
            var data = await _sql.FetchData<OParentModel?, dynamic>(sql, new { EMPNUMBER = empnumber, NAME = name, CODE = code }, conn);
            return data;
        }

        public async Task<OParentModel?> _03(string? empnumber, OParentModel parent, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Parent set EMPNUMBER = @EMPNUMBER, CODE = @CODE, NAME = @NAME, AGE = @AGE, OCC = @OCC, ADDR = @ADDR, dob = @dob where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, parent, conn);

            sql = $@" select  * from {schema}.Parent x where x.EMPNUMBER = @EMPNUMBER ;";
            var data = await _sql.FetchData<OParentModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OParentModel?> _03(string? empnumber, string? name, string? code, OParentModel parent, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Parent set EMPNUMBER = @EMPNUMBER, CODE = @CODE, NAME = @NAME, AGE = @AGE, OCC = @OCC, ADDR = @ADDR, dob = @dob where EMPNUMBER = @OldEmpnumber  AND LOWER(TRIM(NAME)) = LOWER(TRIM(@OldName)) AND LOWER(TRIM(CODE)) = LOWER(TRIM(@OldCode));";
            var parameters = new
            {
                parent.EmpNumber,
                parent.Code,
                parent.Name,
                parent.Age,
                parent.Occ,
                parent.Addr,
                parent.DoB,
                OldEmpnumber = empnumber,
                OldName = name,
                OldCode = code
            };
            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);
            sql = $@"select * from {schema}.Parent x where x.EMPNUMBER = @EMPNUMBER;";
            var data = await _sql.FetchData<OParentModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<OParentModel?> _04(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Parent where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber }, conn);

            sql = $@" select  * from {schema}.Parent x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OParentModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<OParentModel?> _04(string? empnumber, string? name, string? code, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Parent where Empnumber = @Empnumber AND Name = @Name AND Code = @Code;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber, Name = name, Code = code }, conn);

            sql = $@" select  * from {schema}.Parent x where Empnumber = @Empnumber AND Name = @Name AND Code = @Code ;";
            var data = await _sql.FetchData<OParentModel?, dynamic>(sql, new { Empnumber = empnumber, Name = name, Code = code }, conn);
            return data?.FirstOrDefault();
        }


    }
}


public interface IOParentDataAccess
{
    Task<OParentModel?> _01(OParentModel parent, string? schema, string? conn);
    Task<List<OParentModel?>?> _02(string? empnumber, string? schema, string? conn);
    Task<List<OParentModel?>?> _02CheckExisting(string? empnumber, string? name, string? code, string? schema, string? conn);
    Task<OParentModel?> _03(string? empnumber, OParentModel parent, string? schema, string? conn);
    Task<OParentModel?> _03(string? empnumber, string? name,  string? code, OParentModel parent, string? schema, string? conn);
    Task<OParentModel?> _04(string? empnumber, string? schema, string? conn);
    Task<OParentModel?> _04(string? empnumber, string? name, string? code, string? schema, string? conn);
}