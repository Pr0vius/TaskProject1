using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ms.auth.domain.Interfaces;
using ms.auth.domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ms.auth.application.Services
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        private readonly IConfiguration _configuration = configuration;


       public string CreateToken(Guid id, string username, Roles role)
        {
            var privateKey = _configuration.GetSection("JWT:PrivateKey").Value!;
            int expirationHours = int.Parse(_configuration.GetSection("JWT:ExpirationHours").Value!);

            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));
            var signInCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim> {
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Sub, id.ToString()),
                new (JwtRegisteredClaimNames.UniqueName, username),
                new (ClaimTypes.Role, role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(expirationHours),
                SigningCredentials = signInCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
