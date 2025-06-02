using testbills.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using testbills.Models;

namespace testbills.Services
{
    // Service pour la gestion des tokens JWT
    public class TokenService(ILogger<TokenService> logger, IConfiguration configuration)
    {
        private const int ExpirationMinutes = 60;
        private readonly ILogger<TokenService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        // Créer un token JWT pour un utilisateur
        public string CreateToken(ApplicationUser user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var token = CreateJwtToken(
                CreateClaims(user),
                CreateSigningCredentials(),
                expiration
            );
            var tokenHandler = new JwtSecurityTokenHandler();

            _logger.LogInformation("JWT Token created");

            return tokenHandler.WriteToken(token);
        }

        // Créer un objet JwtSecurityToken
        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
            DateTime expiration) =>
            new(
                _configuration["JwtTokenSettings:ValidIssuer"],
                _configuration["JwtTokenSettings:ValidAudience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        // Créer les claims pour le token JWT
        private static List<Claim> CreateClaims(ApplicationUser user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new (JwtRegisteredClaimNames.Sub, user.Id),
                    new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new (ClaimTypes.NameIdentifier, user.Id),
                    new (ClaimTypes.Name, user.UserName),
                    new (ClaimTypes.Email, user.Email),
                    new (ClaimTypes.Role, user.Role.ToString())
                };

                return claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // Créer les informations d'identification pour signer le token JWT
        private SigningCredentials CreateSigningCredentials()
        {
            var symmetricSecurityKey = _configuration["JwtTokenSettings:SymmetricSecurityKey"];

            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(symmetricSecurityKey)
                ),
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}
