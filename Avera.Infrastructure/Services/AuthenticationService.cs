using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Abstractions.Services;
using Avera.Application.Authentication.Common;
using Avera.Application.Authentication.ForgotPassword;
using Avera.Application.Authentication.Login;
using Avera.Application.Authentication.Register;
using Avera.Application.Authentication.ResetPassword;
using Avera.Application.MemberRequests.Notifications;
using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Authentication;
using Avera.Infrastructure.Configuration;
using Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using SharedKernel;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Security.Cryptography.Pkcs;
using System.Security.Principal;
using System.Text;

namespace Avera.Infrastructure.Services
{
    internal sealed class AuthenticationService
        (
            UserManager<User> _userManager,
            SignInManager<User> _signInManager,
            JwtProvider _jwtProvider,
            IEmailService emailService,
            IConfiguration configuration,
            IIdentityDbContext identityDbContext,
            IUserContext _userContext,
            IMemberRequestNotifier memberRequestNotifier,
            IDateTimeProvider dateTime,
            IOptions<AppOptions> appOptions,
            IDomainEventsDispatcher domainEventsDispatcher
        ) : IAuthenticationService
    {
        private static readonly ILogger Logger = Log.ForContext<AuthenticationService>();

        private const string TEMP_SUPPORT_EMAIL = "sirpatrick121402@gmail.com";

        public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            Logger.Information("Starting password change for user {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                Logger.Warning("Password change failed because user {UserId} was not found", userId);
                return Result.Failure(UserErrors.UserNotFound);
            }

            Logger.Information("Changing password for user {UserId}", userId);
            IdentityResult? result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                Logger.Warning("Password change failed for user {UserId}: {Errors}", userId, result.Errors.Select(e => e.Description));
                return HandleIdentityResult(result);
            }

            Logger.Information("Password change completed successfully for user {UserId}", userId);
            return Result.Success();
        }
        public async Task<Result<TokenExpiryResponse>> ForgotPasswordAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            Logger.Information(
                "Starting forgot password flow for email {Email}",
                email);

            var codeExpiryInMinutes =
                Convert.ToInt32(
                    configuration["Identity:TokenExpiryInMinutes"]!);

            User? user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                Logger.Warning(
                    "Forgot password failed because email {Email} was not found",
                    email);

                return Result.Success(new TokenExpiryResponse(codeExpiryInMinutes));
            }

            Logger.Information(
                "Generating reset token for user {UserId}",
                user.Id);

            string code =
                await _userManager.GeneratePasswordResetTokenAsync(user);

            Logger.Information(
                "Forgot password token generated for user {UserId}",
                user.Id);

            var emailResult =
                await emailService.SendForgotPasswordEmailAsync(
                    email,
                    user.FirstName!,
                    code,
                    TEMP_SUPPORT_EMAIL,
                    codeExpiryInMinutes,
                    cancellationToken);

            if (emailResult.IsFailure)
            {
                Logger.Warning("Forgot password email delivery failed for {Email}: {Error}", email, emailResult.Error);
            }

            return Result.Success(
                new TokenExpiryResponse(codeExpiryInMinutes));
        }
        public async Task<Result<LoginResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            Logger.Information("Starting login for email {Email}", email);

            User? user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                Logger.Warning("Login failed because email {Email} was not found", email);
                return Result.Failure<LoginResponse>(UserErrors.EmailNotFound);
            }

            Logger.Information("Checking password for user {UserId}", user.Id);
            var result = await _signInManager.CheckPasswordSignInAsync
            (
                user,
                password,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                return Result.Failure<LoginResponse>(UserErrors.InvalidPassword);
            }

            if (!user.EmailConfirmed)
            {
                return Result.Failure<LoginResponse>(UserErrors.EmailNotConfirmed);
            }

            if (user.IsSuspended)
            {
                Logger.Warning("Login failed because user {UserId} is suspended", user.Id);
                return Result.Failure<LoginResponse>(UserErrors.IsSuspended);
            }

            var role = await _userManager.GetRolesAsync(user);

            var roleList = role.ToList();

            string accessToken = await _jwtProvider.GenerateAccessTokenAsync(user, roleList, cancellationToken);

            LoginResponse response = new LoginResponse
            (
                accessToken,
                dateTime.PhilippineNow.AddDays(1)
            );

            Logger.Information("Login completed successfully for user {UserId}", user.Id);
            return Result.Success(response);
        }
        public async Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            Logger.Information("Starting logout for user {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user is null)
            {
                Logger.Warning("Logout failed because user {UserId} was not found", userId);
                return Result.Failure(UserErrors.UserNotFound);
            }

            Logger.Information("Updating security stamp for user {UserId}", userId);
            await _userManager.UpdateSecurityStampAsync(user);

            Logger.Information("Logout completed successfully for user {UserId}", userId);
            return Result.Success();
        }
        public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            Logger.Information("Starting registration for email {Email} with role {Role}", request.Email, request.Role);

            User user = new User()
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DailyCaseLimit = 5,
                JoinedAt = dateTime.PhilippineNow
            };

            Logger.Information("Creating identity user {UserName} for email {Email}", user.UserName, user.Email);
            var userResult = await _userManager.CreateAsync(user, request.Password);

            if (!userResult.Succeeded)
            {
                Logger.Warning("User creation failed for email {Email}: {Errors}", request.Email, userResult.Errors.Select(e => e.Description));
                return HandleIdentityResult(userResult);
            }

            Logger.Information("Identity user created successfully for {Email}", user.Email);

            Logger.Information("Assigning role {Role} to user {UserId}", request.Role, user.Id);
            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);

            if (!roleResult.Succeeded)
            {
                Logger.Warning("Role assignment failed for user {UserId} with role {Role}: {Errors}", user.Id, request.Role, roleResult.Errors.Select(e => e.Description));
                return HandleIdentityResult(roleResult);
            }

            var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var apiUrl = appOptions.Value.PublicBaseUrl.TrimEnd('/');

            var codeExpiryInMinutes = Convert.ToInt32(configuration["Identity:TokenExpiryInMinutes"]!);

            var verificationUrl =
                $"{apiUrl}/auth/verify-email" +
                $"?userId={Uri.EscapeDataString(user.Id.ToString())}" +
                $"&token={Uri.EscapeDataString(verificationToken)}";

            await emailService.SendEmailVerificationAsync(
                user.Email,
                user.FirstName, 
                verificationUrl, 
                TEMP_SUPPORT_EMAIL,
                codeExpiryInMinutes,
                cancellationToken);

            Logger.Information("Registration completed successfully for user {UserId}", user.Id);
            return Result.Success();
        }
        public async Task<Result> ResetPasswordAsync(string email, string token, string password, CancellationToken cancellationToken = default)
        {
            Logger.Information("Starting password reset for user {email}", email);

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                Logger.Warning("Password reset failed because user {UserId} was not found", email);
                return Result.Failure(UserErrors.UserNotFound);
            }

            Logger.Information("Resetting password for user {UserId}", email);
            IdentityResult? result = await _userManager.ResetPasswordAsync(user, token, password);

            if (!result.Succeeded)
            {
                Logger.Warning("Password reset failed for user {UserId}: {Errors}", email, result.Errors.Select(e => e.Description));
                return HandleIdentityResult(result);
            }

            Logger.Information("Password reset completed successfully for user {UserId}", email);

            var tenant = await identityDbContext.Tenants.SingleOrDefaultAsync(t => t.Id == user.TenantId);

            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            var admin = admins.SingleOrDefault(a => a.TenantId == tenant.Id);

            var notificationResult = await emailService.ResetPasswordNotificationAsync(
                email,
                user.FirstName,
                tenant?.Name ?? string.Empty,
                admin is null ? string.Empty : admin.FirstName + " " + admin.LastName,
                TEMP_SUPPORT_EMAIL,
                cancellationToken);

            if (notificationResult.IsFailure)
            {
                Logger.Warning("Password reset notification failed for user {UserId}: {Error}", user.Id, notificationResult.Error);
            }

            return Result.Success();
        }
        public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if(user is null)
            {
                return Result.Failure<Result>(UserErrors.UserNotFound);
            }

            var tenantId = user.TenantId;

            // Notify admins (if any) that the user deleted their account
            await domainEventsDispatcher.DispatchAsync(new IDomainEvent[] {
                new Avera.Domain.Identity.Users.Events.UserDeletedDomainEvent(
                    user.Id,
                    tenantId,
                    DateTime.UtcNow)
            }, cancellationToken);

            var result = await _userManager.DeleteAsync(user);

            if(!result.Succeeded)
            {
                return HandleIdentityResult(result);
            }

            return Result.Success();
        }
        public async Task<Result> JoinInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default)
        {
            if(inviteCode is null)
                return Result.Failure(UserErrors.InvalidInviteCode);

            var tenant = await identityDbContext.Tenants.SingleOrDefaultAsync(
                t => t.InviteCode == inviteCode, cancellationToken);

            if (tenant is null)
                return Result.Failure(UserErrors.JoinInviteCodeFailed);

            var user = await _userManager.FindByIdAsync(_userContext.UserId.ToString());

            if(user == null)
            {
                return Result.Failure(UserErrors.UserNotFound);
            }

            var memberRequest = new MemberRequest(
                _userContext.UserId,
                tenant.Id
             );

            memberRequest.CreatedAt = dateTime.PhilippineNow;

            var duplicateRequestResult = await CheckDuplicateMemberRequest(_userContext.UserId, tenant.Id, cancellationToken);

            if (duplicateRequestResult.IsFailure)
                return duplicateRequestResult;

            var multipleTenantsResult = await CheckJoiningMultipleTenants(
                _userContext.UserId,
                cancellationToken);

            if (multipleTenantsResult.IsFailure)
                return multipleTenantsResult;

            await identityDbContext.MemberRequests.AddAsync(memberRequest, cancellationToken);

            // Raise domain event so notification is dispatched through the domain event pipeline
            memberRequest.Raise(new Avera.Domain.Identity.MemberRequests.Events.MemberRequestCreatedDomainEvent(
                memberRequest.Id,
                memberRequest.TenantId,
                memberRequest.UserId,
                memberRequest.CreatedAt
            ));

            await identityDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        public async Task<Result> VerifyPasswordResetCodeAsync(
            string email,
            string code,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Result.Failure(UserErrors.EmailNotFound);

            var isValid = await _userManager.VerifyUserTokenAsync(
                user,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                UserManager<User>.ResetPasswordTokenPurpose,
                code);

            if (!isValid)
                return Result.Failure(UserErrors.InvalidToken);

            return Result.Success();
        }
        public async Task<Result<TokenExpiryResponse>> ChangeEmailAsync(
             Guid userId,
             string newEmail,
             string currentPassword,
             CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(
                userId.ToString());

            if (user is null)
            {
                return Result.Failure<TokenExpiryResponse>(
                    UserErrors.UserNotFound);
            }

            var passwordValid =
                await _userManager.CheckPasswordAsync(
                    user,
                    currentPassword);

            if (!passwordValid)
            {
                return Result.Failure<TokenExpiryResponse>(
                    UserErrors.InvalidPassword);
            }

            var existingUser =
                await _userManager.FindByEmailAsync(newEmail);

            if (existingUser is not null &&
                existingUser.Id != user.Id)
            {
                return Result.Failure<TokenExpiryResponse>(
                    UserErrors.EmailAlreadyExists);
            }

            var verificationToken =
                await _userManager.GenerateChangeEmailTokenAsync(
                    user,
                    newEmail);

            var apiUrl =
                appOptions.Value.PublicBaseUrl.TrimEnd('/');

            var codeExpiryInMinutes =
                Convert.ToInt32(
                    configuration["Identity:TokenExpiryInMinutes"]!);

            var verificationUrl =
                $"{apiUrl}/auth/verify-email" +
                $"?userId={Uri.EscapeDataString(user.Id.ToString())}" +
                $"&type=change-email" +
                $"&email={Uri.EscapeDataString(newEmail)}" +
                $"&token={Uri.EscapeDataString(verificationToken)}";

            var emailResult =
                await emailService.SendEmailChangeVerificationAsync(
                    newEmail,
                    TEMP_SUPPORT_EMAIL,
                    user.FirstName,
                    verificationUrl,
                    codeExpiryInMinutes,
                    cancellationToken);

            if (emailResult.IsFailure)
            {
                Logger.Warning("Verification email delivery failed for {Email}: {Error}", newEmail, emailResult.Error);
            }

            return Result.Success(
                new TokenExpiryResponse(codeExpiryInMinutes));
        }

        public async Task<Result<string>> VerifyEmailAsync(
            Guid userId,
            string token,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(
                userId.ToString());

            if (user is null)
                return Result.Failure<string>(UserErrors.UserNotFound);

            if (user.EmailConfirmed)
                return Result.Failure<string>(UserErrors.EmailAlreadyVerified);

            var result = await _userManager.ConfirmEmailAsync(
                user,
                token);

            if (!result.Succeeded)
            {
                Logger.Warning(
                    "Email verification failed for user {UserId}: {Errors}",
                    userId,
                    result.Errors.Select(x => x.Description));

                return HandleIdentityResult<string>(result);
            }

            var app = appOptions.Value.DeepLinkBase + "_sucessPage/emailVerified";

            await emailService.SendEmailVerified(user.Email!, user.FirstName!, app!, cancellationToken);

            Logger.Information(
                "Email verified successfully for user {UserId}",
                userId);

            return Result.Success<string>(app);
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

        private async Task<Result> CheckDuplicateMemberRequest(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            var duplicateRequest = await identityDbContext
                .MemberRequests
                .AnyAsync(mr => mr.UserId == userId 
                             && mr.TenantId == tenantId 
                             && mr.Status != MemberRequestStatus.Pending,
                             cancellationToken);

            if (duplicateRequest)
                return Result.Failure(UserErrors.MemberRequestIsDuplicate);
            return Result.Success();
        }

        private async Task<Result> CheckJoiningMultipleTenants(Guid userId, CancellationToken cancellationToken = default)
        {
            var joiningMultipleTimes = await identityDbContext
                .MemberRequests
                .AnyAsync(mr => mr.UserId == userId 
                             && mr.Status == MemberRequestStatus.Pending,
                             cancellationToken);

            if (joiningMultipleTimes)
                return Result.Failure(UserErrors.MemberIsJoiningMultipleTimes);
            return Result.Success();
        }

        public async Task<Result<string>> VerifyEmailChangeAsync(Guid userId, string newEmail, string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains('@'))
                return Result.Failure<string>(UserErrors.InvalidEmail);

            var user = await _userManager.FindByIdAsync(
             userId.ToString());

            if (user is null)
                return Result.Failure<string>(UserErrors.UserNotFound);

            var oldEmail = user.Email;

            var existingUser = await _userManager.FindByEmailAsync(
                newEmail);

            if (existingUser is not null &&
                existingUser.Id != user.Id)
            {
                return Result.Failure<string>(UserErrors.EmailAlreadyExists);
            }

            await using var transaction = await identityDbContext.Database.BeginTransactionAsync(cancellationToken);

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

            if (!result.Succeeded)
                return HandleIdentityResult<string>(result);

            await transaction.CommitAsync(cancellationToken);

            user.UserName = newEmail;

            result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return HandleIdentityResult<string>(result);

            await _userManager.UpdateSecurityStampAsync(user);

            string app = string.Empty;
            
            if(await _userManager.IsInRoleAsync(user, "Admin"))
            {
                app = appOptions.Value.DeepLinkBase + "Admin/profileScreens/EditProfileScreen";
            }
            else
            {
                app = appOptions.Value.DeepLinkBase + "_User/user_profile";
            }

            await emailService.SendEmailNotificationToNewEmail(
                    user.Email!,
                    user.FirstName!,
                    DateOnly.FromDateTime(dateTime.PhilippineNow),
                    TimeOnly.FromDateTime(dateTime.PhilippineNow),
                    app,
                    cancellationToken
                    );

            await emailService.SendEmailNotificationToOldEmail(
                    oldEmail!,
                    user.FirstName!,
                    user.Email!,
                    DateOnly.FromDateTime(dateTime.PhilippineNow),
                    TimeOnly.FromDateTime(dateTime.PhilippineNow),
                    TEMP_SUPPORT_EMAIL,
                    cancellationToken
                    );

            return Result.Success<string>(app);
        }

        public async Task<Result<TokenExpiryResponse>> ResendVerificationEmailAsync(string email, CancellationToken cancellation = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result.Success(new TokenExpiryResponse(
                    Convert.ToInt32(configuration["Identity:TokenExpiryInMinutes"]!)));
            }

            if (user.EmailConfirmed)
            {
                return Result.Success(new TokenExpiryResponse(
                    Convert.ToInt32(configuration["Identity:TokenExpiryInMinutes"]!)));
            }


            var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var apiUrl = appOptions.Value.PublicBaseUrl.TrimEnd('/');

            var codeExpiryInMinutes = Convert.ToInt32(configuration["Identity:TokenExpiryInMinutes"]!);

            var verificationUrl =
                $"{apiUrl}/auth/verify-email" +
                $"?userId={Uri.EscapeDataString(user.Id.ToString())}" +
                $"&token={Uri.EscapeDataString(verificationToken)}";

            var emailResult = await emailService.SendEmailVerificationAsync(
                user.Email!,
                user.FirstName!,
                verificationUrl,
                TEMP_SUPPORT_EMAIL,
                codeExpiryInMinutes,
                cancellation);

            if (emailResult.IsFailure)
            {
                return Result.Failure<TokenExpiryResponse>(
                    emailResult.Error);
            }

            return Result.Success(
                new TokenExpiryResponse(codeExpiryInMinutes));
        }

        public async Task<Result<bool>> GetEmailVerificationStatusAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(
                userId.ToString());

            if (user is null)
            {
                return Result.Failure<bool>(
                    UserErrors.UserNotFound);
            }

            return Result.Success(user.EmailConfirmed);
        }

        public async Task<Result<TokenExpiryResponse>> ResendEmailChangeVerificationAsync(
                string email,
                string newEmail,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains('@'))
                return Result.Failure<TokenExpiryResponse>(UserErrors.InvalidEmail);

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null || user.Id != _userContext.UserId)
            {
                return Result.Failure<TokenExpiryResponse>(
                    UserErrors.UserNotFound);
            }

            var existingUser =
                await _userManager.FindByEmailAsync(newEmail);

            if (existingUser is not null &&
                existingUser.Id != user.Id)
            {
                return Result.Failure<TokenExpiryResponse>(
                    UserErrors.EmailAlreadyExists);
            }

            var verificationToken =
                await _userManager.GenerateChangeEmailTokenAsync(
                    user,
                    newEmail);

            var apiUrl =
                appOptions.Value.PublicBaseUrl.TrimEnd('/');

            var codeExpiryInMinutes =
                Convert.ToInt32(
                    configuration["Identity:TokenExpiryInMinutes"]!);

            var verificationUrl =
                $"{apiUrl}/auth/verify-email" +
                $"?userId={Uri.EscapeDataString(user.Id.ToString())}" +
                $"&type=change-email" +
                $"&email={Uri.EscapeDataString(newEmail)}" +
                $"&token={Uri.EscapeDataString(verificationToken)}";

            var emailResult =
                await emailService.SendEmailChangeVerificationAsync(
                    newEmail,
                    TEMP_SUPPORT_EMAIL,
                    user.FirstName ?? string.Empty,
                    verificationUrl,
                    codeExpiryInMinutes,
                    cancellationToken);

            if (emailResult.IsFailure)
            {
                return Result.Failure<TokenExpiryResponse>(
                    emailResult.Error);
            }

            return Result.Success(
                new TokenExpiryResponse(codeExpiryInMinutes));
        }
    }
}