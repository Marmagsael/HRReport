using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HRMvc.StartupConfig;

// -- Backblaze B2 (native API v3) for the private bucket in appsettings "BlackBlaze" :
//    uploads with the RW key, downloads with the RO key. Tokens are kept for 23 hours (B2 gives 24) and renewed on a 401 --
public class BackblazeB2Service
{
    private readonly IHttpClientFactory _http;
    private readonly IConfiguration     _config;
    private readonly SemaphoreSlim      _lock = new(1, 1);

    private Account? rw;
    private Account? ro;
    private string?  bucketId;

    private sealed record Account(string AccountId, string ApiUrl, string DownloadUrl, string Token, DateTime Expires);

    public BackblazeB2Service(IHttpClientFactory http, IConfiguration config)
    {
        _http   = http;
        _config = config;
    }

    private string BucketName => _config["BlackBlaze:bucketname"] ?? throw new InvalidOperationException("BlackBlaze:bucketname is not set.");

    // -- Upload : b2_get_upload_url, then the file with its SHA1 --
    public async Task UploadAsync(string fileName, byte[] data, string contentType)
    {
        for (int attempt = 0; ; attempt++)
        {
            var acc = await AccountAsync("RW", attempt > 0);
            var id  = await BucketIdAsync(acc);

            using var client = _http.CreateClient();
            using var urlReq = new HttpRequestMessage(HttpMethod.Post, $"{acc.ApiUrl}/b2api/v3/b2_get_upload_url")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { bucketId = id }), Encoding.UTF8, "application/json")
            };
            urlReq.Headers.TryAddWithoutValidation("Authorization", acc.Token);
            using var urlRes = await client.SendAsync(urlReq);
            if (urlRes.StatusCode == HttpStatusCode.Unauthorized && attempt == 0) continue;
            await EnsureOk(urlRes, "b2_get_upload_url");

            using var urlDoc   = JsonDocument.Parse(await urlRes.Content.ReadAsStringAsync());
            var uploadUrl      = urlDoc.RootElement.GetProperty("uploadUrl").GetString()!;
            var uploadToken    = urlDoc.RootElement.GetProperty("authorizationToken").GetString()!;

            using var upReq = new HttpRequestMessage(HttpMethod.Post, uploadUrl) { Content = new ByteArrayContent(data) };
            upReq.Headers.TryAddWithoutValidation("Authorization", uploadToken);
            upReq.Headers.TryAddWithoutValidation("X-Bz-File-Name", EncodeName(fileName));
            upReq.Headers.TryAddWithoutValidation("X-Bz-Content-Sha1", Convert.ToHexString(SHA1.HashData(data)).ToLowerInvariant());
            upReq.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            using var upRes = await client.SendAsync(upReq);
            if (upRes.StatusCode == HttpStatusCode.Unauthorized && attempt == 0) continue;
            await EnsureOk(upRes, "b2_upload_file");
            return;
        }
    }

    // -- Delete every version of a file (b2_list_file_versions for the ids, then b2_delete_file_version); nothing when it is not there --
    public async Task DeleteAsync(string fileName)
    {
        for (int attempt = 0; ; attempt++)
        {
            var acc = await AccountAsync("RW", attempt > 0);
            var id  = await BucketIdAsync(acc);

            using var client = _http.CreateClient();
            using var listReq = new HttpRequestMessage(HttpMethod.Post, $"{acc.ApiUrl}/b2api/v3/b2_list_file_versions")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { bucketId = id, startFileName = fileName, prefix = fileName, maxFileCount = 100 }),
                                            Encoding.UTF8, "application/json")
            };
            listReq.Headers.TryAddWithoutValidation("Authorization", acc.Token);
            using var listRes = await client.SendAsync(listReq);
            if (listRes.StatusCode == HttpStatusCode.Unauthorized && attempt == 0) continue;
            await EnsureOk(listRes, "b2_list_file_versions");

            using var doc = JsonDocument.Parse(await listRes.Content.ReadAsStringAsync());
            foreach (var f in doc.RootElement.GetProperty("files").EnumerateArray())
            {
                if (f.GetProperty("fileName").GetString() != fileName) continue;

                using var delReq = new HttpRequestMessage(HttpMethod.Post, $"{acc.ApiUrl}/b2api/v3/b2_delete_file_version")
                {
                    Content = new StringContent(JsonSerializer.Serialize(new { fileName, fileId = f.GetProperty("fileId").GetString() }),
                                                Encoding.UTF8, "application/json")
                };
                delReq.Headers.TryAddWithoutValidation("Authorization", acc.Token);
                using var delRes = await client.SendAsync(delReq);
                await EnsureOk(delRes, "b2_delete_file_version");
            }
            return;
        }
    }

    // -- Download by name; null when the file is not there --
    public async Task<(byte[] Data, string ContentType)?> DownloadAsync(string fileName)
    {
        for (int attempt = 0; ; attempt++)
        {
            var acc = await AccountAsync("RO", attempt > 0);
            using var client = _http.CreateClient();
            using var req = new HttpRequestMessage(HttpMethod.Get, $"{acc.DownloadUrl}/file/{BucketName}/{EncodeName(fileName)}");
            req.Headers.TryAddWithoutValidation("Authorization", acc.Token);
            using var res = await client.SendAsync(req);
            if (res.StatusCode == HttpStatusCode.Unauthorized && attempt == 0) continue;
            if (res.StatusCode == HttpStatusCode.NotFound) return null;
            await EnsureOk(res, "download");
            return (await res.Content.ReadAsByteArrayAsync(), res.Content.Headers.ContentType?.MediaType ?? "application/octet-stream");
        }
    }

    private async Task<Account> AccountAsync(string key, bool renew)
    {
        var current = key == "RW" ? rw : ro;
        if (!renew && current != null && current.Expires > DateTime.UtcNow) return current;

        await _lock.WaitAsync();
        try
        {
            current = key == "RW" ? rw : ro;
            if (!renew && current != null && current.Expires > DateTime.UtcNow) return current;

            var keyId  = _config[$"BlackBlaze:{key}:keyID"];
            var appKey = _config[$"BlackBlaze:{key}:applicationKey"];
            if (string.IsNullOrWhiteSpace(keyId) || string.IsNullOrWhiteSpace(appKey))
                throw new InvalidOperationException($"BlackBlaze:{key} keys are not set.");

            using var client = _http.CreateClient();
            using var req = new HttpRequestMessage(HttpMethod.Get, "https://api.backblazeb2.com/b2api/v3/b2_authorize_account");
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{keyId}:{appKey}")));
            using var res = await client.SendAsync(req);
            await EnsureOk(res, "b2_authorize_account");

            using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
            var storage   = doc.RootElement.GetProperty("apiInfo").GetProperty("storageApi");
            var acc = new Account(doc.RootElement.GetProperty("accountId").GetString()!,
                                  storage.GetProperty("apiUrl").GetString()!, storage.GetProperty("downloadUrl").GetString()!,
                                  doc.RootElement.GetProperty("authorizationToken").GetString()!, DateTime.UtcNow.AddHours(23));
            if (key == "RW") rw = acc; else ro = acc;
            return acc;
        }
        finally { _lock.Release(); }
    }

    private async Task<string> BucketIdAsync(Account acc)
    {
        if (bucketId != null) return bucketId;

        using var client = _http.CreateClient();
        using var req = new HttpRequestMessage(HttpMethod.Post, $"{acc.ApiUrl}/b2api/v3/b2_list_buckets")
        {
            Content = new StringContent(JsonSerializer.Serialize(new { accountId = acc.AccountId, bucketName = BucketName }), Encoding.UTF8, "application/json")
        };
        req.Headers.TryAddWithoutValidation("Authorization", acc.Token);
        using var res = await client.SendAsync(req);
        await EnsureOk(res, "b2_list_buckets");

        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var b = doc.RootElement.GetProperty("buckets").EnumerateArray().FirstOrDefault();
        if (b.ValueKind != JsonValueKind.Object) throw new InvalidOperationException($"Bucket {BucketName} was not found.");
        return bucketId = b.GetProperty("bucketId").GetString()!;
    }

    // -- B2 file names are URL encoded, keeping the folder slashes --
    private static string EncodeName(string name) => string.Join("/", name.Split('/').Select(Uri.EscapeDataString));

    private static async Task EnsureOk(HttpResponseMessage res, string step)
    {
        if (res.IsSuccessStatusCode) return;
        var body = await res.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Backblaze {step} failed ({(int)res.StatusCode}): {body}");
    }
}
