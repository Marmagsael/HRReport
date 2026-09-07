using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace HRMvc.Controllers
{
    [Route("12")]
    public class _12Controller : Controller
    {
        private static readonly Dictionary<string, string> ReportViews = new()
        {
            // Standard
            ["002"] = "_12_002_Password",
            ["003"] = "_12_003_MyEngagement",
            ["004"] = "_12_004_PayrollSettings",
            // ["010"] = "_12_010_Logout",
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

        [HttpGet("{code}")]
        public IActionResult Report(string? code)
        {
            if (!ReportViews.TryGetValue(code, out var viewName))
                return NotFound();

            return View($"~/Applications/_12EmployeeProfile/Pages/{viewName}.cshtml");
        }

        [HttpGet("010")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            // 👉 optional: if switching company
            // await CreateClaims(user, uc);

            return Redirect("/13"); // change if needed
        }

    }
}
