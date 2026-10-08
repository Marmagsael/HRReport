
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OOfstaffDataAccess : IOOfstaffDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OOfstaffDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OOfstaffModel?> _01(OOfstaffModel ofstaff, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Ofstaff (staff_id, staff_nm, staff_pos, defa) values (@staff_id, @staff_nm, @staff_pos, @defa)";
            await _sql.ExecuteCmd<dynamic>(sql, ofstaff, conn);

            sql = $@"SELECT * FROM {schema}.Ofstaff WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OOfstaffModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<OOfstaffModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  staff_id, staff_nm, staff_pos, defa from {schema}.Ofstaff where Id = @Id";
            var data = await _sql.FetchData<OOfstaffModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<List<OOfstaffModel?>?> _02s( string schema, string conn)
        {
            string sql = $@"select  staff_id, staff_nm, staff_pos, defa from {schema}.Ofstaff ORDER BY staff_nm";
            var data = await _sql.FetchData<OOfstaffModel?, dynamic>(sql, new { }, conn);
            return data;
        }



        public async Task<OOfstaffModel?> _03(int id, OOfstaffModel ofstaff, string schema, string conn)
        {
            string sql = $@"Update {schema}.Ofstaff set staff_id = @staff_id, staff_nm = @staff_nm, staff_pos = @staff_pos, defa = @defa where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, ofstaff, conn);

            sql = $@" select  * from {schema}.Ofstaff x where x.Id = @Id ;";
            var data = await _sql.FetchData<OOfstaffModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OOfstaffModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Ofstaff where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Ofstaff x where x.Id = @Id ;";
            var data = await _sql.FetchData<OOfstaffModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IOOfstaffDataAccess
    {
        Task<OOfstaffModel?> _01(OOfstaffModel ofstaff, string schema, string conn);
        Task<OOfstaffModel?> _02(int id, string schema, string conn);
        Task<List<OOfstaffModel?>?> _02s( string schema, string conn);
        Task<OOfstaffModel?> _03(int id, OOfstaffModel ofstaff, string schema, string conn);
        Task<OOfstaffModel?> _04(int id, string schema, string conn);
    }
}
