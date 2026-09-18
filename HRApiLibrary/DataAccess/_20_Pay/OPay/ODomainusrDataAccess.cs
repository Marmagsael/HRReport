using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay.OPay;


namespace HRApiLibrary.DataAccess._20_Pay.OPay
{
    public class ODomainusrDataAccess : IODomainusrDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public ODomainusrDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<ODomainusrModel?> _01(ODomainusrModel domainusr, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Domainusr (Empnumber, Status, CreatedBy, DateCreated) values (@Empnumber, @Status, @CreatedBy, Now())";
            await _sql.ExecuteCmd<dynamic>(sql, domainusr, conn);

            sql = $@"SELECT * FROM {schema}.Domainusr WHERE Empnumber = @Empnumber";

            var res = await _sql.FetchData<ODomainusrModel?, dynamic>(sql, new { domainusr.Empnumber }, conn);

            return res.FirstOrDefault();
        }


        public async Task<ODomainusrModel?> _02(string empnumber, string schema, string conn)
        {
            string sql = $@"select  Empnumber, Status, CreatedBy, DateCreated from {schema}.Domainusr where Empnumber = @Empnumber";
            var data = await _sql.FetchData<ODomainusrModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<List<ODomainusrModel?>?> _02WithEmployeeName(string pisSchema, string paySchema, string conn)
        {
            string sql = $@"select   CONCAT_WS(' ',CONCAT(e.emplastnm, ','), e.empfirstnm, e.empmidnm) AS FullName, d.* from {paySchema}.Domainusr d 
                            LEFT JOIN {pisSchema}.empmas e on e.empnumber = d.empnumber";
            var data = await _sql.FetchData<ODomainusrModel?, dynamic>(sql, new { }, conn);
            return data;
        }


        public async Task<ODomainusrModel?> _03(int id, ODomainusrModel domainusr, string schema, string conn)
        {
            string sql = $@"Update {schema}.Domainusr set Empnumber = @Empnumber, Status = @Status, CreatedBy = @CreatedBy, DateCreated = @DateCreated where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, domainusr, conn);

            sql = $@" select  * from {schema}.Domainusr x where x.Id = @Id ;";
            var data = await _sql.FetchData<ODomainusrModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<ODomainusrModel?> _03StatusOnly(string empnumber, string status, string schema, string conn)
        {
            string sql = $@"Update {schema}.Domainusr set  Status = @Status where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber, Status = status }, conn);

            sql = $@" select  * from {schema}.Domainusr x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<ODomainusrModel?, dynamic>(sql, new { Empnumber = empnumber, Status = status }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<ODomainusrModel?> _04(string empnumber, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Domainusr where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber }, conn);

            sql = $@" select  * from {schema}.Domainusr x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<ODomainusrModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IODomainusrDataAccess
    {
        Task<ODomainusrModel?> _01(ODomainusrModel domainusr, string schema, string conn);
        Task<ODomainusrModel?> _02(string empnumber, string schema, string conn);
        Task<List<ODomainusrModel?>?> _02WithEmployeeName(string pisSchema, string paySchema, string conn);
        Task<ODomainusrModel?> _03(int id, ODomainusrModel domainusr, string schema, string conn);
        Task<ODomainusrModel?> _03StatusOnly(string empnumber, string status, string schema, string conn);
        Task<ODomainusrModel?> _04(string empnumber, string schema, string conn);
    }
}
