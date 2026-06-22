using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;


namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OChildrenDataAccess : IOChildrenDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OChildrenDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OChildrenModel?> _01(OChildrenModel children, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Children (empnumber, name, bday) values (@empnumber, @name, @bday)";
            await _sql.ExecuteCmd<dynamic>(sql, children, conn);

            sql = $@"SELECT * FROM {schema}.Children WHERE empnumber = @empnumber";

            var res = await _sql.FetchData<OChildrenModel?, dynamic>(sql, new { children.EmpNumber }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OChildrenModel?>?> _02(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"select  empnumber, name, bday from {schema}.Children where Empnumber = @Empnumber";
            var data = await _sql.FetchData<OChildrenModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data;
        }


        public async Task<OChildrenModel?> _03(int? id, OChildrenModel children, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Children set empnumber = @empnumber, name = @name, bday = @bday where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, children, conn);

            sql = $@" select  * from {schema}.Children x where x.Id = @Id ;";
            var data = await _sql.FetchData<OChildrenModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OChildrenModel?> _04(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Children where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber }, conn);

            sql = $@" select  * from {schema}.Children x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OChildrenModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOChildrenDataAccess
{
    Task<OChildrenModel?> _01(OChildrenModel children, string? schema, string? conn);
    Task<List<OChildrenModel?>?> _02(string? empnumber, string? schema, string? conn);
    Task<OChildrenModel?> _03(int? id, OChildrenModel children, string? schema, string? conn);
    Task<OChildrenModel?> _04(string? empnumber, string? schema, string? conn);
}