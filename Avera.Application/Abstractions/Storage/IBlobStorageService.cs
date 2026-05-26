namespace Avera.Application.Abstractions.Storage
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default);

        Task<Stream?> DownloadAsync(
            string fileUrl,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string fileUrl,
            CancellationToken cancellationToken = default); 
    }
}