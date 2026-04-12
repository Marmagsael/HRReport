using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.Vars;

public class V12_102Lv
{

    public LeaveapplicationModel            LeaveApplication        { get; set; } = new();        
    public List<LeaveapplicationModel>?     LeaveApplications       { get; set; } = new();        
    public LeaveapplicationdtlModel         LeaveApplicationdtl     { get; set; } = new();  
    public List<LeaveapplicationdtlModel>   LeaveApplicationdtls    { get; set; } = [];  
    public List<LeavetypeModel>             LeaveTypes              { get; set; } = [];    
    public List<TimeoptionModel>?           Hrs                     { get; set; } = [];   
    public List<LeavegrpapproverModel>      LvFirstApprovers        { get; set; } = [];
    public List<LeavegrpapproverModel>      LvFinalApprovers        { get; set; } = [];

       
    
    public List<int>?                       LvYrs                   { get; set; } = [];   
    public List<TimedurationModel>?         Durations               { get; set; } = [];   
    public List<string>?                    DutyType               { get; set; } = ["R","RD"];   
    public AtttemplateModel                 AttTemplate             { get; set; } = new();                  
    public bool                             ShowMyReqquest          { get; set; } = false; 
    public bool                             ShowSendForApproval     { get; set; } = false; 
    public bool                             ShowCancel              { get; set; } = false; 
    public bool                             ShowLoadTransaction     { get; set; } = false; 
    public bool                             ShowPrint               { get; set; } = false; 
    public string                           Action                  { get; set; } = string.Empty;


    // ------------------------------------------------------
    
}