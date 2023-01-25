using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaylodeWeather.Domain.Model;
using ILogger = Serilog.ILogger;

namespace PaylodeWeather.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger _logger;

        public WeatherForecastController(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Attempts to retrieve weather forecast data
        /// </summary>
        /// <returns>A simulation of weather forecast</returns>
        [HttpGet("get-weather")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.Information("Incoming HTTP get weather request");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}