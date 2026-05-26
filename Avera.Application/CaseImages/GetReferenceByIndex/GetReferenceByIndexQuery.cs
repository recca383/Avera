using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.CaseImages.GetReferenceByIndex
{
    public record GetReferenceByIndexQuery (
        Guid CaseId,
        int Index
    ) : IQuery<GetReferenceByIndexQueryResponse>;
}