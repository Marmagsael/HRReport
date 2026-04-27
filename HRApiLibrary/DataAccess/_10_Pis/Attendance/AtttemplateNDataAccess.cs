using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis.Attendance
{
    public class AtttemplateNDataAccess : IAtttemplateNDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public AtttemplateNDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<AtttemplateModel?> _01(AtttemplateModel atttemplate, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Atttemplate (EmpmasId, AttendanceTypeId, D1_In, D1_HrsLength, D1_DutyType, D2_In, D2_HrsLength, D2_DutyType, D3_In, D3_HrsLength, D3_DutyType, D4_In, D4_HrsLength, D4_DutyType, D5_In, D5_HrsLength, D5_DutyType, D6_In, D6_HrsLength, D6_DutyType, D7_In, D7_HrsLength, D7_DutyType) values (@EmpmasId, @AttendanceTypeId, @D1_In, @D1_HrsLength, @D1_DutyType, @D2_In, @D2_HrsLength, @D2_DutyType, @D3_In, @D3_HrsLength, @D3_DutyType, @D4_In, @D4_HrsLength, @D4_DutyType, @D5_In, @D5_HrsLength, @D5_DutyType, @D6_In, @D6_HrsLength, @D6_DutyType, @D7_In, @D7_HrsLength, @D7_DutyType)";
            await _sql.ExecuteCmd<dynamic>(sql, atttemplate, conn);

            sql = $@"SELECT * FROM {schema}.Atttemplate WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<AtttemplateModel?>?> _02(int empmasId, string schema, string conn)
        {
            string sql = $@"select  EmpmasId, AttendanceTypeId, D1_In, D1_HrsLength, D1_DutyType, D2_In, D2_HrsLength, D2_DutyType, D3_In, D3_HrsLength, D3_DutyType, D4_In, D4_HrsLength, D4_DutyType, D5_In, D5_HrsLength, D5_DutyType, D6_In, D6_HrsLength, D6_DutyType, D7_In, D7_HrsLength, D7_DutyType from {schema}.Atttemplate where EmpmasId = @EmpmasId";
            var data = await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
            return data;
        }


        public async Task<AtttemplateModel?> _03(int id, AtttemplateModel atttemplate, string schema, string conn)
        {
            string sql = $@"Update {schema}.Atttemplate set EmpmasId = @EmpmasId, AttendanceTypeId = @AttendanceTypeId, D1_In = @D1_In, D1_HrsLength = @D1_HrsLength, D1_DutyType = @D1_DutyType, D2_In = @D2_In, D2_HrsLength = @D2_HrsLength, D2_DutyType = @D2_DutyType, D3_In = @D3_In, D3_HrsLength = @D3_HrsLength, D3_DutyType = @D3_DutyType, D4_In = @D4_In, D4_HrsLength = @D4_HrsLength, D4_DutyType = @D4_DutyType, D5_In = @D5_In, D5_HrsLength = @D5_HrsLength, D5_DutyType = @D5_DutyType, D6_In = @D6_In, D6_HrsLength = @D6_HrsLength, D6_DutyType = @D6_DutyType, D7_In = @D7_In, D7_HrsLength = @D7_HrsLength, D7_DutyType = @D7_DutyType where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, atttemplate, conn);

            sql = $@" select  * from {schema}.Atttemplate x where x.Id = @Id ;";
            var data = await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<AtttemplateModel?> _04(int id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Atttemplate where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Atttemplate x where x.Id = @Id ;";
            var data = await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IAtttemplateNDataAccess
{
    Task<AtttemplateModel?> _01(AtttemplateModel atttemplate, string schema, string conn);
    Task <List<AtttemplateModel?>?> _02(int empmasId, string schema, string conn);
    Task<AtttemplateModel?> _03(int id, AtttemplateModel atttemplate, string schema, string conn);
    Task<AtttemplateModel?> _04(int id, string schema, string conn);
}