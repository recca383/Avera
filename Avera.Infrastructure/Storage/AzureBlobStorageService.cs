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

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var serviceUrl = configuration["AzureBlobStorage:AccountUrl"];

            var containerName = configuration["AzureBlobStorage:ContainerName"];

            var credential = new DefaultAzureCredential();

            var blobServiceClient = new BlobServiceClient(new Uri(serviceUrl!), credential);

            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }
        
        public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(fileUrl);
            return blobClient.DeleteAsync(cancellationToken: cancellationToken);
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