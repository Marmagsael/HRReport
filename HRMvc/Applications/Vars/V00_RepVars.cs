using System;

namespace HRMvc.Applications.Vars;

public class V00_RepVars
{
    public  int      RefreshToken    { get; set; } = 0; 
    public  string   SelectedFormat  { get; set; } = "PDF"; 
    public  string   RelativePath    { get; set; } = "Reports/_03Payroll/m1152.trdp"; 
    public  Dictionary<string, object>  
            ReportParams             { get; set; } = new() {   
                ["CoName"]      = "Morpheusbox Inc.", 
                ["pPrintedBy"]  = "SYSTEM", 
                ["pPrintDate"]  = DateTime.Now };
}
