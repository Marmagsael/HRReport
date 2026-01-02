using Telerik.Reporting;
using Telerik.Reporting.Cache.File;
using Telerik.Reporting.Services;

namespace HRMvc.StartupConfig; 

/*
public static class TelerikReportingExt
{
    public static void AddTelerikReporting(this WebApplicationBuilder builder)
    {
        var reportsPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports");

        builder.Services.AddSingleton<IReportServiceConfiguration>(sp =>
            new ReportServiceConfiguration
            {
                HostAppId = "HRMvcApp",
                Storage = new FileStorage(), // stores cached reports
                ReportSourceResolver = new UriReportSourceResolver(reportsPath)
            });
    }
}
*/
