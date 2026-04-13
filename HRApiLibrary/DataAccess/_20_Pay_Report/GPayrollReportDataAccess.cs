using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._20_PayGeneric;
using System.Security.AccessControl;

namespace HRApiLibrary.DataAccess._20_Pay_Report;

public class GPayrollReportDataAccess : IGPayrollReportDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public GPayrollReportDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<GChartofacctModel?>?> _02s(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Chartofacct order by Type Desc, AcctName ";
        var data = await _sql.FetchData<GChartofacctModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    public async Task<List<GChartofacctModel?>?> _02ByAcctTypes(string acctType, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Chartofacct where AcctType = @AcctType order by AcctName ";
        var data = await _sql.FetchData<GChartofacctModel?, dynamic>(sql, new { AcctType = acctType }, conn);
        return data;
    }


    public async Task<List<RSssPremModel?>?> _02SSSPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm, string acctNumber, string opaydb, string opisdb, string conn)
    {




        string prd = $"{yyyy.ToString().Trim().Substring(2, 2)}{mm}";
        string sql = $@" DROP TEMPORARY TABLE IF EXISTS tTbltran; 
                        DROP TEMPORARY TABLE IF EXISTS tRefssstbl; 
                        DROP TEMPORARY TABLE IF EXISTS tEmplist;

                        CREATE TEMPORARY TABLE tTbltran AS 
                        SELECT TRN, AcctNumber, EmpNumber, Amount FROM {opaydb}.tbltran  WHERE LEFT(trn,4) = @Prd; 

                        CREATE TEMPORARY TABLE tRefssstbl AS 
                        SELECT * FROM {opaydb}.refssstbl WHERE @Yyyy BETWEEN yrstart AND yrend; 

                        CREATE TEMPORARY TABLE tEmplist as 
                        SELECT DISTINCT empnumber FROM tTbltran WHERE RIGHT(trn,5) IN @Pgrps; 

                        SELECT  
                            t.EmpNumber, 
                            e.EmpLastnm, 
                            e.EmpFirstNm, 
                            e.EmpMidNm,  
                            c.ClName Payrollgrp, 
                            IF(e.DateHired IS NULL OR e.DateHired < '1000-01-01', NULL, e.DateHired) AS DateHired,
                            t.Ee,  
                            m.Ecc AS Ec, 
                            m.Er   
                        FROM (
                            SELECT EmpNumber, SUM(Amount) Ee  
                            FROM tTbltran 
                            WHERE AcctNumber = @AcctNumber 
                            GROUP BY EmpNumber
                        ) t 
                        LEFT JOIN {opisdb}.Empmas e ON e.empnumber = t.EmpNumber
                        LEFT JOIN {opisdb}.Client c ON c.ClNumber = e.Client_
                        LEFT JOIN tRefssstbl m ON m.ee = t.Ee 
                        WHERE t.empnumber IN (select * from tEmplist) 
                        Order by EmplastNm, EmpfirstNm; ";


        var data = await _sql.FetchData<RSssPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy, AcctNumber = acctNumber }, conn);
        return data;
    }

    public async Task<List<RPhicPremModel?>?> _02PHICPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm, string acctNumber, string opaydb, string opisdb, string conn)
    {
        string prd = $"{yyyy.ToString().Trim().Substring(2, 2)}{mm}";
        string sql = $@"DROP TEMPORARY TABLE IF EXISTS tTbltran; 
                        DROP TEMPORARY TABLE IF EXISTS t2Tbltran;
                        DROP TEMPORARY TABLE IF EXISTS tEmplist;

                        CREATE TEMPORARY TABLE tTbltran AS 
                        SELECT TRN, AcctNumber, EmpNumber, Amount FROM {opaydb}.tbltran  WHERE LEFT(trn,4) = @Prd; 

                        CREATE TEMPORARY TABLE t2Tbltran AS 
                        SELECT TRN, AcctNumber, EmpNumber, Amount FROM {opaydb}.tbltran  WHERE LEFT(trn,4) = @Prd; 

                        CREATE TEMPORARY TABLE tEmplist as 
                        SELECT DISTINCT empnumber FROM tTbltran WHERE RIGHT(trn,5) IN @Pgrps; 
                        

                        SELECT  
                            t.EmpNumber, 
                            e.EmpLastnm, 
                            e.EmpFirstNm, 
                            e.EmpMidNm,  
                            e.Phic, 
                            e.EmpBirth, 
                            e.Sex_ Gender,
                            t.Ee,  
                            t.Ee AS Er, 
                            t1.Compensation   
                        FROM (SELECT EmpNumber, SUM(Amount) Ee  
                            FROM tTbltran 
                            WHERE AcctNumber = @AcctNumber 
                            GROUP BY EmpNumber
                        ) t 
                        LEFT JOIN {opisdb}.Empmas e ON e.empnumber = t.EmpNumber
                        LEFT JOIN (SELECT EmpNumber, sum(Amount) Compensation 
                                     FROM t2Tbltran 
                                     WHERE AcctNumber = 'E001' 
                                     GROUP BY EmpNumber) t1 on t1.EmpNumber = t.EmpNumber
                        WHERE t.empnumber IN (select * from tEmplist)
                        Order by EmplastNm, EmpfirstNm; ";

        var data = await _sql.FetchData<RPhicPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy, AcctNumber = acctNumber }, conn);
        return data;
    }

    public async Task<List<RPagIbigPremModel?>?> _02PagIbigPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm, string acctNumber, string opaydb, string opisdb, string conn)
    {
        string prd = $"{yyyy.ToString().Trim().Substring(2, 2)}{mm}";
        string sql = $@" DROP TEMPORARY TABLE IF EXISTS tTbltran; 
                        DROP TEMPORARY TABLE IF EXISTS tRefpagibigtbl; 
                        DROP TEMPORARY TABLE IF EXISTS tEmplist;

                        CREATE TEMPORARY TABLE tTbltran AS 
                        SELECT TRN, AcctNumber, EmpNumber, Amount FROM {opaydb}.tbltran WHERE LEFT(trn,4) = @Prd; 

                        CREATE TEMPORARY TABLE tRefpagibigtbl AS 
                        SELECT * FROM {opaydb}.refpagibigtbl WHERE @Yyyy BETWEEN yrstart AND yrend; 

                        CREATE TEMPORARY TABLE tEmplist as 
                        SELECT DISTINCT empnumber FROM tTbltran WHERE RIGHT(trn,5) IN @Pgrps; 

                        SELECT  
                            t.EmpNumber, 
                            e.EmpLastnm, 
                            e.EmpFirstNm, 
                            e.EmpMidNm,  
                            e.PagIbiGNo, 
                            e.EmpBirth, 
                            e.Tin,
                              IF(e.DateHired IS NULL OR e.DateHired < '1000-01-01', NULL, e.DateHired) AS DateHired,
                           IF(e.DateHired IS NULL OR e.DateHired < '1000-01-01', NULL, e.DateHired) AS DateHired,
                            t.Ee,  
                            t.Ee AS Er, 
                            m.Compensation   
                        FROM (
                            SELECT EmpNumber, SUM(Amount) Ee  
                            FROM tTbltran 
                            WHERE AcctNumber = @AcctNumber 
                            GROUP BY EmpNumber
                        ) t 
                        LEFT JOIN {opisdb}.Empmas e ON e.empnumber = t.EmpNumber
                        LEFT JOIN tRefpagibigtbl m ON m.Fee = t.Ee 
                        WHERE t.empnumber IN (select * from tEmplist)
                        Order by EmplastNm, EmpfirstNm; ";  

        var data = await _sql.FetchData<RPagIbigPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy, AcctNumber = acctNumber }, conn);
        return data;
    }

    // --- --------------- 

}


public interface IGPayrollReportDataAccess
{
    Task<List<GChartofacctModel?>?> _02ByAcctTypes(string acctType, string schema, string conn);
    Task<List<GChartofacctModel?>?> _02s(string schema, string conn);
    Task<List<RSssPremModel?>?> _02SSSPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm, string acctType, string opaydb, string opisdb, string conn);
    Task<List<RPhicPremModel?>?> _02PHICPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm, string acctType, string opaydb, string opisdb, string conn);
    Task<List<RPagIbigPremModel?>?> _02PagIbigPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm, string acctType, string opaydb, string opisdb, string conn);

}
