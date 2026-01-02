using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;
using MySqlX.XDevAPI;

namespace HRApiLibrary.DataAccess._20_Pay;

public class EmpratesDataAccess : IEmpratesDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EmpratesDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }
    
    public async Task<EmpratesModel?> _01(EmpratesModel emprates, int userId, string paySchema, string pisSchema, string conn)
    {

        if(emprates.EmpmasId == 0 )
        {
            //--- Emp Number is empty ---------------------------------
            EmpmasInternalModel empmas = new()
            {
                EmpNumber   = emprates.EmpNumber,
                EmpLastNm   = emprates.EmpLastNm,
                EmpFirstNm  = emprates.EmpFirstNm,
                EmpMidNm    = emprates.EmpMidNm,
                EmpAlias    = emprates.EmpAlias,
                Suffix      = emprates.Suffix
            };
            
            string msql1 = $@"insert into {pisSchema}.empmas 
                                    (Id,  EmpNumber,  EmpLastNm,  EmpFirstNm,  EmpMidNm,  Suffix,  EmpAlias) values 
                                    (@Id, @EmpNumber, @EmpLastNm, @EmpFirstNm, @EmpMidNm, @Suffix, @EmpAlias); 
                            SELECT r.* FROM {pisSchema}.empmas r WHERE r.ID = (SELECT @@IDENTITY)";
            var empmasRes = await _sql.FetchData<EmpmasInternalModel?, dynamic>(msql1, empmas, conn);

            if (empmasRes != null && empmasRes.Count > 0) emprates.EmpmasId = empmasRes.FirstOrDefault()!.Id;

            //--- Employee Rates --------------------------------------------------------
            var empRates = new  EmpratesModel()
                                    {
                                        EmpmasId        = emprates.EmpmasId,
                                        EmpNumber       = emprates.EmpNumber,
                                        PayrollgrpId    = emprates.PayrollgrpId,
                                        UsePaygrpRates  = emprates.UsePaygrpRates,
                                        RatePerHr       = emprates.RatePerHr,
                                        RatePerDay      = emprates.RatePerDay,
                                        RatePerMonth    = emprates.RatePerMonth,
                                        RatePerYr       = emprates.RatePerYr,
                                        UserId          = userId, 
                                        EmpRate         = emprates.EmpRate,
                                        PayRateId       = emprates.PayRateId,   
                                    };
            var erRes=await _02(emprates.EmpmasId,paySchema, pisSchema, conn);
            
            EmpratesModel? newEmprates = new EmpratesModel(); 
            if (erRes == null)  {   newEmprates     =   await _01(empRates, paySchema, pisSchema, conn); }
            else                {   newEmprates     =   await _03(empRates, paySchema, conn);  }

            var empRatesHist = new  EmprateshistModel()
                                    {
                                        EmpmasId        = emprates.EmpmasId,
                                        EmpNumber       = emprates.EmpNumber,
                                        PayrollgrpId    = emprates.PayrollgrpId,
                                        UsePaygrpRates  = emprates.UsePaygrpRates,
                                        EmpRate         = emprates.EmpRate, 
                                        PayRateId       = emprates.EmpRateId,
                                        RatePerHr       = emprates.RatePerHr,
                                        RatePerDay      = emprates.RatePerDay,
                                        RatePerMonth    = emprates.RatePerMonth,
                                        RatePerYr       = emprates.RatePerYr,
                                        UserId          = userId
                                    };
        }

        var er = await _02(emprates.EmpmasId, paySchema, pisSchema, conn); 
        return er;        
    }

    public async Task<EmpratesModel?> _01(EmpratesModel emprates, string paySchema, string pisSchema, string conn)
    {
        string sql = $@"Insert into {paySchema}.Emprates 
                            (EmpmasId,  EmpNumber,  EmpRate,  PayRateId,  PayrollgrpId,  UsePaygrpRates,  RatePerHr,  RatePerDay,  RatePerMonth,  RatePerYr) values 
                            (@EmpmasId, @EmpNumber, @EmpRate, @PayRateId, @PayrollgrpId, @UsePaygrpRates, @RatePerHr, @RatePerDay, @RatePerMonth, @RatePerYr); 
                        SELECT r.*, pr.RateName FROM {paySchema}.Emprates r 
                            left join {paySchema}.PayRate pr on pr.Id = r.PayRateId 
                        WHERE r.EmpmasId = @EmpmasId and PayrollgrpId = @PayrollgrpId";
        var res = await _sql.FetchData<EmpratesModel?, dynamic>(sql, emprates, conn);
        var empmasId    = res.FirstOrDefault()!.EmpmasId; 

        var newER = await _02(empmasId,paySchema, pisSchema, conn);

        return newER;
    }

    public async Task<EmpratesModel?> _02(int empmasId, string paySchema, string pisSchema, string conn)
    {
        string sql = $@"select p.Name PayrollGrpName,   
                            e.SystemId, e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm, e.Suffix, e.EmpAlias, 
                            r.*, pr.RateName 
                        from {paySchema}.Emprates r 
                            left join {pisSchema}.empmas e on e.Id = r.EmpmasId 
                            left join {paySchema}.payrollgrp p on p.Id = r.payrollgrpId 
                            left join {paySchema}.PayRate   pr on pr.Id = r.PayRateId 
                        where r.EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpratesModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<EmpratesEmpCntPerPGModel?>?> _02EmpCnt_Per_PG(string paySchema, string conn)
    {
        string sql = $@"select PayrollgrpId, Count(*) Count 
                        from {paySchema}.Emprates r 
                        group by PayrollgrpId
                        order by PayrollgrpId";
        var data = await _sql.FetchData<EmpratesEmpCntPerPGModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    
    public async Task<List<EmpratesEmpCntPerPGModel?>?> _02EmpCnt_Per_Tbltran(string trn, string paySchema, string conn)
    {
        string sql = $@"select right(trn,5) PayrollgrpId, Count(*) Count
                        from {paySchema}.Tbltran t
                        where Left(Trn,6) = Left(@Trn,6)
                        group by PayrollgrpId
                        order by PayrollgrpId";
        var data = await _sql.FetchData<EmpratesEmpCntPerPGModel?, dynamic>(sql, new { Trn=trn }, conn);
        return data;
    }
    
    public async Task<List<EmpratesModel?>?> _02ByName(string name, string paySchema, string pisSchema, string conn)
    {
        string sql = $@"select p.Name PayrollGrpName,   
                            e.SystemId, e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm, e.Suffix, e.EmpAlias, 
                            concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName, 
                            r.*, pr.RateName 
                        from {paySchema}.Emprates r 
                            left join {pisSchema}.empmas e on e.Id = r.EmpmasId 
                            left join {paySchema}.payrollgrp p on p.Id = r.payrollgrpId 
                            left join {paySchema}.PayRate   pr on pr.Id = r.PayRateId 
                        where e.EmpLastNm like @Name or e.EmpFirstNm like @Name; " ;
        var data = await _sql.FetchData<EmpratesModel?, dynamic>(sql, new { Name = "%"+name.Trim()+"%"  }, conn);
        return data;
    }
    
    public async Task<EmpratesModel?> _02ByEmpNumber(string empnumber , string paySchema, string pisSchema, string conn)
    {
        string sql = $@"select  e.SystemId, e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm, e.Suffix, e.EmpAlias, 
                                concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName, 
                                r.*, pr.RateName, p.Name PayrollGrpName
                        from {paySchema}.Emprates r 
                            left join {pisSchema}.empmas        e  on e.Id  = r.EmpmasId 
                            left join {paySchema}.payrollgrp    p  on p.Id  = r.payrollgrpId 
                            left join {paySchema}.PayRate       pr on pr.Id = r.PayRateId 
                        where e.Empnumber = @Empnumber ";
        var data = await _sql.FetchData<EmpratesModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data.FirstOrDefault();
    }

    public async Task<List<EmpratesModel?>?> _02PerPG(int payrollgrpId, string paySchema, string pisSchema, string conn)
    {
        string sql = $@"select  
                            e.SystemId, e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm, e.Suffix, e.EmpAlias, 
                            concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName, 
                            r.*, pr.RateName, p.Name PayrollGrpName
                        from {paySchema}.Emprates r 
                            left join {pisSchema}.empmas        e  on e.Id        = r.EmpmasId 
                            left join {pisSchema}.deprec        d  on d.EmpmasId  = e.Id 
                            left join {paySchema}.payrollgrp    p  on p.Id        = d.payrollgrpId 
                            left join {paySchema}.PayRate       pr on pr.Id       = r.PayRateId 
                        where r.PayrollgrpId = @PayrollgrpId 
                        order by e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm";
        var data = await _sql.FetchData<EmpratesModel?, dynamic>(sql, new { PayrollgrpId = payrollgrpId }, conn);
        return data;
    }
    public async Task<List<EmpratesModel?>?> _02Deployed(int payrollgrpId, string paySchema, string pisSchema, string conn)
    {
        string sql = $@"select  
                            e.Id EmpmasId, e.SystemId, ifnull(e.EmpNumber,'') EmpNumber, 
                            e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm, e.Suffix, e.EmpAlias, 
                            concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName, 
                            r.PayrollgrpId, r.EmpRate, r.PayrateId, r.UsePaygrpRates, r.RatePerHr, r.RatePerDay, 
                            r.RatePerMonth, r.RatePerYr, 
                            pr.RateName, p.Name PayrollGrpName
                        from {pisSchema}.deprec d 
                            left join {pisSchema}.empmas        e  on e.Id        = d.EmpmasId 
                            left join {paySchema}.Emprates      r  on r.EmpmasId  = d.EmpmasId and r.PayrollGrpId = @PayrollgrpId  
                            left join {paySchema}.payrollgrp    p  on p.Id        = d.payrollgrpId 
                            left join {paySchema}.PayRate       pr on pr.Id       = r.PayRateId 
                        where d.PayrollgrpId = @PayrollgrpId 
                        order by e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm";
        var data = await _sql.FetchData<EmpratesModel?, dynamic>(sql, new { PayrollgrpId = payrollgrpId }, conn);
        return data;
    }
    
    public async Task<List<EmpratesModel?>?> _02NotInTmpTbltran(string trn, string paySchema, string pisSchema, string conn)
    {
        string sql = $@"select  concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ', trim(e.EmpMidNm), ' [ ',trim(ifnull(g.Name,'No Payroll Group')) ,' ]') FullName,
                            e.SystemId, e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm, e.Suffix, e.EmpAlias, 
                            r.*, pr.RateName, g.Name PayrollGrpName
                        from {paySchema}.Emprates r
                            left join {paySchema}.payrollgrp g on g.Id      = r.PayrollgrpId  
                            left join {pisSchema}.empmas     e on e.Id      = r.EmpmasId 
                            left join {paySchema}.PayRate    pr on pr.Id    = r.PayRateId 
                        where r.empmasId not in (select empmasId from {paySchema}.Tmptbltran where Trn = @Trn)  
                        order by e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm";
        var data = await _sql.FetchData<EmpratesModel?, dynamic>(sql, new { Trn = trn }, conn);
        return data;
    }

    public async Task<EmpratesModel?> _03(EmpratesModel emprates, string schema, string conn)
    {
        //int empmasId = emprates.EmpmasId;
        string sql = $@"Update {schema}.Emprates set 
                            PayrollgrpId    = @PayrollgrpId,  
                            UsePaygrpRates  = @UsePaygrpRates,  
                            RatePerHr       = @RatePerHr,  
                            RatePerDay      = @RatePerDay,  
                            RatePerMonth    = @RatePerMonth,  
                            RatePerYr       = @RatePerYr, 
                            EmpRate         = @EmpRate, 
                            PayRateId       = @PayRateId
                        where EmpmasId      = @EmpmasId and PayrollgrpId = @PayrollgrpId;
                        select  * from {schema}.Emprates x where x.EmpmasId = @EmpmasId ;";
        var data = await _sql.FetchData<EmpratesModel?, dynamic>(sql, emprates, conn);
        return data?.FirstOrDefault();
    }
    public async Task<EmpratesModel?> _03Rates(EmpratesModel emprates, string schema, string conn)
    {
        //int empmasId = emprates.EmpmasId;
        string sql = $@"Update {schema}.Emprates set 
                            RatePerHr       = @RatePerHr,  
                            RatePerDay      = @RatePerDay,  
                            RatePerMonth    = @RatePerMonth,  
                            RatePerYr       = @RatePerYr, 
                            EmpRate         = @EmpRate, 
                            PayRateId       = @PayRateId
                        where EmpmasId = @EmpmasId and PayrollgrpId = @PayrollgrpId;
                        select  * from {schema}.Emprates where EmpmasId = @EmpmasId and PayrollgrpId = @PayrollgrpId;";
        var data = await _sql.FetchData<EmpratesModel?, dynamic>(sql, emprates, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<EmpratesModel?> _04(int empmasId, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Emprates where EmpmasId = @EmpmasId;
                        select  * from {schema}.Emprates where EmpmasId = @EmpmasId;";
        var data = await _sql.FetchData<EmpratesModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task _04ByFK(int empmasId, int payrollgrpId, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Emprates    where EmpmasId = @EmpmasId and PayrollgrpId = @PayrollgrpId;
                        Delete from {schema}.Empratesdtl where EmpmasId = @EmpmasId and PayrollgrpId = @PayrollgrpId;";
        await _sql.ExecuteCmd<dynamic>(sql, new { EmpmasId = empmasId, PayrollgrpId = payrollgrpId }, conn);
    }


}
