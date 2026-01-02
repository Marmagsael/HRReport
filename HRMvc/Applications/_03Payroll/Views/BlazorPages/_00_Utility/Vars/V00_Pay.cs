using HRApiLibrary.Models._00_Main;
namespace HRMvc.Applications._03Payroll.Views.BlazorPages._00_Utility.Vars; 

public class V00_Pay
{
    
    public string?                      paydb               { get; set; } = String.Empty;
    public string?                      pisdb               { get; set; } = String.Empty;
    public string?                      conn                { get; set; } = String.Empty;
    
    public UserClaimsModel?             UserClaims          { get; set; } = new();
    public string?                      Action              { get; set; } = string.Empty;
    public UserCompanyModel?            UserCompany         { get; set; } = new(); 

    public bool                         IsModalVisible      { get; set; } = false;
    public bool                         ShowSearchEmployee  { get; set; } = false;

    public List<Uc_accessreqModel?>?    Uc_acccessreqs      { get; set; } = new(); 

}
