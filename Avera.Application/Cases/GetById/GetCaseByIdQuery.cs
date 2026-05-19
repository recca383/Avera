using Avera.Application.Messaging;

namespace Avera.Application.Cases.GetById
{
    public sealed class GetCaseByIdQuery : IQuery<GetCaseByIdQueryResult>
    {
        public Guid CaseId { get; set; }
    }
}