using System.Security.Cryptography;
using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
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
        IIdentityDbContext _db,
        JwtProvider jwtProvider
    ) : IAdminService
    {
        private const int INVITECODELENGTH = 8;
        public async Task<Result<string>> CreateTenantAsync(string name, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(
            _userContext.UserId.ToString());

            if (user is null)
                return Result.Failure<string>(UserErrors.UserNotFound);

            if (user.TenantId.HasValue)
                return Result.Failure<string>(TenantErrors.AlreadyBelongsToTenant);

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
                return HandleIdentityResult<string>(result);

            await _db.SaveChangesAsync(cancellationToken);

            var iListroles = await _userManager.GetRolesAsync(user);

            var listroles = iListroles.Cast<string>().ToList();

            var newToken = await jwtProvider.GenerateAccessTokenAsync(user, listroles, cancellationToken);

            return Result.Success<string>(newToken);
        }

        public async Task<Result<TenantMemberDto>> GetMemberByIdAsync(Guid userId, CancellationToken cancellationToken = default)
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

            return Result.Success(
                new TenantMemberDto(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!));
        }

        public async Task<Result<List<TenantMemberDto>>> GetMembersAsync(
            bool? IsAlphabetical = null,
            bool? IsMostCases = null,
            string? Name = null,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _userContext.TenantId;

            if (!tenantId.HasValue)
                return Result.Failure<List<TenantMemberDto>>(TenantErrors.NotMember);

            var users = _userManager.Users
                .Where(x => x.TenantId == tenantId);
            

            if (IsAlphabetical.HasValue)
            {
                users = users.OrderBy(x => x.FirstName);
            }

            //if (IsMostCases.HasValue)
            //{
            //    users = users
            //}

            if (Name != null)
            {
                users = users.Where(x => x.FirstName!.Contains(Name) 
                || x.LastName!.Contains(Name));
            }

            var result = await users
                .Select(x => new TenantMemberDto(
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Email!))  
                .ToListAsync(cancellationToken);


            return Result.Success(result);
        }

        public async Task<Result> RemoveUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(
                userId.ToString());

            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            if (user.TenantId != _userContext.TenantId)
                return Result.Failure(TenantErrors.UserNotInTenant);

            // Remove the user from the tenant
            user.TenantId = null;

            var role = await _userManager.GetRolesAsync(user);

            // Remove user roles
            var roleresult = await _userManager.RemoveFromRoleAsync(user, role.FirstOrDefault()!);

            if (!roleresult.Succeeded)
                return HandleIdentityResult(roleresult);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return HandleIdentityResult(result);

            // Invalidate the user's security stamp to force re-authentication
            await _userManager.UpdateSecurityStampAsync(user);

            return Result.Success();
        }

        public Task<Result> SendInviteCodeAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> SuspendUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            user.IsSuspended = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return HandleIdentityResult(result);
            }

            await _userManager.UpdateSecurityStampAsync(user);

            return Result.Success();
        }

        private static string ProduceInviteCode()
        {
            const string lettersPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string alphanumericPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

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
        private static Result<T> HandleIdentityResult<T>(IdentityResult result)
        {
            var errors = result.Errors.Select(
                e => new Error(
                    string.IsNullOrWhiteSpace(e.Code) ? "Identity.Unknown" : e.Code,
                    string.IsNullOrWhiteSpace(e.Description) ? "Identity validation failed" : e.Description,
                    ErrorType.Validation)
            ).
            ToArray();

            var validationErrors = new ValidationError(errors);
            return Result.Failure<T>(validationErrors);
        }
        private static Result HandleIdentityResult(IdentityResult result)
        {
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