using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleWeatherApp.Api.Models;
using SimpleWeatherApp.Api.Attributes;
using Microsoft.Extensions.Options;

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
        public string Get(string city, string country)
        {
            if(!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(country))
            {
                string appId = _weatherSettings.APIKeyList[0];

                string url = $"{_weatherSettings.BaseAddress}?q={city},{country}&appid={appId}";
                
                //TODO - make a call to above URL and retrieve data

                return string.Empty;
            }
            return "hello";
        }
    }
}
