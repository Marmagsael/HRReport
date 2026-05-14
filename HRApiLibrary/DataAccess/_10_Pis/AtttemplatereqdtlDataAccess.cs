using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class AtttemplatereqdtlDataAccess : IAtttemplatereqdtlDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AtttemplatereqdtlDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01(AtttemplatereqdtlModel atttemplatereqdtl, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Atttemplatereqdtl 
							(AtttemplateReqHdrId, EmpmasId, AttendanceTypeId, D1_In, D1_HrsLength, 
							 D1_DutyType, D2_In, D2_HrsLength, D2_DutyType, D3_In, D3_HrsLength, D3_DutyType, 
							 D4_In, D4_HrsLength, D4_DutyType, D5_In, D5_HrsLength, D5_DutyType, D6_In, D6_HrsLength, D6_DutyType, 
							 D7_In, D7_HrsLength, D7_DutyType) values 
							(@AtttemplateReqHdrId, @EmpmasId, @AttendanceTypeId, @D1_In, @D1_HrsLength, 
							 @D1_DutyType, @D2_In, @D2_HrsLength, @D2_DutyType, @D3_In, @D3_HrsLength, @D3_DutyType, 
							 @D4_In, @D4_HrsLength, @D4_DutyType, @D5_In, @D5_HrsLength, @D5_DutyType, @D6_In, @D6_HrsLength, @D6_DutyType, 
							 @D7_In, @D7_HrsLength, @D7_DutyType)";
        await _sql.ExecuteCmd<dynamic>(sql, atttemplatereqdtl, conn);
    }


    public async Task<List<AtttemplatereqdtlModel?>?> _02ByAtttemplateReqHdrId(int atttemplateReqHdrId, string schema, string conn)
    {
        string sql = $@"select * from {schema}.Atttemplatereqdtl where AtttemplateReqHdrId = @AtttemplateReqHdrId";
        var data = await _sql.FetchData<AtttemplatereqdtlModel?, dynamic>(sql, new { AtttemplateReqHdrId = atttemplateReqHdrId }, conn);
        return data;
    }


    public async Task<List<AtttemplatereqdtlModel?>?> _03ByAtttemplateReqHdrId(int atttemplateReqHdrId, AtttemplatereqdtlModel atttemplatereqdtl, string schema, string conn)
    {
        string sql = $@"Update {schema}.Atttemplatereqdtl set 
							AtttemplateReqHdrId = @AtttemplateReqHdrId, 
							EmpmasId 			= @EmpmasId, 
							AttendanceTypeId 	= @AttendanceTypeId, 
							D1_In 				= @D1_In, 
							D1_HrsLength 		= @D1_HrsLength, 
							D1_DutyType 		= @D1_DutyType, 
							D2_In 				= @D2_In, 
							D2_HrsLength 		= @D2_HrsLength, 
							D2_DutyType 		= @D2_DutyType, 
							D3_In 				= @D3_In, 
							D3_HrsLength 		= @D3_HrsLength, 
							D3_DutyType 		= @D3_DutyType, 
							D4_In 				= @D4_In, 
							D4_HrsLength 		= @D4_HrsLength, 
							D4_DutyType 		= @D4_DutyType, 
							D5_In 				= @D5_In, 
							D5_HrsLength 		= @D5_HrsLength, 
							D5_DutyType 		= @D5_DutyType, 
							D6_In 				= @D6_In, 
							D6_HrsLength 		= @D6_HrsLength, 
							D6_DutyType 		= @D6_DutyType, 
							D7_In 				= @D7_In, 
							D7_HrsLength 		= @D7_HrsLength, 
							D7_DutyType 		= @D7_DutyType where AtttemplateReqHdrId = @AtttemplateReqHdrId;
						Select  * from {schema}.Atttemplatereqdtl x where x.AtttemplateReqHdrId = @AtttemplateReqHdrId ;";
        var data = await _sql.FetchData<AtttemplatereqdtlModel?, dynamic>(sql, atttemplatereqdtl, conn);
        return data;
    }

    public async Task _04(int atttemplateReqHdrId, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Atttemplatereqdtl where AtttemplateReqHdrId = @AtttemplateReqHdrId;";
        await _sql.ExecuteCmd<dynamic>(sql, new { AtttemplateReqHdrId = atttemplateReqHdrId }, conn);

    }

}

public interface IAtttemplatereqdtlDataAccess
{
    Task _01(AtttemplatereqdtlModel atttemplatereqdtl, string schema, string conn);
    Task<List<AtttemplatereqdtlModel?>?> _02ByAtttemplateReqHdrId(int atttemplateReqHdrId, string schema, string conn);
    Task<List<AtttemplatereqdtlModel?>?> _03ByAtttemplateReqHdrId(int atttemplateReqHdrId, AtttemplatereqdtlModel atttemplatereqdtl, string schema, string conn);
    Task _04(int atttemplateReqHdrId, string schema, string conn);
}