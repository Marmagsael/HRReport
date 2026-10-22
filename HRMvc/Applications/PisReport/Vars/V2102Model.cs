using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisReport.Vars;

public class V2102Model
{
    public string?              Msg         { get; set; } = "";
    public bool                 IsLoading   { get; set; } = true;
    public List<OEmpmasModel>   Empmass     { get; set; } = []; 
    public List<R2102Model>     RepDtls     { get; set; } = []; 
}

public class R2102Model
{
    public string?   EmpNumber       { get; set; } = ""; 
    public string?   EmpName         { get; set; } = ""; 
    public string?   ClName          { get; set; } = ""; 
    public string?   EmpStat         { get; set; } = ""; 
    public string?   StatusName      { get; set; } = ""; 
    
}
