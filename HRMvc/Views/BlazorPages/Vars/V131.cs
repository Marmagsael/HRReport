using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._90_Utils;

namespace HRMvc.Views.BlazorPages.Vars;

public class V131
{
    public string?                  pisdb           { get; set; } = string.Empty;
    public string?                  paydb           { get; set; } = string.Empty;
    public string?                  conn            { get; set; } = string.Empty;
    public int                      userId          { get; set; } = 0; 
    public string?                  Action          { get; set; } = string.Empty;
    public PisEmpmasModel?          PisEmpmas       { get; set; } = new(); 
    public string?                  MacAddress      { get; set; } = string.Empty;
    public int                      TimezoneId      { get; set; } = 0;
    public UserCompanyModel?        UserCompany     { get; set; } = new UserCompanyModel();
    public AtttemplateModel?        AttTemplate     { get; set; } = new();
    public AttdailyModel?           CurrPunch       { get; set; } = new AttdailyModel();
    public AttdailyModel?           PrevPunch       { get; set; } = new AttdailyModel();
    public int                      DtrYr           { get; set; }  = DateTime.Now.Year;
    public int                      DtrMo           { get; set; }  = DateTime.Now.Month;
    public List<MyDTRModel?>?       MyDTR           { get; set; } = new(); 

}

