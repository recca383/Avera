using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Tenants.Create;
using Avera.Domain.Identity.Roles;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Authentication;
using Avera.Infrastructure.Database.Application;
using Avera.Infrastructure.Database.Identity;
using Avera.Infrastructure.Identity.Tenants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Infrastructure.DomainEvents;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

namespace Avera.Infrastructure.Services
{
    internal sealed class AdminService(
        UserManager<User> _userManager,
        IUserContext _userContext,
        IIdentityDbContext _identityDbContext,
        JwtProvider jwtProvider,
        IApplicationDbContext _applicationDbContext,
        IDateTimeProvider dateTime,
        IDomainEventsDispatcher domainEventsDispatcher
    ) : IAdminService
    {
        private const int INVITECODELENGTH = 8;
        public async Task<Result<CreateTenantResponse>> CreateTenantAsync(string name, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(
            _userContext.UserId.ToString());

            if (user is null)
                return Result.Failure<CreateTenantResponse>(UserErrors.UserNotFound);

            if (user.TenantId.HasValue)
                return Result.Failure<CreateTenantResponse>(TenantErrors.AlreadyBelongsToTenant);

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = name,
                InviteCode = ProduceInviteCode(),
                Status = TenantStatus.Active,
                CreatedAt = dateTime.PhilippineNow
            };

            user.TenantId = tenant.Id;

            _identityDbContext.Tenants.Add(tenant);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return HandleIdentityResult<CreateTenantResponse>(result);

            await _identityDbContext.SaveChangesAsync(cancellationToken);

            var iListroles = await _userManager.GetRolesAsync(user);

            var listroles = iListroles.Cast<string>().ToList();

            var newToken = await jwtProvider.GenerateAccessTokenAsync(user, listroles, cancellationToken);

            return Result.Success<CreateTenantResponse>(new CreateTenantResponse(newToken, dateTime.PhilippineNow.AddDays(1)));
        }

        public async Task<Result<TenantMemberDto>> GetMemberByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var tenantId = _userContext.TenantId;

            if (!tenantId.HasValue)
                return Result.Failure<TenantMemberDto>(
                    TenantErrors.NotMember);

            var user = await _userManager.Users
                .FirstOrDefaultAsync(x =>
                        x.Id == userId &&
                        x.TenantId == tenantId.Value,
                    cancellationToken);

            if (user == null)
                return Result.Failure<TenantMemberDto>(
                    UserErrors.UserNotFound);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var caseHandled = await _applicationDbContext.Cases.Where(c =>
                              c.TenantId == user.TenantId &&
                              c.CreatedByUserId == user.Id).CountAsync(cancellationToken);
            if (user is null)
                return Result.Failure<TenantMemberDto>(
                    UserErrors.UserNotFound);

            return Result.Success(
                new TenantMemberDto(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!,
                    role,
                    user.IsSuspended,
                    caseHandled));
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

            var users = _userManager.Users.Where(x => x.TenantId == tenantId.Value);

            var caseCounts = await _applicationDbContext.Cases
               .Where(c =>
                   c.TenantId == tenantId.Value &&
                   c.DeletedAt == DateTime.MaxValue)
               .GroupBy(c => c.CreatedByUserId)
               .Select(g => new
               {
                   UserId = g.Key,
                   Count = g.Count()
               })
               .ToDictionaryAsync(
                   x => x.UserId,
                   x => x.Count,
                   cancellationToken);

            var userList = await users.ToListAsync(cancellationToken);

            var result = new List<TenantMemberDto>();

            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var role = roles.FirstOrDefault() ?? "User";

                var casesHandled = caseCounts.TryGetValue(
                    user.Id,
                    out var count)
                    ? count
                    : 0;

                result.Add(new TenantMemberDto(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!,
                    role,
                    user.IsSuspended,
                    casesHandled));
            }

            if (IsAlphabetical.HasValue)
            {
                result = [.. result.OrderBy(x => x.FirstName)];
            }

            if (IsMostCases.HasValue)
            {
                result = [.. result.OrderBy(x => x.CasesHandled)];
            }

            if (Name != null)
            {
                result = [.. result.Where(x => x.FirstName!.Contains(Name) 
                || x.LastName!.Contains(Name))];
            }

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
            var previousTenantId = user.TenantId;
            user.TenantId = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return HandleIdentityResult(result);

            // Notify the user that they have been removed before invalidating session
            await domainEventsDispatcher.DispatchAsync(new IDomainEvent[] {
                new Avera.Domain.Identity.Users.Events.UserRemovedFromTenantDomainEvent(
                    user.Id,
                    previousTenantId ?? Guid.Empty,
                    _userContext.UserId,
                    DateTime.UtcNow)
            }, cancellationToken);

            // Invalidate the user's security stamp to force re-authentication
            await _userManager.UpdateSecurityStampAsync(user);

            return Result.Success();
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

            // Dispatch domain event to notify the user first
            await domainEventsDispatcher.DispatchAsync(new IDomainEvent[] {
                new Avera.Domain.Identity.Users.Events.UserSuspendedDomainEvent(
                    user.Id,
                    user.TenantId,
                    DateTime.UtcNow)
            }, cancellationToken);

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

        public async Task<Result> RenameOrganization(string newName, CancellationToken cancellationToken = default)
        {
            var tenant = await _identityDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == _userContext.TenantId, cancellationToken);

            if (tenant == null)
            {
                Result.Failure(TenantErrors.TenantNotFound);
            }

            tenant!.Name = newName;

            await _identityDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}