using HRApiLibrary.Models._00_Main;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace HRMvc.DataAccess.Main;

public class ClaimsAccess : Controller
{
    public async Task<ActionResult> CreateClaims(int empmasId, UserCompanyModel uc)
    {
        var claims = new List<Claim>();
        claims.Add(new Claim("UserId", empmasId.ToString()));
        claims.Add(new Claim("DefCompayId", uc.Id.ToString()!));
        claims.Add(new Claim("PisSchema", uc.PisSchema!));
        claims.Add(new Claim("PaySchema", uc.PaySchema!));
        claims.Add(new Claim("ApplicantSchema", uc.ApplicantSchema!));
        claims.Add(new Claim("AmsSchema", uc.AmsSchema!));

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(claimPrincipal);

        return Ok(claimsIdentity);

    }
}
