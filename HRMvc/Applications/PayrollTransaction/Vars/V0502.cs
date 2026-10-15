using System.Security.Claims;
using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._20_PayGeneric;
using HRApiLibrary.Models._90_Utils;
using HRMvc.StartupConfig.Library;

namespace HRMvc.Applications.PayrollTransaction.Vars;

public class V0502
{
    public string?              acctType        { get; set; } = "Earnings";
    public string?              cssEarnings     { get; set; } = "bg-white text-primary"; 
    public string?              cssDeductions   { get; set; } = "";
    public string?              cssTax          { get; set; } = "";
    public string?              cssSSS          { get; set; } = "";
    public string?              cssPHIC         { get; set; } = "";
    
    public string?              TRN             { get; set; } = "YYMMPP-00000";
    public string?              Year            { get; set; } = string.Empty;
    public string?              Month           { get; set; } = "XX";
    public string?              Period          { get; set; } = "XX";
    public string?              PayrollgrpCode  { get; set; } = "XXXXX";
    public string?              PayrollgrpName  { get; set; } = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
    public DateTime             AttStart        { get; set; }
    public DateTime             AttEnd          { get; set; }
    
    public bool?                ShowDataEntry           { get; set; } = false;
    public bool?                ShowNewTransaction      { get; set; } = false;

    //---- New Payroll Entry ------------------------------------------------------------------
    public bool                 IsDFecthTransaction     { get; set; }   = true;   
    public bool                 IsDLoadTransaction     { get; set; }    = true;
    //*** New Payroll Entry ******************************************************************

    public CoaModel?                        Coa                 { get; set; } = new() ; 
    public List<YearsModel>                Years               { get; set; } = [] ;
    public List<MonthModel>                Months              { get; set; } = [] ;
    public List<PeriodModel>               Periods             { get; set; } = [] ;
    public List<PayrollgrpModel>           Payrollgrps         { get; set; } = [] ;
    public PayrollgrpModel?                Payrollgrp          { get; set; } = new() ;
    public List<CoaModel?>?                Coas                { get; set; } = [] ;
    public List<GChartofacctModel?>?       AccountsAll          { get; set; } = [] ;
    public List<V0502PayAccountModel?>?    AccountSelecteds    { get; set; } = [] ;
    public List<V0502PayAccountModel?>?    AccountEarnings     { get; set; } = [] ;
    public List<V0502PayAccountModel?>?    AccountDeductions   { get; set; } = [] ;
    
    public List<PaymaindtlModel?>?         Paymaindtls         { get; set; } = [] ;
    public List<V0502PayEmpmasModel>       PayEmpmass          { get; set; } = [];


}

public class V0502PayEmpmasModel
{
    public int          No              { get; set; } = 0;
    public bool         IsSelected      { get; set; } = false;
    public string       EmpNumber       { get; set; } = string.Empty;
    public string       EmpName         { get; set; } = string.Empty;
    public string       Status          { get; set; } = string.Empty; 

}

public class V0502PayAccountModel
{
    public int          No              { get; set; } = 0;
    public bool         IsSelected      { get; set; } = false;
    public string?      AcctNumber      { get; set; } = "";
    public string?      AcctName        { get; set; } = "";
    public string?      AcctType        { get; set; } = "";
    public string?      ShortDesc       { get; set; } = string.Empty;

}


