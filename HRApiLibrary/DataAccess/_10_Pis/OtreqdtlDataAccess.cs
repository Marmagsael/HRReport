using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class OtreqdtlDataAccess : IOtreqdtlDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;
    public OtreqdtlDataAccess(I_90_001_MySqlDataAccess sql)    { _sql = sql; }

    public async Task<OtreqdtlModel?> _01(OtreqdtlModel otreqdtl, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Otreqdtl 
							(OtReqHdrId,  DStart,  DEnd,  TotHrs,  DutyTypeId,  DayTypeId) values 
							(@OtReqHdrId, @DStart, @DEnd, @TotHrs, @DutyTypeId, @DayTypeId)";
        await _sql.ExecuteCmd<dynamic>(sql, otreqdtl, conn);
        sql = $@"SELECT * FROM {schema}.Otreqdtl WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<OtreqdtlModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }

    public async Task<OtreqdtlModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  d.*, d1.Name DutyTypeName, d2.Name DayTypeName,  
						from {schema}.Otreqdtl d
						left join {schema}.OtDutyType 	d1 on d1.Id = d.OtDutyTypeId 
						left join {schema}.OtDayType 	d2 on d2.Id = d.OtDayTypeId  
						where d.Id = @Id";
        var data = await _sql.FetchData<OtreqdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OtreqdtlModel?> _02ByOtReqHdrId(int otReqHdrId, string schema, string conn)
    {
        string sql = $@"select  d.*, d1.Name DutyTypeName, d2.Name DayTypeName,  
						from {schema}.Otreqdtl d
						left join {schema}.OtDutyType 	d1 on d1.Id = d.OtDutyTypeId 
						left join {schema}.OtDayType 	d2 on d2.Id = d.OtDayTypeId  
						where d.OtReqHdrId = @OtReqHdrId";
        var data = await _sql.FetchData<OtreqdtlModel?, dynamic>(sql, new { OtReqHdrId = otReqHdrId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OtreqdtlModel?> _03(int id, OtreqdtlModel otreqdtl, string schema, string conn)
    {
        string sql = $@"Update {schema}.Otreqdtl set 
							OtReqHdrId 	= @OtReqHdrId, 
							DStart 		= @DStart, 
							DEnd 		= @DEnd, 
							TotHrs 		= @TotHrs, 
							DutyTypeId 	= @DutyTypeId, 
							DayTypeId 	= @DayTypeId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, otreqdtl, conn);

        sql = $@" select  * from {schema}.Otreqdtl x where x.Id = @Id ;";
        var data = await _sql.FetchData<OtreqdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Otreqdtl where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);
    }

    public async Task _04ByOtReqHdrId(int otReqHdrId, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Otreqdtl where OtReqHdrId = @OtReqHdrId;";
        await _sql.ExecuteCmd<dynamic>(sql, new { OtReqHdrId = otReqHdrId }, conn);
    }

}

public interface IOtreqdtlDataAccess
{
    Task<OtreqdtlModel?> _01(OtreqdtlModel otreqdtl, string schema, string conn);
    Task<OtreqdtlModel?> _02(int id, string schema, string conn);
    Task<OtreqdtlModel?> _02ByOtReqHdrId(int otReqHdrId, string schema, string conn);
    Task<OtreqdtlModel?> _03(int id, OtreqdtlModel otreqdtl, string schema, string conn);
    Task _04(int id, string schema, string conn);
    Task _04ByOtReqHdrId(int otReqHdrId, string schema, string conn);
}
