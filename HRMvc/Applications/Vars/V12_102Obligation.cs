using System;
using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.Vars;

public class V12_102Obligation
{
    public AttreqhdrModel               Attreqhdr       { get; set; } = new();
    public List<AttreqhdrModel?>?       Attreqhdrs      { get; set; } = []; 

    public AttreqdtlModel               Attreqdtl       { get; set; } = new(); 
    public List<AttreqdtlModel?>?       Attreqdtls      { get; set; } = []; 
    public List<AtttemplateModel?>?     Atttemplates    { get; set; } = [];
    public List<AttreqtypeModel>        Attreqtypes     { get; set; } = []; 

    public string                       Action          { get; set; } = string.Empty;
     
}
