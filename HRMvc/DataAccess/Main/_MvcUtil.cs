using HRApiLibrary.Models._00_Main;
using System.Security.Claims;

namespace HRMvc.DataAccess.Main;

public class _MvcUtil
{
    private readonly IConfiguration _config;

    public _MvcUtil(IConfiguration config)
    {
        _config = config;
    }


    public UserClaimsModel _02UserClaimsContent(List<Claim?> claims)
    {
        UserClaimsModel uc = new UserClaimsModel();
        

        uc.SchemaMain       = _config.GetSection("Schema:Main").Value;
        uc.SchemaMainPis    = _config.GetSection("Schema:MainPis").Value;

        if (claims.Count > 0)
        {
            string? val = claims.Where(c => c?.Type == "UserId").FirstOrDefault()?.Value;
            if (val != null) uc.UserId = int.Parse(val);

            val = claims.Where(c => c?.Type == "DefCompayId").FirstOrDefault()?.Value;
            if (val != null) uc.DefCompanyId = val;

            val = claims.Where(c => c?.Type == "PisSchema").FirstOrDefault()?.Value;
            if (val != null) uc.SchemaUserPis = val;

            val = claims.Where(c => c?.Type == "PaySchema").FirstOrDefault()?.Value;
            if (val != null) uc.SchemaUserPay = val;

            val = claims.Where(c => c?.Type == "ApplicantSchema").FirstOrDefault()?.Value;
            if (val != null) uc.SchemaUserApp = val;

            val = claims.Where(c => c?.Type == "AmsSchema").FirstOrDefault()?.Value;
            if (val != null) uc.SchemaUserAms = val;



        }

        return uc;
    }
}
