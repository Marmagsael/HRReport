using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using Org.BouncyCastle.Ocsp;


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


        public async Task<List<OEmployModel?>?> _02CheckExisting(string? empnumber, string? position, string? from, string? to, string? schema, string? conn)
        {
            string? sql = $@" SELECT * FROM {schema}.Employ  WHERE EMPNUMBER = @Empnumber
                              AND LOWER(TRIM(POSI)) = LOWER(TRIM(@Position))
                              AND LOWER(TRIM(FROM_)) = LOWER(TRIM(@From))
                              AND LOWER(TRIM(TO_)) = LOWER(TRIM(@To))";

            var data = await _sql.FetchData<OEmployModel?, dynamic>(sql, new {
                                                                                Empnumber = empnumber,
                                                                                Position = position,
                                                                                From = from,
                                                                                To = to
                                                                            }, conn);

            return data;
        }



        public async Task<List<OEmployModel?>?> _03(string? empnumber, string? position, string? from, string? to, string? schema, string? conn)
        {
            string? sql = $@" SELECT * FROM {schema}.Employ  WHERE EMPNUMBER = @Empnumber
                              AND LOWER(TRIM(POSI)) = LOWER(TRIM(@Position))
                              AND LOWER(TRIM(FROM_)) = LOWER(TRIM(@From))
                              AND LOWER(TRIM(TO_)) = LOWER(TRIM(@To))";

            var data = await _sql.FetchData<OEmployModel?, dynamic>(sql, new
            {
                Empnumber = empnumber,
                Position = position,
                From = from,
                To = to
            }, conn);

            return data;
        }

        public async Task<OEmployModel?> _03(string? empnumber, string? position, string? from, string? to, OEmployModel employ, string? schema, string? conn)
        {
            string? sql = $@"
                        UPDATE {schema}.Employ
                        SET EMPNUMBER = @EMPNUMBER, COMP = @COMP, ADDR1 = @ADDR1, ADDR2 = @ADDR2, TEL = @TEL,
                            POSI = @POSI, FROM_ = @FROM_, TO_ = @TO_, SAL = @SAL, REM1 = @REM1, REM2 = @REM2
                        WHERE EMPNUMBER = @OldEmpnumber
                          AND LOWER(TRIM(POSI)) = LOWER(TRIM(@OldPosition))
                          AND LOWER(TRIM(FROM_)) = LOWER(TRIM(@OldFrom))
                          AND LOWER(TRIM(TO_)) = LOWER(TRIM(@OldTo));";

            var parameters = new
            {
                employ.EmpNumber,
                employ.Comp,
                employ.Addr1,
                employ.Addr2,
                employ.Tel,
                employ.Posi,
                employ.From_,
                employ.To_,
                employ.Sal,
                employ.Rem1,
                employ.Rem2,
                OldEmpnumber = empnumber,
                OldPosition = position,
                OldFrom = from,
                OldTo = to
            };

            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);

            sql = $@"SELECT * FROM {schema}.Employ x WHERE x.Empnumber = @Empnumber;";
            var data = await _sql.FetchData<OEmployModel?, dynamic>(sql, new { Empnumber = employ.EmpNumber }, conn);

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

        public async Task<OEmployModel?> _04(string? empnumber, string? position, string? from, string? to, string? schema, string? conn)
        {
            string? sql = $@" DELETE FROM {schema}.Employ
                                WHERE EMPNUMBER = @Empnumber
                                  AND LOWER(TRIM(POSI)) = LOWER(TRIM(@Position))
                                  AND LOWER(TRIM(FROM_)) = LOWER(TRIM(@From))
                                  AND LOWER(TRIM(TO_)) = LOWER(TRIM(@To));";

            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber, Position = position, From = from, To = to }, conn);

            sql = $@"SELECT * FROM {schema}.Employ x WHERE x.Empnumber = @Empnumber;";
            var data = await _sql.FetchData<OEmployModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);

            return data?.FirstOrDefault();
        }
    }
}

public interface IOEmployDataAccess
{
    Task<OEmployModel?> _01(OEmployModel employ, string? schema, string? conn);
    Task<List<OEmployModel?>?> _02(string? empnumber, string? schema, string? conn);
    Task<List<OEmployModel?>?> _02CheckExisting(string? empnumber, string? position, string? from, string? to, string? schema, string? conn);
    Task<List<OEmployModel?>?> _03(string? empnumber, string? position, string? from, string? to, string? schema, string? conn);
    Task<OEmployModel?> _03(string? empnumber, string? position, string? from, string? to, OEmployModel employ, string? schema, string? conn);
    Task<OEmployModel?> _04(string? empnumber, string? schema, string? conn);
    Task<OEmployModel?> _04(string? empnumber, string? position, string? from, string? to, string? schema, string? conn);
    
}