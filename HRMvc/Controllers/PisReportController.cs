using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers
{
    [Route("14")]
    public class PisReportController : Controller
    {
        private static readonly Dictionary<string, string> ReportViews = new()
        {
            // Standard
            ["2102"] = "_2102_EmployeeMasterList",
            ["2103"] = "_2103_EmployeeStatusReport",
            ["2104"] = "_2104_EmployeeAgeReport",
            ["2105"] = "_2105_NewlyHiredfortheMonth",
            ["2106"] = "_2106_HireDateReport",
            ["2107"] = "_2107_ForRegularization",
            ["2108"] = "_2108_ResignedPersonnelfortheMonth",
            ["2109"] = "_2109_FloatingEmployees",
            ["2110"] = "_2110_OnLeaveEmployees",
            ["2111"] = "_2111_DetailedDeviationReport",
            ["2112"] = "_2112_SummarizedDeviationReport",
            ["2113"] = "_2113_LicenseVerification",
            ["2114"] = "_2114_LicenseExpiration",
            ["2115"] = "_2115_ManpowerMovement",
            ["2116"] = "_2116_InsurancePolicy",

            // Compliance
            ["2202"] = "_2202_ClientGuardDetail",
            ["2203"] = "_2203_EmployeeClearance",
            ["2204"] = "_2204_PNPSAGSDReport",
            ["2205"] = "_2205_FEDReport",
            ["2206"] = "_2206_GroupDDO",
            ["2207"] = "_2207_RTUReport",
            ["2208"] = "_2208_RecalledEmployeeReport",
            ["2209"] = "_2209_AssignmentHistory",
            ["2210"] = "_2210_ClientContractExpirationReport",
            ["2211"] = "_2211_UniformQuery"
        };

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Applications/PisReport/Views/Pages/Index.cshtml");
        }
        
        // 🔥 Clean numeric route
        [HttpGet("{pisCode:int}")]
        public IActionResult Report(int pisCode)
        {
            var key = pisCode.ToString();

            if (!ReportViews.TryGetValue(key, out var viewName))
                return NotFound();

            return View($"~/Applications/PisReport/Views/Pages/{viewName}.cshtml");
        }
    }
}
