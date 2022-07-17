using System.Collections.Generic;
using System.Linq;

namespace SimpleWeatherApp.Api.Models
{
    public class WeatherSettings
    {
        public string BaseAddress { get; set; }
        public string APIKeys { get; set; }

        public List<string> APIKeyList
        {
            get
            {
                return APIKeys.Split(",").ToList();
            }
        }
    }
}
