using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers;

[Route("100")]
public class _100Controller : Controller
{
    private static readonly Dictionary<string, string> ReportViews = new()
    {
        // Main Menu
        ["102"] = "100.002Profile",
        ["103"] = "100.003TimeSheet"
    };
        
    
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Applications/_100EmployeeProfile/Pages/Index.cshtml");
    }

    // 🔥 Clean numeric route
    [HttpGet("{reportCode:int}")]
    public IActionResult Report(int reportCode)
    {
        var key = reportCode.ToString();

        if (!ReportViews.TryGetValue(key, out var viewName))
            return NotFound();

        return View($"~/Applications/_100EmployeeProfile/Pages/{viewName}.cshtml");
    }
}