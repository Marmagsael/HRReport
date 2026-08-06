using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers;

[Route("14")]
public class PisReportController : Controller
{
    // Management
    private static readonly Dictionary<string, string> ManagementViews = new()
    {
        ["2002"] = "_2002_UserManagement",
        ["2003"] = "_2003_TimeOffSettings",
        
        
    };

    // Data Entry
    private static readonly Dictionary<string, string> DataEntryViews = new()
    {
        ["2052"] = "_2052_EmployeeEntry"
    };

    // Transaction Views 
    private static readonly Dictionary<string, string> TransactionViews = new()
    {
        ["2302"] = "_2302_StatusMgt",
        ["2303"] = "_2303_Deployment",
        ["2304"] = "_2304_Deviation",
        ["2305"] = "_2305_DisciplinaryAction",
        ["2306"] = "_2306_Reinstatement",
        ["2307"] = "_2307_ChangeDeployment",
        ["2308"] = "_2308_GroupRecall",
    };


    // Reports 
    private static readonly Dictionary<string, string> ReportViews = new()
    {
        ["2102"] = "_2102_EmployeeMasterList",
        ["2103"] = "_2103_EmployeeStatusReport",
        ["2104"] = "_2104_EmployeeAgeReport",
        ["2105"] = "_2105_NewlyHiredfortheMonth",
        ["2106"] = "_2106_HireDateReport",
        ["2107"] = "_2107_ForRegularization",
        ["2108"] = "_2108_ResignedPersonnelfortheMonth",
        ["2109"] = "_2109_ManpowerMovement",
        ["2110"] = "_2110_InsurancePolicy",

        ["2202"] = "_2202_ClientGuardDetail",
        ["2203"] = "_2203_EmployeeClearance",
        ["2204"] = "_2204_PNPSAGSDReport",
        ["2205"] = "_2205_FEDReport",
        ["2206"] = "_2206_GroupDDO",
        ["2207"] = "_2207_RTUReport",
        ["2208"] = "_2208_RecalledEmployeeReport",
        ["2209"] = "_2209_AssignmentHistory",
        ["2210"] = "_2210_ClientContractExpirationReport",
        ["2211"] = "_2211_UniformQuery",
        ["2212"] = "_2212_FloatingEmployees",
        ["2213"] = "_2213_OnLeaveEmployees",
        ["2214"] = "_2214_DetailedDeviationReport",
        ["2215"] = "_2215_SummarizedDeviationReport",
        ["2216"] = "_2216_LicenseVerification",
        ["2217"] = "_2217_LicenseExpiration",
    };

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Applications/PisReport/Views/Pages/Index.cshtml");
    }

    [HttpGet("{pisCode:int}")]
    public IActionResult Report(int? pisCode)
    {
        var key = pisCode.ToString();

        if (ManagementViews.TryGetValue(key, out var mgmtView)) return View($"~/Applications/PisModules/Views/Pages/{mgmtView}.cshtml");

        if (DataEntryViews.TryGetValue(key, out var deView)) return View($"~/Applications/PisModules/Views/Pages/{deView}.cshtml");
        
        if (TransactionViews.TryGetValue(key, out var trntView)) return View($"~/Applications/PisModules/Views/Pages/{trntView}.cshtml");
                                                                             
        if (ReportViews.TryGetValue(key, out var reportView))   return View($"~/Applications/PisReport/Views/Pages/{reportView}.cshtml");

        return NotFound();
    }
}