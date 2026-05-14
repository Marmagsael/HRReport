using HRApiLibrary.DataAccess._20_Pay;
using HRApiLibrary.Models._20_Pay;
using Microsoft.Extensions.DependencyInjection;

namespace HRApiLibrary.Reporting.Providers.Payroll;

public static class CoaReportProvider
{
    private static IServiceScopeFactory? _scopeFactory;

    // Called ONCE during app startup
    public static void Configure(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    // === USED BY TRDP ObjectDataSource ===
    public static List<CoaModel> GetAll(string schema, string conn)
    {
        using var scope = _scopeFactory!.CreateScope();
        var da = scope.ServiceProvider.GetRequiredService<CoaDataAccess>();

        return da._02(schema, conn)
            .GetAwaiter()
            .GetResult()!
            .Where(x => x != null)
            .Select(x => x!)
            .ToList();
    }

    public static List<CoaModel> GetByType(string acctType, string schema, string conn)
    {
        using var scope = _scopeFactory!.CreateScope();
        var da = scope.ServiceProvider.GetRequiredService<CoaDataAccess>();

        return da._02ByType(acctType, schema, conn)
            .GetAwaiter()
            .GetResult()!
            .Where(x => x != null)
            .Select(x => x!)
            .ToList();
    }
}