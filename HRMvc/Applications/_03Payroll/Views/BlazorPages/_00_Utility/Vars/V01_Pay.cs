using HRApiLibrary.DataAccess._00_Main;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.Models._00_Main;
using System.Runtime.CompilerServices;

namespace HRMvc.Applications._03Payroll.Views.BlazorPages._00_Utility.Vars; 

public class V01_Pay
{
    private readonly I_00UserscompanyDataAccess  _usercompany;
    private readonly I_00MainDA                 _main;

    public V01_Pay(I_00UserscompanyDataAccess usercompany, I_00MainDA main)
    {
        _usercompany = usercompany;
        _main = main;
    }


    public string?              Action          { get; set; } = string.Empty;
    public UserClaimsModel?     UserClaims      { get; set; } = new();
    public UserCompanyModel?    UserCompany     { get; set; } = new();

    
}
