using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers
{
    [Route("12")]
    public class _12Controller : Controller
    {
        private static readonly Dictionary<string, string> ReportViews = new()
        {
            // Standard
            ["102"] = "_12_102_Dashboard",
            ["103"] = "_12_103_201Record",
            ["104"] = "_12_104_Attendance",
            ["105"] = "_12_105_Leave",
            ["202"] = "_12_202_Payslip",
            ["203"] = "_12_203_SSSRemittance",
            ["204"] = "_12_204_PHICRemittance",
            ["205"] = "_12_205_PagibigRemittance",
            ["206"] = "_12_206_LoanObligation",
            ["302"] = "_12_302_LeaveApproval",

            
        };

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Applications/_12EmployeeProfile/Pages/Index.cshtml");
        }
        
        // 🔥 Clean numeric route
        [HttpGet("{code:int}")]
        public IActionResult Report(int code)
        {
            var key = code.ToString();

            if (!ReportViews.TryGetValue(key, out var viewName))
                return NotFound();

            return View($"~/Applications/_12EmployeeProfile/Pages/{viewName}.cshtml");
        }
    }
}
