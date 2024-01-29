using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DotNetWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> logger = logger;
        private readonly string[] summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet]
        public object[] GetWeatherForecast()
        {
            //// Create a new Activity scoped to the method
            //var weatherForecastGetActivitySource = new ActivitySource("DotNetWebAPI.WeatherForecast.Get");
            //using var activity = weatherForecastGetActivitySource.StartActivity("GreeterActivity");

            var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
            logger.LogWarning("This warning should reach ELK stack...");
            //activity.Stop();
            return forecast;
        }
    }
}
