using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.Vars;

public class V12_102Lv
{

    public LeaveapplicationModel        LeaveApplication        { get; set; } = new();        
    public LeaveapplicationModel        LeaveApplicationdtl     { get; set; } = new();  
    public List<LeavetypeModel>         LeaveTypes              { get; set; } = [];       

    public bool                         ShowMyReqquest          { get; set; } = false; 
    public bool                         ShowSendForApproval     { get; set; } = false; 
    public bool                         ShowCancel              { get; set; } = false; 
    public bool                         ShowLoadTransaction     { get; set; } = false; 
    public string                           Action                  { get; set; } = string.Empty;
}