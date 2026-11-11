using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRMvc.StartupConfig;

public class CompanyLogoService
{
    private readonly ISettingsDataAccess _settings;
    private readonly BackblazeB2Service  _b2;

    public CompanyLogoService(ISettingsDataAccess settings, BackblazeB2Service b2)
    {
        _settings = settings;
        _b2       = b2;
    }

    // -- The company logo saved in Company Information (PIS) / Company Settings (Payroll); null when there is none or it cannot be read --
    public async Task<(byte[] Data, string ContentType)?> GetAsync(UserClaimsModel uc)
    {
        try
        {
            var key = (await _settings._02(1, uc.SchemaUserPay, uc.Conn))?.LogoAddress;
            if (string.IsNullOrWhiteSpace(key)) return null;

            if (key.StartsWith("/images/"))
            {
                var f = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", key.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                return File.Exists(f) ? (await File.ReadAllBytesAsync(f), "image/png") : null;
            }
            return await _b2.DownloadAsync(key);
        }
        catch (Exception ex) { Console.Error.WriteLine("company logo: " + ex.Message); return null; }
    }
}
