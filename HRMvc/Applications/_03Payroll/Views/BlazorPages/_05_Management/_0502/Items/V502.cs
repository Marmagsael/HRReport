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

    
    public CoaModel?            Coa             { get; set; } = new() ; 
    public List<CoaModel?>?     Coas            { get; set; } = [] ;

}
