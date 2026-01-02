using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class PaymainvisacctDataAccess : IPaymainvisacctDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PaymainvisacctDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PaymainvisacctModel?> _01(PaymainvisacctModel paymainvisacct, int payrollgrpId,  double defRate, string paydb, string pisdb,int userId, string conn)
    {
        var trn = paymainvisacct.Trn;
        var acctNumber = paymainvisacct.AcctNumber; 
        
        var sql = $"""
                   Insert into {paydb}.TblTran (TRN,          
                                                   EmpmasId, 
                                                   acctNumber,  
                                                   Qty, 
                                                   Rate,
                                                   RateTypeId, 
                                                   amount, 
                                                   dTimeStamp, 
                                                   postedby) 
                   Select distinct                 @Trn         as Trn, 
                                                   t.EmpmasId   as EmpmasId, 
                                                   @AcctNumber  as AcctNumber, 
                                                   0            as Qty,   
                                                   if(rd.rate is null or rd.rate = 0, @DefRate, rd.Rate) as Rate,
                                                   1            as RateTypeId,
                                                   0            as Amount    ,  
                                                   now()        as DTimeStamp,     
                                                   @UserId      as PostedBy
                   from {paydb}.TblTran t 
                   left join {paydb}.Empratesdtl rd on rd.EmpmasId = t.EmpmasId and rd.AcctNumber = @AcctNumber and rd.PayrollgrpId = @PayrollgrpId
                   where t.Trn = @Trn and t.AcctNumber = 'E001'
                   on duplicate key Update dTimeStamp = now();  
                   """;
        
        
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, AcctNumber = acctNumber, PayrollgrpId=payrollgrpId, DefRate=defRate, UserId = userId}, conn);
        
        sql = $"""
                   Insert into {paydb}.Paymainvisacct (Trn, AcctNumber) values (@Trn, @AcctNumber); 
                   SELECT * FROM {paydb}.Paymainvisacct WHERE Trn=@Trn and AcctNumber=@AcctNumber; 
                   """;
        var res = await _sql.FetchData<PaymainvisacctModel?, dynamic>(sql, new {Trn=trn, AcctNumber = acctNumber}, conn);
        
        return res.FirstOrDefault();
    }
    
    public async Task<PaymainvisacctModel?> _01Tmp(PaymainvisacctModel paymainvisacct, int payrollgrpId,  double defRate, string paydb, string pisdb,int userId, string conn)
    {
        var trn = paymainvisacct.Trn;
        var acctNumber = paymainvisacct.AcctNumber; 
        
        var sql = $"""
                   Insert into {paydb}.TmpTblTran (TRN,          
                                                   EmpmasId, 
                                                   acctNumber,  
                                                   Qty, 
                                                   Rate,
                                                   RateTypeId, 
                                                   amount, 
                                                   dTimeStamp, 
                                                   postedby) 
                   Select distinct                 @Trn         as Trn, 
                                                   t.EmpmasId   as EmpmasId, 
                                                   @AcctNumber  as AcctNumber, 
                                                   0            as Qty,   
                                                   if(rd.rate is null or rd.rate = 0, @DefRate, rd.Rate) as Rate,
                                                   1            as RateTypeId,
                                                   0            as Amount    ,  
                                                   now()        as DTimeStamp,     
                                                   @UserId      as PostedBy
                   from {paydb}.TmpTblTran t 
                   left join {paydb}.Empratesdtl rd on rd.EmpmasId = t.EmpmasId and rd.AcctNumber = @AcctNumber and rd.PayrollgrpId = @PayrollgrpId
                   where t.Trn = @Trn and t.AcctNumber = 'E001'
                   on duplicate key Update dTimeStamp = now();  
                   """;
        
        
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, AcctNumber = acctNumber, PayrollgrpId=payrollgrpId, DefRate=defRate, UserId = userId}, conn);
        
        sql = $"""
                   Insert into {paydb}.TmpPaymainvisacct (Trn, AcctNumber) values (@Trn, @AcctNumber); 
                   SELECT * FROM {paydb}.TmpPaymainvisacct WHERE Trn=@Trn and AcctNumber=@AcctNumber; 
                   """;
        var res = await _sql.FetchData<PaymainvisacctModel?, dynamic>(sql, new {Trn=trn, AcctNumber = acctNumber}, conn);
        
        return res.FirstOrDefault();
    }
    
    public async Task<List<PaymainvisacctModel?>?> _01s(List<PaymainvisacctModel?>? paymainvisaccts, string trn, string schema, string conn)
    {
        //--- 1) Delete Current Visibility -------------------------------
        var cmd = $@"Delete from {schema}.Paymainvisacct where Trn=@Trn";
        await _sql.ExecuteCmd(cmd, new { Trn = trn }, conn); 
        
        //--- 2) Insert Datas --------------------------------------------
        paymainvisaccts?.ForEach(p =>
        {
            var sql = $@"Insert into {schema}.Paymainvisacct (Trn, AcctNumber) values (@Trn, @AcctNumber) 
                            on duplicate key update AcctNumber = @AcctNumber;"; 
            _sql.ExecuteCmd(sql,p,conn); 
        });
        
        //--- 2) Fetch Datas --------------------------------------------
        var sql1 = $@"select * from {schema}.Paymainvisacct where Trn=@Trn"; 
        var res = await _sql.FetchData<PaymainvisacctModel?, dynamic>(sql1, new{Trn=trn}, conn);
        return res;
    }
    

    public async Task<List<PaymainvisacctModel?>?> _02ByTrns(string trn, string schema, string conn)
    {
        string sql = $@"select  Trn, AcctNumber from {schema}.Paymainvisacct where Trn = @Trn";
        var data = await _sql.FetchData<PaymainvisacctModel?, dynamic>(sql, new { Trn = trn }, conn);
        return data;
    }
    
    public async Task<PaymainvisacctModel?> _04(PaymainvisacctModel paymainvisacct, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Paymainvisacct where trn = @Trn and acctNumber = @AcctNumber;";
        await _sql.ExecuteCmd<dynamic>(sql, paymainvisacct, conn);

        sql = $@" select  * from {schema}.Paymainvisacct where  trn = @trn and acctNumber = @AcctNumber;";
        var data = await _sql.FetchData<PaymainvisacctModel?, dynamic>(sql, paymainvisacct, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task _04(string trn, string acctNumber, string schema, string conn)
    {
        string sql = $"""
                      Delete from {schema}.Paymainvisacct 
                      where trn = @Trn and acctNumber = @AcctNumber;
                      Delete from {schema}.TmpTbltran 
                      where trn = @Trn and acctNumber = @AcctNumber;
                      
                      """;
        await _sql.ExecuteCmd<dynamic>(sql, new {Trn=trn, AcctNumber = acctNumber}, conn);
    }
    
}