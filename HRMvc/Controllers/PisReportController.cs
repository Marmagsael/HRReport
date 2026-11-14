using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers;

[Route("14")]
public class PisReportController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Applications/PisReport/Views/Pages/Index.cshtml");
    }
}
