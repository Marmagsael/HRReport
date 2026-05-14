using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;


namespace HRApiLibrary.DataAccess._20_Pay;

public class FixedearningsDataAccess : IFixedearningsDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public FixedearningsDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<FixedearningsModel?> _01(FixedearningsModel fixedearnings, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Fixedearnings 
                                (PayrollgrpId,   Empnumber,  DStart,  DEnd,  AcctNumber,  Amount,  CreatedBy, 
                                 PerdayEarnings, DaysPara,   P1,  P2,  P3,  P4,  P5) 
                        values  (@PayrollgrpId,   @Empnumber, @DStart, @DEnd, @AcctNumber, @Amount, @CreatedBy, 
                                 @PerdayEarnings, @DaysPara,  @P1, @P2, @P3, @P4, @P5);
                        select  f.*, c.AcctName 
                        from {schema}.Fixedearnings f
                        left join {schema}.Coa c on c.AcctNumber = f.AcctNumber    
                        WHERE f.ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<FixedearningsModel?, dynamic>(sql, fixedearnings, conn);
        return res.FirstOrDefault();
    }


    public async Task<FixedearningsModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  f.*, c.AcctName 
                        from {schema}.Fixedearnings f
                        left join {schema}.Coa c on c.AcctNumber = f.AcctNumber   
                        where Id = @Id";
        var data = await _sql.FetchData<FixedearningsModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<FixedearningsModel?>?> _02ByEmpnumber(string empnumber, string paydb, string conn)
    {
        string sql = $@"select  f.*, c.AcctName, g.Name PayrollgrpName  
                        from {paydb}.Fixedearnings    f
                        left join {paydb}.Coa         c on c.AcctNumber   = f.AcctNumber                  
                        left join {paydb}.Payrollgrp  g on g.Id       = f.PayrollgrpId                  
                        where f.empnumber = @empnumber and f.status   = 'A'";
        var data = await _sql.FetchData<FixedearningsModel?, dynamic>(sql, new { EmpNumber = empnumber }, conn);
        return data;
    }
    public async Task<List<FixedearningsModel?>?> _02By_PayTrnPrd(string trn, string paydb, string conn)
    {
        var fldPrd = trn.Substring(4, 2);
        var fld = fldPrd switch
        {
            "02" => "P2",
            "03" => "P3",
            "04" => "P4",
            "05" => "P5",
            _ => "P1"
        };
        
        
        var sql = $@"select  f.*, c.AcctName, g.Name PayrollgrpName  
                        from {paydb}.Fixedearnings    f
                        left join {paydb}.Coa         c on c.AcctNumber   = f.AcctNumber                  
                        left join {paydb}.Payrollgrp  g on g.Id           = f.PayrollgrpId                  
                        where f.status = 'A' and 
                              f.empnumber in (select empNumber from {paydb}.TmpTbltran where Trn = @Trn) and 
                              f.{fld} = 1 ";
        var data = await _sql.FetchData<FixedearningsModel?, dynamic>(sql, new { Trn=trn  }, conn);
        return data;
    }
    

    public async Task<FixedearningsModel?> _03(int id, FixedearningsModel fixedearnings, string schema, string conn)
    {
        string sql = $@"Update {schema}.Fixedearnings set 
                            PayrollgrpId        = @PayrollgrpId,  
                            Empnumber           = @Empnumber,  
                            DStart              = @DStart,  
                            DEnd                = @DEnd,  
                            AcctNumber          = @AcctNumber,  
                            Amount              = @Amount,  
                            IdSched             = @IdSched,  
                            TerminatedBy        = @TerminatedBy,  
                            DaysPara            = @DaysPara,  
                            PerdayEarnings      = @PerdayEarnings,
                            P1                  = @P1,  
                            P2                  = @P2,  
                            P3                  = @P3,  
                            P4                  = @P4,  
                            P5                  = @P5,  
                            Status              = @Status, 
                            TrnPosted           = @TrnPosted  
                            where            Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, fixedearnings, conn);
        sql = $@" select  * from {schema}.Fixedearnings x where x.Id = @Id ;";
        var data = await _sql.FetchData<FixedearningsModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<FixedearningsModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete    from {schema}.Fixedearnings   where Id   = @Id;
                        select  * from {schema}.Fixedearnings x where x.Id = @Id ;";
        var data = await _sql.FetchData<FixedearningsModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
