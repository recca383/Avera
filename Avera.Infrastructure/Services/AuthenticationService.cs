using System.Reflection.Metadata;
using System.Security.Cryptography.Pkcs;
using System.Security.Principal;
using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Services;
using Avera.Application.Authentication.ForgotPassword;
using Avera.Application.Authentication.Login;
using Avera.Application.Authentication.Register;
using Avera.Application.Authentication.ResetPassword;
using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.Configuration;
using Serilog;
using SharedKernel;

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
            IUserContext _userContext
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

            var role = await _userManager.GetRolesAsync(user);

            if (!result.Succeeded)
            {
                return Result.Failure<LoginResponse>(UserErrors.InvalidPassword);
            }

            var roleList = role.ToList();
            string accessToken = await _jwtProvider.GenerateAccessTokenAsync(user, roleList, cancellationToken);

            LoginResponse response = new LoginResponse
            (
                accessToken,
                System.DateTime.UtcNow.AddDays(1)
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

        public async Task<Result> JoinInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default)
        {
            if(inviteCode is null)
                return Result.Failure(UserErrors.InvalidInviteCode);

            var tenant = identityDbContext.Tenants.SingleOrDefault(t => t.InviteCode == inviteCode);

            if(tenant is null)
                return Result.Failure(UserErrors.JoinInviteCodeFailed);

            // TEMPORARY : NO MEMBER REQUEST YET FOR DEVELOPMENT, UNCOMMENT TO REMOVE
            // var memberRequest = new MemberRequest(
            //     _userContext.UserId,
            //     tenant.Id
            // );

            // await identityDbContext.MemberRequests.AddAsync(memberRequest, cancellationToken);

            var user = await _userManager.FindByIdAsync(_userContext.UserId.ToString());

            user!.TenantId = tenant.Id;

            await identityDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}