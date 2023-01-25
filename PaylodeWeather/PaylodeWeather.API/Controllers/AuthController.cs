using Microsoft.AspNetCore.Mvc;
using PaylodeWeather.Core.Dtos;
using PaylodeWeather.Core.Interfaces;
using ILogger = Serilog.ILogger;

namespace PaylodeWeather.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger _logger;

        public AuthController(IAuthService authService, ILogger logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Attempts to register a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>201 created status code if successful, and appropriate error codes if otherwise</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegistrationDTO model)
        {
            _logger.Information("Incoming HTTP registration request.");
            var result = await _authService.Register(model);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Attempts to login a registered user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>201 if successful and the user's data, else it returns the appropriate error codes</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            _logger.Information("Incoming HTTP login request.");
            var response = await _authService.Login(model);
            return StatusCode(response.StatusCode, response);
        }
    }
}
