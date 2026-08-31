using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.Vars;

public class V12_102CS
{
    public AtttemplateModel                 Atttemplate             { get; set; } = new();
    public List<AtttemplateModel>           Atttemplates            { get; set; } = [];
    public AtttemplatereqhdrModel           Atttemplatereqhdr       { get; set; } = new();
    public List<AtttemplatereqhdrModel?>    Atttemplatereqhdrs      { get; set; } = [];
    public AtttemplatereqdtlModel           Atttemplatereqdtl       { get; set; } = new();
    public List<AtttemplatereqdtlModel>     Atttemplatereqdtls      { get; set; } = [];

    public bool                             ShowMyReqquest          { get; set; } = false; 
    public bool                             ShowSendForApproval     { get; set; } = false; 
    public bool                             ShowCancel              { get; set; } = false; 
    public bool                             ShowLoadTransaction     { get; set; } = false; 

    
    public string?                   Action              { get; set; } = string.Empty;
}