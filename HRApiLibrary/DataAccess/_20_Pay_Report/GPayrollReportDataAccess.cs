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
        string sql  = $@"select  * from {schema}.Chartofacct where AcctType = @AcctType order by AcctName ";
        var data    = await _sql.FetchData<GChartofacctModel?, dynamic>(sql, new { AcctType = acctType }, conn);
        return data;
    }
   
    
    public async Task<List<RSssPremModel?>?> _02SSSPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm,  string acctNumber, string opaydb,  string opisdb, string conn)
    {
        string prd = $"{yyyy.ToString().Trim().Substring(2,2)}{mm}"; 
        string sql  = $@"
                            select  t.EmpNumber, e.EmpLastnm, e.EmpFirstNm, e.EmpMidNm, c.ClName Payrollgrp, 
                                    e.DateHired, t.Ee, m.Ecc Ec, m.Er, m.Compensation  from
                                (SELECT EmpNumber, sum(Amount) Ee  FROM {opaydb}.tbltran t
                                    where AcctNumber = @AcctNumber and left(trn,4) = @Prd 
                                    group by EmpNumber) t

                                left join {opisdb}.Empmas e on e.empnumber = t.EmpNumber
                                left join {opisdb}.Client c on c.ClNumber = e.Client_
                                left join (SELECT * FROM {opaydb}.refssstbl where @Yyyy between yrstart and yrend) m on m.ee = t.ee  
                            where t.empnumber in (select empnumber  FROM {opaydb}.tbltran 
                                                    where AcctNumber = @AcctNumber and left(trn,4) = @Prd and right(trn,5) in @Pgrps ) ";
        var data    = await _sql.FetchData<RSssPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy, AcctNumber=acctNumber }, conn);
        return data;
    }

    public async Task<List<RPhicPremModel?>?> _02PHICPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm, string acctNumber, string opaydb, string opisdb, string conn)
    {
        string prd = $"{yyyy.ToString().Trim().Substring(2, 2)}{mm}";
        string sql = $@"
                            select  t.EmpNumber, e.EmpLastnm, e.EmpFirstNm, e.EmpMidNm,  e.EmpBirth, E.Sex_ Gender, 
                                    e.Phic, t.Ee,  t.Ee Er, t1.Compensation  from
                                (SELECT EmpNumber, sum(Amount) Ee  FROM {opaydb}.tbltran t
                                    where AcctNumber = @AcctNumber and left(trn,4) = @Prd 
                                    group by EmpNumber) t

                                left join {opisdb}.Empmas e on e.empnumber = t.EmpNumber
                                left join (SELECT EmpNumber, sum(Amount) Compensation  FROM {opaydb}.tbltran t
                                     where AcctNumber = 'E001' and left(trn,4) = @Prd 
                                     group by EmpNumber) t1 on t1.empnumber = t.empnumber
                                left join {opisdb}.Client c on c.ClNumber = e.Client_
                            where t.empnumber in (select DISTINCT empnumber  FROM {opaydb}.tbltran 
                                                    where left(trn,4) = @Prd and right(trn,5) in @Pgrps ) ";
        var data = await _sql.FetchData<RPhicPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy, AcctNumber = acctNumber }, conn);
        return data;
    }

    public async Task<List<RPagIbigPremModel?>?> _02PagIbigPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm, string acctNumber, string opaydb, string opisdb, string conn)
    {
        string prd = $"{yyyy.ToString().Trim().Substring(2, 2)}{mm}";
        string sql = $@" DROP TEMPORARY TABLE IF EXISTS tTbltran; 
                         DROP TEMPORARY TABLE IF EXISTS tRefpagibigtbl; 
                          
                          CREATE TEMPORARY TABLE tTbltran AS 
                                select * from {opaydb}.tbltran where left(trn,4) = @Prd; 

                           Create Temporary table tRefpagibigtbl as 
                           SELECT * FROM {opaydb}.refpagibigtbl where @Yyyy between yrstart and yrend; 

                          
                            select  t.EmpNumber, e.EmpLastnm, e.EmpFirstNm, e.EmpMidNm,  e.PagIbiGNo, e.EmpBirth, e.Tin,
                                    e.DateHired, t.FEe,  t.FEe FEr, m.Compensation   
                            from (SELECT EmpNumber, sum(Amount) Ee  FROM tTbltran where AcctNumber = @AcctNumber group by EmpNumber ) t 
                                left join {opisdb}.Empmas e on e.empnumber  = t.EmpNumber
                                left join {opisdb}.Client c on c.ClNumber   = e.Client_
                                left join tRefpagibigtbl m on m.Fee         = t.ee 
                            where t.empnumber in (select distinct empnumber from tTbltran  where right(trn,5) in @Pgrps ) ";  

        var data = await _sql.FetchData<RPagIbigPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy, AcctNumber = acctNumber }, conn);
        return data;
    }

}


public interface IGPayrollReportDataAccess
{
    Task<List<GChartofacctModel?>?>     _02ByAcctTypes(string acctType, string schema, string conn);
    Task<List<GChartofacctModel?>?>     _02s(string schema, string conn);
    Task<List<RSssPremModel?>?>         _02SSSPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm,  string acctType, string opaydb,  string opisdb, string conn); 
    Task<List<RPhicPremModel?>?>        _02PHICPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm,  string acctType, string opaydb,  string opisdb, string conn); 
    Task<List<RPagIbigPremModel?>?>     _02PagIbigPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm,  string acctType, string opaydb,  string opisdb, string conn); 
    
}
