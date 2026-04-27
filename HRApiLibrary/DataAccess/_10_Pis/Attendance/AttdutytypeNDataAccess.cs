using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.Attendance
{
    public class AttdutytypeNDataAccess : IAttdutytypeNDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public AttdutytypeNDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<AttdutytypeModel?> _01(AttdutytypeModel attdutytype, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Attdutytype (Code, Name) values (@Code, @Name)";
            await _sql.ExecuteCmd<dynamic>(sql, attdutytype, conn);

            sql = $@"SELECT * FROM {schema}.Attdutytype WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<AttdutytypeModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<AttdutytypeModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  Id, Code, Name from {schema}.Attdutytype where Id = @Id";
            var data = await _sql.FetchData<AttdutytypeModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<List<AttdutytypeModel?>?> _02(string schema, string conn)
        {
            string sql = $@"select  Id, Code, Name from {schema}.Attdutytype ";
            var data = await _sql.FetchData<AttdutytypeModel?, dynamic>(sql, new { }, conn);
            return data;
        }


        public async Task<AttdutytypeModel?> _03(int id, AttdutytypeModel attdutytype, string schema, string conn)
        {
            string sql = $@"Update {schema}.Attdutytype set Code = @Code, Name = @Name where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, attdutytype, conn);

            sql = $@" select  * from {schema}.Attdutytype x where x.Id = @Id ;";
            var data = await _sql.FetchData<AttdutytypeModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<AttdutytypeModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Attdutytype where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Attdutytype x where x.Id = @Id ;";
            var data = await _sql.FetchData<AttdutytypeModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IAttdutytypeNDataAccess
    {
        Task<AttdutytypeModel?> _01(AttdutytypeModel attdutytype, string schema, string conn);
        Task<AttdutytypeModel?> _02(int id, string schema, string conn);
        Task<List<AttdutytypeModel?>?> _02(string schema, string conn);
        Task<AttdutytypeModel?> _03(int id, AttdutytypeModel attdutytype, string schema, string conn);
        Task<AttdutytypeModel?> _04(int id, string schema, string conn);
    }
}
