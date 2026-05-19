using Avera.Application.Messaging;

namespace Avera.Application.PatchIsViewed
{
    public sealed class PatchIsViewedCaseCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public bool IsViewed { get; set; }
    }
}