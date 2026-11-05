using Google.Apis.Drive.v3;
using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis
{


    public class GoogleDriveDataAccess : IGoogleDriveDataAccess
    {

        private readonly IGoogleDriveServiceProvider _driveService;
        private readonly IConfiguration _config;

        public GoogleDriveDataAccess(IGoogleDriveServiceProvider driveServiceProvider, IConfiguration config)
        {
            _config = config;
            _driveService = driveServiceProvider;
        }


        public async Task<string> _03UploadEmployeeImage(string employeeNumber, byte[] imageBytes, string fileName, string contentType)
        {
            //Console.WriteLine("========== UPLOAD EMPLOYEE IMAGE ==========");
            //Console.WriteLine($"Employee Number: {employeeNumber}");
            //Console.WriteLine($"File Name: {fileName}");
            //Console.WriteLine($"Content Type: {contentType}");
            //Console.WriteLine($"Image Size: {imageBytes?.Length ?? 0} bytes");

            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(employeeNumber))
                    throw new ArgumentException("Employee number is required.");

                if (imageBytes == null || imageBytes.Length == 0)
                    throw new ArgumentException("Image data is required.");

                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentException("File name is required.");

                if (string.IsNullOrWhiteSpace(contentType))
                    throw new ArgumentException("Content type is required.");

                // Get authenticated Google Drive service
                var driveService = await _driveService.GetDriveServiceAsync();
                var folderId = await _02GetOrCreateEmployeeFolder(employeeNumber);

                //Console.WriteLine($"Employee Folder ID: {folderId}");

                // Prepare the file information
                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = fileName,
                    Parents = new List<string> { folderId },
                    MimeType = contentType
                };


                // Prepare the image data
                using var stream = new MemoryStream(imageBytes);

                // Upload to Google Drive
                var request = driveService.Files.Create(fileMetadata, stream, contentType);

                request.Fields = "id, name, webViewLink";

                var uploadResult = await request.UploadAsync();


                if (uploadResult.Status != Google.Apis.Upload.UploadStatus.Completed || request.ResponseBody == null)
                {
                    throw new InvalidOperationException($"Google Drive upload failed: " + $"{uploadResult.Exception?.Message ?? uploadResult.Status.ToString()}");
                }

                // Return the uploaded file ID
                var uploadedFileId = request.ResponseBody.Id;

                //Console.WriteLine($"Uploaded File ID: {uploadedFileId}");
                //Console.WriteLine($"Uploaded File Name: {request.ResponseBody.Name}");
                //Console.WriteLine($"Uploaded File Link: {request.ResponseBody.WebViewLink}");

                return uploadedFileId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("UPLOAD FAILED!");
                Console.WriteLine(ex.ToString());

                throw;
            }
        }


        // ============================================================
        // GET OR CREATE EMPLOYEE FOLDER
        // ============================================================

        public async Task<string> _02GetOrCreateEmployeeFolder(
            string employeeNumber)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(employeeNumber)) throw new ArgumentException("Employee number is required.");

                // Get authenticated Google Drive service
                var driveService = await _driveService.GetDriveServiceAsync();
                var parentFolderId = _config["GoogleDrive:ParentId"];

                //Console.WriteLine($"Parent Folder ID: {parentFolderId}");

                if (string.IsNullOrWhiteSpace(parentFolderId)) throw new InvalidOperationException("Google Drive ParentId is not configured.");

                // Search existing employee folder
                var listRequest = driveService.Files.List();

                listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' " + $"and name = '{employeeNumber.Replace("'", "\\'")}' " + $"and '{parentFolderId}' in parents " + $"and trashed = false";

                listRequest.Fields = "files(id, name)";
                listRequest.PageSize = 1;

                //Console.WriteLine($"Google Drive Search Query: {listRequest.Q}");

                var result = await listRequest.ExecuteAsync();

                //Console.WriteLine($"Folders Found: {result.Files.Count}");

                var existingFolder = result.Files.FirstOrDefault();

                if (existingFolder != null)
                {
                    //Console.WriteLine($"Folder Name: {existingFolder.Name}");
                    //Console.WriteLine($"Folder ID: {existingFolder.Id}");

                    return existingFolder.Id;
                }

                // Create employee folder if not existing
                var folder = new Google.Apis.Drive.v3.Data.File
                {
                    Name = employeeNumber,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new List<string> { parentFolderId }
                };

                var createRequest = driveService.Files.Create(folder);

                createRequest.Fields = "id, name";

                var createdFolder = await createRequest.ExecuteAsync();

                //Console.WriteLine($"Folder Name: {createdFolder.Name}");
                //Console.WriteLine($"Folder ID: {createdFolder.Id}");

                return createdFolder.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }


        // ============================================================
        // RETRIEVE EMPLOYEE IMAGE
        // ============================================================

        public async Task<(byte[] Content, string MimeType)?> _02GetEmployeeImage(string fileId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileId))
                    return null;

                // Get authenticated Google Drive service
                var driveService = await _driveService.GetDriveServiceAsync();

                // Get file information
                var request = driveService.Files.Get(fileId);

                request.Fields = "id, name, mimeType";

                var file = await request.ExecuteAsync();

                // Validate image type
                if (string.IsNullOrWhiteSpace(file.MimeType) || !file.MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("The requested file is not an image.");
                }

                // Download image content
                using var stream = new MemoryStream();

                await request.DownloadAsync(stream);
                return (stream.ToArray(), file.MimeType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }



        // ============================================================
        // DELETE EMPLOYEE IMAGE
        // ============================================================

        public async Task _04DeleteEmployeeImage(string fileId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileId)) throw new ArgumentException("File ID is required.");

                // Get authenticated Google Drive service
                var driveService = await _driveService.GetDriveServiceAsync();

                // Delete image from Google Drive
                var request = driveService.Files.Delete(fileId);

                await request.ExecuteAsync();

                //Console.WriteLine($"Deleted File ID: {fileId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

    }

    public interface IGoogleDriveDataAccess
    {
        Task<(byte[] Content, string MimeType)?> _02GetEmployeeImage(string fileId);
        Task<string> _02GetOrCreateEmployeeFolder(string employeeNumber);
        Task<string> _03UploadEmployeeImage(string employeeNumber, byte[] imageBytes, string fileName, string contentType);
        Task _04DeleteEmployeeImage(string fileId);
    }
}
