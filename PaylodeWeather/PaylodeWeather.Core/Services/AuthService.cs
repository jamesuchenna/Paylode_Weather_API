using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PaylodeWeather.Core.Dtos;
using PaylodeWeather.Core.Interfaces;
using PaylodeWeather.Domain.Model;
using Serilog;
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
            IMapper mapper, ILogger logger)
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
        /// <returns> 200 if successful and the user's data, else it returns the appropriate error codes</returns>
        public async Task<ResponseDto<CredentialResponseDTO>> Login(LoginDTO model)
        {
            _logger.Information($"Attempting to find user {model.Email} records in the database.");
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.Information($"User {model.Email} does not exist in the database.");
                return ResponseDto<CredentialResponseDTO>.Fail("User does not exist", (int)HttpStatusCode.NotFound);
            }


            _logger.Information($"Attempting to find verify user {model.Email} password.");
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                _logger.Information($"User {model.Email} password not valid.");
                return ResponseDto<CredentialResponseDTO>.Fail("Invalid user credential", (int)HttpStatusCode.BadRequest);
            }
            _logger.Information($"User {model.Email} password is valid.");

            _logger.Information($"Generating refresh token for user.");
            user.RefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); //sets refresh token for 7 days

            _logger.Information($"Mapping refresh token to user details.");
            var credentialResponse = new CredentialResponseDTO()
            {
                Id = user.Id,
                Token = await _tokenService.GenerateToken(user),
                RefreshToken = user.RefreshToken
            };

            _logger.Information($"Updating user details with generated tokens.");
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.Information($"User {model.Email} successfully logged in");
                return ResponseDto<CredentialResponseDTO>.Success("Login successful", credentialResponse);
            }

            _logger.Information($"User {model.Email} not logged in");
            return ResponseDto<CredentialResponseDTO>.Fail("Failed to login user", (int)HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Attempts to register a new user
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns> 201 status code if successful, and 400 Bad request if otherwise</returns>
        public async Task<ResponseDto<RegistrationResponseDTO>> Register(RegistrationDTO userDetails)
        {
            _logger.Information($"Attempting to find user {userDetails.Email} records in the database.");
            var checkEmail = await _userManager.FindByEmailAsync(userDetails.Email);
            if (checkEmail != null)
            {
                _logger.Information($"Unable to register {userDetails.Email} because it already exist in the database.");
                return ResponseDto<RegistrationResponseDTO>.Fail("Email already Exists", (int)HttpStatusCode.BadRequest);
            }

            _logger.Information("Mapping registration Dto to AppUser.");
            var userModel = _mapper.Map<AppUser>(userDetails);


            _logger.Information($"Attempting to register {userDetails.Email}.");
            await _userManager.CreateAsync(userModel, userDetails.Password);

            _logger.Information($"{userDetails.Email} successfully registered.");
            return ResponseDto<RegistrationResponseDTO>.Success("Registration Successful",
                new RegistrationResponseDTO { Id = userModel.Id, Email = userModel.Email },
                (int)HttpStatusCode.Created);
        }
    }
}
