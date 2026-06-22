using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;


namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OEmployDataAccess : IOEmployDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OEmployDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OEmployModel?> _01(OEmployModel employ, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Employ (EMPNUMBER, COMP, ADDR1, ADDR2, TEL, POSI, FROM_, TO_, SAL, REM1, REM2) values (@EMPNUMBER, @COMP, @ADDR1, @ADDR2, @TEL, @POSI, @FROM_, @TO_, @SAL, @REM1, @REM2)";
            await _sql.ExecuteCmd<dynamic>(sql, employ, conn);

            sql = $@"SELECT * FROM {schema}.Employ WHERE Empnumber = @Empnumber";

            var res = await _sql.FetchData<OEmployModel?, dynamic>(sql, new { employ.EmpNumber }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OEmployModel?>?> _02(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"select  * from {schema}.Employ where EMPNUMBER = @Empnumber";
            var data = await _sql.FetchData<OEmployModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data;
        }


        public async Task<OEmployModel?> _03(string? empnumber, OEmployModel employ, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Employ set EMPNUMBER = @EMPNUMBER, COMP = @COMP, ADDR1 = @ADDR1, ADDR2 = @ADDR2, TEL = @TEL, POSI = @POSI, FROM_ = @FROM_, TO_ = @TO_, SAL = @SAL, REM1 = @REM1, REM2 = @REM2 where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, employ, conn);

            sql = $@" select  * from {schema}.Employ x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OEmployModel?, dynamic>(sql, new { employ.EmpNumber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OEmployModel?> _04(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Employ where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber }, conn);

            sql = $@" select  * from {schema}.Employ x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OEmployModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOEmployDataAccess
{
    Task<OEmployModel?> _01(OEmployModel employ, string? schema, string? conn);
    Task<List<OEmployModel?>?> _02(string? empnumber, string? schema, string? conn);
    Task<OEmployModel?> _03(string? empnumber, OEmployModel employ, string? schema, string? conn);
    Task<OEmployModel?> _04(string? empnumber, string? schema, string? conn);
}