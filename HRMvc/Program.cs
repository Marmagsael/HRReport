using Blazored.LocalStorage;
using HRApiLibrary.Reporting.Providers.Payroll;
using HRMvc.StartupConfig;

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

//Api Injection -------------------------
builder.AddApiInjectionServices();
builder.AddApiServices();

var app = builder.Build();
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
//app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapBlazorHub();

app.Run();
