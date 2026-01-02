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

	public async Task<AtttemplateModel?> _01(AtttemplateModel atttemplate, string schema, string conn )
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
                        SELECT * FROM {schema}.Atttemplate WHERE EmpmasID = @Empmasid;    " ; 
		var res = await _sql.FetchData<AtttemplateModel?,dynamic>(sql,atttemplate,conn);
		return res.FirstOrDefault();
	}

	
	public async Task<AtttemplateModel?> _02(int id, string schema, string conn)
	{
		string sql = $@"select  * from {schema}.Atttemplate where EmpmasId = @Id" ; 
		var data = await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { Id = id }, conn); 
		return data?.FirstOrDefault();
	}

    public AtttemplateModel? _02NoSchedule(int empmasId)
    {
        AtttemplateModel at = new()
        {
	        Empmasid = empmasId, 
	        
	        AttendancetypeId = 1,
	        
	        D1_dutytype 	= "RD", 
	        D2_dutytype 	= "RD", 
	        D3_dutytype 	= "RD", 
	        D4_dutytype 	= "RD", 
	        D5_dutytype 	= "RD", 
	        D6_dutytype 	= "RD", 
	        D7_dutytype 	= "RD",
	        
	        D1_hrslength 	= 0, 
	        D2_hrslength 	= 0, 
	        D3_hrslength 	= 0, 
	        D4_hrslength 	= 0, 
	        D5_hrslength 	= 0,
	        D6_hrslength 	= 0, 
	        D7_hrslength 	= 0,
	        
	        D1_in 			= 0, 
	        D2_in 			= 0, 
	        D3_in 			= 0, 
	        D4_in 			= 0, 
	        D5_in 			= 0, 
	        D6_in 			= 0, 
	        D7_in 			= 0, 
        };

        return at;
    }

    public async Task<AtttemplateModel?> _03(int id,AtttemplateModel atttemplate, string schema, string conn)
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

	public async Task<AtttemplateModel?> _04(int id, string schema, string conn)
	{
		string sql = $@"Delete    from {schema}.Atttemplate where EmpmasId = @Id;
                        select  * from {schema}.Atttemplate where EmpmasId = @Id;";
		var data = await _sql.FetchData<AtttemplateModel?, dynamic>(sql, new { Id = id }, conn);
		return data?.FirstOrDefault();
	}

}