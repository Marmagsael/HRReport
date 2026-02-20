using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisReport.Vars;

public class V2103Model
{
    public string           Msg         { get; set; } = ""; 

    public List<OEmpmasModel> Empmass   { get; set; } = []; 
    public List<R2103Model> RepDtls     { get; set; } = []; 
}

public class R2103Model
{
    public string   EmpNumber       { get; set; } = ""; 
    public string   EmpName         { get; set; } = ""; 
    public string   ClName          { get; set; } = ""; 
    public string   EmpStat         { get; set; } = ""; 
    public string   StatusName      { get; set; } = ""; 
    
}
