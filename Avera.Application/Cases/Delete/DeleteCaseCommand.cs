using Avera.Application.Messaging;

namespace Avera.Application.Delete
{
    public sealed class DeleteCaseCommand : ICommand<Guid>
    {
         public Guid Id { get; set; }
    }
}