using Avera.Domain.Identity.Tenants;
using SharedKernel;

namespace Avera.Application.Abstractions.Authentication
{
    public interface IAdminService
    {
        Task<Result<Guid>> CreateTenantAsync(
            string name,
            CancellationToken cancellationToken = default);

        Task<Result> SendInviteCodeAsync(
            CancellationToken cancellationToken = default);

        Task<Result> RemoveUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<Result> SuspendUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        );

        Task<Result<List<TenantMemberDto>>> GetMembersAsync(
            CancellationToken cancellationToken = default);

        Task<Result<TenantMemberDto>> GetMemberByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}