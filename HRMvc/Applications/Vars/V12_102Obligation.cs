using System;
using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.Vars;

public class V12_102Obligation
{
    public AttreqhdrModel               Attreqhdr       { get; set; } = new();
    public List<AttreqhdrModel?>?       Attreqhdrs      { get; set; } = []; 

    public AttreqdtlModel               Attreqdtl       { get; set; } = new(); 
    public List<AttreqdtlModel?>?       Attreqdtls      { get; set; } = []; 
    public AtttemplateModel             Atttemplate     { get; set; } = new();
    public List<AtttemplateModel?>?     Atttemplates    { get; set; } = [];
    public List<AttreqtypeModel>        Attreqtypes     { get; set; } = []; 
    public string                       Action          { get; set; } = string.Empty;


    public bool                         ShowOTApproval              { get; set; } = false;
    public OtreqhdrModel                Otreqhdr                    { get; set; } = new();
    public List<OtreqhdrModel>          Otreqhdrs                   { get; set; } = [];
    public List<OtreqdtlModel>          Otreqdtls                   { get; set; } = [];
    public List<Attpunches1Model>       Attpunches1s                { get; set; } = [];
    public List<OtdaytypeModel>         Otdaytypes                  { get; set; } = [];
    public List<OtdutytypeModel>        Otdutytypes                 { get; set; } = [];

    
    public bool                         ShowAttTemplateApproval     { get; set; } = false;
    public AtttemplatereqhdrModel       Atttemplatereqhdr           { get; set; } = new();
    public List<AtttemplatereqhdrModel> Atttemplatereqhdrs          { get; set; } = [];
    public AtttemplatereqdtlModel       Atttemplatereqdtl           { get; set; } = new();
    public List<AtttemplatereqdtlModel> Atttemplatereqdtls          { get; set; } = [];

    
    public bool                         ShowLvApproval              { get; set; } = false;
    public LeaveapplicationModel        LvApp                       { get; set; } = new();
    public List<LeaveapplicationModel>  LvApps                      { get; set; } = [];



     
}
