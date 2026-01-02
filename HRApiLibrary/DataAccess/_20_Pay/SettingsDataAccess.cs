using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class SettingsDataAccess : ISettingsDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public SettingsDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<SettingsModel?> _02(int id, string schema, string conn)
    {
        /*string sql = $@"select  * from {schema}.Settings where Id = @Id";
        var data = await _sql.FetchData<SettingsModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();*/
        var sql = "select  * from Settings where Id = @Id";
        var data = await _sql.FetchData<SettingsModel?, dynamic>(sql, new { Id = id },schema, conn);
        return data?.FirstOrDefault();
        
    }


    public async Task<SettingsModel?> _03(int id, SettingsModel settings, string schema, string conn)
    {
        var sql = $"""
                   Update {schema}.Settings set 
                         Yeartodays         = @Yeartodays,
                         Semiannualtodays   = @Semiannualtodays, 
                         Monthtodays        = @Monthtodays, 
                         SemiMonthtodays    = @SemiMonthtodays, 
                         DaysPerWeek        = @DaysPerWeek, 
                         Daytohours         = @Daytohours, 
                         NDStart            = @NDStart, 
                         NDEnd              = @NDEnd, 
                         PayrollType        = @PayrollType, 
                         TaxPeriodCode      = @TaxPeriodCode, 
                         AllowedMoPrd       = @AllowedMoPrd, 
                         CoShortName        = @CoShortName, 
                         CoFullName         = @CoFullName, 
                         CoAddress          = @CoAddress, 
                         CoContactNos       = @CoContactNos, 
                         RegNo              = @RegNo, 
                         TIN                = @TIN, 
                         SSSNo              = @SSSNo, 
                         PhicNo             = @PhicNo, 
                         PagibigNo          = @PagibigNo, 
                         PremContSourceId   = @PremContSourceId where Id = @Id;
                   """;
        await _sql.ExecuteCmd<dynamic>(sql, settings, conn);

        sql = $@" select  * from {schema}.Settings x where x.Id = @Id ;";
        var data = await _sql.FetchData<SettingsModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();

    }

}