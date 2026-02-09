namespace HRMvc.StartupConfig;

public class SessionService
{
    private readonly IHttpContextAccessor _http;

    public SessionService(IHttpContextAccessor http)
    {
        _http = http;
    }

    public string? OldPis => _http.HttpContext?.Session.GetString("OldPis");
    public string? OldPay => _http.HttpContext?.Session.GetString("OldPay");
    public string? EmpNumber => _http.HttpContext?.Session.GetString("EmpNumber");
}