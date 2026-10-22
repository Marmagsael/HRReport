using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class ObapplicationdtlDataAccess : IObapplicationdtlDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public ObapplicationdtlDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01(ObapplicationdtlModel obapplicationdtl, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.ObApplicationDtl
                            (ObApplicationId,  EmpmasId,  Date,  DutyType,  ObDurationId,  IsPayable) values
                            (@ObApplicationId, @EmpmasId, @Date, @DutyType, @ObDurationId, @IsPayable); ";
        await _sql.ExecuteCmd<dynamic>(sql, obapplicationdtl, conn);
    }

    public async Task<ObapplicationdtlModel?> _02(int? id, string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.ObApplicationDtl where Id = @Id";
        var data = await _sql.FetchData<ObapplicationdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<ObapplicationdtlModel?>?> _02ByObApplicationId(int? obApplicationId, string? schema, string? conn)
    {
        string? sql = $@"select  *
                        from {schema}.ObApplicationDtl
                        where ObApplicationId = @ObApplicationId
                        order by Date ";
        var data = await _sql.FetchData<ObapplicationdtlModel?, dynamic>(sql, new { ObApplicationId = obApplicationId }, conn);
        return data;
    }

    public async Task<ObapplicationdtlModel?> _03(int? id, ObapplicationdtlModel obapplicationdtl, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.ObApplicationDtl set
                            Date            = @Date,
                            DutyType        = @DutyType,
                            ObDurationId    = @ObDurationId,
                            IsPayable       = @IsPayable
                        where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, obapplicationdtl, conn);

        sql = $@" select  * from {schema}.ObApplicationDtl x where x.Id = @Id ;";
        var data = await _sql.FetchData<ObapplicationdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<ObapplicationdtlModel?> _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.ObApplicationDtl where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.ObApplicationDtl x where x.Id = @Id ;";
        var data = await _sql.FetchData<ObapplicationdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // --- used on re-save : clear the old rows then insert the current ObStart..ObEnd rows
    public async Task _04ByObApplicationId(int? obApplicationId, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.ObApplicationDtl where ObApplicationId = @ObApplicationId;";
        await _sql.ExecuteCmd<dynamic>(sql, new { ObApplicationId = obApplicationId }, conn);
    }

}


public interface IObapplicationdtlDataAccess
{
    Task                                    _01(ObapplicationdtlModel obapplicationdtl, string? schema, string? conn);
    Task<ObapplicationdtlModel?>            _02(int? id, string? schema, string? conn);
    Task<List<ObapplicationdtlModel?>?>     _02ByObApplicationId(int? obApplicationId, string? schema, string? conn);
    Task<ObapplicationdtlModel?>            _03(int? id, ObapplicationdtlModel obapplicationdtl, string? schema, string? conn);
    Task<ObapplicationdtlModel?>            _04(int? id, string? schema, string? conn);
    Task                                    _04ByObApplicationId(int? obApplicationId, string? schema, string? conn);
}
