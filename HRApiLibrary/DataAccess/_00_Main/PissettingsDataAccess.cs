using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._00_Main;

public class PissettingsDataAccess : IPissettingsDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PissettingsDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }


    public async Task<PissettingsModel?> _02(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Pissettings limit 1 ";
        var data = await _sql.FetchData<PissettingsModel?, dynamic>(sql, new { }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<PissettingsModel?> _03(PissettingsModel pissettings, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Pissettingstrail (
                                LeaveYrImplementation, LeaveAnniversaryStart, LeaveAnniversaryEnd, UserId, Changed) 
                        select  LeaveYrImplementation, LeaveAnniversaryStart, LeaveAnniversaryEnd, @UserId, now() 
                            from {schema}.Pissettings ";

        await _sql.ExecuteCmd<dynamic>(sql, pissettings, conn);

        sql = $@"Update {schema}.Pissettings 
                            set LeaveYrImplementation   = @LeaveYrImplementation, 
                                LeaveAnniversaryStart   = @LeaveAnniversaryStart, 
                                LeaveAnniversaryEnd     = @LeaveAnniversaryEnd 
                            where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, pissettings, conn);

        sql = $@" select  * from {schema}.Pissettings x limit 1 ;";
        var data = await _sql.FetchData<PissettingsModel?, dynamic>(sql, new { }, conn);
        return data?.FirstOrDefault();
    }
    



}