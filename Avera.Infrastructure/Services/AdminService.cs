using System.Security.Cryptography;
using Avera.Application.Abstractions.Authentication;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Authentication;
using Avera.Infrastructure.Database.Identity;
using Avera.Infrastructure.Identity.Tenants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Avera.Infrastructure.Services
{
    internal sealed class AdminService(
        UserManager<User> _userManager,
        IUserContext _userContext,
        IdentityDbContext _db
    ) : IAdminService
    {
        private const int INVITECODELENGTH = 8;
        public async Task<Result> CreateTenantAsync(string name, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(
            _userContext.UserId.ToString());

            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            if (user.TenantId.HasValue)
                return Result.Failure(TenantErrors.AlreadyBelongsToTenant);

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = name,
                InviteCode = ProduceInviteCode(),
                Status = TenantStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            user.TenantId = tenant.Id;

            _db.Tenants.Add(tenant);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return HandleIdentityResult(result);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<TenantMemberDto>> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var tenantId = _userContext.TenantId;

            if (!tenantId.HasValue)
                return Result.Failure<TenantMemberDto>(
                    TenantErrors.NotMember);

            var user = await _userManager.Users
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == userId &&
                        x.TenantId == tenantId,
                    cancellationToken);

            if (user is null)
                return Result.Failure<TenantMemberDto>(
                    UserErrors.UserNotFound);

            var roles = await _userManager.GetRolesAsync(user);

            return Result.Success(
                new TenantMemberDto(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!));
        }

        public async Task<Result<List<TenantMemberDto>>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            var tenantId = _userContext.TenantId;

            if (!tenantId.HasValue)
                return Result.Failure<List<TenantMemberDto>>(TenantErrors.NotMember);

            var users = await _userManager.Users
                .Where(x => x.TenantId == tenantId)
                .ToListAsync(cancellationToken);

            var result = new List<TenantMemberDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new TenantMemberDto(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!));
            }

            return Result.Success(result);
        }

        public Task<Result> JoinInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> RemoveUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(
                userId.ToString());

            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            if (user.TenantId != _userContext.TenantId)
                return Result.Failure(TenantErrors.UserNotInTenant);

            user.TenantId = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return HandleIdentityResult(result);

            return Result.Success();
        }

        public Task<Result> SendInviteCodeAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> SuspendUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private static string ProduceInviteCode()
        {
            const string lettersPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            const string alphanumericPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            char[] resultChars = new char[INVITECODELENGTH]; // 3 letters + 1 hyphen + 4 alphanumeric = 8 chars

            // Fill letters
            for (int i = 0; i < 3; i++)
            {
                resultChars[i] = lettersPool[RandomNumberGenerator.GetInt32(lettersPool.Length)];
            }

            resultChars[3] = '-'; // Add hyphen

            // Fill alphanumeric
            for (int i = 4; i < 8; i++)
            {
                resultChars[i] = alphanumericPool[RandomNumberGenerator.GetInt32(alphanumericPool.Length)];
            }

            return new string(resultChars);
        }
        private static Result HandleIdentityResult(IdentityResult result)
        {
            if (result.Succeeded)
            {
                return Result.Success();
            }

            var errors = result.Errors.Select(
                e => new Error(
                    string.IsNullOrWhiteSpace(e.Code) ? "Identity.Unknown" : e.Code,
                    string.IsNullOrWhiteSpace(e.Description) ? "Identity validation failed" : e.Description,
                    ErrorType.Validation)
            ).
            ToArray();

            var validationErrors = new ValidationError(errors);
            return Result.Failure(validationErrors);
        }

    }
}