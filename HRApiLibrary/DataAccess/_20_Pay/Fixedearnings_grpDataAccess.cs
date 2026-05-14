using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;


namespace HRApiLibrary.DataAccess._20_Pay;

public class Fixedearnings_grpDataAccess : IFixedearnings_grpDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public Fixedearnings_grpDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<Fixedearnings_grpModel?> _01(Fixedearnings_grpModel fixedearnings_grp, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Fixedearnings_grp 
                            (PayrollgrpId,  DStart,  DEnd,  AcctNumber,  Amount,  Status,  CreatedbyId,  TerminatedbyId,  DaysPara,  P1, P2, P3, P4, P5, PerdayEarnings, TRNPosted) values 
                            (@PayrollgrpId, @DStart, @DEnd, @AcctNumber, @Amount, @Status, @CreatedbyId, @TerminatedbyId, @DaysPara, @P1, @P2, @P3, @P4, @P5, @PerdayEarnings, @TRNPosted)";
        await _sql.ExecuteCmd<dynamic>(sql, fixedearnings_grp, conn);
        
        sql = $@"select  c.AcctName, f.* 
                        from {schema}.Fixedearnings_grp f
                        left join {schema}.Coa c on c.AcctNumber = f.AcctNumber WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<Fixedearnings_grpModel?, dynamic>(sql, new { }, conn);
        
        return res.FirstOrDefault();
    }


    public async Task<Fixedearnings_grpModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  c.AcctName, f.* 
                        from {schema}.Fixedearnings_grp f
                        left join {schema}.Coa c on c.AcctNumber = f.AcctNumber  
                        where Id = @Id";
        var data = await _sql.FetchData<Fixedearnings_grpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<Fixedearnings_grpModel?>?> _02_Active(string schema, string conn)
    {
        string sql = $@"select  c.AcctName, f.* 
                        from {schema}.Fixedearnings_grp f
                        left join {schema}.Coa c on c.AcctNumber = f.AcctNumber  
                        where Status = 'A'";
        var data = await _sql.FetchData<Fixedearnings_grpModel?, dynamic>(sql, new {  }, conn);
        return data;
    }
    
    public async Task<List<Fixedearnings_grpModel?>?> _02_Active(string fld, string schema, string conn)
    {
        string sql = $@"select  c.AcctName, f.* 
                        from {schema}.Fixedearnings_grp f
                        left join {schema}.Coa c on c.AcctNumber = f.AcctNumber  
                        where Status = 'A'";
        var data = await _sql.FetchData<Fixedearnings_grpModel?, dynamic>(sql, new {  }, conn);
        return data;
    }
    
    public async Task<List<Fixedearnings_grpModel?>?> _02_ByPgrpId_Active(int pgrpId, string schema, string conn)
    {
        string sql = $@"select  c.AcctName, f.* 
                            from {schema}.Fixedearnings_grp f
                            left join {schema}.Coa c on c.AcctNumber = f.AcctNumber  
                        where Status = 'A' and PayrollgrpId = @PayrollgrpId";
        var data = await _sql.FetchData<Fixedearnings_grpModel?, dynamic>(sql, new {  PayrollgrpId = pgrpId}, conn);
        return data;
    }
    

    public async Task<Fixedearnings_grpModel?> _03(int id, Fixedearnings_grpModel fixedearnings_grp, string schema, string conn)
    {
        string sql = $@"Update {schema}.Fixedearnings_grp set 
                              PayrollgrpId      = @PayrollgrpId, 
                              DStart            = @DStart, 
                              DEnd              = @DEnd, 
                              AcctNumber        = @AcctNumber, 
                              Amount            = @Amount, 
                              CreatedbyId       = @CreatedbyId, 
                              TerminatedbyId    = @TerminatedbyId,
                              DaysPara          = @DaysPara, 
                              P1                = @P1, 
                              P2                = @P2, 
                              P3                = @P3, 
                              P4                = @P4, 
                              P5                = @P5,
                              Status            = @Status, 
                              PerdayEarnings    = @PerdayEarnings, 
                              TRNPosted         = @TRNPosted where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, fixedearnings_grp, conn);

        sql = $@"select  c.AcctName, f.* 
                    from {schema}.Fixedearnings_grp f
                    left join {schema}.Coa c on c.AcctNumber = f.AcctNumber  
                 where Id = @Id";
        var data = await _sql.FetchData<Fixedearnings_grpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<Fixedearnings_grpModel?> _03_Terminate(int id, int terminatedbyId, string schema, string conn)
    {
        string sql = $@"Update {schema}.Fixedearnings_grp set 
                            DEnd              = now(), 
                            TerminatedbyId    = @TerminatedbyId,
                            Status            = 'T', 
                        where Id = @Id;

                        Delete from {schema}.fixedearnings_grp_emp where FixedEarnings_grpId = @Id; 
                        
                       Select  c.AcctName, f.* 
                            from {schema}.Fixedearnings_grp f
                            left join {schema}.Coa c on c.AcctNumber = f.AcctNumber  
                        where Id = @Id ";
        var data = await _sql.FetchData<Fixedearnings_grpModel?, dynamic>(sql, new { Id = id, TerminatedbyId = terminatedbyId}, conn);
        return data?.FirstOrDefault();
    }

    public async Task<Fixedearnings_grpModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Fixedearnings_grp where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@"select  c.AcctName, f.* 
                    from {schema}.Fixedearnings_grp f
                    left join {schema}.Coa c on c.AcctNumber = f.AcctNumber  
                 where Id = @Id";
        var data = await _sql.FetchData<Fixedearnings_grpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
