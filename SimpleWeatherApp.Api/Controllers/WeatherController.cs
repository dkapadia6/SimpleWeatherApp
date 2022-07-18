using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleWeatherApp.Api.Models;
using SimpleWeatherApp.Api.Attributes;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Newtonsoft.Json;

namespace SimpleWeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherSettings _weatherSettings;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(ILogger<WeatherController> logger, IOptions<WeatherSettings> settingsOptions)
        {
            _logger = logger;
            _weatherSettings = settingsOptions.Value;
        }

        [HttpGet]
        [RequestLimit(MaxRequests = 5, TimeLimit = 60)]
        public async Task<ActionResult> Get(string city, string country)
        {
            if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(country))
            {
                string appId = _weatherSettings.APIKeyList[0];

                string url = $"{_weatherSettings.BaseAddress}?q={city},{country}&appid={appId}";

                using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri(_weatherSettings.BaseAddress);
                        var response = await client.GetAsync($"?q={city},{country}&appid={appId}");
                        response.EnsureSuccessStatusCode();

                        var stringResult = await response.Content.ReadAsStringAsync();
                        var weatherResponse = JsonConvert.DeserializeObject<OpenWeatherReponse>(stringResult);

                        if (weatherResponse != null && weatherResponse.weather.Count > 0)
                        {
                            return Ok(new
                            {
                                Message = string.Join(",", weatherResponse.weather.Select(x => x.description))
                            });
                        }

                        return Ok(new
                        {
                            Message = "No weather description returned"
                        });
                    }
                    catch (HttpRequestException httpRequestException)
                    {
                        return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                    }
                }
            }

            return BadRequest("Missing parameters - city and country");
        }
    }
}
