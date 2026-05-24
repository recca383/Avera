using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Cases.GetById
{
    public sealed record GetCaseByIdQuery(Guid CaseId) : IQuery<GetCaseByIdQueryResult>;
}