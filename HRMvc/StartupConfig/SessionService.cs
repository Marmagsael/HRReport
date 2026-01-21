namespace HRMvc.StartupConfig;

public class SessionService
{
    private readonly IHttpContextAccessor _http;

    public SessionService(IHttpContextAccessor http)
    {
        _http = http;
    }

    public string? OldPis => _http.HttpContext?.Session.GetString("OldPisDb");
    public string? OldPay => _http.HttpContext?.Session.GetString("OldPayDb");
    public string? EmpNumber => _http.HttpContext?.Session.GetString("EmpNumber");
}