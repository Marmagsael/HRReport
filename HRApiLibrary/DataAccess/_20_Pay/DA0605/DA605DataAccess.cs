using Blazorise;
using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._20_Pay.M0605;

namespace HRApiLibrary.DataAccess._20_Pay.DA0605;

public class Da605DataAccess : IDa605DataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public Da605DataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01CreatePayroll(Model605? m605)
    {
        if (m605?.Paymainhdr == null || m605.Conn == null) return;

        var trn   = m605.Trn;
        var paydb = m605.Paydb;
        var pisdb = m605.Pisdb;
        var conn  = m605.Conn;
        var payrollgrpId = m605.PayrollgrpId;
        var userId = m605.UserId;
        
        //--- 0) Security Check (Paymainhdr) --------------------------------------------------------------------------------------
        var sql = $@"select * from {paydb}.Paymainhdr where trn = @Trn";
        var res = await _sql.FetchData<PaymainhdrModel, dynamic>(sql, new { Trn = trn }, m605.Conn);
        if (res.Count > 0) return;
        
        //--- 1) Delete Tmp Data --------------------------------------------------------------------------------------------------
        sql = $"""
               Delete from {paydb}.TmpTblTran         where Trn = @Trn;
               Delete from {paydb}.TmpPaymainvisacct  where Trn = @Trn;
               Delete from {paydb}.tmptbltranemplist  where Trn = @Trn;
               """;
        await _sql.ExecuteCmd<dynamic>(sql!, new{Trn=trn}, conn);
        
        //--- 2) Paymainhdr ----------------------------------------------------------------------------------------
        sql = $"""
               Insert into {paydb}.Paymainhdr 
               		(Trn,  ClRate,  MinRate,  UserId,  Status,  DateCreated,  DatePosted,  AttStart,  AttEnd) values 
               		(@Trn, @ClRate, @MinRate, @UserId, @Status, @DateCreated, @DatePosted, @AttStart, @AttEnd); 
               """;
        await _sql.ExecuteCmd<dynamic>(sql!, m605.Paymainhdr, conn);

        //--- 3) PaymainVisAcct -----------------------------------------------------------------------------------
        sql = $"""
               Insert into {paydb}.TmpPaymainvisacct 
                   (Trn, AcctNumber ) values 
                   (@Trn, 'E001'), 
                   (@Trn, 'E003'); 
               """;
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn }, conn);

        //--- 4) TmpTbltranEmplist ----------------------------------------------------
        if (payrollgrpId > 0)
        {
            sql = $"""
                      insert into {paydb}.TmpTbltranEmplist
                            (Trn,         EmpmasId,   Emprate,                   PayrateId,                       PayrollgrpId) 
                      Select @Trn as Trn, d.EmpmasId, ifnull(r.Rate,0) emprate,  ifnull(r.PayRateId,0) PayrateId, ifnull(d.PayrollgrpId,0) PgId  
                      FROM {pisdb}.Deprec d
                      left join 
                          (select * from {paydb}.EmpRatesDtl where PayrollgrpId = @PayrollgrpId and AcctNumber = 'E001') r on r.EmpmasId = d.EmpmasId   
                      where d.PayrollgrpId = @PayrollgrpId; 
                     """;
            await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, PayrollgrpId = payrollgrpId , UserId = userId}, conn);
        }
        
        //--- 5) TmpTblTran ----------------------------------------------------
        if (payrollgrpId > 0)
        {
            sql = $"""
                    insert into {paydb}.TmpTbltran
                          (TRN,  EmpmasId,    empNumber,   acctNumber,   Qty,   Rate,   RateTypeId,                amount,   dTimeStamp,          postedby) 
                    Select @Trn, el.EmpmasId, e.EmpNumber, v.AcctNumber, 0 Qty, v.Rate, v.PayRateId as RateTypeId, 0 amount, now() as dTimeStamp, @UserId as PostedBy   
                    FROM {paydb}.TmpTbltranEmplist el  
                        left join {pisdb}.Empmas e on e.Id = el.EmpmasId 
                        left join 
                            ( select * from {paydb}.EmpRatesdtl 
                                       where acctnumber in (select acctNumber from {paydb}.TmpPaymainVisAcct where Trn = @Trn) and 
                                             PayrollgrpId = @PayrollgrpId ) v on v.EmpmasId = el.EmpmasId                                                        
                    WHERE el.Trn = @Trn and length(v.AcctNumber) > 1  ;
                   """;
            await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, PayrollgrpId = payrollgrpId , UserId = userId}, conn);
        }
    }

    public async Task<Model605> _01EmpByCurrPayrollgrp(string? trn, int? payrollgrpId, int? empmasId, string? empNumber, double rate, string? paydb, string? pisdb, string? conn, int? userId)
    {
        Model605 m605 = new();
        var sql = $"""
                      Insert into {paydb}.TmpTbltran (TRN,          EmpmasId,  empNumber,  acctNumber,   Qty,       Rate,                       RateTypeId, amount,   dTimeStamp, postedby) 
                      SELECT                          @Trn as Trn,  @EmpmasId, @EmpNumber, v.AcctNumber, 0 as Qty,  @Rate*c.RateOverBasic Rate, 2,          0 amount, now(),      @UserId as PostedBy  
                      FROM      `{paydb}`.`TmpPaymainVisAcct` v
                      Left join `{paydb}`.coa                 c on c.AcctNumber = v.AcctNumber
                       where Trn = @Trn;

                      Insert  into {paydb}.TmpTbltranEmplist (trn,  empmasId,  Emprate, PayrateId, PayrollgrpId) values 
                                                             (@Trn, @EmpmasId, @Rate,   2,         @PayrollgrpId); 
                                          
                   """; 
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, EmpmasId = empmasId, EmpNumber=empNumber, Rate=rate, UserId = userId, PayrollgrpId = payrollgrpId }, conn!);
        
        //--- 1) Tmptbltran -----------------------------------------------------------------------------------
        m605.Tmptbltrans = await _02Tmptbltran_ByTrn(trn??"", paydb, pisdb!, conn); 
        
        //--- 2) Tmptbltran Employee List -----------------------------------------------------------------------------------
        m605.TmptbltranEmpLists = await _02TmptbltranEmpList_ByTrn(trn??"", paydb, pisdb, conn);
        
        return m605;
    }

    public async Task<Model605> _01EmpByPayrollgrpRate(string? trn, int? payrollgrpId, int? empmasId, string? empNumber, int? userId, string? paydb,
        string? pisdb, string? conn)
    {
        Model605 m605 = new();
        var sql = $"select * from {paydb}.Payrollgrp where Id = @PayrollgrpId ";
        var pg = await _sql.FetchData<PayrollgrpModel,dynamic>(sql, new { PayrollgrpId = payrollgrpId }, conn!);

        var rate = pg.FirstOrDefault()?.RatePerDay??0; 
        sql = $"""
                      Insert into {paydb}.TmpTbltran (TRN,          EmpmasId,  empNumber,  acctNumber,   Qty,       Rate,                       RateTypeId                              , amount,   dTimeStamp, postedby) 
                      SELECT                          @Trn as Trn,  @EmpmasId, @EmpNumber, v.AcctNumber, 0 as Qty,  @Rate*c.RateOverBasic Rate, if((v.AcctNumber='E001'),2,1) RateTypeId, 0 amount, now(),      @UserId as PostedBy  
                      FROM      `{paydb}`.`TmpPaymainVisAcct` v
                      Left join `{paydb}`.coa                 c on c.AcctNumber = v.AcctNumber
                       where Trn = @Trn;

                      Insert  into {paydb}.TmpTbltranEmplist (trn,  empmasId,  Emprate, PayrateId, PayrollgrpId ) values 
                                                             (@Trn, @EmpmasId, @Rate,   2,         @PayrollgrpId); 
                                          
                   """; 
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, EmpmasId = empmasId, EmpNumber=empNumber, Rate=rate, UserId = userId, PayrollgrpId = payrollgrpId}, conn!);
        
        //--- 1) Tmptbltran -----------------------------------------------------------------------------------
        m605.Tmptbltrans = await _02Tmptbltran_ByTrn(trn??"", paydb, pisdb!, conn); 
        
        //--- 2) Tmptbltran Employee List -----------------------------------------------------------------------------------
        m605.TmptbltranEmpLists = await _02TmptbltranEmpList_ByTrn(trn??"", paydb, pisdb, conn);
        
        return m605; 
    }

    public async Task<Model605> _01EmpAssignedRates(string? trn, EmpratesModel empRates, int? userId, string? paydb, string? pisdb, string? conn)
    {
        Model605 m605 = new();
        
        var payrollgrpId = empRates.PayrollgrpId;
        var empmasId = empRates.EmpmasId;
        var empNumber = empRates.EmpNumber;

        var cmd = $"select * from {paydb}.Empratesdtl where payrollgrpId = @PayrollgrpId and EmpmasId = @EmpmasId and AcctNumber = 'E001'";
        var empRatesdtls = await _sql.FetchData<EmpratesdtlModel?, dynamic>(cmd, new { PayrollgrpId = payrollgrpId, EmpmasId = empmasId }, conn!);  
        var rates = empRatesdtls.FirstOrDefault()?.Rate??0;
        var rateTypeId = empRatesdtls.FirstOrDefault()?.PayrateId??0; 
        
        var sql = $"""
                      Insert into {paydb}.TmpTbltran (TRN,          EmpmasId,   empNumber,   acctNumber,   Qty,   Rate,   RateTypeId, amount,   dTimeStamp, postedby) 
                      SELECT                          @Trn as Trn,  r.EmpmasId, e.EmpNumber, r.AcctNumber, 0 Qty, r.Rate, PayrateId , 0 amount, now(),      @UserId as PostedBy  
                      FROM      `{paydb}`.`empratesdtl` r
                      left join  {pisdb}.Empmas e on e.Id = r.EmpmasId 
                      where r.EmpmasId = @EmpmasId and r.PayrollgrpId = @PayrollgrpId and r.AcctNumber in (select AcctNumber from {paydb}.tmppaymainvisacct where Trn =@Trn) ;

                      Insert  into {paydb}.TmpTbltranEmplist (trn,  empmasId,  Emprate, PayrateId, PayrollgrpId ) values 
                                                             (@Trn, @EmpmasId, @Rate,   2,         @PayrollgrpId); 
                                          
                   """; 
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, PayrollgrpId=payrollgrpId, EmpmasId = empmasId, Rate=rates, UserId = userId}, conn!);
        
        //--- 1) Tmptbltran -----------------------------------------------------------------------------------
        m605.Tmptbltrans = await _02Tmptbltran_ByTrn(trn??"", paydb, pisdb!, conn); 
        
        //--- 2) Tmptbltran Employee List -----------------------------------------------------------------------------------
        m605.TmptbltranEmpLists = await _02TmptbltranEmpList_ByTrn(trn??"", paydb, pisdb, conn);
        
        return m605;
    }
    

    public async Task<Model605> _01Acct_per_Trn(string? trn, string? acctNumber, double defPaygrpRate,  string? paydb, string? pisdb,  string? conn)
    {
        Model605 m605 = new();
        if(trn.Trim().Length < 1 || paydb?.Trim().Length < 1 || conn?.Trim().Length < 1 ) return m605;
        
        var sql = $"""
                        Insert into {paydb}.tmppaymainvisacct (Trn, AcctNumber) values (@Trn, Trn, AcctNumberAcctNumber);
                        Insert into {paydb}.TmpTbltran        
                                (TRN,         EmpmasId,   EmpNumber,   acctNumber,   Qty,   Rate,   RateTypeId, amount,   dTimeStamp, postedby) 
                        SELECT  @Trn as Trn,  r.EmpmasId, e.EmpNumber, r.AcctNumber, 0 Qty, r.Rate, PayrateId , 0 amount, now(),      @UserId as PostedBy  
                   FROM      `{paydb}`.`empratesdtl` r
                   left join  {pisdb}.Empmas e on e.Id = r.EmpmasId
                   where r.EmpmasId = @EmpmasId and r.PayrollgrpId = @PayrollgrpId and 
                         r.AcctNumber in (select AcctNumber from {paydb}.tmppaymainvisacct where Trn =@Trn) ;
                   """;
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, AcctNumber = acctNumber}, conn!); 
        
        //--- 1) Tmptbltran -----------------------------------------------------------------------------------
        m605.Tmptbltrans = await _02Tmptbltran_ByTrn(trn??"", paydb, pisdb!, conn); 
        
        //--- 2) Period Accounts ------------------------------------------------------------------------------
        m605.PrdAccts = await _02PrdAccts_ByTrn(trn??"", paydb, conn);
        
        return m605;
    }

    public async Task _01_FE_to_Tmptbltran(string? trn, string? fldName, string? paydb, string? pisdb, int? idUser, string? conn)
    {
        //--- Settings ---------------------------------------------------------------------------------
        var sql = $@"Select * from {paydb}.Settings";
        var ss  = await _sql.FetchData<SettingsModel, dynamic>(sql, new { }, conn!);
        var s = ss.FirstOrDefault();
        
        var yrToDay       = s?.Yeartodays; 
        var biAnnualToDay = s?.Semiannualtodays; 
        var moToDay       = s?.Monthtodays; 
        var biMoToDay     = s?.SemiMonthtodays; 
        var dayToHr       = s?.Daytohours;
        //=============================================================================================================


        //--- Delete Earning Accounts ---------------------------------------------------------------------------------
        sql = $@"delete from {paydb}.tmpTbltran where trn = @Trn and Source != '-'"; 
        await _sql.ExecuteCmd(sql, new { Trn=trn }, conn!); 
        //=============================================================================================================
        
        //--- Push Fixed Earnings sa Tmptbltran, stop from if no fixed earnings founc ---------------------------------
        sql = @$"select distinct fe.Acctnumber from {paydb}.fixedEarnings fe
                 where fe.{fldName} = 1 and fe.Status = 'A' and 
                       left(fe.trnPosted,6) != left(@Trn,6) and
                       EmpNumber in ( SELECT e.EmpNumber 
                                      FROM {paydb}.tmptbltranemplist l
                                      left join {pisdb}.Empmas e on e.Id = l.EmpmasId
                                      where l.trn = @Trn); ";
        var fes = await _sql.FetchData<FixedearningsModel, dynamic>(sql, new { Trn=trn }, conn!);
        
        if(fes.Count < 1) return;
        foreach (var f in fes)
        {
            var acctNumber = f.Acctnumber; 
            sql = $@"insert into {paydb}.tmptbltran (TRN,   EmpmasId,   EmpNumber,   acctNumber,     Qty, Rate, RateTypeId, Amount, dTimeStamp, postedby, Source) 
                        select                       t.Trn, t.EmpmasId, e.EmpNumber, '{acctNumber}', 0,   0,    0,          0,      now(),      {idUser}, 'FE' 
                            from {paydb}.tmptbltranemplist t 
                            left join  {pisdb}.Empmas e on e.Id = t.EmpmasId
                        where t.trn = @Trn 
                      on duplicate key update Source = 'FE'; 
                     insert into {paydb}.tmppaymainvisacct (trn, AcctNumber) values (@Trn, '{acctNumber}') on duplicate key update trn = @Trn; "; 
            await _sql.ExecuteCmd(sql, new { Trn=trn }, conn!);

           



        }
        //==============================================================================================================

        sql = $@"update {paydb}.Tmptbltran t
                    join (select t.Trn, t.EmpmasId, fe.EmpNumber, fe.AcctNumber,
                              if(fe.perdayEarnings=1,
                                case
                                  when t.RateTypeId = 1 then t.Qty / (select DayToHours       from {paydb}.settings limit 1)   /* hourly */
                                  when t.RateTypeId = 2 then t.Qty                                                             /* per day */
                                  when t.RateTypeId = 3 then t.Qty * (select DayToHours       from  {paydb}.settings limit 1)  /* bi-monthly */
                                  when t.RateTypeId = 4 then t.Qty * (select SemiMonthtodays  from  {paydb}.settings limit 1)  /* monthly */
                                  when t.RateTypeId = 5 then t.Qty * (select Semiannualtodays from  {paydb}.settings limit 1)  /* biAnnual */
                                  when t.RateTypeId = 6 then t.Qty * (select Yeartodays       from  {paydb}.settings limit 1)  /* Annual */
                                  else t.Qty
                                end
                              , 1) Qty,
                              fe.Amount Rate
                         from {paydb}.fixedEarnings fe
                        left join (select * from {paydb}.TmpTbltran t where t.trn = @Trn and t.Acctnumber = 'E001') t on t.EmpNumber = fe.EmpNumber
                        where fe.{fldName} = 1 and fe.Status = 'A' and
                               left(fe.trnPosted,6) != left(@Trn,6) and
                               fe.EmpNumber in ( SELECT e.EmpNumber
                                                 FROM {paydb}.tmptbltranemplist l
                                                 left join {pisdb}.Empmas e on e.Id = l.EmpmasId
                                                 where l.trn = @Trn)) 
                fe on fe.EmpmasId = t.EmpmasId and fe.AcctNumber = t.AcctNumber
                    set t.Qty = fe.Qty, t.Rate = fe.Rate 
                 where t.Source = 'FE' and t.Trn = @Trn; 

              update {paydb}.Tmptbltran set amount=qty*rate where Source = 'FE' and Trn = @Trn;";
        
        await _sql.ExecuteCmd(sql, new { Trn=trn }, conn!);
        
    }

    public async Task _01_FEG_to_Tmptbltran(string? trn, string? fldName, int? payrollgrpId, string? paydb, string? pisdb, int? idUser,
        SettingsModel s, string? conn)
    {
        //--- Test kung may Fixed Earnings Group na active  [ Exit pag wala ] ------------------------------------------
        var sql = @$"select distinct fe.AcctNumber from {paydb}.fixedEarnings_grp fe
                 where fe.{fldName} = 1 and fe.Status = 'A' and fe.PayrollgrpId = @PayrollgrpId ; ";
        var res = await _sql.FetchData<Fixedearnings_grpModel, dynamic>(sql, new { Trn=trn, PayrollgrpId=payrollgrpId }, conn!);
        
        if(res.Count<1) return;

        //--- Tingnan kung may employee na may fixed earnings group account  [ Exit pag wala ] -------------------------
        sql = @$"SELECT distinct g.AcctNumber FROM {paydb}.`fixedearnings_grp` g
                    left join {paydb}.`fixedearnings_grp_emp` ge on g.Id = ge.fixedearnings_grpId
                 where ge.EmpmasId in 
                       (SELECT EmpmasId FROM {paydb}.`tmptbltranemplist` where trn = @Trn) and  
                     g.{fldName} = 1 and g.Status = 'A' and g.PayrollgrpId = @PayrollgrpId ";
        var fegs = await _sql.FetchData<Fixedearnings_grpModel, dynamic>(sql, new { Trn=trn, PayrollgrpId=payrollgrpId }, conn!);
        
        if(res.Count<1) return;
        
        //--- Delete Earning Accounts ---------------------------------------------------------------------------------
        sql = @$" delete from {paydb}.tmpTbltran where trn = @Trn and Source = 'FEG' ";
        await _sql.ExecuteCmd(sql, new { Trn=trn }, conn!); 
        
        //--- Insert ang mga accounts sa Tmptbltran --------------------------------------------------------------------
        foreach (var feg in fegs)
        {
            var acctNumber = feg.AcctNumber;
            sql = $@"insert into {paydb}.tmptbltran (TRN,   EmpmasId,   EmpNumber,   acctNumber,         Qty, Rate, RateTypeId, Amount, dTimeStamp, postedby,   Source)
                        Select                      t.Trn,  t.EmpmasId, e.EmpNumber, @AcctNumber acctNo, 0,   0,    0,          0,      now(),        @IdUser pb, 'FEG'
                        from {paydb}.tmptbltranemplist t 
                        left join  {pisdb}.Empmas e on e.Id = t.EmpmasId
                        where t.trn = @Trn 
                      on duplicate key update Source = 'FEG' ; 
                      insert into {paydb}.tmppaymainvisacct (trn, AcctNumber) values (@Trn, '{acctNumber}') on duplicate key update trn = @Trn;";
            
            await _sql.ExecuteCmd(sql, new { Trn=trn, AcctNumber = acctNumber, Iduser = idUser }, conn!);
        }
        
        //--- Integrate figure -----------------------------------------------------------------------------------------
        sql = $@"update {paydb}.Tmptbltran t 
                 join 
                 (   
                     select EmpmasId, AcctNumber, sum(ifnull(Qty,0)*ifnull(Amount,0)) Amount From 
                     (
                         SELECT ge.EmpmasId, g.AcctNumber,
                             if(g.perdayEarnings=1,
                                  case
                                    when tt.RateTypeId = 1 then tt.Qty / (select DayToHours       from {paydb}.settings limit 1)   /* hourly */
                                    when tt.RateTypeId = 2 then tt.Qty                                                             /* per day */
                                    when tt.RateTypeId = 3 then tt.Qty * (select DayToHours       from {paydb}.settings limit 1)   /* bi-monthly */
                                    when tt.RateTypeId = 4 then tt.Qty * (select SemiMonthtodays  from {paydb}.settings limit 1)   /* monthly */
                                    when tt.RateTypeId = 5 then tt.Qty * (select Semiannualtodays from {paydb}.settings limit 1)   /* biAnnual */
                                    when tt.RateTypeId = 6 then tt.Qty * (select Yeartodays       from {paydb}.settings limit 1)   /* Annual */
                                    else tt.Qty
                                  end
                                , 1) Qty, g.Amount
                         FROM {paydb}.`fixedearnings_grp` g
                            Left join {paydb}.`fixedearnings_grp_emp` ge on g.Id = ge.fixedearnings_grpId
                            Left join (select * from {paydb}.TmpTbltran tt where tt.trn = @Trn and tt.Acctnumber = 'E001') tt on tt.EmpmasId = ge.EmpmasId
                         where ge.EmpmasId in 
                                    (SELECT EmpmasId FROM {paydb}.`tmptbltranemplist` where trn = @Trn) and  
                               g.{fldName}      =  1 and 
                               g.Status         =  'A' and 
                               g.PayrollgrpId   =  @PayrollgrpId and 
                               ge.empmasId      >= 0
                     ) as q
                     group by EmpmasId, AcctNumber
                 ) 
                 feg on feg.EmpmasId = t.EmpmasId and t.AcctNumber = feg.AcctNumber 
                 set t.Qty = 1, t.Rate = feg.Amount, t.Amount = feg.Amount
                 where t.Source =  'FEG' and t.Trn = @Trn;";
        
        await _sql.ExecuteCmd(sql, new { Trn=trn, PayrollgrpId=payrollgrpId }, conn!);
        
    }
    
    public async Task _01_FE_to_Tmptbltran_1(string? trn, string? fldName, string? paydb, string? pisdb, int? idUser, string? conn)
    {
        //--- Settings ---------------------------------------------------------------------------------
        var sql = $@"Select * from {paydb}.Settings";
        var ss  = await _sql.FetchData<SettingsModel, dynamic>(sql, new { }, conn!);
        var s = ss.FirstOrDefault();
        
        var yrToDay       = s?.Yeartodays; 
        var biAnnualToDay = s?.Semiannualtodays; 
        var moToDay       = s?.Monthtodays; 
        var biMoToDay     = s?.Semiannualtodays; 
        var dayToHr       = s?.Daytohours;
        //=============================================================================================================


        //--- Delete Earning Accounts ---------------------------------------------------------------------------------
        sql = @$"delete from {paydb}.tmpTbltran 
                 where trn = @Trn and 
                       acctNumber in (select acctNumber from {paydb}.FixedEarnings 
                                      where Status = 'A' and 
                                            empNumber in (select e.empNumber from {paydb}.Tmptbltran t 
                                                          left join {pisdb}.Empmas e on e.Id = t.EmpmasId 
                                                          where Trn = @Trn))";
        await _sql.ExecuteCmd(sql, new { Trn=trn }, conn!); 
        //=============================================================================================================
        
        sql = $@"insert into {paydb}.tmptbltran 
                        (TRN,   EmpmasId,   EmpNumber,   acctNumber,   
                         Qty,   
                         Rate,   RateTypeId,   
                         Amount,              
                         dTimeStamp,       postedby,         Source) 
                     select 
                         r.Trn, r.EmpmasId, r.EmpNumber, ifnull(r.AcctNumber,'') AcctNumber, 
                         if(r.PerdayEarnings=1, 
                          case
                            when t.RateTypeId = 1 then ifnull(t.Qty / {dayToHr}       ,0)     /*-- (hourly)       */
                            when t.RateTypeId = 2 then ifnull(t.Qty                   ,0)     /*-- (daily)        */
                            when t.RateTypeId = 3 then ifnull(t.qty * {biMoToDay}     ,0)     /*-- (semi-monthly) */ 
                            when t.RateTypeId = 4 then ifnull(t.qty * {moToDay}       ,0)     /*-- (monthly)      */ 
                            when t.RateTypeId = 5 then ifnull(t.qty * {biAnnualToDay} ,0)     /*-- (biAnnual)     */ 
                            when t.RateTypeId = 6 then ifnull(t.qty * {yrToDay}       ,0)     /*-- (Annually)     */
                            else 1 
                          end,
                          1) as Qty,  
                         ifnull(r.Rate,0) Rate, r.RateTypeId, 
                         ifnull(r.Rate,0) as Amount, 
                         now() dTimeStamp, @IdUser PostedBy, 'FE' as Source 
                    from
                        ( select el.trn, 
                                 el.EmpmasId, 
                                 e.EmpNUmber, 
                                 fe.AcctNumber, 
                                 1 Qty, 
                                 fe.Amount Rate, 
                                 0 RateTypeId, 
                                 fe.PerdayEarnings 
                          FROM {paydb}.tmptbltranemplist el
                            left join {pisdb}.empmas e on e.Id = el.EmpmasId
                            left join (
                                select fe.* from {paydb}.fixedEarnings fe
                                where fe.{fldName} = 1 and fe.Status = 'A' and 
                                      left(fe.trnPosted,6) != left(@Trn,6) and
                                      EmpNumber in ( SELECT e.EmpNumber 
                                                        FROM {paydb}.tmptbltranemplist l
                                                        left join {pisdb}.Empmas e on e.Id = l.EmpmasId
                                                        where l.trn = @Trn )
                                group by EmpNumber, AcctNumber 
                        ) fe on fe.Empnumber = e.Empnumber
                    where el.trn = @Trn ) r 
                    left join (select * from {paydb}.TmpTbltran where acctNumber = 'E001' and trn = @Trn ) t on t.EmpmasId = r.EmpmasId  
                    on duplicate key update Qty=1, Rate = r.Rate, Amount = r.Qty*r.Rate; ";
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn= trn, IdUser = idUser }, conn!); 
    }
    public async Task _01_Account_to_Tmptbltran(string? trn, string? acctNumber, string? source, int? idUser, string? paydb, string? pisdb, string? conn)
    {
        var sql = $@"select * from {paydb}.Tmptbltran where acctNumber = @AcctNumber and  trn = @Trn";
        var res = await _sql.FetchData<TbltranModel,dynamic>(sql, new { Trn = trn, AcctNumber = acctNumber }, conn!);
        if (res.Count > 0) return;

        switch (acctNumber)
        {
            case "D001":
                await _01_Tax(trn,      source, idUser, paydb??"-", pisdb??"-", conn??"-");               //--- Tax 
                break;
            
            case "D002":
                await _01_PremSSS(trn,  source, idUser, paydb??"-", pisdb??"-", conn??"-");               //--- SSS 
                break;
            
            case "D003":
                await _01_PremPHIC(trn, source, idUser, paydb??"-", pisdb??"-", conn??"-");               //--- PHIC 
                break;
            
            case "D004":
                await _01_PremPagibig(trn, source, idUser, paydb??"-", pisdb??"-", conn??"-");            //--- Pagibig  
                break;
            
            
        }
    }

    //--- Premiums and Tax ---------------------------------------------------------------------------------------------------
    //--- Data is loaded here; the computation itself is in PayrollCalculator (rules ported from FoxPro payroll4.scx).
    private class PremEmp
    {
        public int?    EmpmasId  { get; set; }
        public string? EmpNumber { get; set; }
        public int?    Wtax      { get; set; }
        public int?    Wsss      { get; set; }
        public int?    Wphic     { get; set; }
        public int?    Wpagibig  { get; set; }
    }
    private class PremLine
    {
        public int?    EmpmasId   { get; set; }
        public string? AcctNumber { get; set; }
        public double  Amount     { get; set; }
    }

    private async Task<(List<PremEmp> emps, List<PremLine> curr, List<PremLine> saved)> _01_PremData(string? trn, string paydb, string pisdb, string conn)
    {
        var sql = $@"select t.EmpmasId, max(e.EmpNumber) EmpNumber,
                            max(ifnull(s.Wtax,0)) Wtax, max(ifnull(s.Wsss,0)) Wsss, max(ifnull(s.Wphic,0)) Wphic, max(ifnull(s.Wpagibig,0)) Wpagibig
                     from {paydb}.Tmptbltran t
                     left join {pisdb}.Empmas e          on e.Id       = t.EmpmasId
                     left join {paydb}.deprecsettings s  on s.EmpmasId = t.EmpmasId
                     where t.Trn = @Trn
                     group by t.EmpmasId";
        var emps = await _sql.FetchData<PremEmp, dynamic>(sql, new { Trn = trn }, conn);

        sql = $@"select EmpmasId, AcctNumber, ifnull(Amount,0) Amount from {paydb}.Tmptbltran where Trn = @Trn";
        var curr = await _sql.FetchData<PremLine, dynamic>(sql, new { Trn = trn }, conn);

        //--- Posted earlier this month (same yyMM) ----------------------------------------------------------------------
        sql = $@"select EmpmasId, AcctNumber, ifnull(Amount,0) Amount from {paydb}.Tmptbltran
                 where Status = 'P' and Trn != @Trn and left(Trn,4) = left(@Trn,4) and
                       EmpmasId in (select EmpmasId from {paydb}.Tmptbltran where Trn = @Trn)";
        var saved = await _sql.FetchData<PremLine, dynamic>(sql, new { Trn = trn }, conn);

        return (emps, curr, saved);
    }

    private async Task<HashSet<string>> _01_CoaList(string coaTbl, string paydb, string conn)
    {
        var res = await _sql.FetchData<string, dynamic>($"select AcctNumber from {paydb}.{coaTbl}", new { }, conn);
        return res.Where(a => a != null).ToHashSet();
    }

    private static double SumAmt(IEnumerable<PremLine> lines, int? empmasId, ICollection<string> accts)
        => lines.Where(l => l.EmpmasId == empmasId && accts.Contains(l.AcctNumber ?? "")).Sum(l => l.Amount);

    private async Task _01_InsertComputed(string? trn, string acctNumber, string source, int? idUser, string paydb, string conn,
                                          IEnumerable<(PremEmp e, double amt)> rows)
    {
        var sql = $@"insert into {paydb}.tmptbltran (TRN, EmpmasId, EmpNumber, acctNumber, Qty, Rate, RateTypeId, Amount, dTimeStamp, postedby, Source)
                     values (@Trn, @EmpmasId, @EmpNumber, @AcctNumber, 1, @Amount, 0, @Amount, now(), @IdUser, @Source)";
        var prm = rows.Select(r => new { Trn = trn, r.e.EmpmasId, r.e.EmpNumber, AcctNumber = acctNumber, Amount = r.amt, IdUser = idUser, Source = source }).ToList();
        if (prm.Count > 0) await _sql.ExecuteCmd(sql, prm, conn);
    }

    private async Task _01_Premium(string? trn, string acctNumber, int? idUser, string paydb, string pisdb, string conn)
    {
        var (coaTbl, matrixTbl, revFld, cols, source) = acctNumber switch
        {
            "D002" => ("coaSSS",     "matrixSSS",     "RevSSS",     "FStart, FEnd, EE Ee, ER Er, ECC Ecc, 0 Percent", "SSS"),
            "D003" => ("coaPHIC",    "matrixPHIC",    "RevPHIC",    "FStart, FEnd, EE Ee, ER Er, 0 Ecc, Percent",     "PHIC"),
            _      => ("coaPagibig", "matrixPagibig", "RevPagibig", "FStart, FEnd, EE Ee, ER Er, 0 Ecc, 0 Percent",   "Pibig")
        };

        var sql = $"select {cols} from {paydb}.{matrixTbl} where Revision = (select {revFld} from {paydb}.Settings limit 1) order by FStart";
        var table = await _sql.FetchData<PremiumBracket, dynamic>(sql, new { }, conn);

        var accts = await _01_CoaList(coaTbl, paydb, conn);
        var (emps, curr, saved) = await _01_PremData(trn, paydb, pisdb, conn);
        var premAcct = new[] { acctNumber };

        var rows = emps.Select(e =>
        {
            var calcEmp  = new PayCalcEmployee { WithSss = e.Wsss == 1, WithPhic = e.Wphic == 1, WithPagibig = e.Wpagibig == 1 };
            var mtd      = SumAmt(curr, e.EmpmasId, accts) + SumAmt(saved, e.EmpmasId, accts);
            var deducted = SumAmt(saved, e.EmpmasId, premAcct);
            var r = acctNumber switch
            {
                "D002" => PayrollCalculator.Sss(calcEmp, mtd, deducted, 0, table),
                "D003" => PayrollCalculator.Phic(calcEmp, mtd, deducted, 0, table),
                _      => PayrollCalculator.Pagibig(calcEmp, mtd, deducted, 0, table)
            };
            return (e, r.Ee);
        });

        await _01_InsertComputed(trn, acctNumber, source, idUser, paydb, conn, rows);
    }

    private Task _01_PremSSS(string? trn, string? source, int? idUser, string paydb, string pisdb, string conn)
        => _01_Premium(trn, "D002", idUser, paydb, pisdb, conn);

    private Task _01_PremPHIC(string? trn, string? source, int? idUser, string? paydb, string? pisdb, string? conn)
        => _01_Premium(trn, "D003", idUser, paydb ?? "-", pisdb ?? "-", conn ?? "-");

    private Task _01_PremPagibig(string? trn, string? source, int? idUser, string paydb, string pisdb, string conn)
        => _01_Premium(trn, "D004", idUser, paydb, pisdb, conn);

    //--- Tax needs the current-period premiums, so they are computed first when missing (FoxPro order) ---------------------
    private async Task _01_Tax(string? trn, string? source, int? idUser, string paydb, string pisdb, string conn)
    {
        foreach (var prem in new[] { "D002", "D003", "D004" })
            await _01_Account_to_Tmptbltran(trn, prem, source, idUser, paydb, pisdb, conn);

        var sql = $@"select SAmt, EAmt, Fix, Percentage from {paydb}.MatrixWTax
                     where Revision   = (select RevTax        from {paydb}.Settings limit 1) and
                           PeriodCode = (select TaxPeriodCode from {paydb}.Settings limit 1)
                     order by SAmt";
        var table = await _sql.FetchData<TaxBracket, dynamic>(sql, new { }, conn);

        var taxAccts = await _01_CoaList("coaTax", paydb, conn);
        var (emps, curr, _) = await _01_PremData(trn, paydb, pisdb, conn);

        var rows = emps.Select(e =>
        {
            var calcEmp = new PayCalcEmployee { WithTax = e.Wtax == 1 };
            var taxable = SumAmt(curr, e.EmpmasId, taxAccts);
            var tax = PayrollCalculator.Tax(calcEmp, taxable,
                                            SumAmt(curr, e.EmpmasId, ["D002"]),
                                            SumAmt(curr, e.EmpmasId, ["D003"]),
                                            SumAmt(curr, e.EmpmasId, ["D004"]), table);
            return (e, tax);
        });

        await _01_InsertComputed(trn, "D001", "Tax", idUser, paydb, conn, rows);
    }
    
    private async Task _01_Account_to_Tmptbltran_D001(string? trn, string? acctNumber, string? source, int? idUser, string? paydb, string? pisdb, string? conn)
    {
        var sql =  @$"insert into {paydb}.tmptbltran (TRN,      EmpmasId,   EmpNumber,    acctNumber,         Qty, Rate,       RateTypeId, Amount,      dTimeStamp, postedby,   Source)
                        select                   @Trn Trn, t.EmpmasId, t.EmpNumber,  @AcctNumber acctNo, 1,  t.TaxAmount, 0,          t.TaxAmount, now(),      @IdUser pb, @Source source 
                        from 
                        (
                            select t.EmpmasId, e.EmpNumber, t.Amount, tax.Fix, tax.Percentage, if(ds.wtax=1, (tax.Fix + ((t.Amount-tax.SAmt))*tax.Percentage),0) TaxAmount
                            from 
                            ( 
                                SELECT t.EmpmasId, Sum(t.Amount) Amount FROM {paydb}.tmptbltran t where t.trn = @Trn and 
                                       t.AcctNumber in (select AcctNumber from {paydb}.coatax)
                                group by t.EmpmasId 
                            ) t
                            left join 
                            ( 
                                SELECT * FROM {paydb}.`matrixwtax` where periodcode = (select TaxPeriodCode from {paydb}.Settings limit 1 ) 
                            ) tax on t.Amount between tax.SAmt and tax.EAmt
                            left join {paydb}.`deprecsettings` ds on ds.EmpmasId = t.EmpmasId
                            left join {pisdb}.empmas e on e.Id = t.EmpmasId 
                        ) t
                        order by t.EmpmasId; 
                 
                 insert into {paydb}.tmppaymainvisacct (Trn,   AcctNumber)     
                        select                          t.trn, @AcctNumber AcctNumber 
                        from {paydb}.tmptbltranemplist t  
                        where t.trn = @Trn 
                 on duplicate key update AcctNumber = @AcctNumber; ";
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn= trn, IdUser = idUser, AcctNumber = acctNumber, Source = source }, conn!); 
    }
    private async Task _01_Account_to_Tmptbltran_Premium(string? trn, string? acctNumber, string? source, int? idUser, string? paydb, string? pisdb, string? conn)
    {
        var coaRefTbl = "coaSSS";       //--- Default for SSS COA Reference Table 
        var matrixTbl = "matrixSSS";    //--- Default for SSS Matrix Table
        var tAmt      = "";
        var joinCond = ""; 
        
        switch (acctNumber)
        {
            case "D001":
                coaRefTbl = "coaTax";
                matrixTbl = "matrixTax";
                tAmt      = " if(ds.wtax=1, (tax.Fix + ((t.Amount-tax.SAmt))*tax.Percentage),0) ";
                joinCond  = " t.Amount between z.SAmt and z.EAmt ";
                break;
            
            case "D002":
                coaRefTbl = "coaSSS";
                matrixTbl = "matrixSSS";
                tAmt      = " t.ee ";
                joinCond  =  " t.Amount between z.FStart and z.FEnd ";
                break;
            
            case "D003":
                coaRefTbl = "coaPhic";
                matrixTbl = "matrixPhic";
                tAmt      = " t.ee + (t.Amount * z.Percent / 100 )";
                joinCond  =  " t.Amount between z.FStart and z.FEnd ";
                break;
        }
        
        
        var sql =  @$"insert into {paydb}.tmptbltran (TRN,      EmpmasId,   EmpNumber,    acctNumber,         Qty, Rate,       RateTypeId, Amount,    dTimeStamp, postedby,   Source)
                        select                        @Trn Trn, t.EmpmasId, t.EmpNumber,  @AcctNumber acctNo, 1,  t.TaxAmount, 0,          t.tamount, now(),      @IdUser pb, @Source source 
                        from 
                        (
                            select t.EmpmasId, e.EmpNumber, t.Amount, tax.Fix, tax.Percentage, {tAmt} tamount 
                            from 
                            ( 
                                SELECT t.EmpmasId, Sum(t.Amount) Amount FROM {paydb}.tmptbltran t where t.trn = @Trn and 
                                       t.AcctNumber in (select AcctNumber from {paydb}.{coaRefTbl})
                                group by t.EmpmasId 
                            ) t
                            left join 
                            ( 
                                SELECT * FROM {paydb}.{matrixTbl} where Revision = (select RevSSS from {paydb}.Settings limit 1 ) 
                            ) z on {joinCond}
                            left join {paydb}.`deprecsettings` ds on ds.EmpmasId = t.EmpmasId
                            left join {pisdb}.empmas e on e.Id = t.EmpmasId 
                        ) t
                        order by t.EmpmasId; 
                 
                 insert into {paydb}.tmppaymainvisacct (Trn,   AcctNumber)     
                        select                          t.trn, @AcctNumber AcctNumber 
                        from {paydb}.tmptbltranemplist t where t.trn = @Trn 
                 on duplicate key update AcctNumber = @AcctNumber; ";
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn= trn, IdUser = idUser, AcctNumber = acctNumber, Source = source }, conn!); 
    }

    public async Task _01_Loans(string? trn, int? idUser, string? source, string? paydb, string? pisdb, string? conn)
    {
        var prd = MsdsDataAccess.Extract_FieldPrd(trn);

        // 1) --- Get Employee Loans -----------------------------------------------------------------------------------
        var sql = @$"select *, if(balance < Amort, Balance, Amort) as Amort  
                        from {paydb}.Loans l
                     where l.{prd}  =  1  and  
                           Status 	= 'A' and
                           balance 	> 0   and 
                           ( now() 	> payres or now() > date ) and
                           empnumber in ( select distinct EmpNumber from {paydb}.Tmptbltran where Trn = @Trn )";
        var loans = await _sql.FetchData<LoansModel, dynamic>(sql, new { Trn = trn }, conn!); 
        
        if(!loans.Any()) return;
        
        // 2) --- Get Loan List (naka distinct ) -----------------------------------------------------------------------
        sql = @$"select distinct DedNCode    
                        from {paydb}.Loans l
                     where l.{prd}  =  1  and  
                           Status 	= 'A' and
                           balance 	> 0   and 
                           ( now() 	> payres or now() > date ) and
                           empnumber in ( select distinct EmpNumber from {paydb}.Tmptbltran where Trn = @Trn )";
        var loanList = await _sql.FetchData<LoansModel, dynamic>(sql, new { Trn = trn }, conn!);
        
        
        // 3) --- Get Employee List ------------------------------------------------------------------------------------
        sql = @$"select distinct t.EmpmasId, e.EmpNumber  from {paydb}.Tmptbltran t  
                 left join {pisdb}.Empmas e on e.Id = t.EmpmasId
                          where Trn = @Trn;";
        var emplist = await _sql.FetchData<TbltranModel, dynamic>(sql, new { Trn = trn }, conn!);

        // 4) --- Loans already deducted in this period by another TRN (FoxPro calc_loans skips these) -----------------
        sql = $@"SELECT * FROM {paydb}.tmptbltran WHERE trn            != @Trn
                                                  AND   LEFT(trn,6)    =  LEFT(@Trn,6)
                                                  AND   Status         =  'P'
                                                  AND   Source         =  'Loans'";
        var ploans = await _sql.FetchData<TbltranModel, dynamic>(sql, new { Trn = trn }, conn!);
        var deductedIds = ploans.Where(p => p.RefId > 0).Select(p => p.RefId!.Value).ToHashSet();
        loans = loans.Where(l => !(l.Id.HasValue && deductedIds.Contains(l.Id.Value))).ToList();

        foreach (var ll in loanList)
        {
            var rateTypeId    = 0;  
            var dTimeStamp    = DateTime.Now; 
            
            var acctNumber    = ll.DedNCode??"-";
            

            foreach (var e in emplist)
            {
                var  empmasId     = e.EmpmasId; 
                var empNumber  = e.EmpNumber;
                var refId         = 0;
                var qty           = 1.00; 
                var rate          = 0.00; 
                var amount        = 0.00;
                
                var ploan = loans.Where(pl => pl.EmpNumber == e.EmpNumber &&  pl.DedNCode == acctNumber);
                
                if (ploan?.Count() > 0 )
                {
                    var cloans = loans.Where(l1=> l1?.EmpNumber   == e.EmpNumber && l1?.DedNCode  == acctNumber);
                    foreach (var cloan in cloans)
                    {
                        qty    = 1; 
                        amount = cloan.Amort;
                        rate   = cloan.Amount;
                        refId  = cloan.Id??0;
                        
                        sql = $@" INSERT INTO {paydb}.tmptbltran  
                             (Trn,  EmpmasId,  EmpNumber,  acctNumber,  Qty,  Rate,  RateTypeId,  Amount,  dTimeStamp,  postedby,  Source,  RefId) values 
                             (@Trn, @EmpmasId, @EmpNumber, @AcctNumber, @Qty, @Rate, @RateTypeId, @Amount, @dTimeStamp, @Postedby, @Source, @RefId) ";
                
                        await  _sql.ExecuteCmd<dynamic>(sql, new {  Trn = trn, EmpmasId = empmasId, EmpNumber=empNumber, 
                            AcctNumber=acctNumber, Qty = qty, Rate=rate, RateTypeId=rateTypeId, 
                            Amount=amount, DTimeStamp=dTimeStamp, Postedby=idUser, 
                            Source=source, RefId=refId }, conn!);

                    }
                }
                else
                {
                    
                    sql = $@" INSERT INTO {paydb}.tmptbltran
                                 (TRN,  EmpmasId,  EmpNumber,  acctNumber,  Qty,  Rate,  RateTypeId,  Amount,  dTimeStamp,  postedby,  Source,  RefId) values
                                 (@Trn, @EmpmasId, @EmpNumber, @AcctNumber, @Qty, @Rate, @RateTypeId, @Amount, @dTimeStamp, @Postedby, @Source, @RefId); ";

                    await  _sql.ExecuteCmd<dynamic>(sql, new {  Trn = trn, EmpmasId = empmasId, EmpNumber=empNumber,
                                                                AcctNumber=acctNumber, Qty = qty, Rate=rate, RateTypeId=rateTypeId,
                                                                Amount=amount, DTimeStamp=dTimeStamp, Postedby=idUser,
                                                                Source=source, RefId=refId }, conn!);
                }
            }
        }
        
    }

    public async Task _01_MandatoryDeduction(string? trn, int? idUser, string? paydb, string? conn, string? source="MDed" )
    {
        
        var sql = $@" INSERT INTO {paydb}.tmptbltran
                                 (TRN,  EmpmasId,   EmpNumber,   acctNumber,  Qty,  Rate,  RateTypeId,  Amount,  dTimeStamp,  postedby,  Source,  RefId) 
                        with 
                        t1 as 
	                        (select distinct t.empmasId, t.EmpNumber from {paydb}.Tmptbltran t  
	                         where Trn = @Trn), 
                        trans as 
	                        ( select EmpmasId, AcctNumber, sum(Amount) Amount from {paydb}.dedmandatorytran 
                              where EmpmasId in (select EmpmasId from t1 ) 
                              group by EmpmasId, AcctNumber ), 
                        t2 as 
	                        ( SELECT ifnull(d.Id,0) RefId, t1.*, d.AcctNumber, d.ContAmt, d.MaxAmt  FROM {paydb}.`dedmandatory` d, t1  
		                        where d.status = 'A' and P2 = 1 ), 
                        t3 as 
	                        ( select t2.*, ifnull(t2.maxamt, 0) - ifnull(t.Amount,0) Rem  
		                      from t2 
	                          left join trans t on t.EmpmasId = t2.EmpmasId and t.AcctNumber = t2.AcctNumber), 
                        t4 as 
                            ( select *, if(rem > 0, ContAmt, 0) as Ded from t3 )
                             /* TRN,     EmpmasId,    EmpNumber,    acctNumber,    Qty,    Rate,   RateTypeId,   Amount, dTimeStamp,  postedby,   Source,  RefId */ 
                        select @Trn Trn, t4.EmpmasId, t4.Empnumber, t4.AcctNumber, 1 Qty,  t4.Ded, 0 RateTypeId, t4.Ded, now() dts,   @IdUser pb, @Source, t4.RefId 
                            from t4;";

        await  _sql.ExecuteCmd<dynamic>(sql, new {  Trn = trn, IdUser=idUser, Source=source }, conn!);
        
        
    }
    public async Task _01_PostTrn(string? trn, int? idUser, string? paydb, string? conn)
    {
        await PostTmpTbltran(trn, idUser, paydb, conn);
        
        //Post TmptblTran(); 
        //Post Tax(); 
        //Post SSS(); 
        //Post PHIC(); 
        //Post Pagibig(); 
        await PostLoans(); 
        //Post Mandatory Deductions(); 
        
        
        
        
        
        
        return; 

        
        async Task PostTmpTbltran(string? ptrn, int? pidUser, string? ppaydb, string? pconn)
        {
            var psql = @$"update {ppaydb}.Tmptbltran set  
                                dTimeStamp  = now(), 
                                Status      = 'P', 
                                PostedBy    = @PostedBy 
                          where Trn = @Trn;

                          Delete from {ppaydb}.TblTran where trn = @Trn;

                          Insert into {ppaydb}.TblTran 
                          select * from {ppaydb}.Tmptbltran where trn = @Trn and Amount != 0
                          on duplicate key update Status = 'P'; 

                          update {ppaydb}.paymainhdr set status = 'Posted', DatePosted = now() where Trn = @Trn "; 
            await  _sql.ExecuteCmd<dynamic>(psql, new {  Trn = ptrn, PostedBy=pidUser }, conn!);
        }
        
        async Task PostLoans()
        {
            var psql = @$"with
                            IdList as 
                                (Select RefId from {paydb}.TmpTblTran Where Trn = @Trn and RefId > 0 and Source = 'Loans' ), 
                            EmpmasIdList as 
                                ( Select EmpmasId from {paydb}.TmpTblTran Where Trn = @Trn ), 
                            LoansTot as 
                                ( Select EmpmasId, RefId, Sum(Amount) Amount from {paydb}.Tbltran 
                                        Where EmpmasId in ( Select EmpmasId from EmpmasIdList ) and 
                                              RefId    in ( Select RefId from IdList ) and 
                                              Source = 'Loans' 
                                  group by EmpmasId, RefId ) 
                            
                             update {paydb}.Loans as l 
                                join LoansTot lt on l.Id = lt.RefId 
                                set l.Balance = l.Amount - lt.Amount, trnLastPosted = @Trn   
                            where l.Id in (select RefId from IdList ); 

                           "; 
            await  _sql.ExecuteCmd<dynamic>(psql, new {  Trn = trn }, conn!);
            
            psql = @$"with
                        IdList as 
                            (Select RefId from {paydb}.TmpTblTran Where Trn = @Trn and RefId > 0 and Source = 'Loans' )
                        
                        update {paydb}.Loans set  Status = 'I'
                        where Id in (select RefId from IdList ) and Balance < 1 ;

                           "; 
            await  _sql.ExecuteCmd<dynamic>(psql, new {  Trn = trn }, conn!);
            
            
        }
        
        
        
        
    }




    public async Task<Model605> _02TmpPayroll(string? trn, string? paydb, string? pisdb, string? conn)
    {
        Model605 m605 = new();
        if (trn == null || paydb == null || pisdb == null || conn == null) return m605;

        m605.Trn   = trn;
        m605.Paydb = paydb;
        m605.Pisdb = pisdb;
        m605.Conn  = conn;

        //--- 1) Paymainhdr --------------------------------------------------------------------------------------
        var sql = $"select * from {paydb}.Paymainhdr where trn = @Trn";
        var res = await _sql.FetchData<PaymainhdrModel?, dynamic>(sql, new { Trn = trn }, conn);
        m605.Paymainhdr = res.FirstOrDefault();
        m605.Paymainhdrs = res;

        //--- 2) PaymainVisAcct -----------------------------------------------------------------------------------
        sql = $@"Select distinct @Trn Trn, t.AcctNumber, c.AcctName, c.ShortDesc  from {paydb}.TmpTbltran t   
                 left join {paydb}.Coa c on c.AcctNumber = t.AcctNumber                     
                 where trn = @Trn 
                 order by  left(t.acctNumber,1) Desc, t.Acctnumber  ; ";
        m605.Paymainvisaccts = await _sql.FetchData<PaymainvisacctModel?, dynamic>(sql, new { Trn = trn }, conn);

        //--- 3) Tmptbltran -----------------------------------------------------------------------------------
        m605.Tmptbltrans = await _02Tmptbltran_ByTrn(trn??"", paydb, pisdb!, conn); 
        
        //--- 4) Tmptbltran Employee List -----------------------------------------------------------------------------------
        m605.TmptbltranEmpLists = await _02TmptbltranEmpList_ByTrn(trn??"", paydb, pisdb, conn);
        

        return m605;


    }

    public async Task<Model605> _02NewPayroll(string? trn, string? paydb, string? pisdb, string? conn)
    {
        Model605 m605 = new(); 
        if (trn == null || paydb == null || pisdb==null || conn == null ) return m605;
        
        m605.Trn = trn;
        m605.Paydb = paydb;
        m605.Pisdb = pisdb;
        m605.Conn = conn;
        
        //--- 1) Paymainhdr --------------------------------------------------------------------------------------
        var sql = $"select * from {paydb}.Paymainhdr where trn = @Trn";
        var res = await _sql.FetchData<PaymainhdrModel?, dynamic>(sql, new { Trn = trn }, conn);
        m605.Paymainhdr  = res.FirstOrDefault();     
        m605.Paymainhdrs = res;

        //--- 2) PaymainVisAcct -----------------------------------------------------------------------------------
        sql = $"Select * from  {paydb}.Paymainvisacct where trn = @Trn ; ";
        m605.Paymainvisaccts  = await _sql.FetchData<PaymainvisacctModel?,dynamic>(sql, new { Trn = trn }, conn);
        
        //--- 3) Tmptbltran -----------------------------------------------------------------------------------
        m605.Tmptbltrans = await _02Tmptbltran_ByTrn(trn??"", paydb, pisdb!, conn); 
        
        //--- 4) Tmptbltran Employee List -----------------------------------------------------------------------------------
        m605.TmptbltranEmpLists = await _02TmptbltranEmpList_ByTrn(trn??"", paydb, pisdb, conn);
        
        return m605; 
    }

    
    public async Task<List<LoansModel?>?> _02LoansCheck(string? trn, string? paydb, string? conn)
    {
        var sql = @$" SELECT * FROM {paydb}.dedmandatory where status = 'A';"; 
        var res = await _sql.FetchData<LoansModel?, dynamic>(sql, new { Trn = trn }, conn??"-");
        return res;
        
    }
    public async Task<List<DedmandatorytranModel?>?> _02MandatoryDedutionCheck(string? trn, string? paydb, string? conn)
    {
        var sql = @$"select * from {paydb}.DedMandatory where Status = 'A' "; 
        var res = await _sql.FetchData<DedmandatorytranModel?, dynamic>(sql, new {  }, conn??"-");
        return res;
        
    }
    
    
    
    public async Task<List<EmpratesModel>> _02EmployeeToAdd(string? skey, string? trn, int? payrollgrpId, string? paydb, string? pisdb, string? conn)
    {
        var sql = $"""
                    Select concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName, 
                           g.Name PayrollGrpName, 
                           e.Id EmpmasId, e.EmpNumber, r.PayrollgrpId, r.EmpRate, 
                           r.PayrateId, r.UsePaygrpRates, r.RatePerHr, r.RatePerDay, r.RatePerMonth, r.RatePerYr, 
                           pr.RateName, r.*, s.Name EmpStatus
                    from {pisdb}.empmas        e
                  left join {paydb}.Emprates   r  on r.EmpmasId   = e.Id
                  left join {paydb}.Payrate    pr on pr.Id        = r.PayrateId
                  left join {pisdb}.Deprec     d  on d.EmpmasId   = e.Id
                  left join {pisdb}.REmpstat   s  on s.Id         = d.EmpStatusId
                  left join {paydb}.payrollgrp g  on g.Id         = r.PayrollgrpId 
                  where e.Id not in (select EmpmasId from {paydb}.TmpTbltran where Trn = @Trn ) and
                         (e.EmpLastNm like @Skey or e.EmpFirstNm like @Skey)
                  order by FullName ;   
                  """;
        var emprates  = await _sql.FetchData<EmpratesModel, dynamic>(sql, new { Trn = trn, Skey = $"%{skey.Trim()}%" }, conn!);
        
        return emprates??[];
    }
    
    public async Task<List<DeprecsettingsModel>> _02TaxableEmployees(string? trn, string? paydb, string? conn)
    {
        var sql = $@"select * from {paydb}.DeprecSettings where EmpmasId in 
                            (select EmpmasId from {paydb}.Tmptbltran where Trn = @Trn) and wtax = 1 ";
        var res = await _sql.FetchData<DeprecsettingsModel,dynamic>(sql, new { Trn = trn }, conn!);
        
        return res;
    }
    public async Task<List<DeprecsettingsModel>> _02TaxableEmployees_per_Trn(string? trn, string? paydb, string? conn)
    {
        var sql = $@"select * from {paydb}.DepRecSettings 
                     where EmpmasId in (select EmpmasId from {paydb}.Tmptbltran where Trn = @Trn) and 
                           wtax = 1 ; ";
        var res = await _sql.FetchData<DeprecsettingsModel,dynamic>(sql, new { Trn = trn }, conn!);
        return res;
    }
    
    public async Task<List<DeprecsettingsModel>> _02WithPremEmployees_per_Trn(string? trn, string? paydb, string? conn, string? premSw="wSSS")
    {
        var sql = $@"select * from {paydb}.DeprecSettings 
                     where EmpmasId in 
                           (
                            select EmpmasId from {paydb}.Tmptbltran where Trn = @Trn
                           ) and {premSw} = 1 ";
        var res = await _sql.FetchData<DeprecsettingsModel,dynamic>(sql, new { Trn = trn }, conn!);
        return res;
    }
    
    public async Task<List<CoaModel?>?> _02PrdAccts_ByTrn(string? trn, string? paydb, string? conn)
    {
        var sql = $"""
                   Select * from  {paydb}.Coa where AcctNumber in 
                          (select distinct AcctNumber from {paydb}.`TmpTbltran` where trn = @Trn) 
                   order by left(AcctNumber,1) desc, AcctNumber; 
                   ; 
                   """;
        
        var res = await _sql.FetchData<CoaModel?, dynamic>(sql, new { Trn = trn }, conn!);
        return res;
    }
    
    public async Task _03TmpTbltran(TbltranModel tbltran, string? paydb, string? conn)
    {
        var sql = @$"update {paydb}.tmpTbltran set 
                            Qty         = @Qty, 
                            Rate        = @Rate, 
                            RateTypeId  = @RateTypeId, 
                            Amount      = @Amount, 
                            dTimeStamp  =  now()  
                     where TRN=@TRN and EmpmasId = @EmpmasId and AcctNumber=@AcctNumber ";
        await _sql.ExecuteCmd<dynamic>(sql, tbltran, conn!);

    }
    
    public async Task _03PaymainhdrCoverage(PaymainhdrModel paymainhdr, string? paydb, string? conn)
    {
        var sql = @$"update {paydb}.Paymainhdr set  AttStart = @AttStart, AttEnd = @AttEnd where TRN=@TRN  ";
        await _sql.ExecuteCmd<dynamic>(sql, paymainhdr, conn!);

    }
    
    
    
    public async Task<Model605> _04Tmp_Employee(string? trn, int? empmasId, string? paydb, string? pisdb, string? conn)
    {
        Model605 m605 = new();
        if(trn.Trim().Length < 1 || paydb?.Trim().Length < 1 || conn?.Trim().Length < 1 ) return m605;
        
        var sql = $"""
                    Delete from {paydb}.TmpTbltran        where trn = @Trn and EmpmasId = @EmpmasId; 
                    Delete from {paydb}.tmptbltranemplist where trn = @Trn and EmpmasId = @EmpmasId; 
                   """;
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, EmpmasId = empmasId }, conn!); 
        
        //--- 1) Tmptbltran -----------------------------------------------------------------------------------
        m605.Tmptbltrans = await _02Tmptbltran_ByTrn(trn??"", paydb, pisdb, conn); 
        
        //--- 2) Tmptbltran Employee List -----------------------------------------------------------------------------------
        m605.TmptbltranEmpLists = await _02TmptbltranEmpList_ByTrn(trn??"", paydb, pisdb, conn);
        
        return m605;
        
    }

    public async Task _04PayrollDtlRecord(string? trn, int? empmasId, string? paydb, string? conn)
    {
        if(trn.Trim().Length < 1 || paydb?.Trim().Length < 1 || conn?.Trim().Length < 1 ) return;
        
        var sql = $"select * from {paydb}.Paymainhdr where trn = @Trn";
        var res = await _sql.FetchData<PaymainhdrModel, dynamic>(sql, new { Trn = trn }, conn!);

        var status = res.FirstOrDefault()?.Status;
        if (status=="Lock" || status=="Locked") return;

        sql = @$"Delete from {paydb}.Paymaindtl   where trn = @Trn and EmpmasId = @EmpmasId; 
                 Delete from {paydb}.Paytran      where trn = @Trn and EmpmasId = @EmpmasId; 
                 ";
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, EmpmasId = empmasId }, conn!); 
    }

    public async Task _04UnlockedPayroll(string? trn, string? paydb, string? conn)
    {
        if(trn.Trim().Length < 1 || paydb?.Trim().Length < 1 || conn?.Trim().Length < 1 ) return;
        
        var sql = $"select * from {paydb}.Paymainhdr where trn = @Trn";
        var res = await _sql.FetchData<PaymainhdrModel, dynamic>(sql, new { Trn = trn }, conn!);

        var status = res.FirstOrDefault()?.Status;
        if (status=="Lock" || status=="Locked") return;

        sql = @$"Delete from {paydb}.Paymainhdr            where trn = @Trn; 
                 Delete from {paydb}.TmpTbltran           where trn = @Trn; 
                 Delete from {paydb}.TmpPaymainvisacct    where trn = @Trn; 
                 Delete from {paydb}.tmptbltranemplist    where trn = @Trn; 
                 ";
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn}, conn!); 
    }
    
    public async Task _04UEmployee(string? trn, int? empmasId, string? paydb, string? conn)
    {
        if(trn.Trim().Length < 1 || paydb?.Trim().Length < 1 || conn?.Trim().Length < 1 ) return;
        
        
        var sql = $"""
                       Delete from {paydb}.TmpTbltran             where trn = @Trn    and EmpmasId = @EmpmasId; 
                       Delete from {paydb}.tmptbltranemplist      where trn = @Trn    and EmpmasId = @EmpmasId; 
                   """;
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, EmpmasId = empmasId}, conn!); 
    }

    public async Task<Model605> _04Acct_per_Trn(string? trn, string? acctNumber, string? paydb, string? pisdb, string? conn)
    {
        Model605 m605 = new();
        if(trn.Trim().Length < 1 || paydb?.Trim().Length < 1 || conn?.Trim().Length < 1 ) return m605;
        
        var sql = $"""
                       Delete from {paydb}.TmpTbltran             where trn = @Trn    and AcctNumber = @AcctNumber ; 
                       Delete from {paydb}.tmppaymainvisacct      where trn = @Trn    and AcctNumber = @AcctNumber ; 
                   """;
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, AcctNumber = acctNumber}, conn!); 
        
        //--- 1) Tmptbltran -----------------------------------------------------------------------------------
        m605.Tmptbltrans = await _02Tmptbltran_ByTrn(trn??"", paydb, pisdb!, conn); 
        
        //--- 2) Period Accounts ------------------------------------------------------------------------------
        m605.PrdAccts = await _02PrdAccts_ByTrn(trn??"", paydb, conn);
        
        return m605;
    }

    public async Task _04Deductions_per_Trn(string? trn, string? paydb, string? conn)
    {
        
        var sql = $"""
                       Delete from {paydb}.TmpTbltran             where trn = @Trn    and left(AcctNumber,1) = "D" ; 
                       Delete from {paydb}.tmppaymainvisacct      where trn = @Trn    and left(AcctNumber,1) = "D" ; 
                   """;
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn}, conn!); 
        
        
    }

    
    //---- Private Functions -------------------------------------------------------------------------------------------
    private async Task<List<TbltranModel?>?> _02Tmptbltran_ByTrn(string? trn, string? paydb, string? pisdb, string? conn)
    {
        var sql = $@"
                     with 
                        t1 as    
                            ( select t.*, 
                                 concat(trim(ifnull(e.Emplastnm,'')),', ', trim(ifnull(e.EmpfirstNm,'')),' ', trim(ifnull(e.EmpmidNm,'')) ) EmpName 
                                 from  {paydb}.Tmptbltran t 
                                 left join {pisdb}.Empmas e on e.Id = t.EmpmasId 
                                 where trn = @Trn and Source = '-' ), 
                        t2 as 
                            ( select t.Trn, t.EmpmasId, e.EmpNumber, t.AcctNumber, sum(t.Amount) Amount, 
                                concat(trim(ifnull(e.Emplastnm,'')),', ', trim(ifnull(e.EmpfirstNm,'')),' ', trim(ifnull(e.EmpmidNm,'')) ) EmpName
                              from  {paydb}.Tmptbltran t 
                              left join {pisdb}.Empmas e on e.Id = t.EmpmasId 
                              where trn = @Trn and Source != '-' 
                              group by t.trn, t.EmpmasId, t.EmpNumber, t.AcctNumber, EmpName), 
                        t3 as 
                            ( select * from t1 
                              union 
                              select TRN, EmpmasId, EmpNumber, AcctNumber, 1 Qty, Amount Rate, 0 RateTypeId, Amount, now() dTimeStamp, 0 postedby, ' ' Source, '*' Status, 0 RefId, EmpName 
                              from t2 ) 
                    select * from t3 t  
                    order by left(t.AcctNumber,1) desc, t.AcctNumber, EmpName; ";
        
        var res = await _sql.FetchData<TbltranModel?, dynamic>(sql, new { Trn = trn }, conn!);
        return res;
    }
    
    private async Task<List<TmptbltranemplistModel?>?> _02TmptbltranEmpList_ByTrn(string? trn, string? paydb, string? pisdb, string? conn)
    {
        var sql = $"""
                   Select distinct 
                          t.EmpmasId, e.EmpNumber, concat(trim(e.EmpLastNm), ', ',trim(e.empFirstNm), ' ', trim(e.empmidNm)) EmpName, 
                          t.Rate            Emprate, 
                          t.RateTypeId      PayrateId, 
                          el.PayrollgrpId, 
                          pr.RateName       PayrateName          
                   from  {paydb}.Tmptbltran t
                        left join {pisdb}.Empmas              e  on e.Id  = t.EmpmasId
                        left join {paydb}.payrate             pr on pr.Id = t.RateTypeId 
                        left join {paydb}.tmptbltranemplist   el on el.Trn = t.trn and el.EmpmasId = e.Id 
                   where t.trn = @Trn and AcctNumber = 'E001'
                   order by EmpName; 
                   """;;
        var res = await _sql.FetchData<TmptbltranemplistModel?, dynamic>(sql, new { Trn = trn }, conn!);
        return res;
    }
    
    
    
}