using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class PayrollprdDataAccess : IPayrollprdDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PayrollprdDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PayrollprdModel?> _01(PayrollprdModel payrollprd, string schema, string conn)
    {
        
        
        string sql = $@"Update {schema}.Payrollprd set Status = 'O' where Status = 'A'; 
                        Insert into {schema}.Payrollprd 
                            (Yr,  Mo,  Prd,  Openby,  DateOpened,  Status) values 
                            (@Yr, @Mo, @Prd, @Openby, now(),       'A')";
        
        
        await _sql.ExecuteCmd<dynamic>(sql, payrollprd, conn);

        sql = $@"SELECT * FROM {schema}.Payrollprd WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<PayrollprdModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<PayrollprdModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Yr, Mo, Prd, Openby, DateOpened, Closedby, DateClosed, Status 
                        from {schema}.Payrollprd where Id = @Id";
        var data = await _sql.FetchData<PayrollprdModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<PayrollprdModel?> _02(string schema, string conn)
    {
        string sql = $@"select  Id, Yr, Mo, Prd, Openby, DateOpened, Closedby, DateClosed, Status 
                        from {schema}.Payrollprd order by Yr desc, Mo desc, Prd";
        var data = await _sql.FetchData<PayrollprdModel?, dynamic>(sql, new {  }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<PayrollprdModel?>?> _02PerYr(int yr, string schema, string conn)
    {
        var sql = $@"select  Id, Yr, Mo, Prd, Openby, DateOpened, Closedby, DateClosed, Status 
                        from {schema}.Payrollprd
                        where Yr = @Yr 
                        order by Yr desc, Mo desc, Prd ";
        var data = await _sql.FetchData<PayrollprdModel?, dynamic>(sql, new { Yr=yr }, conn);
        return data;
    }
    
    public async Task<List<PayrollprdModel?>?> _02Open(string schema, string conn)
    {
        string sql = $@"select distinct  Id, Yr, Mo, Prd, Openby, DateOpened, Closedby, DateClosed, Status 
                        from {schema}.Payrollprd 
                        where Status in ('O','A','P')
                        order by Yr desc, Mo desc, Prd ";
        var data = await _sql.FetchData<PayrollprdModel?, dynamic>(sql, new {  }, conn);
        return data;
    }
    public async Task<List<PayrollprdModel?>?> _02OpenPerMonth(string schema, string conn)
    {
        var sql = $@"select  distinct Yr, Mo, Prd from {schema}.Payrollprd 
                        where Status in ('O','A') order by Yr desc, Mo desc, Prd ";
        var data = await _sql.FetchData<PayrollprdModel?, dynamic>(sql, new {  }, conn);
        return data;
    }
    
    public async Task<PayrollprdModel?> _03(int id, PayrollprdModel payrollprd, string schema, string conn)
    {
        string sql = $@"Update {schema}.Payrollprd set 
                                Yr          = @Yr,  
                                Mo          = @Mo,  
                                Prd         = @Prd,  
                                Openby      = @Openby,  
                                DateOpened  = @DateOpened,  
                                Closedby    = @Closedby,  
                                DateClosed  = @DateClosed,  
                                Status      = @Status where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, payrollprd, conn);

        sql = $@" select  * from {schema}.Payrollprd x where x.Id = @Id ;";
        var data = await _sql.FetchData<PayrollprdModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task _03SetActive(int id, string schema, string conn)
    {
        var sql = $@"Update {schema}.Payrollprd set Status = 'O' where Status   = 'A';
                        Update {schema}.Payrollprd set Status = 'A' where Id       = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new {Id = id }, conn);
    }
    
    public async Task _03LockPeriod(int payrollprdId, int userId, string schema, string conn)
    {
        var sql = $"select * from {schema}.payrollprd where Id = @Id";
        var res = await _sql.FetchData<PayrollprdModel?, dynamic>(sql, new { Id = payrollprdId }, conn);
        if (res.Count > 0)
        {

            var yr       = res.FirstOrDefault()?.Yr; 
            var mo     = res.FirstOrDefault()?.Mo;
            var prd    = res.FirstOrDefault()?.Prd;
            var trnStr = yr.ToString()?.Trim();

            var trn = trnStr?.Substring(trnStr.Length - 3, 2) + mo?.Trim() ;

            sql = $@"Delete from {schema}.Tmptbltran where left(trn,4) = @Trn; 
                     Update {schema}.Payrollprd set Status = 'L', Closedby = @UserId, DateClosed = now() 
                     where Yr = {yr} and Mo = '{mo}' ;";
            await _sql.ExecuteCmd<dynamic>(sql, new {Trn = trn, UserId = userId, Yr =yr, Mo=mo }, conn);
        }
        
    }
    
    public async Task<PayrollprdModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Payrollprd where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Payrollprd x where x.Id = @Id ;";
        var data = await _sql.FetchData<PayrollprdModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
