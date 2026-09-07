using Avera.Domain.Identity.Tenants;
using SharedKernel;

namespace Avera.Application.Abstractions.Authentication
{
    public interface IAdminService
    {
        Task<Result<string>> CreateTenantAsync(
            string name,
            CancellationToken cancellationToken = default);

        Task<Result> RemoveUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<Result> SuspendUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        );

        Task<Result<List<TenantMemberDto>>> GetMembersAsync(
            bool? IsAlphabetical,
            bool? IsMostCases,
            string Name,
            CancellationToken cancellationToken = default);

        Task<Result<TenantMemberDto>> GetMemberByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}