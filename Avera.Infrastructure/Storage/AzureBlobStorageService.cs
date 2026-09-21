using System.Reflection.Metadata;
using Avera.Application.Abstractions.Storage;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Avera.Infrastructure.Storage
{
    public class AzureBlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly BlobServiceClient _serviceClient;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var serviceUrl = configuration["AzureBlobStorage:AccountUrl"];

            var containerName = configuration["AzureBlobStorage:ContainerName"];

            var credential = new DefaultAzureCredential();

            _serviceClient = new BlobServiceClient(new Uri(serviceUrl!), credential);

            _containerClient = _serviceClient.GetBlobContainerClient(containerName);
        }
        
        public async Task DeleteFolderAsync(string folderUrl, CancellationToken cancellationToken = default)
        {
            if (!folderUrl.EndsWith('/'))
                folderUrl += "/";

            var options = new GetBlobsOptions { Prefix = folderUrl };

            await foreach (BlobItem blob in _containerClient.GetBlobsAsync(options))
            {
                await _containerClient.GetBlobClient(blob.Name)
                    .DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
            }
        }
        public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(fileUrl);
            return blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        public async Task<Stream?> DownloadAsync(string fileUrl, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(fileUrl);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                return null;
            }

            var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

            return response.Value.Content;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync
            (fileStream,
             new BlobUploadOptions 
                { 
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                }, cancellationToken);

            return blobClient.Uri.ToString();
        }
    }
}