using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.Cases;

namespace Avera.Application.PatchStatus
{
    public sealed class PatchStatusCaseCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public Status Status { get; set; }
    }
}