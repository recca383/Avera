using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.CaseImages;

namespace Avera.Application.CaseImages.UploadSuspected
{
    public record UploadSuspectedCaseImageCommand
    (
        Guid CaseId,
        int Index,
        string MimeType,
        float Size,
        Stream File
    ): ICommand<Guid>;
}