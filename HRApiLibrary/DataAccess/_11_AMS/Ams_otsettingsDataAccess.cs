using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._11_AMS;

namespace HRApiLibrary.DataAccess._11_AMS;

public class Ams_otsettingsDataAccess : IAms_otsettingsDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public Ams_otsettingsDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01(Ams_otsettingsModel ams_otsettings, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Ams_otsettings 
                            (Empnumber, NeedsOTFiling, MaxOTHour) values 
                            (@Empnumber, @NeedsOTFiling, @MaxOTHour) 
                        on duplicate key update NeedsOTFiling = @NeedsOTFiling, MaxOTHour = @MaxOTHour ";
        await _sql.ExecuteCmd<dynamic>(sql, ams_otsettings, conn);
    }


    public async Task<Ams_otsettingsModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Empnumber, NeedsOTFiling, MaxOTHour from {schema}.Ams_otsettings where Id = @Id";
        var data = await _sql.FetchData<Ams_otsettingsModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<Ams_otsettingsModel?>?> _02sByPayrollgrpid(int payrollgrpid, string pisdb, string opisdb, string conn)
    {
        string sql = $@" SELECT e.Empnumber, COALESCE(o.NeedsOTFiling, 0) AS NeedsOTFiling, COALESCE(o.MaxOTHour, 4) AS MaxOTHour,
                            CONCAT(
                                TRIM(COALESCE(e.EmpLastNm, '')), ', ',
                                TRIM(COALESCE(e.EmpFirstNm, '')), ' ',
                                TRIM(COALESCE(e.EmpMidNm, ''))) AS EmpName
                            FROM {opisdb}.Deprec d
                            LEFT JOIN {opisdb}.Empmas        e  ON e.Empnumber  = d.Empnumber
                            LEFT JOIN {pisdb}.Ams_otsettings o  ON o.EmpNumber = e.empnumber
                            WHERE d.PayrollgrpId = @PayrollgrpId
                            ORDER BY e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm";
        var data = await _sql.FetchData<Ams_otsettingsModel?, dynamic>(sql, new { PayrollgrpId = payrollgrpid }, conn);
        return data;
    }



    public async Task<Ams_otsettingsModel?> _03(int id, Ams_otsettingsModel ams_otsettings, string schema, string conn)
    {
        string sql = $@"Update {schema}.Ams_otsettings set Empnumber = @Empnumber, NeedsOTFiling = @NeedsOTFiling, MaxOTHour = @MaxOTHour where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, ams_otsettings, conn);

        sql = $@" select  * from {schema}.Ams_otsettings x where x.Id = @Id ;";
        var data = await _sql.FetchData<Ams_otsettingsModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<Ams_otsettingsModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Ams_otsettings where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Ams_otsettings x where x.Id = @Id ;";
        var data = await _sql.FetchData<Ams_otsettingsModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IAms_otsettingsDataAccess
{
    Task _01(Ams_otsettingsModel ams_otsettings, string schema, string conn);
    Task<Ams_otsettingsModel?> _02(int id, string schema, string conn);
    Task<List<Ams_otsettingsModel?>?> _02sByPayrollgrpid(int payrollgrpid, string pisdb, string opisdb, string conn);
    Task<Ams_otsettingsModel?> _03(int id, Ams_otsettingsModel ams_otsettings, string schema, string conn);
    Task<Ams_otsettingsModel?> _04(int id, string schema, string conn);
}