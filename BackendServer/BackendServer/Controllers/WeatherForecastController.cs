using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SharedModule;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController :ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    private readonly ILogger _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }
    [HttpGet]
    [Authorize]
    public ActionResult<IEnumerable<WeatherForecast>> Get()
    {  
        var ipAdd = HttpContext.Connection.RemoteIpAddress;
            _logger.LogInformation($"The ip address is {ipAdd}");

            var scheme = HttpContext.Request.Scheme;
            _logger.LogInformation($"The scheme address is {scheme}");


            var jtiFromUser = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            _logger.LogInformation("Fetching weather forecast data.");

            try
            {
                var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                }).ToArray();

                return Ok(forecasts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching weather forecast data.");
                throw; // Re-throw the exception after logging
            }
        
    }
}