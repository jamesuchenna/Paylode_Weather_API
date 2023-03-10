using PaylodeWeather.Domain.Model;

namespace PaylodeWeather.Core.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates JWT token for a logged in user
        /// </summary>
        /// <param name="user"></param>
        /// <returns> JWT token</returns>
        Task<string> GenerateToken(AppUser user);

        /// <summary>
        /// Generates a JWT refresh token
        /// </summary>
        /// <returns> JWT refresh token</returns>
        string GenerateRefreshToken();
    }
}
