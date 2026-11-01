using HRMvc.StartupConfig;
using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers;

// -- Employee photo from the private Backblaze bucket, for the logged-in user's company :
//    /pis/photo/{EmpNumber}            the photo (14/2052)
//    /pis/photo/{EmpNumber}?size=icon  the 96x96 icon (grids)
//    /pis/photo/{EmpNumber}?fallback=wide  the wide default picture when there is no photo (12/102 dashboard)
//    No photo yet : the default picture. An Employee (portal only) gets only their own photo --
[Route("pis/photo")]
public class PisPhotoController : Controller
{
    private readonly EmployeePhotoService       _photo;
    private readonly UserClaimsContextService   _claims;

    public PisPhotoController(EmployeePhotoService photo, UserClaimsContextService claims)
    {
        _photo  = photo;
        _claims = claims;
    }

    [HttpGet("{empNumber}")]
    public async Task<IActionResult> Get(string empNumber, string? size, string? fallback)
    {
        var uc = _claims.Build(User);
        if (CompanyRole.IsEmployee(User) && !string.Equals(uc.OempNumber?.Trim(), empNumber.Trim(), StringComparison.OrdinalIgnoreCase))
            return Forbid();

        var file = await _photo.GetAsync(uc, empNumber, icon: string.Equals(size, "icon", StringComparison.OrdinalIgnoreCase));
        if (file == null) return Redirect(string.Equals(fallback, "wide", StringComparison.OrdinalIgnoreCase) ? "~/images/employee1.png" : "~/images/employee.png");

        Response.Headers.CacheControl = "private, max-age=300";
        return File(file.Value.Data, file.Value.ContentType);
    }
}
