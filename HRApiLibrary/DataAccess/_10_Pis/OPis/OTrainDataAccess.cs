using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System.Xml.Linq;


namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OTrainDataAccess : IOTrainDataAccess
    {
        private readonly I_90_001_MySqlDataAccess _sql;

        public OTrainDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OTrainModel?> _01(OTrainModel train, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Train (EMPNUMBER, PROGRAM, TAKEN, SCHOOL, TRAINOR, TYPE, idtrainhdr) values (@EMPNUMBER, @PROGRAM, @TAKEN, @SCHOOL, @TRAINOR, @TYPE, @idtrainhdr)";
            await _sql.ExecuteCmd<dynamic>(sql, train, conn);

            sql = $@"SELECT * FROM {schema}.Train WHERE EMPNUMBER = EMPNUMBER";

            var res = await _sql.FetchData<OTrainModel?, dynamic>(sql, new { train.EmpNumber}, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OTrainModel?>?> _02(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"select  EMPNUMBER, PROGRAM, TAKEN, SCHOOL, TRAINOR, TYPE, idtrainhdr from {schema}.Train WHERE empnumber = @Empnumber";
            var data = await _sql.FetchData<OTrainModel?, dynamic>(sql, new { Empnumber = empnumber}, conn);
            return data;
        }

        public async Task<List<OTrainModel?>?> _02CheckExisting(string? empnumber, string? program, string? taken, string? schema, string? conn)
        {
            string? sql = $@"select EMPNUMBER, PROGRAM, TAKEN, SCHOOL, TRAINOR, TYPE, idtrainhdr from {schema}.Train where EMPNUMBER = @EMPNUMBER AND LOWER(TRIM(PROGRAM)) = LOWER(TRIM(@PROGRAM)) AND LOWER(TRIM(TAKEN)) = LOWER(TRIM(@TAKEN))";
            var data = await _sql.FetchData<OTrainModel?, dynamic>(sql, new { EMPNUMBER = empnumber, PROGRAM = program?.Trim().ToLower(), TAKEN = taken?.Trim().ToLower() }, conn);
            return data;
        }

        public async Task<OTrainModel?> _03(int? id, OTrainModel train, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Train set EMPNUMBER = @EMPNUMBER, PROGRAM = @PROGRAM, TAKEN = @TAKEN, SCHOOL = @SCHOOL, TRAINOR = @TRAINOR, TYPE = @TYPE, idtrainhdr = @idtrainhdr where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, train, conn);

            sql = $@" select  * from {schema}.Train x where x.Id = @Id ;";
            var data = await _sql.FetchData<OTrainModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OTrainModel?> _03(string? empnumber, string? program, string? taken, OTrainModel train, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Train set EMPNUMBER = @EMPNUMBER, PROGRAM = @PROGRAM, TAKEN = @TAKEN, SCHOOL = @SCHOOL, TRAINOR = @TRAINOR, TYPE = @TYPE, idtrainhdr = @idtrainhdr where LOWER(TRIM(EMPNUMBER)) = LOWER(TRIM(@OldEmpnumber)) AND LOWER(TRIM(PROGRAM)) = LOWER(TRIM(@OldProgram)) AND LOWER(TRIM(TAKEN)) = LOWER(TRIM(@OldTaken));";
            var parameters = new
            {
                EMPNUMBER = train.EmpNumber,
                PROGRAM = train.Program,
                TAKEN = train.Taken,
                SCHOOL = train.School,
                TRAINOR = train.Trainor,
                TYPE = train.Type,
                idtrainhdr = 0,
                OldEmpnumber = empnumber?.Trim().ToLower(),
                OldProgram = program?.Trim().ToLower(),
                OldTaken = taken?.Trim().ToLower()
            };
            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);
            sql = $@"select * from {schema}.Train where EMPNUMBER = @EMPNUMBER;";
            var data = await _sql.FetchData<OTrainModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OTrainModel?> _04(string? empnmber, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Train where EMPNUMBER = @EMPNUMBER;";
            await _sql.ExecuteCmd<dynamic>(sql, new { EMPNUMBER = empnmber }, conn);

            sql = $@" select  * from {schema}.Train x where x.EMPNUMBER = @EMPNUMBER ;";
            var data = await _sql.FetchData<OTrainModel?, dynamic>(sql, new { EMPNUMBER = empnmber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OTrainModel?> _04(string? empnmber, string? program, string? taken, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Train where EMPNUMBER = @EMPNUMBER AND  PROGRAM = @PROGRAM AND TAKEN =@TAKEN;";
            await _sql.ExecuteCmd<dynamic>(sql, new { EMPNUMBER = empnmber, PROGRAM = program, TAKEN = taken }, conn);

            sql = $@" select  * from {schema}.Train x where x.EMPNUMBER = @EMPNUMBER ;";
            var data = await _sql.FetchData<OTrainModel?, dynamic>(sql, new { EMPNUMBER = empnmber }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOTrainDataAccess
{
    Task<OTrainModel?> _01(OTrainModel train, string? schema, string? conn);
    Task<List<OTrainModel?>?> _02( string? empnumber, string? schema, string? conn);
    Task<List<OTrainModel?>?> _02CheckExisting(string? empnumber, string? program, string? taken, string? schema, string? conn);
    Task<OTrainModel?> _03(int? id, OTrainModel train, string? schema, string? conn);
    Task<OTrainModel?> _03(string? empnumber, string? program, string? taken, OTrainModel train, string? schema, string? conn);
    Task<OTrainModel?> _04(string? empnumber, string? schema, string? conn);
    Task<OTrainModel?> _04(string? empnmber, string? program, string? taken, string? schema, string? conn);
}