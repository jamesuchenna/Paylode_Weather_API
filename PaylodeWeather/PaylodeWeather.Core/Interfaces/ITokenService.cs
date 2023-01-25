using PaylodeWeather.Domain.Model;

namespace PaylodeWeather.Core.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(AppUser user);
        string GenerateRefreshToken();
    }
}
