using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.ML.GetBlobImage
{
    public sealed record GetBlobImageQuery(
        Guid CaseId,
        string Folder,
        string FileName
    ) :IQuery<GetBlobImageResponse>;
}