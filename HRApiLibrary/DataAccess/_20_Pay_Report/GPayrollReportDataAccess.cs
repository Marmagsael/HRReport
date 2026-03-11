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
   
    public async Task<List<RSssPremModel?>?> _02SSSPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm,  string opaydb,  string opisdb, string conn)
    {
        string prd = $"{yyyy.ToString().Trim().Substring(2,2)}{mm}"; 
        string sql  = $@"
                            select  t.EmpNumber, e.EmpLastnm, e.EmpFirstNm, e.EmpMidNm, c.ClName Payrollgrp, 
                                    e.DateHired, t.Ee, m.Ecc Cc, m.Er, m.Compensation  from
                                (SELECT EmpNumber, sum(Amount) Ee  FROM {opaydb}.tbltran t
                                    where AcctNumber = 'D002' and left(trn,4) = @Prd and right(trn,5) in @Pgrps
                                    group by EmpNumber) t

                                left join {opisdb}.Empmas e on e.empnumber = t.EmpNumber
                                left join {opisdb}.Client c on c.ClNumber = e.Client_
                                left join (SELECT * FROM {opaydb}.refssstbl where @Yyyy between yrstart and yrend) m on m.ee = t.ee ";
        var data    = await _sql.FetchData<RSssPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy }, conn);
        
        Console.WriteLine($"Count {data.Count}");
        return data;
    }

    public async Task<List<RSssPremModel?>?> _02Tbltran_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm,  string acctType, string opaydb,  string opisdb, string conn)
    {
        string prd = $"{yyyy.ToString().Trim().Substring(2,2)}{mm}"; 
        string sql  = $@"
                            select  t.EmpNumber, e.EmpLastnm, e.EmpFirstNm, e.EmpMidNm, c.ClName Payrollgrp, 
                                    e.DateHired, t.Ee, m.Ecc Cc, m.Er, m.Compensation  from
                                (SELECT EmpNumber, sum(Amount) Ee  FROM {opaydb}.tbltran t
                                    where AcctNumber = @AcctType and left(trn,4) = @Prd and right(trn,5) in @Pgrps
                                    group by EmpNumber) t

                                left join {opisdb}.Empmas e on e.empnumber = t.EmpNumber
                                left join {opisdb}.Client c on c.ClNumber = e.Client_
                                left join (SELECT * FROM {opaydb}.refssstbl where @Yyyy between yrstart and yrend) m on m.ee = t.ee ";
        var data    = await _sql.FetchData<RSssPremModel?, dynamic>(sql, new { Prd = prd, Pgrps = pgrps, Yyyy = yyyy, AcctType=acctType }, conn);
        return data;
    }

}


public interface IGPayrollReportDataAccess
{
    Task<List<GChartofacctModel?>?>     _02ByAcctTypes(string acctType, string schema, string conn);
    Task<List<GChartofacctModel?>?>     _02s(string schema, string conn);
    Task<List<RSssPremModel?>?>         _02SSSPrem_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm,  string opaydb,  string opisdb, string conn); 
    Task<List<RSssPremModel?>?> _02Tbltran_ByPGrps_ByYYMM(List<string> pgrps, int yyyy, string mm,  string acctType, string opaydb,  string opisdb, string conn); 
    
}
