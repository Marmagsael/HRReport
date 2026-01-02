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


//Api Injection -------------------------
builder.AddApiInjectionServices();
builder.AddApiServices();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();


//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt+QHFqVk9rXVNbdV5dVGpAd0N3RGlcdlR1fUUmHVdTRHRcQlliT3xTdkxjWHhddnw=;Mgo+DSMBPh8sVXJ1S0d+X1hPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXpScEdjWXZecXNRT2g=;ORg4AjUWIQA/Gnt2VFhhQlJNfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Xd0NhWX9XcHVSQGJU;MTc0MTYwMkAzMjMxMmUzMTJlMzMzOVlDMnhhMU4vV1pjSUUrelhtR0dPSTdhZmNzLzhKNHhOR1pvQWRtSERqRDg9;MTc0MTYwM0AzMjMxMmUzMTJlMzMzOUlmWlFDT1lKcngyNkdsc2psSUNkVHZwN0wrNWpuSkErbVlTL3RFMms4c1k9;NRAiBiAaIQQuGjN/V0d+XU9Hf1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TckRlW39eeXRUTmJZWQ==;MTc0MTYwNUAzMjMxMmUzMTJlMzMzOVQrcnZ0dXBUNFpJTXdBZ2k2YkE0NC8yMXJlTXFzZUtoU3pwdmUrQmpnR1U9;MTc0MTYwNkAzMjMxMmUzMTJlMzMzOUFIRlJ0VmNJZ3FaRlJOVGZBQ0VLd2VGWmxVeFVYblAyWlo2YVVwa1BiNEk9;Mgo+DSMBMAY9C3t2VFhhQlJNfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Xd0NhWX9XcHVcRGdU;MTc0MTYwOEAzMjMxMmUzMTJlMzMzOU9LbjN0d0h1MERDZUlDckxUc1NUQUk2amk2bUxJMkl4RU5rc1oxSU1wREU9;MTc0MTYwOUAzMjMxMmUzMTJlMzMzOVF4dlh5bzJCa2YxWjFNSWlDTHQ2R0EvMUVlc1c1aXg3MVJ5TzF5cnNCekU9;MTc0MTYxMEAzMjMxMmUzMTJlMzMzOVQrcnZ0dXBUNFpJTXdBZ2k2YkE0NC8yMXJlTXFzZUtoU3pwdmUrQmpnR1U9");
// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

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
