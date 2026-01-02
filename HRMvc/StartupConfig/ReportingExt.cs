using HRApiLibrary.Reporting.Providers.Payroll;

namespace HRMvc.StartupConfig;

public static class ReportingExt
{
    public static void AddReportProviders(this WebApplication app)
    {
        var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

        // Register ALL report providers here
        CoaReportProvider.Configure(scopeFactory);

        // Future-proof
        // PayrollRegisterReportProvider.Configure(scopeFactory);
        // EarningsSummaryReportProvider.Configure(scopeFactory);
        // PayslipReportProvider.Configure(scopeFactory);
    }
}