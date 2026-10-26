using System.Security.Claims;

namespace HRMvc.StartupConfig;

// -- Role of the logged-in user in the current company : main.CompanyUsers.CompanyUserTypeId, set in the claims --
// -- at login and at Switch. Users with no CompanyUsers row (older accounts) get the User role -----------------
public static class CompanyRole
{
    public const string ClaimType   = "CompanyUserTypeId";

    public const int    Owner       = 1;
    public const int    User        = 2;
    public const int    Employee    = 3;    // portal only : no System Application, no module access
    public const int    SystemAdmin = 5;

    public static int Of(ClaimsPrincipal user)
    {
        return int.TryParse(user.FindFirst(ClaimType)?.Value, out var type) ? type : User;
    }

    public static bool IsEmployee(ClaimsPrincipal user) => Of(user) == Employee;
}
