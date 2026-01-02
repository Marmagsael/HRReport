using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class LoansDataAccess : ILoansDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LoansDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<LoansModel?> _01(LoansModel loans, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Loans (EMPNUMBER,  DATE,  DEDNCODE,  DEDNDESC,  Principal,  AMOUNT,  AMORT,  BALANCE,  STATUS,  ENCODEBY,   ENCODEDT,   POSTED,  POSTFLAG,  REMARKS,  payMode,   payStart,  payRes,  cvno,  P1,  P2,  P3,  P4,  P5) values 
                                                   (@EmpNumber, @Date, @DedNCode, @DedNDesc, @Principal, @Amount, @Amort, @Balance, 'A',     @EncodedBy, @EncodedT,  @Posted, @PostFlag, @Remarks, @PayMode,  @PayStart, @PayRes, @Cvno, @P1, @P2, @P3, @P4, @P5)
                        on duplicate key update DATE        = @Date,  
                                                Principal   = @Principal,  
                                                AMOUNT      = @Amount,  
                                                AMORT       = @Amort, 
                                                CHANGEBY    = @ChangeBy,  
                                                CHANGEDT    = @ChangedT,  
                                                REMARKS     = @Remarks,  
                                                payMode     = @PayMode,  
                                                payStart    = @PayStart,  
                                                payRes      = @PayRes,  
                                                cvno        = @Cvno,  
                                                P1          = @P1, 
                                                P2          = @P2, 
                                                P3          = @P3, 
                                                P4          = @P4, 
                                                P5          = @P5 ";
        await _sql.ExecuteCmd<dynamic>(sql, loans, conn);

        sql = $@"select c.AcctName DedNDesc, l.* from {schema}.Loans l 
                    left join {schema}.coa c on c.acctNumber = l.DedNCode 
                 WHERE ID = (SELECT @@IDENTITY)";
        if(loans.Id > 0) sql = $@"select c.AcctName DedNDesc, l.* from {schema}.Loans l 
                                    left join {schema}.coa c on c.acctNumber = l.DedNCode  
                                  WHERE ID = {loans.Id} ";

        var res = await _sql.FetchData<LoansModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<LoansModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Loans where Id = @Id";
        var data = await _sql.FetchData<LoansModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<LoansModel?>?> _02ByEmpNumbers(string empNumber, string schema, string conn)
    {
        var sql = $@"select  c.AcctName DedNDesc, l.*  from {schema}.Loans l 
                        left join {schema}.coa c on c.acctNumber = l.DedNCode 
                     where EmpNumber = @EmpNumber ";
        var data = await _sql.FetchData<LoansModel?, dynamic>(sql, new { EmpNumber = empNumber }, conn);
        return data;
    }
    
    public async Task<LoansModel?> _03(int id, LoansModel loans, string schema, string conn)
    {
        string sql = $@"Update {schema}.Loans set 
                            DATE        = @Date,  
                            Principal   = @Principal,  
                            AMOUNT      = @Amount,  
                            AMORT       = @Amort, 
                            CHANGEBY    = @ChangeBy,  
                            CHANGEDT    = @ChangedT,  
                            REMARKS     = @Remarks,  
                            payMode     = @PayMode,  
                            payStart    = @PayStart,  
                            payRes      = @PayRes,  
                            cvno        = @Cvno,  
                            P1          = @P1, 
                            P2          = @P2, 
                            P3          = @P3, 
                            P4          = @P4, 
                            P5          = @P5 
                         where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, loans, conn);

        sql = $@" select c.AcctName DedNDesc, l.* from {schema}.Loans l 
                    left join {schema}.coa c on c.acctNumber = l.DedNCode 
                  where l.Id = @Id ;";
        var data = await _sql.FetchData<LoansModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<LoansModel?> _03ChangeStatus(int id, LoansModel loans, string schema, string conn)
    {
        string sql = $@"Update {schema}.Loans set 
                            CHANGEBY    = @ChangeBy,  
                            CHANGEDT    = @ChangedT,  
                            Status      = '{loans.Status}'  
                         where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, loans, conn);
        
        sql = $@" select c.AcctName DedNDesc, l.* from {schema}.Loans l 
                    left join {schema}.coa c on c.acctNumber = l.DedNCode 
                  where l.Id = @Id ;";
        var data = await _sql.FetchData<LoansModel?, dynamic>(sql, new { Id = id }, conn);
        
        
        return data?.FirstOrDefault();
    }

    public async Task<LoansModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Loans where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Loans x where x.Id = @Id ;";
        var data = await _sql.FetchData<LoansModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}