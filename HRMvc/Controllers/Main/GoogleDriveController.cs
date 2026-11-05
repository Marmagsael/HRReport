
using HRApiLibrary.DataAccess._10_Pis;
using HRMvc.StartupConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using static HRMvc.StartupConfig.DependencyExt;

namespace HRMvc.Controllers.Main
{
    [Authorize]
    public class GoogleDriveController : Controller
    {
        private readonly GoogleDriveOAuthService _googleDrive;

        public GoogleDriveController(GoogleDriveOAuthService googleDrive)
        {
            _googleDrive = googleDrive;
        }

        // CONNECT  TO GOOGLE DRIVE
        [HttpGet]
        public IActionResult Connect()
        {
            var state = Guid.NewGuid().ToString("N");

            HttpContext.Session.SetString("GoogleDriveState", state);

            var authorizationUrl = _googleDrive.GetAuthorizationUrl(state);

            return Redirect(authorizationUrl);
        }

        // RECEIVE GOOGLE'S AUTHORIZATION RESPONSE
        [AllowAnonymous]
        [HttpGet("/signin-google-drive")]
        public async Task<IActionResult> Callback(
            string? code,
            string? state,
            string? error)
        {
            // Check Google's authorization error
            if (!string.IsNullOrWhiteSpace(error))
            {
                return BadRequest(  "Google authorization failed: " + error);
            }

            // Retrieve the original state from session
            var savedState = HttpContext.Session.GetString("GoogleDriveState");

            // Validate the state
            if (string.IsNullOrWhiteSpace(savedState) || string.IsNullOrWhiteSpace(state) || !CryptographicOperations.FixedTimeEquals( Encoding.UTF8.GetBytes(savedState), Encoding.UTF8.GetBytes(state)))
            {
                return BadRequest("Invalid authorization request.");
            }

            // Remove state after successful validation
            HttpContext.Session.Remove("GoogleDriveState");

            // Check authorization code
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Authorization code is missing.");
            }

            // Exchange authorization code for OAuth tokens
            await _googleDrive.CompleteAuthorizationAsync(code);

            return Content("Google Drive connected successfully!");
        }
    }
}
