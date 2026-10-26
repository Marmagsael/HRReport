using HRApiLibrary.DataAccess._90_Utils;
using Microsoft.JSInterop;

namespace HRMvc.StartupConfig;

// --- Terminal of the user in the browser ( not the server ) :
//     IP        : remote address of the request ( X-Forwarded-For first when behind a proxy )
//     TimeZone  : browser IANA zone -> Windows id -> MsdsDataAccess.Get_TimeZone_Id
//     MacAddress: browsers do not expose the MAC, so short browser info is stored instead ( e.g. "Chrome/Win" )
public record ClientTerminal(int? TimeZoneId, string? IpAddress, string? MacAddress);

public class ClientTerminalService
{
    private readonly IJSRuntime             _js;
    private readonly IHttpContextAccessor   _http;

    public ClientTerminalService(IJSRuntime js, IHttpContextAccessor http)
    {
        _js     = js;
        _http   = http;
    }

    public async Task<ClientTerminal> GetAsync()
    {
        string? ianaZone = null;
        string? browser  = null;
        try {
            var t    = await _js.InvokeAsync<JsTerminal>("GetClientTerminal");
            ianaZone = t.TimeZone;
            browser  = t.Browser;
        }
        catch { /* prerender / JS not ready : fall back below */ }

        int? tzId = 0;
        if (!string.IsNullOrWhiteSpace(ianaZone) && TimeZoneInfo.TryConvertIanaIdToWindowsId(ianaZone, out var winZone))
            tzId = MsdsDataAccess.Get_TimeZone_Id(winZone);

        return new ClientTerminal(tzId, ClientIp(), browser ?? "Browser");
    }

    string? ClientIp()
    {
        var ctx = _http.HttpContext;
        if (ctx == null) return "";

        var forwarded = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwarded)) return forwarded.Split(',')[0].Trim();

        var ip = ctx.Connection.RemoteIpAddress;
        if (ip == null) return "";
        return (ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip).ToString();
    }

    record JsTerminal(string? TimeZone, string? Browser);
}
