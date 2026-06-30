namespace Avera.Application.ML.GetBlobImage
{
    public sealed record GetBlobImageResponse(Stream ImageStream, string ContentType);
}