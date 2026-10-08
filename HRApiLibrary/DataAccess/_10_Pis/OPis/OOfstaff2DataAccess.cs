
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OOfstaff2DataAccess : IOOfstaff2DataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public OOfstaff2DataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OOfstaff2Model?> _01(OOfstaff2Model ofstaff, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Ofstaff2 (staff_id, staff_nm, staff_pos, defa) values (@staff_id, @staff_nm, @staff_pos, @defa)";
            await _sql.ExecuteCmd<dynamic>(sql, ofstaff, conn);

            sql = $@"SELECT * FROM {schema}.Ofstaff2 WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<OOfstaff2Model?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<OOfstaff2Model?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  staff_id, staff_nm, staff_pos, defa from {schema}.Ofstaff2 where Id = @Id";
            var data = await _sql.FetchData<OOfstaff2Model?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<List<OOfstaff2Model?>?> _02s( string schema, string conn)
        {
            string sql = $@"select  staff_id, staff_nm, staff_pos, defa from {schema}.Ofstaff2 ORDER BY staff_nm";
            var data = await _sql.FetchData<OOfstaff2Model?, dynamic>(sql, new {  }, conn);
            return data;
        }



        public async Task<OOfstaff2Model?> _03(int id, OOfstaff2Model ofstaff, string schema, string conn)
        {
            string sql = $@"Update {schema}.Ofstaff2 set staff_id = @staff_id, staff_nm = @staff_nm, staff_pos = @staff_pos, defa = @defa where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, ofstaff, conn);

            sql = $@" select  * from {schema}.Ofstaff2 x where x.Id = @Id ;";
            var data = await _sql.FetchData<OOfstaff2Model?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OOfstaff2Model?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Ofstaff2 where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Ofstaff2 x where x.Id = @Id ;";
            var data = await _sql.FetchData<OOfstaff2Model?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IOOfstaff2DataAccess
    {
        Task<OOfstaff2Model?> _01(OOfstaff2Model ofstaff, string schema, string conn);
        Task<OOfstaff2Model?> _02(int id, string schema, string conn);
        Task<List<OOfstaff2Model?>?> _02s(string schema, string conn);
        Task<OOfstaff2Model?> _03(int id, OOfstaff2Model ofstaff, string schema, string conn);
        Task<OOfstaff2Model?> _04(int id, string schema, string conn);
    }
}
