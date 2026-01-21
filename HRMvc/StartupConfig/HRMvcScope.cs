using HRMvc.StartupConfig.Library;

namespace HRMvc.StartupConfig;

public static class HRMvcScope
{
    public static IServiceCollection AddHRMvcScope(this IServiceCollection services)
    {
        services.AddHttpContextAccessor(); // kung kailangan
        services.AddScoped<SessionService>();
        services.AddScoped<UserClaimsContextService>();
        services.AddScoped<L12_102>();
        services.AddScoped<L12_120>();

        return services;
    }
}