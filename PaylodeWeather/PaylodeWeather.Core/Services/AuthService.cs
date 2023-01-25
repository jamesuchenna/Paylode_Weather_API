using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PaylodeWeather.Core.Dtos;
using PaylodeWeather.Core.Interfaces;
using PaylodeWeather.Domain.Enums;
using PaylodeWeather.Domain.Model;
using System.Net;

namespace PaylodWeather.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public AuthService(ITokenService tokenService, UserManager<AppUser> userManager,
            IMapper mapper, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }
        
        /// <summary>
        /// Attempts to login a registered user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>If successful it returns Ok and the user's data, else it returns the appropriate error codes</returns>
        public async Task<ResponseDto<CredentialResponseDTO>> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return ResponseDto<CredentialResponseDTO>.Fail("User does not exist", (int)HttpStatusCode.NotFound);
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return ResponseDto<CredentialResponseDTO>.Fail("Invalid user credential", (int)HttpStatusCode.BadRequest);
            }

            user.RefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); //sets refresh token for 7 days
            var credentialResponse = new CredentialResponseDTO()
            {
                Id = user.Id,
                Token = await _tokenService.GenerateToken(user),
                RefreshToken = user.RefreshToken
            };

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                //_logger.Information("User successfully logged in");
                return ResponseDto<CredentialResponseDTO>.Success("Login successful", credentialResponse);
            }
            return ResponseDto<CredentialResponseDTO>.Fail("Failed to login user", (int)HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Attempts to register a new user
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns>201 created status code if successful, and 400 Bad request if otherwise</returns>
        public async Task<ResponseDto<RegistrationResponseDTO>> Register(RegistrationDTO userDetails)
        {
            var checkEmail = await _userManager.FindByEmailAsync(userDetails.Email);
            if (checkEmail != null)
            {
                return ResponseDto<RegistrationResponseDTO>.Fail("Email already Exists", (int)HttpStatusCode.BadRequest);
            }
            var userModel = _mapper.Map<AppUser>(userDetails);
            await _userManager.CreateAsync(userModel, userDetails.Password);

            return ResponseDto<RegistrationResponseDTO>.Success("Registration Successful",
                new RegistrationResponseDTO { Id = userModel.Id, Email = userModel.Email },
                (int)HttpStatusCode.Created);
        }
    }
}
