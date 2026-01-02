using HRApiLibrary.DataAccess._00_Main;
using HRApiLibrary.DataAccess._10_Pis;
using HRApiLibrary.DataAccess._90_Utils;
using HRMvc.DataAccess.Main;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
//using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
//using Syncfusion.Blazor;
//using Syncfusion.Blazor.Popups;
using Radzen;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.DataAccess._20_Pay;
using HRApiLibrary.DataAccess._20_Pay.Interface;

namespace HRMvc.StartupConfig;

public static class DependencyExt
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        // ---- Blazor Components -----
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        //builder.Services.AddSyncfusionBlazor();
        builder.Services.AddRadzenComponents();

        //--- Session ---------------------------
        builder.Services.AddDistributedMemoryCache(); 
        builder.Services.AddSession(opts => {
            opts.IdleTimeout = TimeSpan.FromHours(12); 
        }); 
    }


    public static void AddInjectServices(this WebApplicationBuilder builder)
    {
        // --- Session --------------------------------------------------------------------------
        builder.Services.AddSingleton<ISessionDataAccess, SessionDataAccess>();

        // --- 00_MainPis -----------------------------------------------------------------------
        builder.Services.AddSingleton<I_00MainDA, _00MainDA>();
        builder.Services.AddSingleton<ClaimsAccess>(); 
        builder.Services.AddSingleton<I_00UsersAccess, _00UsersAccess>();
        builder.Services.AddSingleton<I_00MainPisTblMakerAccess, _00MainPisTblMakerAccess>();

        // --- 00_MainPis -----------------------------------------------------------------------
        builder.Services.AddSingleton<I_00MainPisAccess, _00MainPisAccess>();
        builder.Services.AddSingleton<I_00CountryDataAccess, _00CountryDataAccess>();
        
        builder.Services.AddSingleton<I_00CityDataAccess, _00CityDataAccess>();
        builder.Services.AddSingleton<I_00ClientDataAccess, _00ClientDataAccess>();
        builder.Services.AddSingleton<I_00CompanyusersDataAccess, _00CompanyusersDataAccess>();
        builder.Services.AddSingleton<I_00ProvincestateDataAccess, _00ProvincestateDataAccess>();
        builder.Services.AddSingleton<I_00UserscompanyDataAccess, _00UserscompanyDataAccess>();
        builder.Services.AddSingleton<I_00_CurrencyDataAccess, _00_CurrencyDataAccess>();

        // --- 10_Pis -----------------------------------------------------------------------
        builder.Services.AddSingleton<I_10_EmpmasDataAccess, _10_EmpmasDataAccess>();

        // --- 20 Pay -----------------------------------------------------------------------
        builder.Services.AddSingleton<ICoaDataAccess, CoaDataAccess>();
        builder.Services.AddSingleton<IPayrollgrpDataAccess, PayrollgrpDataAccess>(); 
        builder.Services.AddSingleton<IPayrollgrpratesDataAccess, PayrollgrpratesDataAccess>();

        // --- 90_Utils -----------------------------------------------------------------------
        builder.Services.AddSingleton<I_90_001_MySqlDataAccess, _90_001_MySqlDataAccess>();
        builder.Services.AddSingleton<I_09_02_VarsGlobal, _09_02_VarsGlobal>();
        builder.Services.AddSingleton<IMsdsDataAccess, MsdsDataAccess>();


        //--- Radzen Requirements ---------------------------------------
        builder.Services.AddScoped<DialogService>();
        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddScoped<TooltipService>();
        builder.Services.AddScoped<ContextMenuService>();

    }
    public static void AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(ops =>
        {
            ops.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        });


        builder.Services.AddAuthentication(ops =>
        {
            ops.DefaultScheme           = CookieAuthenticationDefaults.AuthenticationScheme;
            //ops.DefaultChallengeScheme  = GoogleDefaults.AuthenticationScheme;
        })
            .AddCookie(ops =>
            {
                ops.LoginPath = "/login";

            })
            //.AddGoogle(options =>
            //{
            //    options.ClientId = builder.Configuration.GetValue<string>("GoogleLogin:ClientId");
            //    options.ClientSecret = builder.Configuration.GetValue<string>("GoogleLogin:ClientSecret");
            //    options.CallbackPath = builder.Configuration.GetValue<string>("GoogleLogin:CallbackPath");
            //    options.AuthorizationEndpoint += "?prompt=consent";
            //})
            //.AddOpenIdConnect("OpenId", options =>
            //{
            //    options.Authority = "https://accounts.google.com";
            //    options.ClientId = builder.Configuration.GetValue<string>("GoogleLogin:ClientId");
            //    options.ClientSecret = builder.Configuration.GetValue<string>("GoogleLogin:ClientSecret");
            //    options.CallbackPath = builder.Configuration.GetValue<string>("GoogleLogin:CallbackPath");
            //    options.GetClaimsFromUserInfoEndpoint = true;
            //})
            ;

        //Microsoft Identity Authentication ---------------------------------------------------------
        //IEnumerable<string>? initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ');

        //builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd")
        //    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
        //        .AddDownstreamWebApi("DownstreamApi", builder.Configuration.GetSection("DownstreamApi"))
        //        .AddInMemoryTokenCaches();

        //builder.Services.AddRazorPages().AddMvcOptions(options =>
        //{
        //    var policy = new AuthorizationPolicyBuilder()
        //                  .RequireAuthenticatedUser()
        //                  .Build();
        //    options.Filters.Add(new AuthorizeFilter(policy));
        //}).AddMicrosoftIdentityUI();
    }

    public static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(p => p.AddPolicy("corspolicy",
            build => build.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod()));
    }

    public static void AddHttpClient(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient("api", opt =>
        {
            opt.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiAddress"));
        });
    }
}
