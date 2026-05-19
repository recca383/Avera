using Avera.Application.Messaging;
using Avera.Domain.Application.Cases;

namespace Avera.Application.PatchStatus
{
    public sealed class PatchStatusCaseCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public Status Status { get; set; }
    }
}