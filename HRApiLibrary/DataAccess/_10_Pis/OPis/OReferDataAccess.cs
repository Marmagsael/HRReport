using Blazorise;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System.Xml.Linq;


namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OReferDataAccess : IOReferDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OReferDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OReferModel?> _01(OReferModel refer, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Refer (EMPNUMBER, NAME, ADDR, TEL, POSITION) values (@EMPNUMBER, @NAME, @ADDR, @TEL, @POSITION)";
            await _sql.ExecuteCmd<dynamic>(sql, refer, conn);

            sql = $@"SELECT * FROM {schema}.Refer WHERE EMPNUMBER = @EMPNUMBER";

            var res = await _sql.FetchData<OReferModel?, dynamic>(sql, new { refer.EmpNumber}, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OReferModel?>?> _02(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"select  EMPNUMBER, NAME, ADDR, TEL, POSITION from {schema}.Refer where Empnumber = @Empnumber";
            var data = await _sql.FetchData<OReferModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data;
        }

        public async Task<List<OReferModel?>?> _02CheckExisting(string? empnumber, string? name, string? position, string? schema, string? conn)
        {
            string? sql = $@"select EMPNUMBER, NAME, ADDR, TEL, POSITION from {schema}.Refer where EMPNUMBER = @EMPNUMBER AND LOWER(TRIM(NAME)) = LOWER(TRIM(@NAME)) AND LOWER(TRIM(POSITION)) = LOWER(TRIM(@POSITION))";
            var data = await _sql.FetchData<OReferModel?, dynamic>(sql, new { EMPNUMBER = empnumber, NAME = name?.Trim().ToLower(), POSITION = position?.Trim().ToLower() }, conn);
            return data;
        }


        public async Task<OReferModel?> _03(int? id, OReferModel refer, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Refer set EMPNUMBER = @EMPNUMBER, NAME = @NAME, ADDR = @ADDR, TEL = @TEL, POSITION = @POSITION where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, refer, conn);

            sql = $@" select  * from {schema}.Refer x where x.Id = @Id ;";
            var data = await _sql.FetchData<OReferModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OReferModel?> _03(string? empnumber, string? name, string? position, OReferModel refer, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Refer set EMPNUMBER = @EMPNUMBER, NAME = @NAME, ADDR = @ADDR, TEL = @TEL, POSITION = @POSITION
                where LOWER(TRIM(EMPNUMBER)) = LOWER(TRIM(@OldEmpnumber)) AND LOWER(TRIM(NAME)) = LOWER(TRIM(@OldName)) AND LOWER(TRIM(POSITION)) = LOWER(TRIM(@OldPosition));";
            var parameters = new
            {
                EMPNUMBER = refer.EmpNumber,
                NAME = refer.Name,
                ADDR = refer.Addr,
                TEL = refer.Tel,
                POSITION = refer.Position,
                OldEmpnumber = empnumber?.Trim().ToLower(),
                OldName = name?.Trim().ToLower(),
                OldPosition = position?.Trim().ToLower()
            };
            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);
            sql = $@"select * from {schema}.Refer where EMPNUMBER = @EMPNUMBER;";
            var data = await _sql.FetchData<OReferModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OReferModel?> _04(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Refer where EMPNUMBER = @EMPNUMBER;";
            await _sql.ExecuteCmd<dynamic>(sql, new { EMPNUMBER = empnumber }, conn);

            sql = $@" select  * from {schema}.Refer x where x.EMPNUMBER = @EMPNUMBER ;";
            var data = await _sql.FetchData<OReferModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OReferModel?> _04(string? empnumber, string? name, string? position, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Refer where EMPNUMBER = @EMPNUMBER AND NAME = @NAME AND POSITION = @POSITION;";
            await _sql.ExecuteCmd<dynamic>(sql, new { EMPNUMBER = empnumber, NAME = name, POSITION = position }, conn);

            sql = $@" select  * from {schema}.Refer x where x.EMPNUMBER = @EMPNUMBER ;";
            var data = await _sql.FetchData<OReferModel?, dynamic>(sql, new { EMPNUMBER = empnumber }, conn);
            return data?.FirstOrDefault();
        }

    }
}


public interface IOReferDataAccess
{
    Task<OReferModel?> _01(OReferModel refer, string? schema, string? conn);
    Task<List<OReferModel?>?> _02(string? empnumber, string? schema, string? conn);
    Task<List<OReferModel?>?> _02CheckExisting(string? empnumber, string? name, string? position, string? schema, string? conn);
    Task<OReferModel?> _03(int? id, OReferModel refer, string? schema, string? conn);
    Task<OReferModel?> _03(string? empnumber, string? name, string? position, OReferModel refer, string? schema, string? conn);
    Task<OReferModel?> _04(string? empnumber, string? schema, string? conn);
    Task<OReferModel?> _04(string? empnumber, string? name, string? position, string? schema, string? conn);
}