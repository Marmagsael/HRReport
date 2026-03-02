using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.Vars;

public class V12_102OT
{
    public OtreqhdrModel           OTReqhdr                 { get; set; } = new(); 
    public List<OtreqhdrModel>     OTReqhdrs                { get; set; } = [];
    public OtreqdtlModel           OTReqdtl                 { get; set; } = new(); 
    public List<OtreqdtlModel>     OTReqdtls                { get; set; } = []; 
    public bool                     ShowMyPunches           { get; set; } = false; 
    public bool                     ShowMyReqquest          { get; set; } = false; 
    public bool                     ShowSendForApproval     { get; set; } = false; 
    public bool                     ShowCancel              { get; set; } = false; 
    public bool                     ShowLoadTransaction     { get; set; } = false; 

    public List<OtdutytypeModel>    OTDutyType              { get; set; }   = []; 
    public List<OtdaytypeModel>     OTDayType               { get; set; }   = []; 
    public List<AttdutytypeModel>   Attdutytypes            { get; set; }   = []; 



    public List<Attpunches1Model>   AttPunches1s        { get; set; } = []; 
    public List<AtttemplateModel>   Atttemplates        { get; set; } = []; 
    // public List<AttreqtypeModel>    Attreqtypes         { get; set; } = []; 

    public string                   Action              { get; set; } = string.Empty;

}
