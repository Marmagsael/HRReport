using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers.DA
{
	[AllowAnonymous]
	public class DAChartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
