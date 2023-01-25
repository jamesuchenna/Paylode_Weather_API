using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PaylodeWeather.Core.Interfaces;
using PaylodeWeather.Domain.Model;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WeatherApi.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _manager;
        private readonly ILogger _logger;

        public TokenService(IConfiguration configuration, UserManager<AppUser> userManager, ILogger logger)
        {
            _configuration = configuration;
            _manager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Generates JWT token for a logged in user
        /// </summary>
        /// <param name="user"></param>
        /// <returns> JWT token</returns>
        public async Task<string> GenerateToken(AppUser user)
        {
            _logger.Information($"Mapping Claims to {user.Email}");
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
            };
            _logger.Information($"Attempting to retrieve {user.Email} roles");
            var roles = await _manager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                _logger.Information($"Adding {user.Email} roles");
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWTSettings:key")));
            var jwtConfig = _configuration.GetSection("JWTSettings");

            _logger.Information($"Generating JWT token for {user.Email}");
            var token = new JwtSecurityToken
            (
            issuer: _configuration.GetValue<string>("JWTSettings:Issuer"),
                audience: _configuration.GetValue<string>("JWTSettings:Audience"),
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtConfig.GetSection("lifetime").Value)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512Signature
             ));

            _logger.Information($"JWT token for {user.Email} successfully generated and returned");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates a JWT refresh token
        /// </summary>
        /// <returns> JWT refresh token</returns>
        public string GenerateRefreshToken()
        {
            _logger.Information("Refresh token successfully generated.");
            return Guid.NewGuid().ToString();
        }

    }
}
