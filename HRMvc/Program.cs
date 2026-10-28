using Blazored.LocalStorage;
using HRApiLibrary.Reporting.Providers.Payroll;
using HRMvc.StartupConfig;
using HRMvc.Applications.PayrollTransaction.Vars;
using HRMvc.Components;

var builder = WebApplication.CreateBuilder(args);

//Register Syncfusion license 
//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetSection("Syncfusion:Key").Value);


// Add services to the container.
builder.AddServices();
builder.Services.AddBlazoredLocalStorage();
builder.AddInjectServices();
builder.AddHttpClient();
builder.AddCors();
builder.AddAuthenticationServices();
builder.Services.AddHRMvcScope();

// -- Blazor Web App : .razor pages (Components/) next to the legacy .cshtml pages --
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//Api Injection -------------------------
builder.AddApiInjectionServices();
builder.AddApiServices();

var app = builder.Build();
await app.RunMigrations();      // -- table makers (main, MainPis), when appsettings "Migrations" is "true"; company schemas run after login --
PayClaimsHelper.Init(app.Configuration);

app.UseSession();  
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.AddReportProviders();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
//app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// -- Replaces MapBlazorHub : also serves the Blazor hub for the components hosted in .cshtml pages --
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
