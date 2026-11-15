using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HRMvc.StartupConfig;

/// <summary>
/// Token that HRReport hands to the POS application (Other Applications > POS) so the user does not log in again.
/// Format :  base64url(json {u,c,e,n}) + "." + base64url(HMAC-SHA256(secret, first part)).
/// Same secret in POS (PosSso:Secret, at least 32 characters). Lives 2 minutes and POS accepts it only once.
/// </summary>
public static class PosSsoToken
{
    public const int MinSecretLength = 32;

    private static string B64(byte[] b) => Convert.ToBase64String(b).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    public static string Create(string secret, int userId, int companyId, int ttlSeconds = 120)
    {
        var payload = JsonSerializer.Serialize(new
        {
            u = userId, c = companyId,
            e = DateTimeOffset.UtcNow.AddSeconds(ttlSeconds).ToUnixTimeSeconds(),
            n = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)),
        });
        var part = B64(Encoding.UTF8.GetBytes(payload));
        var sig  = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(part));
        return part + "." + B64(sig);
    }
}