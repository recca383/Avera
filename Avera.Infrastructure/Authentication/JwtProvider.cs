using System.Security.Claims;
using System.Text;
using Avera.Infrastructure.Identity.Roles;
using Avera.Infrastructure.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Avera.Infrastructure.Authentication
{
    internal sealed class JwtProvider(
        IConfiguration configuration,
        UserManager<User> userManager)
    {
        private static readonly ILogger logger = Log.ForContext<AuthenticationService>();
        public async Task<string> GenerateAccessTokenAsync(User user, List<string> role, CancellationToken cancellationToken = default)
        {
            logger.Information("Starting access token generation for user {UserId}", user.Id);

            logger.Information("Fetching security stamp for user {UserId}", user.Id);
            var securityStamp = await userManager.GetSecurityStampAsync(user);
            logger.Information("Security stamp fetched for user {UserId}", user.Id);

            logger.Information("Retrieving JWT signing key from configuration");
            string secretKey = configuration["Jwt:Secret-Key"] ?? throw new InvalidOperationException("JWT signing key is not configured.");
            logger.Information("JWT signing key loaded successfully");

            logger.Information("Creating signing key for access token");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            logger.Information("Creating signing credentials for access token");
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            logger.Information("Building token descriptor for user {UserId} with {RoleCount} roles", user.Id, role.Count);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.GroupSid, user.TenantId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim("SecurityStamp", securityStamp)
                }),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddDays(1),
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                SigningCredentials = credentials
            };

            logger.Information("Adding {RoleCount} role claims to token", role.Count);
            tokenDescriptor.Subject.AddClaims(role.Select(r => new Claim(ClaimTypes.Role, r)));

            logger.Information("Creating JWT handler and token");
            var handler = new JsonWebTokenHandler();
            string token = handler.CreateToken(tokenDescriptor);

            logger.Information("Access token generated successfully for user {UserId}", user.Id);
            return token;
        }
    }
}