using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.Vars;

public class V12_102TS
{
    public AttreqhdrModel           AttReqhdr           { get; set; } = new(); 
    public List<AttreqhdrModel>     AttReqhdrs          { get; set; } = [];
    public AttreqdtlModel           AttReqdtl           { get; set; } = new(); 
    public List<AttreqdtlModel>     AttReqdtls          { get; set; } = []; 
    public bool                     ShowMyPunches       { get; set; } = false; 
    public bool                     ShowMyReqquest      { get; set; } = false; 
    public bool                     ShowSendForApproval { get; set; } = false; 
    public bool                     ShowCancal          { get; set; } = false; 

    public List<Attpunches1Model>   AttPunches1s        { get; set; } = []; 
    public List<AtttemplateModel>   Atttemplates        { get; set; } = []; 
    public List<AttreqtypeModel>    Attreqtypes         { get; set; } = []; 

}
