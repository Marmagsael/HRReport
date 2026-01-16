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
            
            // Compliance
            ["2202"] = "_2202_ClientGuardDetail"
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
