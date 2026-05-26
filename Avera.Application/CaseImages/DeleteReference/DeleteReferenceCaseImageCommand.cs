using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.CaseImages;

namespace Avera.Application.CaseImages.DeleteReference
{
    public record DeleteReferenceCaseImageCommand
    (
        Guid CaseId,
        int Index
    ) : ICommand;
}