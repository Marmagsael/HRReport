using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class ObapplicationhistDataAccess : IObapplicationhistDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public ObapplicationhistDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // --- e.g. _01(oba.Id, Uc.EmpmasId, "Saved", ...)
    public async Task _01(int? obaId, int? empmasId, string? action, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.ObApplicationHist
                            (ObaId,  EmpmasId,  Date,  Action) values
                            (@ObaId, @EmpmasId, now(), @Action); ";
        await _sql.ExecuteCmd<dynamic>(sql, new { ObaId = obaId, EmpmasId = empmasId, Action = action }, conn);
    }

    public async Task<List<ObapplicationhistModel?>?> _02ByObaId(int? obaId, string? schema, string? conn)
    {
        string? sql = $@"select  h.*,
                                CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS EmpmasName
                        from {schema}.ObApplicationHist h
                            left join {schema}.Empmas e on e.Id = h.EmpmasId
                        where h.ObaId = @ObaId
                        order by h.Date, h.Id ";
        var data = await _sql.FetchData<ObapplicationhistModel?, dynamic>(sql, new { ObaId = obaId }, conn);
        return data;
    }

}


public interface IObapplicationhistDataAccess
{
    Task                                    _01(int? obaId, int? empmasId, string? action, string? schema, string? conn);
    Task<List<ObapplicationhistModel?>?>    _02ByObaId(int? obaId, string? schema, string? conn);
}
