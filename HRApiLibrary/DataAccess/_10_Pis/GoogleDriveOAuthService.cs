using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace HRApiLibrary.DataAccess._10_Pis
{
    public class GoogleDriveOAuthService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;

        private const string UserId = "HRPIS-GoogleDrive";

        private static readonly string[] Scopes =
        {
            DriveService.Scope.DriveFile
        };

        public GoogleDriveOAuthService(IConfiguration config, IWebHostEnvironment environment)
        {
            _config = config;
            _environment = environment;
        }

        // CREATE GOOGLE OAUTH FLOW
        private GoogleAuthorizationCodeFlow CreateFlow()
        {
            var clientId = _config["GoogleDrive:ClientId"];
            var clientSecret = _config["GoogleDrive:ClientSecret"];

            if (string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new InvalidOperationException(
                    "Google Drive OAuth Client ID or Client Secret is missing.");
            }

            var tokenPath = Path.Combine(_environment.ContentRootPath, "GoogleDriveTokens");

            return new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    },
                    Scopes = Scopes,
                    DataStore = new FileDataStore(tokenPath)
                });
        }

        // GENERATE GOOGLE AUTHORIZATION URL
        public string GetAuthorizationUrl(string state)
        {
            var redirectUri = _config["GoogleDrive:RedirectUri"];

            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                throw new InvalidOperationException("Google Drive redirect URI is not configured.");
            }

            var request = CreateFlow().CreateAuthorizationCodeRequest(redirectUri);

            request.State = state;

            var authorizationUrl = QueryHelpers.AddQueryString(request.Build().ToString(),
                new Dictionary<string, string?>
                {
                    ["prompt"] = "consent"
                });

            return authorizationUrl;
        }

        // EXCHANGE AUTHORIZATION CODE FOR TOKEN
        public async Task CompleteAuthorizationAsync(string code)
        {
            var flow = CreateFlow();

            await flow.ExchangeCodeForTokenAsync(UserId, code, _config["GoogleDrive:RedirectUri"], CancellationToken.None);
        }

        // GET AUTHENTICATED GOOGLE DRIVE SERVICE
        public async Task<DriveService> GetDriveServiceAsync()
        {
            var flow = CreateFlow();

            var token = await flow.LoadTokenAsync(UserId, CancellationToken.None);

            if (token == null ||
                string.IsNullOrWhiteSpace(token.RefreshToken))
            {
                throw new InvalidOperationException("Google Drive is not connected. Please connect first.");
            }

            var credential = new UserCredential(flow, UserId, token);

            return new DriveService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "HRPIS"
                });
        }
    }
}
