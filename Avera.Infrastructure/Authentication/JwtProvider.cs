using System.Security.Claims;
using System.Text;
using Avera.Infrastructure.Identity.Roles;
using Avera.Infrastructure.Identity.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Avera.Infrastructure.Authentication
{
    internal sealed class JwtProvider(IConfiguration configuration)
    {
        public async Task<string> GenerateAccessTokenAsync(User user, List<string> role, CancellationToken cancellationToken = default)
        {
            string secretKey = configuration["Secret-Key"]!;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.GroupSid, user.TenantId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = credentials
            };

            tokenDescriptor.Subject.AddClaims(role.Select(r => new Claim(ClaimTypes.Role, r)));

            var handler = new JsonWebTokenHandler();

            string token = handler.CreateToken(tokenDescriptor);

            return token;
        }   
    }
}