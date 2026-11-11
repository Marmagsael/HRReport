using HRMvc.StartupConfig;
using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers;

[Route("company/logo")]
public class CompanyLogoController : Controller
{
    private readonly CompanyLogoService       _logo;
    private readonly UserClaimsContextService _claims;

    public CompanyLogoController(CompanyLogoService logo, UserClaimsContextService claims)
    {
        _logo   = logo;
        _claims = claims;
    }

    [HttpGet("")]
    public async Task<IActionResult> Get()
    {
        var file = await _logo.GetAsync(_claims.Build(User));
        if (file == null) return NotFound();
        Response.Headers.CacheControl = "private, max-age=300";
        return File(file.Value.Data, file.Value.ContentType);
    }
}
