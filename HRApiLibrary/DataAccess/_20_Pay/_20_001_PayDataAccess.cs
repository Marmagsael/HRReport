using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class _20_001_PayDataAccess : I_20_001_PayDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _20_001_PayDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01Payroll(PaymainhdrModel phdr, 
                                 List<TbltranModel?> tmptbltrans, 
                                 string schema, string conn)
    {
        if(phdr==null) return;

        var trn         = phdr.Trn;
        string sql      = $@" Select * {schema}.Paymainhdr where Trn = @Trn;";
        var paymainhdr  = await _sql.FetchData<PaymainhdrModel, dynamic>(sql, new { Trn = trn }, conn);
        if (paymainhdr != null) return; 

        //--- Create Paymainhdr -----------------------------------------------------------------------
        sql = $@"Insert into {schema}.Paymainhdr 
							(Trn,  ClRate,  MinRate,  UserId,  Status,  DateCreated,  DatePosted,  AttStart,  AttEnd) values 
							(@Trn, @ClRate, @MinRate, @UserId, @Status, @DateCreated, @DatePosted, @AttStart, @AttEnd)
                on duplicate key update Trn = @Trn; ";
        await _sql.ExecuteCmd<dynamic>(sql, phdr, conn);

        //--- Create tmpTblTran --------------------------------------------------------
        foreach (var t in tmptbltrans)
        {   
            if(t != null)   {
                sql = $@"Insert into {schema}.tmptbltran 
                            (TRN,  EmpmasId,  empNumber,  acctNumber,  Qty,  Rate,  RateTypeId,  amount,  dTimeStamp,  postedby) values 
                            (@TRN, @EmpmasId, @empNumber, @acctNumber, @Qty, @Rate, @RateTypeId, @amount, @dTimeStamp, @postedby);";
                await _sql.ExecuteCmd<dynamic>(sql, t, conn); 
            }
        }
    }

    public async Task _01EmpmasAndEmpRates(EmpratesModel er, string pisdb, string paydb, string conn) {
        
        if(er==null) return;
        
        string sql;
        var empNumber = er.EmpNumber;
        if (empNumber?.Length > 0)
        {
            sql = $@"select * from {pisdb}.Empmas where EmpNumber = @EmpNumber;";
            var qEmpmas = await _sql.FetchData<EmpmasModel, dynamic>(sql, new { EmpNumber = empNumber });
            if (qEmpmas.Count > 0 ) er.EmpmasId = qEmpmas.First().Id;
            else
            {
                sql  = @$"Insert into {pisdb}.Empmas 
                            (EmpNumber,  EmpLastNm,  EmpFirstNm,  EmpMidNm,  Suffix) values 
                            (@EmpNumber, @EmpLastNm, @EmpFirstNm, @EmpMidNm, @Suffix); 
                        select * from {pisdb}.Empmas where Id  = (SELECT @@IDENTITY)";
                var results = await _sql.FetchData<EmpmasModel, dynamic>(sql, er, conn); 
                var res     = results.FirstOrDefault();
                if (res == null) return;
                er.EmpmasId = res.Id;
            }
        }

        sql = @$"Insert into {paydb}.emprates
                            (EmpmasId,   EmpNumber,   PayrollgrpId,  EmpRate,  PayrateId,  UsePaygrpRates,
                             RatePerHr,  RatePerDay,  RatePerMonth,  RatePerYr) values
                            (@EmpmasId,  @EmpNumber,  @PayrollgrpId, @EmpRate, @PayrateId, @UsePaygrpRates,
                             @RatePerHr, @RatePerDay, @RatePerMonth, @RatePerYr) 
                 on duplicate key update 
                        EmpRate         = @EmpRate,  
                        PayrateId       = @PayrateId,  
                        UsePaygrpRates  = @UsePaygrpRates,  
                        RatePerHr       = @RatePerHr,  
                        RatePerDay      = @RatePerDay,  
                        RatePerMonth    = @RatePerMonth,  
                        RatePerYr       = @RatePerYr; 

                insert into {paydb}.Empratesdtl 
                    (EmpmasId,  PayrollGrpId,  AcctNumber, Rate,     PayrateId) values
                    (@EmpmasId, @PayrollGrpId, 'E001', @EmpRate, @PayrateId) 
                on duplicate key update Rate = @EmpRate, PayrateId = @PayrateId ; 

                insert into {paydb}.Empratesdtl 
                        (EmpmasId,  PayrollGrpId,  AcctNumber, Rate,                   PayrateId) 
                select   {er.EmpmasId}, {er.PayrollgrpId}, AcctNumber, RateOverBasic*{er.RatePerHr}, 1 
                From {paydb}.Coa 
                where AcctType = 'E' and HasRateOverBasic = 1 and Acctnumber != 'E001'
                on duplicate key update Rate = RateOverBasic*{er.EmpRate}, PayrateId = {er.PayRateId}; ";

        await _sql.ExecuteCmd<dynamic>(sql, er, conn);
    }

    public async Task _01TmptbltranEmpList(string trn, int payrollgrpId, int empmasId, string paydb, string conn)
    {
        double  emprate     = 0;
        int     payrateId   = 2;
        
        // --- Check if Employee has Basic Rates -------------------------------------------------------
        string sql = $@"Select * from {paydb}.Empratesdtl  
                            Where AcctNumber = 'E001' and EmpmasId=@EmpmasId and PayrollGrpId=@PayrollgrpId; ";
        var ress    = await _sql.FetchData<EmpratesdtlModel, dynamic>(sql, new {EmpmasId=empmasId,PayrollGrpId = payrollgrpId},conn);
        var res     = ress.FirstOrDefault();
        
        if (res != null)
        {
            emprate     = res.Rate;
            payrateId   = res.PayrateId;
        }   else
        {
            sql     = @$"select * from {paydb}.Payrollgrp  where Id = @Id";
            var pgs = await _sql.FetchData<PayrollgrpModel, dynamic>(sql, new { Id = payrollgrpId}, conn);
            if (pgs != null) emprate = pgs.FirstOrDefault()!.RatePerDay; 
        }
        
        sql = $@" Insert into {paydb}.Tmptbltranemplist 
                    (Trn,  EmpmasId, Emprate, PayrateId) values 
                    (@Trn, @EmpmasId, @Emprate, @PayrateId) 
                  on duplicate key update EmpmasId = @EmpmasId; ";
        await _sql.ExecuteCmd<dynamic>(sql, 
            new { Trn = trn, EmpmasId = empmasId, Emprate = emprate, PayrateId = payrateId },conn);

        // --- 
    }

    public async Task _01TmptbltranCoaList(string trn, string acctNumber, string paydb, string conn)
    {
        string sql = $@" Insert into {paydb}.Tmptbltrancoalist 
                         (trn, AcctNumber) values  (@trn, @AcctNumber) 
                         on duplicate key update AcctNumber = @AcctNumber; ";
        await _sql.ExecuteCmd<dynamic>(sql, new {Trn=trn,AcctNumber = acctNumber}, conn);
    }
    
    public async Task _01Empratesdtl_NewPayroll(int payrollgrpId, int empmasId, double rateHR, double rateDay, string paydb, string conn)
    {
        string sql  = $@" select * from {paydb}.Empratesdtl where PayrollgrpId = @PayrollgrpId and EmpmasId = @EmpmasId limit 1; ";
        var res     = await _sql.FetchData<EmpratesModel,dynamic>(sql, new { PayrollgrpId=payrollgrpId, EmpmasId=empmasId},conn);
        if(res.Count< 1) {
            sql = @$"INSERT INTO {paydb}.empratesdtl 
                        (EmpmasId, PayrollGrpId, AcctNumber, Rate, PayrateId)
                    select @EmpmasId, @PayrollGrpId, AcctNumber, 
                        if(AcctNumber='E001', @RateDay, @RateHr*RateOverBasic ) Rate, 
                        if(AcctNumber='E001', 2, 1) PayrateId 
                    From {paydb}.Coa Where AcctType = 'E' and RateOverBasic > 0 " ; 
            await _sql.ExecuteCmd<dynamic>(sql, new {PayrollgrpId=payrollgrpId, EmpmasId=empmasId, RateHr=rateHR, RateDay=rateDay}, conn);
        } 
        
    }

    
    public async Task<List<TbltranModel?>?> _02Tmptbltran(string trn, int payrollgrpId, int userId, string paydb, string pisdb, string conn)
    {
        string sql  = $@" select * from {paydb}.TmpTbltran where Trn = @Trn ";
        var res     = await _sql.FetchData<TbltranModel, dynamic>(sql, new {Trn=trn}, conn);
        if(res.Count<1)
        {
            sql = $@"INSERT INTO {paydb}.tmptbltran
                        (  TRN,  EmpmasId,   EmpNumber,   AcctNumber,   Qty, Rate,   RateTypeId,  amount, dTimeStamp, postedby)
                    select @Trn, t.EmpmasId, e.EmpNumber, t.AcctNumber, 1,   t.Rate, t.PayrateId, t.Rate, now(),      @UserId 
                    from {paydb}.Empratesdtl t
                    left join {pisdb}.Empmas e on e.Id = t.EmpmasId 
                    where t.AcctNumber in (select AcctNumber from {paydb}.TmpTblTranCoaList where Trn = @Trn) and 
                          t.PayrollgrpId = @PayrollgrpId and 
                          t.EmpmasId in (select EmpmasId from {paydb}.TmpTbltranEmpList where Trn = @Trn); ";
            await _sql.ExecuteCmd(sql, new {Trn=trn,PayrollgrpId=payrollgrpId, UserId=userId},conn); 
        }

        sql         = @$"Select t.Trn, t.EmpmasId, t.EmpNumber, t.AcctNumber, c.AcctName, t.Qty, t.Rate, t.RateTypeId, t.Amount 
                         from {paydb}.TmpTbltran t
                         Left join {paydb}.Coa c on c.AcctNumber = t.AcctNumber 
                         Where Trn = @Trn"; 
        var tbltran = await _sql.FetchData<TbltranModel?,dynamic>(sql, new {Trn=trn},conn); 
        return tbltran; 

    }

        public async Task<SettingsModel?> _02Settings(string schema, string conn)
    {
        string sql = $@" Select * from {schema}.Settings";
        var res = await _sql.FetchData<SettingsModel, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault(); 
    }
    
    public async Task<PaymaindtlsetupModel?> _02PaymainSetup(string schema, string conn)
    {
        string sql = $@" Select * from {schema}.Paymaindtlsetup";
        var paymaindtlsetup = await _sql.FetchData<PaymaindtlsetupModel, dynamic>(sql, new { }, conn);
        return paymaindtlsetup.FirstOrDefault(); 
    }

    public async Task _04ByTrn(string trn, string schema, string conn)
    {
        string sql      = $@" Select * from {schema}.Paymainhdr where Trn = @Trn;";
        var paymainhdr  = await _sql.FetchData<PaymainhdrModel, dynamic>(sql, new { Trn = trn }, conn);

        if (paymainhdr.FirstOrDefault()?.Status == null) return;
        if (paymainhdr.FirstOrDefault()?.Status == "Lock") return;

        sql = @$"Delete from {schema}.PayMainHdr        where Trn = @Trn; 
                 Delete from {schema}.PayMainDtl        where Trn = @Trn; 
                 Delete from {schema}.TmpTblTran        where Trn = @Trn; 
                 Delete from {schema}.TmpTblEmpList     where Trn = @Trn; 
                 Delete from {schema}.TmpTblTranCoaList where Trn = @Trn; ";
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn }, conn);

    }
}