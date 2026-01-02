using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using MyApp.Namespace;

namespace HRMvc.Views.BlazorPages.Vars; 

public class V0132
{
    public  string?                     Pisdb                   { get; set; } = string.Empty;
    public  string?                     Paydb                   { get; set; } = string.Empty;
    public  string?                     Conn                    { get; set; } = string.Empty;
    public  string?                     Action                  { get; set; } = string.Empty;

    public string?                       cssVLink               { get; set; } = "disabled-div";  
    public string?                       cssVLeaveGrp           { get; set; } = "disabled-div";  
    public string?                       cssVApprover           { get; set; } = "disabled-div";  
    public string?                       cssVAnniversaryDate    { get; set; } = "disabled-div";  
    


    public PissettingsModel?            PisSettings         { get; set; } = new();
    public List<LeavetypeModel?>?       LeaveTypes          { get; set; } = new();
    public LeavetypeModel               LeaveType           { get; set; } = new();

    public LeaveapplicationModel        Leaveapplication    { get; set; } = new();
    public List<LeaveapproverModel>     Leaveapprovers      { get; set; } = new();

    public List<PisEmpmasModel?>?       PisEmpmass          { get; set; } = null; 
    public PisEmpmasModel?              PisEmpmas           { get; set; } = new(); 
    public EmpmasModel?                 Empmas              { get; set; } = new(); 
    public DeprecModel                  Deprec              { get; set; } = new();

}
