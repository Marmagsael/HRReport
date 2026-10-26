using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRMvc.StartupConfig;

// -- An Employee (CompanyUserTypeId 3) is portal only : the module pages are sent back to My Portal (12/102) --
// -- Registered for every controller in DependencyExt; the portal (12/*), company/*, login and API calls stay open --
public class EmployeePortalOnlyFilter : IActionFilter
{
    // -- First route segment of the module controllers --
    private static readonly HashSet<string> ModulePrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "02",       // HR
        "03",       // Payroll
        "04",       // Accounting
        "13",       // Payroll reports
        "13T",      // Payroll transactions
        "14",       // PIS reports
        "15",       // System Application
        "100",      // Employee profile (HR)
        "1100",     // Attendance management (AMS)
    };

    // -- Module pages that share a prefix with the portal --
    private static readonly HashSet<string> ModulePaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "12/101",   // Personnel Information
    };

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated != true || !CompanyRole.IsEmployee(user)) return;

        string path     = (context.HttpContext.Request.Path.Value ?? "").Trim('/');
        string first    = path.Split('/')[0];
        bool module     = ModulePrefixes.Contains(first) || ModulePaths.Any(p => path.Equals(p, StringComparison.OrdinalIgnoreCase)
                                                                              || path.StartsWith(p + "/", StringComparison.OrdinalIgnoreCase));
        if (!module) return;

        context.Result = HttpMethods.IsGet(context.HttpContext.Request.Method)
                       ? new RedirectResult("~/12/102")
                       : new ForbidResult();
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
