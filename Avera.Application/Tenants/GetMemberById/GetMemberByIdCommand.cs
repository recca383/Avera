using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;

namespace Avera.Application.Tenants.GetMemberById
{
    public sealed record GetMemberByIdCommand(
        Guid UserId
    ) : ICommand<TenantMemberDto>;
}