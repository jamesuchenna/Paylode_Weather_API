using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PaylodeWeather.Core.Interfaces;
using PaylodeWeather.Domain.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WeatherApi.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _manager;

        public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _manager = userManager;
        }

        /// <summary>
        /// Generates JWT token for a logged in user
        /// </summary>
        /// <param name="user"></param>
        /// <returns> returns JWT token</returns>
        public async Task<string> GenerateToken(AppUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
            };
            var roles = await _manager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWTSettings:key")));
            var jwtConfig = _configuration.GetSection("JWTSettings");
            var token = new JwtSecurityToken
            (
            issuer: _configuration.GetValue<string>("JWTSettings:Issuer"),
                audience: _configuration.GetValue<string>("JWTSettings:Audience"),
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtConfig.GetSection("lifetime").Value)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512Signature
             ));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates a JWT refresh token
        /// </summary>
        /// <returns> returns a JWT refresh token</returns>
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
