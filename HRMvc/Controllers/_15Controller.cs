using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers;

[Route("15")]
public class _15Controller : Controller
{
    private static readonly Dictionary<int, string> MenuViews = new()
    {
        [502] = "~/Applications/Menus/Pages/_1502_Applications.cshtml",
    };

    [HttpGet("{menuCode:int}")]
    public IActionResult Menu(int menuCode)
    {
        return MenuViews.TryGetValue(menuCode, out var viewPath)
            ? View(viewPath)
            : NotFound($"Menu code '{menuCode}' was not found.");
    }
}