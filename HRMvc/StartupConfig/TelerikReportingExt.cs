using Telerik.Reporting;
using Telerik.Reporting.Cache.File;
using Telerik.Reporting.Services;

namespace HRMvc.StartupConfig; 

public static class TelerikReportingExt
{
    public static void AddTelerikReporting(this WebApplicationBuilder builder)
    {
        var reportsPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports");

        // Use minimal resolver — no claims or data injection
        var resolver = new CustomReportSourceResolver(reportsPath);

        builder.Services.AddSingleton<IReportServiceConfiguration>(sp =>
            new ReportServiceConfiguration
            {
                HostAppId = "HRMvcApp",
                Storage = new FileStorage(),
                ReportSourceResolver = resolver
            });
    }
}
