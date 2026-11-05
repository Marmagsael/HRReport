using Google.Apis.Drive.v3;

namespace HRApiLibrary.DataAccess._10_Pis { 
    public class GoogleDriveServiceProvider : IGoogleDriveServiceProvider
    {
        private readonly GoogleDriveOAuthService _googleDriveOAuth;

        public GoogleDriveServiceProvider(GoogleDriveOAuthService googleDriveOAuth)
        {
            _googleDriveOAuth = googleDriveOAuth;
        }

        public async Task<DriveService> GetDriveServiceAsync()
        {
            return await _googleDriveOAuth.GetDriveServiceAsync();
        }
    }

    public interface IGoogleDriveServiceProvider
    {
        Task<DriveService> GetDriveServiceAsync();
    }
}
