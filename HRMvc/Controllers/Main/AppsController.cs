using HRMvc.StartupConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers.Main;

// -- Other Applications : applications with their own build / site that are opened from HRReport, already logged in --
[Authorize]
public class AppsController : Controller
{
    private readonly IConfiguration _config;

    public AppsController(IConfiguration config) => _config = config;

    // -- POS : the logged-in user and his current company go to POS in a short lived, signed token --
    [HttpGet("apps/pos")]
    public IActionResult Pos()
    {
        var url     = _config["PosSso:Url"]?.Trim().TrimEnd('/');
        var secret  = _config["PosSso:Secret"];

        if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            || string.IsNullOrEmpty(secret) || secret.Length < PosSsoToken.MinSecretLength)
            return Content("POS is not set up on this server (PosSso:Url / PosSso:Secret).", "text/plain");

        int.TryParse(User.FindFirst("UserId")?.Value,      out var userId);
        int.TryParse(User.FindFirst("DefCompayId")?.Value, out var companyId);     // -- (sic) the claim name used by CreateClaims --
        if (userId <= 0 || companyId <= 0)
            return Content("Please select or create a company first.", "text/plain");

        return Redirect($"{url}/pos/sso?t={Uri.EscapeDataString(PosSsoToken.Create(secret, userId, companyId))}");
    }
}