using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class EmpratesdtlDataAccess : IEmpratesdtlDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EmpratesdtlDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }




    public async Task _01FromCOA(EmpratesdtlModel empratesdtl, double ratePerHr, double ratePerDay, string schema, string conn)
    {
        if (ratePerHr < 1 || ratePerDay < 1) return;

        var res = await _02ByEmpmasId(empratesdtl.EmpmasId, schema, conn);
        
        if (res == null || res.Count < 1 )
        {
            //--- Create the Account -----------------------------------------
            string sql = $@"insert into {schema}.Empratesdtl 
                                    (EmpmasId,  PayrollGrpId,  AcctNumber,  Rate,        PayrateId) values 
                                    (@EmpmasId, @PayrollGrpId, 'E001',      @RatePerDay, 2) ; 

                             insert into {schema}.Empratesdtl 
                                    (EmpmasId, PayrollGrpId,  AcctNumber, Rate,                       PayrateId) 
                             Select @EmpmasId, @PayrollGrpId, Acctnumber, RateOverBasic * @RatePerHr, 1 
                             from {schema}.Coa where AcctType = 'E' and RateOverBasic > 0  and AcctNumber != 'E001' ";

            await _sql.ExecuteCmd<dynamic>(sql, 
                            new {EmpmasId       = empratesdtl.EmpmasId, 
                                 PayrollgrpId   = empratesdtl.PayrollGrpId,
                                 RatePerDay     = ratePerDay, 
                                 RatePerHr      = ratePerHr}, 
                            conn);
        }
    }

    public async Task _01(EmpratesdtlModel empratesdtl, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empratesdtl 
                        (EmpmasId,  PayrollGrpId,  AcctNumber,  Rate,  PayrateId) values 
                        (@EmpmasId, @PayrollGrpId, @AcctNumber, @Rate, @PayrateId)";
        await _sql.ExecuteCmd<dynamic>(sql, empratesdtl, conn);

    }


    public async Task<List<EmpratesdtlModel?>?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  c.AcctName, c.RateOverBasic, ed.* 
                        from {schema}.Empratesdtl ed
                        left join {schema}.Coa c on c.AcctNumber = ed.AcctNumber 
                        where ed.EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpratesdtlModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }
    
    public async Task<List<EmpratesdtlModel?>?> _02ByEmpmasIdPayrollgrpId(int empmasId, int payrollgrpId, string schema, string conn)
    {
        string sql = $@"select  c.AcctName, c.RateOverBasic, ed.* 
                        from {schema}.Empratesdtl ed
                        left join {schema}.Coa c on c.AcctNumber = ed.AcctNumber 
                        where ed.EmpmasId = @EmpmasId and  PayrollgrpId = @PayrollgrpId";
        var data = await _sql.FetchData<EmpratesdtlModel?, dynamic>(sql, new { EmpmasId = empmasId, 
            PayrollgrpId = payrollgrpId }, conn);
        return data;
    }
    
    public async Task<List<EmpratesdtlModel?>?> _02ByPayrollgrpId(int payrollgrpId, string schema, string conn)
    {
        string sql = $@"select  c.AcctName, c.RateOverBasic, ed.* 
                        from {schema}.Empratesdtl ed
                        left join {schema}.Coa c on c.AcctNumber = ed.AcctNumber 
                        where ed.PayrollGrpId = @PayrollGrpId";
        var data = await _sql.FetchData<EmpratesdtlModel?, dynamic>(sql, new { PayrollGrpId = payrollgrpId }, conn);
        return data;
    }
    
    public async Task<EmpratesdtlModel?> _02ByPK(int empmasId, int payrollgrpId, string acctNumber, string schema, string conn)
    {
        string sql = $@"select  c.AcctName, ed.* 
                        from {schema}.Empratesdtl ed
                        left join {schema}.Coa c on c.AcctNumber = ed.AcctNumber 
                        where ed.EmpmasId = @EmpmasId and ed.PayrollgrpId=@PayrollgrpId and ed.AcctNumber=@AcctNumber";
        var data = await _sql.FetchData<EmpratesdtlModel?, dynamic>(sql,
                        new { EmpmasId = empmasId, PayrollgrpId = payrollgrpId, AcctNumber = acctNumber }, conn);
        return data.FirstOrDefault();
    }


    public async Task<EmpratesdtlModel?> _03(EmpratesdtlModel empratesdtl, string schema, string conn)
    {
        
        var sql = $@"Update {schema}.Empratesdtl set 
                            Rate        = @Rate, 
                            PayrateId   = @PayrateId 
                        where EmpmasId  = @EmpmasId and PayrollgrpId=@PayrollgrpId and AcctNumber=@AcctNumber;";
        await _sql.ExecuteCmd<dynamic>(sql, empratesdtl, conn);
        

        // --- Fetch data for output ( link to return ) -------------------------------------------------------------------------------
        var data = await _02ByPK(empratesdtl.EmpmasId, empratesdtl.PayrollGrpId, empratesdtl.AcctNumber!, schema, conn);
        
        if (empratesdtl.AcctNumber != "E001") return data;
        //---- Fetch Payroll group data 
        var er = empratesdtl;
        sql = $"select * from {schema}.Payrollgrp where Id = @Id ";

        var pgs = await _sql.FetchData<PayrollgrpModel, dynamic>(sql, new { Id = er.PayrateId },conn);
        var pg = pgs.FirstOrDefault();
        var settings =   await _sql.FetchData<SettingsModel, dynamic>($"select * from {schema}.Settings ", new {  }, conn);
        var setting = settings.FirstOrDefault();
        
        var rate = er.Rate;
        var ratePerDay = er.PayrateId switch
        {
            1 => rate * setting?.Daytohours ?? 0,
            3 => (rate / setting?.Monthtodays ?? 1) / 2,
            4 => (rate / setting?.Monthtodays ?? 1),
            5 or 6 => (rate / setting?.Yeartodays ?? 1) / 2,
            _ => rate
        };
        EmpratesModel? er1 = new()
        {
            EmpmasId       = er.EmpmasId, 
            PayrollgrpId   = er.PayrollGrpId, 
            UsePaygrpRates = false,
            EmpRate        = rate,
            PayRateId      = er.PayrateId,
            RatePerHr      = ratePerDay / setting?.Daytohours ?? 1,
            RatePerDay     = ratePerDay,
            RatePerMonth   = ratePerDay * setting?.Monthtodays ?? 1,
            RatePerYr      = ratePerDay * setting?.Yeartodays ?? 1,
        };

        sql = @$"Update {schema}.Emprates set PayrateId         = @PayrateId, 
                                              EmpRate           = @EmpRate, 
                                              RatePerHr         = @RatePerHr , 
                                              RatePerDay        = @RatePerDay, 
                                              RatePerMonth      = @RatePerMonth, 
                                              RatePerYr         = @RatePerYr
                 Where EmpmasId = @EmpmasId and PayrollgrpId    = @PayrollgrpId"; 
        await _sql.ExecuteCmd<dynamic>(sql, er1, conn);

        return data;
    }

    public async Task<EmpratesdtlModel?> _04(EmpratesdtlModel empratesdtl, string schema, string conn)
    {
        var sql = $"""
                   Delete from {schema}.Empratesdtl 
                   Where EmpmasId  = @EmpmasId and PayrollgrpId=@PayrollgrpId and AcctNumber=@AcctNumber;
                   """;
        await _sql.ExecuteCmd<dynamic>(sql, empratesdtl, conn);

        sql = $@" select  * from {schema}.Empratesdtl x where x.Id = @Id ;";
        var data = await _02ByPK(empratesdtl.EmpmasId, empratesdtl.PayrollGrpId, empratesdtl.AcctNumber!, schema, conn);

        
        return data;
    }
}