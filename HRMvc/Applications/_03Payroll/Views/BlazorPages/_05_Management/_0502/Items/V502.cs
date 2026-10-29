using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._20_Pay;

namespace HRMvc;

public class V502
{
    
    public string?              acctType        { get; set; } = "Earnings";
    public string?              cssEarnings     { get; set; } = "bg-white text-primary"; 
    public string?              cssDeductions   { get; set; } = "";
    public string?              cssTax          { get; set; } = "";
    public string?              cssSSS          { get; set; } = "";
    public string?              cssPHIC         { get; set; } = "";
    public bool?                ShowDataEntry   { get; set; } = false;

    public string?              TRN             { get; set; } = "YYMMPP-00000";
    public string?              PayrollgrpCode  { get; set; } = "XXXXX";
    public string?              PayrollgrpName  { get; set; } = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
    public string?              Prd             { get; set; } = "PP";
    public DateTime             AttStart        { get; set; }
    public DateTime             AttEnd          { get; set; }
    
    public CoaModel?                    Coa                 { get; set; } = new() ; 
    public List<CoaModel?>?             Coas                { get; set; } = [] ;
    public List<PaymaindtlModel?>?      Paymaindtls         { get; set; } = [] ;

    //--- Payroll Entry on MBPay tables (PayrollTransaction/_502) ---------------------------------------------------------
    public HRApiLibrary.Models._20_Pay.OPay.OPayrollSession?   Session      { get; set; }
    public HRApiLibrary.Models._20_Pay.OPay.OPayEmpResult?     CurrentEmp   { get; set; }
    public string?                      Msg                 { get; set; } = string.Empty;
    public bool                         IsBusy              { get; set; }
    public bool                         IsProcessed         { get; set; }
    public Func<Task>?                  Changed             { get; set; }       //--- re-render the page after a child changes state

    //--- Upload Excel (same process as 1100/12002) : rows read from the file, split by what they do to the grid ----------
    public bool                         IsUploadExcelOpen   { get; set; }
    public List<HRApiLibrary.Models._20_Pay.OPay.OPayEmpResult> ExcelToUpdate { get; set; } = [];
    public List<HRApiLibrary.Models._20_Pay.OPay.OPayEmpResult> ExcelToAdd    { get; set; } = [];
    public List<HRApiLibrary.Models._20_Pay.OPay.OPayEmpResult> ExcelToRemove { get; set; } = [];
    public List<string>                 ExcelIgnoredCols    { get; set; } = [];
    public bool                         AllowAddFromExcel   { get; set; }
    public bool                         AllowRemoveFromExcel { get; set; }


}
