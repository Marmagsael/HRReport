using Blazored.LocalStorage;
using HRApiLibrary.Reporting.Providers.Payroll;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Extensions;
using HRMvc.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.AddServices();
builder.Services.AddBlazoredLocalStorage();
builder.AddInjectServices();
builder.AddHttpClient();
builder.AddCors();
builder.AddAuthenticationServices();
builder.Services.AddHRMvcScope();

// Api Injection
builder.AddApiInjectionServices();
builder.AddApiServices();

// 🔥 (3) ADD THIS — DataProtection persistence
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys\"))
    .SetApplicationName("HRMvc");

var app = builder.Build();


// 🔴 (1) ORDER FIX — middleware order important
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✔️ Session should be AFTER routing
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.AddReportProviders();

// Swagger (optional placement OK)
app.UseSwagger();
app.UseSwaggerUI();


// ✔️ Controllers
app.MapControllers();

// ✔️ MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// 🔥 (2) CRITICAL — WebSocket enforced Blazor Hub
app.MapBlazorHub(options =>
{
    options.Transports =
        Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
});



app.Run();