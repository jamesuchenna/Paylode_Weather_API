using PaylodeWeather.Core.Dtos;

namespace PaylodeWeather.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto<CredentialResponseDTO>> Login(LoginDTO model);
        Task<ResponseDto<RegistrationResponseDTO>> Register(RegistrationDTO model);
    }
}
