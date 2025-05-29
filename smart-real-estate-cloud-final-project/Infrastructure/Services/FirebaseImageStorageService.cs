using Application.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using FirebaseAdmin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Google.Apis.Storage.v1.Data;
using Domain.Utils;
using System.Net;

namespace Infrastructure.Services
{
    public class FirebaseImageStorageService : IImageStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public FirebaseImageStorageService(IConfiguration configuration)
        {
            // 1. Load the service account credentials
            var serviceAccountPath = configuration["Firebase:ServiceAccountPath"];
            var credential = GoogleCredential.FromFile(serviceAccountPath);

            // 2. Initialize the FirebaseApp (if not already)
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }

            // 3. Create a Google Cloud Storage client
            _storageClient = StorageClient.Create(credential);
            _bucketName = configuration["Firebase:BucketName"];
        }
        public async Task<string> UploadAsync(IFormFile file, Guid id)
        {
            // 1. Convert IFormFile to MemoryStream
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0; // reset to beginning

            // 2. Create a unique name for the uploaded object
            var objectName = $"{id}_{file.FileName}";

            // 3. Upload to the bucket
            // Note: The contentType should match the actual file's MIME type, e.g. "image/png"
            var dataObject = await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: objectName,
                contentType: file.ContentType,
                source: ms
            );

            // 4. Make it publicly readable (optional, but usually needed if you want a public URL)
            dataObject.Acl = new List<ObjectAccessControl>
            {
                new ObjectAccessControl
                {
                    Entity = "allUsers",
                    Role = "READER"
                }
            };
            await _storageClient.UpdateObjectAsync(dataObject);

            // 5. Construct the public URL
            // e.g. https://storage.googleapis.com/<bucketName>/<objectName>
            var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";

            return publicUrl;
        }

        public async Task<Result> DeleteAsync(string imageUrl)
        {
            try
            {
                Uri uri = new Uri(imageUrl);
                var objectName = Path.GetFileName(uri.LocalPath);
                await _storageClient.DeleteObjectAsync(_bucketName, objectName);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
