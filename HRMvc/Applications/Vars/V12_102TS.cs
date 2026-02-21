using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.Vars;

public class V12_102TS
{
    public AttreqhdrModel           AttReqhdr   { get; set; } = new(); 
    public List<AttreqhdrModel>     AttReqhdrs  { get; set; } = [];
    public AttreqdtlModel           AttReqdtl   { get; set; } = new(); 
    public List<AttreqdtlModel>     AttReqdtls  { get; set; } = []; 

}
