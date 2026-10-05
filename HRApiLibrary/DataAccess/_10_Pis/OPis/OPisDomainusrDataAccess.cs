using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OPisDomainusrDataAccess : IOPisDomainusrDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OPisDomainusrDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OPisDomainusrModel?> _01(OPisDomainusrModel domainusr, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Domainusr (Empnumber, Status, CreatedBy, DateCreated) values (@Empnumber, @Status, @CreatedBy, Now())";
            await _sql.ExecuteCmd<dynamic>(sql, domainusr, conn);

            sql = $@"SELECT * FROM {schema}.Domainusr WHERE Empnumber = @Empnumber";

            var res = await _sql.FetchData<OPisDomainusrModel?, dynamic>(sql, new { domainusr.Empnumber }, conn);

            return res.FirstOrDefault();
        }


        public async Task<OPisDomainusrModel?> _02(string empnumber, string schema, string conn)
        {
            string sql = $@"select  Empnumber, Status, CreatedBy, DateCreated from {schema}.Domainusr where Empnumber = @Empnumber";
            var data = await _sql.FetchData<OPisDomainusrModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<List<OPisDomainusrModel?>?> _02(string schema, string conn)
        {
            string sql = $@"select   CONCAT_WS(' ',CONCAT(e.emplastnm, ','), e.empfirstnm, e.empmidnm) AS FullName, d.* from {schema}.Domainusr d 
                            LEFT JOIN {schema}.empmas e on e.empnumber = d.empnumber";
            var data = await _sql.FetchData<OPisDomainusrModel?, dynamic>(sql, new { }, conn);
            return data;
        }


        public async Task<OPisDomainusrModel?> _03(int id, OPisDomainusrModel domainusr, string schema, string conn)
        {
            string sql = $@"Update {schema}.Domainusr set Empnumber = @Empnumber, Status = @Status, CreatedBy = @CreatedBy, DateCreated = @DateCreated where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, domainusr, conn);

            sql = $@" select  * from {schema}.Domainusr x where x.Id = @Id ;";
            var data = await _sql.FetchData<OPisDomainusrModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OPisDomainusrModel?> _03StatusOnly(string empnumber, string status, string schema, string conn)
        {
            string sql = $@"Update {schema}.Domainusr set  Status = @Status where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber, Status = status }, conn);

            sql = $@" select  * from {schema}.Domainusr x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OPisDomainusrModel?, dynamic>(sql, new { Empnumber = empnumber, Status = status }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OPisDomainusrModel?> _04(string empnumber, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Domainusr where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber }, conn);

            sql = $@" select  * from {schema}.Domainusr x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OPisDomainusrModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IOPisDomainusrDataAccess
    {
        Task<OPisDomainusrModel?> _01(OPisDomainusrModel domainusr, string schema, string conn);
        Task<OPisDomainusrModel?> _02(string empnumber, string schema, string conn);
        Task<List<OPisDomainusrModel?>?> _02(string schema, string conn);
        Task<OPisDomainusrModel?> _03(int id, OPisDomainusrModel domainusr, string schema, string conn);
        Task<OPisDomainusrModel?> _03StatusOnly(string empnumber, string status, string schema, string conn);
        Task<OPisDomainusrModel?> _04(string empnumber, string schema, string conn);
    }
}
