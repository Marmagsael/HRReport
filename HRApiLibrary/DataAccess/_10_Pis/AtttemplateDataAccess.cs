using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class AtttemplateDataAccess : IAtttemplateDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

	public AtttemplateDataAccess(I_90_001_MySqlDataAccess sql)
	{
			_sql = sql;
	}

	public async Task<AtttemplateModel?> _01(AtttemplateModel atttemplate, string? schema, string? conn )
	{
		string sql = $@"Insert into {schema}.Atttemplate 
                            (EmpmasId,  AttendanceTypeId,  D1_In,  D1_HrsLength,  D1_DutyType,  D2_In,  D2_HrsLength,  D2_DutyType,  D3_In,  D3_HrsLength,  D3_DutyType,  D4_In,  D4_HrsLength,  D4_DutyType,  D5_In,  D5_HrsLength,  D5_DutyType,  D6_In,  D6_HrsLength,  D6_DutyType,  D7_In,  D7_HrsLength,  D7_DutyType) values 
                            (@EmpmasId, @AttendanceTypeId, @D1_In, @D1_HrsLength, @D1_DutyType, @D2_In, @D2_HrsLength, @D2_DutyType, @D3_In, @D3_HrsLength, @D3_DutyType, @D4_In, @D4_HrsLength, @D4_DutyType, @D5_In, @D5_HrsLength, @D5_DutyType, @D6_In, @D6_HrsLength, @D6_DutyType, @D7_In, @D7_HrsLength, @D7_DutyType)
						on duplicate key update 
						    AttendanceTypeId        = @AttendanceTypeId, 
                            D1_In                   = @D1_In, D1_HrsLength = @D1_HrsLength,
                            D1_DutyType             = @D1_DutyType,
                            D2_In                   = @D2_In,
                            D2_HrsLength            = @D2_HrsLength,
                            D2_DutyType             = @D2_DutyType,
                            D3_In                   = @D3_In,
                            D3_HrsLength            = @D3_HrsLength,
                            D3_DutyType             = @D3_DutyType,
                            D4_In                   = @D4_In,
                            D4_HrsLength            = @D4_HrsLength,
                            D4_DutyType             = @D4_DutyType,
                            D5_In                   = @D5_In,
                            D5_HrsLength            = @D5_HrsLength,
                            D5_DutyType             = @D5_DutyType,
                            D6_In                   = @D6_In,
                            D6_HrsLength            = @D6_HrsLength,
                            D6_DutyType             = @D6_DutyType,
                            D7_In                   = @D7_In,
                            D7_HrsLength            = @D7_HrsLength,
                            D7_DutyType             = @D7_DutyType; 
                        SELECT * FROM {schema}.Atttemplate WHERE EmpmasID = @EmpmasId;    " ; 
		var res = await _sql.FetchData<AtttemplateModel?,dynamic>(sql,atttemplate,conn);
		return res.FirstOrDefault();
	}

	
	public async Task<AtttemplateModel?> _02(int? id, string? schema, string? conn)
	{
		string sql = $@"select  * from {schema}.Atttemplate where EmpmasId = @Id" ; 
		var data = await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { Id = id }, conn);

		return data?.FirstOrDefault();
	}
    
	public async Task<List<AtttemplateModel?>?> _02s(int? id, string? schema, string? conn)
	{
		string sql = $@"select  * from {schema}.Atttemplate where EmpmasId = @Id" ; 
		var data = await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { Id = id }, conn); 
		return data;
	}
	
    private class DailyRow
    {
        public string? DutyType { get; set; }
        public int     PIn      { get; set; }
        public int     Duration { get; set; }
    }

    // -- No schedule assigned : Mon-Fri follow the first Attscheddaily row, Sat the first RN row, Sun the first RD row --
    public async Task<AtttemplateModel?> _02NoSchedule(int? empmasId, string? schema, string? conn)
    {
        var rows = await _sql.FetchData<DailyRow, dynamic>($"select DutyType, PIn, Duration from {schema}.Attscheddaily order by Id", new { }, conn) ?? [];
        var work = rows.FirstOrDefault();
        var sat  = rows.FirstOrDefault(r => r.DutyType == "RN");
        var sun  = rows.FirstOrDefault(r => r.DutyType == "RD");

        string wType = work?.DutyType ?? "RD";
        int    wIn   = work?.PIn      ?? 0;
        int    wLen  = work?.Duration ?? 0;

        return new AtttemplateModel
        {
            EmpmasId         = empmasId,
            AttendancetypeId = 1,

            D1_dutytype = sun?.DutyType ?? "RD", D1_in = sun?.PIn ?? 0, D1_hrslength = sun?.Duration ?? 0,
            D2_dutytype = wType, D2_in = wIn, D2_hrslength = wLen,
            D3_dutytype = wType, D3_in = wIn, D3_hrslength = wLen,
            D4_dutytype = wType, D4_in = wIn, D4_hrslength = wLen,
            D5_dutytype = wType, D5_in = wIn, D5_hrslength = wLen,
            D6_dutytype = wType, D6_in = wIn, D6_hrslength = wLen,
            D7_dutytype = sat?.DutyType ?? "RD", D7_in = sat?.PIn ?? 0, D7_hrslength = sat?.Duration ?? 0,
        };
    }

    public async Task<AtttemplateModel?> _03(int? id,AtttemplateModel atttemplate, string? schema, string? conn)
	{
		string sql = $@"Update {schema}.Atttemplate set 
                            EmpmasId                = @EmpmasId, 
                            AttendanceTypeId        = @AttendanceTypeId, 
                            D1_In                   = @D1_In, D1_HrsLength = @D1_HrsLength,
                            D1_DutyType             = @D1_DutyType,
                            D2_In                   = @D2_In,
                            D2_HrsLength            = @D2_HrsLength,
                            D2_DutyType             = @D2_DutyType,
                            D3_In                   = @D3_In,
                            D3_HrsLength            = @D3_HrsLength,
                            D3_DutyType             = @D3_DutyType,
                            D4_In                   = @D4_In,
                            D4_HrsLength            = @D4_HrsLength,
                            D4_DutyType             = @D4_DutyType,
                            D5_In                   = @D5_In,
                            D5_HrsLength            = @D5_HrsLength,
                            D5_DutyType             = @D5_DutyType,
                            D6_In                   = @D6_In,
                            D6_HrsLength            = @D6_HrsLength,
                            D6_DutyType             = @D6_DutyType,
                            D7_In                   = @D7_In,
                            D7_HrsLength            = @D7_HrsLength,
                            D7_DutyType             = @D7_DutyType 
                        where EmpmasId = @Id;"; 
		await _sql.ExecuteCmd<dynamic>(sql, atttemplate, conn);
		sql = $@" select  * from {schema}.Atttemplate x where x.Id = @Id ;";
		var data = await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { Id = id }, conn);
		return data?.FirstOrDefault();

	}

	public async Task<AtttemplateModel?> _04(int? id, string? schema, string? conn)
	{
		string sql = $@"Delete    from {schema}.Atttemplate where EmpmasId = @Id;
                        select  * from {schema}.Atttemplate where EmpmasId = @Id;";
		var data = await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { Id = id }, conn);
		return data?.FirstOrDefault();
	}

}