using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class EmprateshistDataAccess : IEmprateshistDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EmprateshistDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<EmprateshistModel?> _01(EmprateshistModel emprateshist, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Emprateshist 
                            (EmpmasId,  EmpNumber,  PayrollgrpId,  UsePaygrpRates,  EmpRate,  PayRateId,  RatePerHr,  RatePerDay,  RatePerMonth,  RatePerYr,  Created,  UserId,  Action) values 
                            (@EmpmasId, @EmpNumber, @PayrollgrpId, @UsePaygrpRates, @EmpRate, @PayRateId, @RatePerHr, @RatePerDay, @RatePerMonth, @RatePerYr, now(),    @UserId, @Action); 
                        SELECT * FROM {schema}.Emprateshist WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<EmprateshistModel?, dynamic>(sql, emprateshist, conn);

        return res.FirstOrDefault();
    }


    public async Task<EmprateshistModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Emprateshist where Id = @Id";
        var data = await _sql.FetchData<EmprateshistModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<EmprateshistModel?> _03(int id, EmprateshistModel emprateshist, string schema, string conn)
    {
        string sql = $@"Update {schema}.Emprateshist set 
                            EmpmasId        = @EmpmasId,  
                            EmpNumber       = @EmpNumber,  
                            PayrollgrpId    = @PayrollgrpId,  
                            UsePaygrpRates  = @UsePaygrpRates,  
                            RatePerHr       = @RatePerHr,  
                            RatePerDay      = @RatePerDay,  
                            RatePerMonth    = @RatePerMonth,  
                            RatePerYr       = @RatePerYr,  
                            Created         = @Created,  
                            UserId          = @UserId,  
                            Action          = @Action where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, emprateshist, conn);
        sql = $@" select  * from {schema}.Emprateshist x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmprateshistModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmprateshistModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Emprateshist where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Emprateshist x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmprateshistModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}