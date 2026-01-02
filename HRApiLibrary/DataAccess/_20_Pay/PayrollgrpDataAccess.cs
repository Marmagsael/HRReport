using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class PayrollgrpDataAccess : IPayrollgrpDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;
    public PayrollgrpDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PayrollgrpModel?> _01(PayrollgrpModel payrollgrp, string schema, string conn)
    {
        var sql = $@"Insert into {schema}.Payrollgrp (ClNumber,  Name,  RatePerHr,  RatePerDay,  RatePerMonth,  RatePerYr,  Status) values 
                                                        (@ClNumber, @Name, @RatePerHr, @RatePerDay, @RatePerMonth, @RatePerYr, 'A'); 
                        SELECT * FROM {schema}.Payrollgrp WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, payrollgrp, conn);
        
        var id = res.FirstOrDefault()?.Id;
        
        sql = $@"Update {schema}.Payrollgrp set  clNumber = lpad(Id,5,'0') where Id = @Id ";
        await _sql.ExecuteCmd(sql, new{ Id = id }, conn);
        
        sql = $@"SELECT * FROM {schema}.Payrollgrp where Id = @Id ";
        res = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, payrollgrp, conn);
        
        return res.FirstOrDefault();
    }


    public async Task<PayrollgrpModel?> _02(int id, string schema, string conn)
    {
        string  sql     = $@"select  * from {schema}.Payrollgrp where Id = @Id";
        var     data    = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<PayrollgrpModel?>?> _02(string schema, string conn)
    {
        string  sql     = $@"select  * from {schema}.Payrollgrp order by Name";
        var     data    = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new {  }, conn);
        return data;
    }
    
    public async Task<List<PayrollgrpModel?>?> _02Dashboard(string schema, string conn)
    {
        string  sql     = $@"select  *, 00000 EmpCount from {schema}.Payrollgrp 
                             where Status = 'A'   
                             order by Name";
        var     data    = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new {  }, conn);
        return data;
    }
    
    public async Task<List<PayrollgrpModel?>?> _02PayDashboard(string paydb, string pisdb, string conn)
    {

        
        
        string  sql     = $@"with 
                                Pgrp    	AS ( SELECT * FROM {paydb}.payrollgrp pgrp  ),

                                pprd    	AS ( SELECT TRIM(CONCAT(RIGHT(yr,2), Mo, Prd )) AS prd FROM {paydb}.`payrollprd` p WHERE STATUS = 'A' LIMIT 1 ),
 
			                    AllT    	AS (SELECT DISTINCT Trn, STATUS FROM {paydb}.tmptbltran t WHERE LEFT(trn,6) IN (SELECT prd FROM pprd ) ),
			    
                                ClosedT 	AS ( SELECT DISTINCT CAST(RIGHT(trn, CHAR_LENGTH(trn)-7) AS UNSIGNED) AS pgrp, t.Trn, EmpmasId, STATUS AS EmpStatus 
                                                 FROM {paydb}.tmptbltran t
                                                 WHERE LEFT(trn,6) IN (SELECT prd FROM pprd ) AND STATUS IN ('P','L') ), 
                            
                                openT   	AS ( SELECT DISTINCT CAST(RIGHT(trn, CHAR_LENGTH(trn)-7) AS UNSIGNED) AS pgrp, t.Trn, EmpmasId, STATUS AS EmpStatus 
                                                 FROM {paydb}.tmptbltran t
                                                 WHERE LEFT(trn,6) IN (SELECT prd FROM pprd ) AND STATUS = '-' AND trn NOT IN (SELECT trn FROM ClosedT ) ), 
                                         
                            ClosedCnt 	AS (SELECT pgrp AS payrollgrpId , 'Posted'  AS EmpStatus, IFNULL(COUNT(EmpmasId),0) EmpCount FROM ClosedT GROUP BY pgrp ),
                            OpenCnt 	AS (SELECT pgrp AS payrollgrpId , 'Editing' AS EmpStatus, IFNULL(COUNT(EmpmasId),0) EmpCount FROM OpenT GROUP BY pgrp, EmpStatus),                           
                            EmpCnt  	AS (SELECT payrollgrpId,          'New'     AS EmpStatus, IFNULL(COUNT(*),0) AS     EmpCount FROM {pisdb}.deprec d GROUP BY payrollgrpId) 
                            
                            SELECT p.*, 
                                   IFNULL(IF(c.Empstatus IS NULL,o.EmpStatus , c.EmpStatus),'New') AS PayStatus, 
				                   IFNULL(IF(c.EmpCount IS NULL, IF(o.EmpCount IS NULL, e.EmpCount, o.EmpCount) ,c.EmpCount),0) AS EmpCount
                            FROM pgrp p
                                LEFT JOIN ClosedCnt c ON c.PayrollgrpId = p.Id 
                                LEFT JOIN OpenCnt   o ON o.PayrollgrpId = p.Id 
                                LEFT JOIN EmpCnt    e ON e.PayrollgrpId = p.Id ";     
        var     data= await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new { }, conn);
        
        return data;
    }
    
    public async Task<List<TbltranModel?>?> _02CheckToTblTran(string clNumber, string schema, string conn)
    {
        string  sql     = $@"select  * from {schema}.tbltran where right(trn,5) = @ClNumber limit 1 ";
        var     data    = await _sql.FetchData<TbltranModel?, dynamic>(sql, new { ClNumber = clNumber }, conn);
        return data;
    }
    
    public async Task<List<PayrollgrpModel?>?> _02Active(string schema, string conn)
    {
        string sql  = $@"select  * from {schema}.Payrollgrp where Left(Status,1) = 'A'  order by Name ";
        var data    = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new {  }, conn);
        return data;
    }
    public async Task<List<PayrollgrpModel?>?> _02ByCode(string code, string schema, string conn)
    {
        var sql  = $@"select  * from {schema}.Payrollgrp where upper(ClNumber) = @ClNumber  order by Name ";
        var data    = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new { ClNumber=code  }, conn);
        return data;
    }
    
    public async Task<PayrollgrpModel?> _03(int id, PayrollgrpModel payrollgrp, string schema, string conn)
    {
        string sql = $@"Update {schema}.Payrollgrp set 
                            Name            = @Name, 
                            ClNumber        = @ClNumber,
                            RatePerHr       = @RatePerHr,  
                            RatePerDay      = @RatePerDay,  
                            RatePerMonth    = @RatePerMonth,  
                            RatePerYr       = @RatePerYr,  
                            Status          = @Status, 
                            PayRateId       = @PayRateId 
                        where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, payrollgrp, conn);

        sql = $@" select  * from {schema}.Payrollgrp x where x.Id = @Id ;";
        var data = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    

    public async Task<PayrollgrpModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Payrollgrp where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Payrollgrp x where x.Id = @Id ;";
        var data = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
