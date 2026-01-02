using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers
{
    public class _01Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
