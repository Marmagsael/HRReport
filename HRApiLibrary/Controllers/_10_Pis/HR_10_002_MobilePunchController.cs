using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.Modules._12008O;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace HRApiLibrary.Controllers._10_Pis;

[Route("api/mobile")]
[ApiController]
[AllowAnonymous]
public class HR_10_002_MobilePunchController : ControllerBase
{
    private readonly IConfiguration                 _config;
    private readonly I_00MainDA                     _mainDA;
    private readonly I_00RegisterDataAccess         _register;
    private readonly I_00EngagementDataAccess       _engagement;
    private readonly IEmpmasInternalDataAccess      _empmas;
    private readonly IDA_12008O                     _da;

    public HR_10_002_MobilePunchController(IConfiguration config, I_00MainDA mainDA, I_00RegisterDataAccess register,
                                           I_00EngagementDataAccess engagement, IEmpmasInternalDataAccess empmas, IDA_12008O da)
    {
        _config = config; _mainDA = mainDA; _register = register; _engagement = engagement; _empmas = empmas; _da = da;
    }

    // -- Same limit as the web Punch Capturing (facepunch.js) ; can be set in appsettings "Mobile:FaceLimit" --
    private double FaceLimit => double.TryParse(_config["Mobile:FaceLimit"], out var v) && v > 0 ? v : 0.45;
    private double FaceGap   => double.TryParse(_config["Mobile:FaceGap"], out var v) && v >= 0 ? v : 0.06;

    // ------------------------------------------------------------------------------------------
    // -- 01 Login : company code + username (or email) + password ------------------------------
    // ------------------------------------------------------------------------------------------
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] MobileLoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req?.CompanyCode) || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { ok = false, msg = "Company code, username and password are required." });

        try
        {
            string conn     = _config.GetSection("Schema:DefConn").Value ?? "MySqlConn";
            string schema   = _config.GetSection("Schema:Main").Value ?? "Main";

            var company = await _register._02CompanyByCode(req.CompanyCode.Trim(), schema, conn);
            var user    = await _mainDA._02UsersLoginLoginName(req.Username.Trim(), req.Password, schema, conn)
                          ?? await _mainDA._02UsersLoginEmail(req.Username.Trim(), req.Password, schema, conn);

            // -- one message for every failure, so the app cannot be used to find valid codes or usernames --
            if (company == null || user == null) return Unauthorized(new { ok = false, msg = "Invalid company code, username or password." });

            var uc = await _mainDA._02UserCompany(company.Id, schema, conn);
            var statuses = await _engagement._02ApprovalStatus(user.Id ?? 0, company.Id, schema, conn);
            bool member = statuses.Contains("M") || user.DefaultCoId == company.Id || uc?.OwnerId == user.Id;
            if (uc == null || !member) return Unauthorized(new { ok = false, msg = "Invalid company code, username or password." });

            string userConn = string.IsNullOrWhiteSpace(user.Domain) ? conn : user.Domain!;
            var emp = (await _empmas._02BySystemIds(user.Id ?? 0, uc.PisSchema ?? "", userConn))?.FirstOrDefault(x => x != null);
            if (emp == null || (emp.Id ?? 0) <= 0)
                return Unauthorized(new { ok = false, msg = "Your account has no employee record in this company yet." });

            int empmasId = emp.Id ?? 0;
            DateTime expires = DateTime.UtcNow.AddSeconds(TokenSeconds());
            string token = CreateToken(new MobileSession
            {
                UserId = user.Id ?? 0, EmpmasId = empmasId, CompanyId = company.Id, Pis = uc.PisSchema ?? "", OldPis = uc.OldPis ?? "", Conn = userConn
            }, expires);

            var status  = await _da._02MobileStatus(empmasId, uc.PisSchema ?? "", uc.OldPis ?? "", userConn);
            var samples = await _da._02OwnSamples(empmasId, uc.PisSchema ?? "", userConn);

            return Ok(new
            {
                ok = true, token, expiresAt = expires, company = uc.CompanyName, companyCode = req.CompanyCode.Trim(),
                name = status.EmpName, empNumber = status.EmpNumber, status,
                faceLimit = FaceLimit, faceGap = FaceGap, samples = ParseSamples(samples)
            });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("mobile login failed: " + ex);
            return StatusCode(500, new { ok = false, msg = "Cannot reach the server right now. Please try again shortly." });
        }
    }

    // ------------------------------------------------------------------------------------------
    // -- 02 Refresh : the employee's own face samples + the next action ------------------------
    // ------------------------------------------------------------------------------------------
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var s = ReadSession();
        if (s == null) return Unauthorized(new { ok = false, msg = "Please login again." });

        var status  = await _da._02MobileStatus(s.EmpmasId, s.Pis, s.OldPis, s.Conn);
        var samples = await _da._02OwnSamples(s.EmpmasId, s.Pis, s.Conn);
        return Ok(new { ok = true, name = status.EmpName, empNumber = status.EmpNumber, status, faceLimit = FaceLimit, faceGap = FaceGap, samples = ParseSamples(samples) });
    }

    // ------------------------------------------------------------------------------------------
    // -- 03 Punch : one punch ; the server decides IN or OUT -----------------------------------
    // ------------------------------------------------------------------------------------------
    [HttpPost("punch")]
    public async Task<IActionResult> Punch([FromBody] MobilePunchRequest req)
    {
        var s = ReadSession();
        if (s == null) return Unauthorized(new { ok = false, msg = "Please login again." });
        return Ok(await Record(s, req));
    }

    // -- 03.01 Punches saved on the phone while offline, oldest first --
    [HttpPost("punch/batch")]
    public async Task<IActionResult> PunchBatch([FromBody] List<MobilePunchRequest> reqs)
    {
        var s = ReadSession();
        if (s == null) return Unauthorized(new { ok = false, msg = "Please login again." });

        var results = new List<object>();
        foreach (var r in (reqs ?? []).OrderBy(x => x.PunchAt ?? DateTime.MaxValue))
        {
            r.Offline = true;
            var res = await Record(s, r);
            results.Add(new { clientId = r.ClientId, res.Ok, res.Type, res.Code, res.Msg, at = res.At, atUtc = res.AtUtc });
        }
        return Ok(new { ok = true, results });
    }

    private async Task<M12008_PunchResult> Record(MobileSession s, MobilePunchRequest r)
    {
        try
        {
            if (r.Liveness != true) return new() { Ok = false, Msg = "Liveness check not passed." };

            // -- the face must match the employee's own samples (1 to 1), same limit as the web --
            var samples = ParseSamples(await _da._02OwnSamples(s.EmpmasId, s.Pis, s.Conn));
            if (samples.Count == 0) return new() { Ok = false, Msg = "No face enrolled yet. Please enroll your face first." };
            if (r.Descriptor == null || r.Descriptor.Length != 128) return new() { Ok = false, Msg = "Face not captured." };

            double best = samples.Min(x => Distance(x, r.Descriptor));
            bool matched = best <= FaceLimit;

            // -- online : not recorded when the face does not match ; offline : recorded, but HR must review it --
            if (!matched && r.Offline != true) return new() { Ok = false, Msg = "Face not recognized." };

            var nowUtc = DateTime.UtcNow;
            DateTime? atUtc = null;
            if (r.Offline == true && r.PunchAt != null)
            {
                var p = r.PunchAt.Value.Kind == DateTimeKind.Local ? r.PunchAt.Value.ToUniversalTime() : DateTime.SpecifyKind(r.PunchAt.Value, DateTimeKind.Utc);
                atUtc = p > nowUtc ? nowUtc : p;
            }

            string? ip  = HttpContext.Connection.RemoteIpAddress?.ToString();
            string? dev = string.IsNullOrWhiteSpace(r.DeviceId) ? null : (r.DeviceId.Length > 20 ? r.DeviceId[..20] : r.DeviceId);
            if (ip != null && ip.Length > 20) ip = ip[..20];

            return await _da._01Punch(s.EmpmasId, s.Pis, s.OldPis, s.Conn, null, ip, dev, new M12008_MobileInfo
            {
                AtUtc = atUtc, Lat = r.Lat, Lng = r.Lng, Acc = r.Accuracy, FaceDist = Math.Round(best, 4),
                Offline = r.Offline == true, ForReview = !matched
            });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("mobile punch failed: " + ex);
            return new() { Ok = false, Msg = "Unable to record the punch." };
        }
    }

    // ------------------------------------------------------------------------------------------
    // -- Token ---------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------
    // -- Longer than the 7 days the app allows offline, so punches saved offline can still be sent --
    private int TokenSeconds() => int.TryParse(_config["Mobile:TokenSeconds"], out var v) && v > 0 ? v : 8 * 24 * 3600;

    private SymmetricSecurityKey Key() => new(Encoding.ASCII.GetBytes(_config.GetSection("Authentication:SecretKey").Value ?? ""));

    private string CreateToken(MobileSession s, DateTime expires)
    {
        var claims = new List<Claim>
        {
            new("uid", s.UserId.ToString()), new("emp", s.EmpmasId.ToString()), new("co", s.CompanyId.ToString()),
            new("pis", s.Pis), new("opis", s.OldPis), new("conn", s.Conn), new("typ", "mobile")
        };
        var token = new JwtSecurityToken(_config.GetSection("Authentication:Issuer").Value, _config.GetSection("Authentication:Audience").Value,
                                         claims, DateTime.UtcNow, expires, new SigningCredentials(Key(), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private MobileSession? ReadSession()
    {
        string h = Request.Headers["Authorization"].ToString();
        if (!h.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return null;
        try
        {
            var p = new JwtSecurityTokenHandler().ValidateToken(h[7..].Trim(), new TokenValidationParameters
            {
                ValidateIssuer = true, ValidIssuer = _config.GetSection("Authentication:Issuer").Value,
                ValidateAudience = true, ValidAudience = _config.GetSection("Authentication:Audience").Value,
                ValidateIssuerSigningKey = true, IssuerSigningKey = Key(), ValidateLifetime = true, ClockSkew = TimeSpan.FromMinutes(2)
            }, out _);
            if (p.FindFirst("typ")?.Value != "mobile") return null;
            return new MobileSession
            {
                UserId = int.Parse(p.FindFirst("uid")!.Value), EmpmasId = int.Parse(p.FindFirst("emp")!.Value), CompanyId = int.Parse(p.FindFirst("co")!.Value),
                Pis = p.FindFirst("pis")!.Value, OldPis = p.FindFirst("opis")!.Value, Conn = p.FindFirst("conn")!.Value
            };
        }
        catch { return null; }
    }

    private static List<double[]> ParseSamples(List<string> raw)
    {
        var list = new List<double[]>();
        foreach (var t in raw)
        {
            try { var a = JsonSerializer.Deserialize<double[]>(t ?? "[]"); if (a is { Length: 128 }) list.Add(a); } catch { }
        }
        return list;
    }

    private static double Distance(double[] a, double[] b)
    {
        double sum = 0;
        for (int i = 0; i < a.Length; i++) { var d = a[i] - b[i]; sum += d * d; }
        return Math.Sqrt(sum);
    }
}

public class MobileLoginRequest
{
    public string? CompanyCode { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class MobilePunchRequest
{
    public string? ClientId { get; set; }
    public double[]? Descriptor { get; set; }
    public bool? Liveness { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public double? Accuracy { get; set; }
    public DateTime? PunchAt { get; set; }
    public bool? Offline { get; set; }
    public string? DeviceId { get; set; }
}

public class MobileSession
{
    public int UserId { get; set; }
    public int EmpmasId { get; set; }
    public int CompanyId { get; set; }
    public string Pis { get; set; } = "";
    public string OldPis { get; set; } = "";
    public string Conn { get; set; } = "";
}
