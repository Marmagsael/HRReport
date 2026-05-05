using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;


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

            sql = $@"SELECT * FROM {schema}.Train WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OTrainModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OTrainModel?>?> _02(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"select  EMPNUMBER, PROGRAM, TAKEN, SCHOOL, TRAINOR, TYPE, idtrainhdr from {schema}.Train WHERE empnumber = @Empnumber";
            var data = await _sql.FetchData<OTrainModel?, dynamic>(sql, new { Empnumber = empnumber}, conn);
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

        public async Task<OTrainModel?> _04(int? id, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Train where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Train x where x.Id = @Id ;";
            var data = await _sql.FetchData<OTrainModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOTrainDataAccess
{
    Task<OTrainModel?> _01(OTrainModel train, string? schema, string? conn);
    Task<List<OTrainModel?>?> _02( string? empnumber, string? schema, string? conn);
    Task<OTrainModel?> _03(int? id, OTrainModel train, string? schema, string? conn);
    Task<OTrainModel?> _04(int? id, string? schema, string? conn);
}