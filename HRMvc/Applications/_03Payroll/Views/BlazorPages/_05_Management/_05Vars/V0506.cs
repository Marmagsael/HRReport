using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._90_Utils;

namespace HRMvc.Applications._03Payroll.Views.BlazorPages._05_Management._05Vars;

public class V0506
{
    public string?                      SystemDb                { get; set; } = string.Empty;
    public string?                      userdb                  { get; set; } = string.Empty;
    public string?                      pisdb                   { get; set; } = string.Empty;
    public string?                      paydb                   { get; set; } = string.Empty;
    public string?                      conn                    { get; set; } = string.Empty;
    public string?                      Module                  { get; set; } = "Pay";
                                                                
    public string?                      Action                  { get; set; } = string.Empty;
    public int                          SelectedSystemId        { get; set; } = 0; 
    public string?                      SearchMsg               { get; set; } = string.Empty;
    public string?                      Email                   { get; set; } = string.Empty;
    public SystemuserModel?             LocalUser               { get; set; } = new();
    public List<SystemuserModel?>?      LocalUsers              { get; set; } = new();
    public UsersModel?                  MainUser                { get; set; } = new(); 
    public PisEmpmasModel?              PisEmpmas               { get; set; } = new(); 

    public List<SystemuserModel?>?      ActiveSystemUsers       { get; set; } = new();
    public List<SystemuserModel?>?      InvitedSystemUsers      { get; set; } = new();
    public List<MenusModel?>?           Menus                   { get; set; } = new();
    
    public string?                      UserRemarks             { get; set; } = "-";

}