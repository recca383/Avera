using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.CaseImages.DeleteSuspected
{
    public record DeleteSuspectedCaseImageCommand(
        Guid CaseId,
        int Index
    ) : ICommand;
}