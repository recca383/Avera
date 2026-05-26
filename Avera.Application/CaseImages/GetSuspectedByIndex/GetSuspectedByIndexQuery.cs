using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.CaseImages.GetSuspectedByIndex
{
    public record GetSuspectedByIndexQuery (
        Guid CaseId,
        int Index
    ) : IQuery<GetSuspectedByIndexQueryResponse>;
}