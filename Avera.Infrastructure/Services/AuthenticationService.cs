using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Abstractions.Services;
using Avera.Application.Authentication.ForgotPassword;
using Avera.Application.Authentication.Login;
using Avera.Application.Authentication.Register;
using Avera.Application.Authentication.ResetPassword;
using Avera.Application.MemberRequests.Notifications;
using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Authentication;
using Avera.Infrastructure.Configuration;
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
            IOptions<AppOptions> appOptions
        ) : IAuthenticationService
    {
        private static readonly ILogger Logger = Log.ForContext<AuthenticationService>();

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
        public async Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
        {
            Logger.Information("Starting forgot password flow for email {Email}", email);
            
            string codeExpiryInMinutes = configuration["Identity:TokenExpiryInMinutes"]!;

            User? user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                Logger.Warning("Forgot password failed because email {Email} was not found", email);
                return Result.Failure(UserErrors.EmailNotFound);
            }

            Logger.Information("Generating reset token for user {UserId}", user.Id);
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            Logger.Information("Forgot password token generated for user {UserId}", user.Id);

            return await emailService.SendForgotPasswordEmailAsync(email, token, codeExpiryInMinutes, cancellationToken);
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
                LastName = request.LastName
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

            var verificationUrl =
                $"{apiUrl}/auth/verify-email" +
                $"?userId={Uri.EscapeDataString(user.Id.ToString())}" +
                $"&token={Uri.EscapeDataString(verificationToken)}";

            await emailService.SendEmailVerificationAsync(user.Email, user.FirstName, verificationUrl, cancellationToken);

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

            await emailService.ResetPasswordNotificationAsync(email, cancellationToken);

            return Result.Success();
        }
        public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if(user is null)
            {
                return Result.Failure<Result>(UserErrors.UserNotFound);
            }

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

            var tenant = identityDbContext.Tenants.SingleOrDefault(t => t.InviteCode == inviteCode);

            if(tenant is null)
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

            var results = await Task.WhenAll(
               CheckDuplicateMemberRequest(_userContext.UserId, tenant.Id, cancellationToken),
               CheckJoiningMultipleTenants(_userContext.UserId, cancellationToken));

            if (results.Any(t => t.IsFailure))
                return results.First(t => t.IsFailure);

            await identityDbContext.MemberRequests.AddAsync(memberRequest, cancellationToken);

            await identityDbContext.SaveChangesAsync(cancellationToken);

            var memberRequestCreated = new MemberRequestCreatedNotification(
                memberRequest.Id,
                memberRequest.UserId,
                user.FirstName!,
                user.LastName!,
                user.Email!,
                tenant.Id,
                memberRequest.CreatedAt
            );

            await memberRequestNotifier.NotifyMemberRequestCreatedAsync(memberRequestCreated, cancellationToken);

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
        public async Task<Result> ChangeEmailAsync(
            Guid userId,
            string newEmail,
            string currentPassword,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(
                userId.ToString());

            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            var passwordValid = await _userManager.CheckPasswordAsync(
                user,
                currentPassword);

            if (!passwordValid)
                return Result.Failure(UserErrors.InvalidPassword);

            var existingUser = await _userManager.FindByEmailAsync(
                newEmail);

            if (existingUser is not null &&
                existingUser.Id != user.Id)
            {
                return Result.Failure(UserErrors.EmailAlreadyExists);
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(
                user,
                newEmail);

            var verificationUrl =
                $"{GetAppDeepLinkBase()}" +
                $"verify-email?" +
                $"userId={user.Id}" +
                $"&type=change-email" +
                $"&email={Uri.EscapeDataString(newEmail)}" +
                $"&token={Uri.EscapeDataString(token)}";

            return await emailService.SendEmailChangeVerificationAsync(
                newEmail,
                user.FirstName ?? string.Empty,
                verificationUrl,
                cancellationToken);
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

            var app = appOptions.Value.DeepLinkBase + "_login/_signup/VerifyEmailInstruction";

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
            var duplicateRequest = await identityDbContext.MemberRequests.AnyAsync(mr => mr.UserId == userId && mr.TenantId == tenantId, cancellationToken);
            if (duplicateRequest)
                return Result.Failure(UserErrors.MemberRequestIsDuplicate);
            return Result.Success();
        }

        private async Task<Result> CheckJoiningMultipleTenants(Guid userId, CancellationToken cancellationToken = default)
        {
            var joiningMultipleTimes = await identityDbContext.MemberRequests.AnyAsync(mr => mr.UserId == userId, cancellationToken);
            if (joiningMultipleTimes)
                return Result.Failure(UserErrors.MemberIsJoiningMultipleTimes);
            return Result.Success();
        }

        public async Task<Result<string>> VerifyEmailChangeAsync(Guid userId, string newEmail, string token, CancellationToken cancellationToken = default)
        {
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

            var result = await _userManager.ChangeEmailAsync(
                user,
                newEmail,
                token);

            if (!result.Succeeded)
                return HandleIdentityResult<string>(result);

            user.UserName = newEmail;

            result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return HandleIdentityResult<string>(result);

            await _userManager.UpdateSecurityStampAsync(user);

            await Task.WhenAll(
                emailService.SendEmailNotificationToNewEmail(
                    user.Email!,
                    user.FirstName!,
                    DateOnly.FromDateTime(dateTime.PhilippineNow),
                    TimeOnly.FromDateTime(dateTime.PhilippineNow),
                    GetAppDeepLinkBase()!,
                    cancellationToken
                    ),
                emailService.SendEmailNotificationToOldEmail(
                    oldEmail!,
                    user.FirstName!,
                    user.Email!,
                    DateOnly.FromDateTime(dateTime.PhilippineNow),
                    TimeOnly.FromDateTime(dateTime.PhilippineNow),
                    cancellationToken
                    )
                );

            var app = appOptions.Value.DeepLinkBase + "_login/_signup/VerifyEmailInstruction";


            return Result.Success<string>(app);
        }

        public async Task<Result> ResendVerificationEmailAsync(string email, CancellationToken cancellation = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result.Failure(
                    UserErrors.UserNotFound);
            }

            if (user.EmailConfirmed)
            {
                return Result.Failure(
                    UserErrors.EmailAlreadyVerified);
            }


            var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var verificationUrl = $"{GetAppDeepLinkBase()}" +
                                  $"verify-email?userId={user.Id}" +
                                  $"&token={Uri.EscapeDataString(verificationToken)}";

            return await emailService.SendEmailVerificationAsync(user.Email!, user.FirstName!, verificationUrl, cancellation);
        }

        public async Task<Result> ResendEmailChangeVerificationAsync(string email, string newEmail, CancellationToken cancellation = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result.Failure(
                    UserErrors.UserNotFound);
            }

            if (user.EmailConfirmed)
            {
                return Result.Failure(
                    UserErrors.EmailAlreadyVerified);
            }


            var token = await _userManager.GenerateChangeEmailTokenAsync(
                user,
                newEmail);

            var verificationUrl =
                $"{GetAppDeepLinkBase()}" +
                $"verify-email?" +
                $"userId={user.Id}" +
                $"&type=change-email" +
                $"&email={Uri.EscapeDataString(newEmail)}" +
                $"&token={Uri.EscapeDataString(token)}";

            return await emailService.SendEmailChangeVerificationAsync(
                newEmail,
                user.FirstName ?? string.Empty,
                verificationUrl,
                cancellation);
        }



        private string GetAppDeepLinkBase() => configuration["App:DeepLinkBase"]!;
    }
}