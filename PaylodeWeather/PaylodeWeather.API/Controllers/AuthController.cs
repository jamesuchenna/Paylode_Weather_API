using Microsoft.AspNetCore.Mvc;
using PaylodeWeather.Core.Dtos;
using PaylodeWeather.Core.Interfaces;
using System.Net.Mime;

namespace PaylodeWeather.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Attempts to register a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns 201 created status code if successful, and appropriate error codes if otherwise</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegistrationDTO model)
        {
            var result = await _authService.Register(model);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Attempts to login a registered user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>If successful it returns Ok and the user's data, else it returns the appropriate error codes</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var response = await _authService.Login(model);
            return StatusCode(response.StatusCode, response);
        }
    }
}
