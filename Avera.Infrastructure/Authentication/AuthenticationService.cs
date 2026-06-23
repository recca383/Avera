using Avera.Application.Abstractions.Authentication;
using Avera.Application.Authentication.ForgotPassword;
using Avera.Application.Authentication.Login;
using Avera.Application.Authentication.Register;
using Avera.Application.Authentication.ResetPassword;
using Avera.Infrastructure.Identity.Users;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Avera.Infrastructure.Authentication
{
    internal sealed class AuthenticationService
        (
            UserManager<User> _userManager,
            SignInManager<User> _signInManager,
            JwtProvider _jwtProvider
        ) : IAuthenticationService
    {
        public async Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
        {
            User? user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result.Failure(ForgotPasswordCommandError.EmailNotFound());
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Send email with the token to the user (this part is not implemented in this code snippet)

            throw new NotImplementedException("Email sending functionality is not implemented.");
        }
        public async Task<Result<LoginResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            User? user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result.Failure<LoginResponse>(LoginCommandError.EmailNotFound()).Value;
            }

            var result = await _signInManager.CheckPasswordSignInAsync
            (
                user,
                password,
                lockoutOnFailure: false
            );

            var role = await _userManager.GetRolesAsync(user);

            if (!result.Succeeded)
            {
                return Result.Failure<LoginResponse>(LoginCommandError.InvalidCredentials()).Value;
            }

            var roleList = role.ToList();
            string accessToken = await _jwtProvider.GenerateAccessTokenAsync(user, roleList, cancellationToken);

            LoginResponse response = new LoginResponse
            (
                accessToken,
                System.DateTime.UtcNow.AddDays(1)
            );

            return Result.Success(response);
        }
        public Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            // If this will be changed from JWT to cookie authentication, this method will be implemented to sign out the user.

            return Task.FromResult(Result.Success());
        }
        public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            User user = new User()
            {
                Email = request.Email,
                UserName = request.FirstName + " " + request.LastName
            };

            var userResult = await _userManager.CreateAsync(user, request.Password);

            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);

            if (!userResult.Succeeded)
            {
                return Result.Failure(RegisterCommandError.UserCreationFailed);
            }

            if (!roleResult.Succeeded)
            {
                return Result.Failure(RegisterCommandError.RoleAssignmentFailed);
            }

            return Result.Success();
        }
        public async Task<Result> ResetPasswordAsync(Guid userId, string token, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Result.Failure(ResetPasswordCommandError.UserNotFound());
            }

            var result = await _userManager.ResetPasswordAsync(user, token, password);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure(ResetPasswordCommandError.InvalidToken());
        }
    }
}