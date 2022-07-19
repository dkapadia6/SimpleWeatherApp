using Xunit;
using SimpleWeatherApp.Api.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleWeatherApp.Api.Models;
using Moq;

namespace SimpleWeatherApp.Tests
{
    public class TestWeatherController
    {
        //private readonly ILogger logger = NullLoggerFactory.Instance.CreateLogger("Test");

        [Fact]
        public async Task Get_ShouldReturn200Status()
        {
            /// Arrange
            var mockLogger = Mock.Of<ILogger<WeatherController>>();

            WeatherSettings appSettings = new WeatherSettings()
            {
                BaseAddress = "https://api.openweathermap.org/data/2.5/weather",
                APIKeys = "8b7535b42fe1c551f18028f64e8688f7"
            };
            var mockIOption = new Mock<IOptions<WeatherSettings>>();
            mockIOption.Setup(ap => ap.Value).Returns(appSettings);

            var weatherController = new WeatherController(mockLogger, mockIOption.Object);

            /// Act
            var result = (OkObjectResult)await weatherController.Get("Melbourne", "AU");

            /// Assert
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task Get_ShouldReturn400Status_InvalidCityAndCountry()
        {
            /// Arrange
            var mockLogger = Mock.Of<ILogger<WeatherController>>();

            WeatherSettings appSettings = new WeatherSettings()
            {
                BaseAddress = "https://api.openweathermap.org/data/2.5/weather",
                APIKeys = "8b7535b42fe1c551f18028f64e8688f7"
            };
            var mockIOption = new Mock<IOptions<WeatherSettings>>();
            mockIOption.Setup(ap => ap.Value).Returns(appSettings);

            var weatherController = new WeatherController(mockLogger, mockIOption.Object);

            /// Act
            var result = (BadRequestObjectResult)await weatherController.Get("Random City", "Random Country");

            /// Assert
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task Get_ShouldReturn400Status_MissingCityAndCountry()
        {
            /// Arrange
            var mockLogger = Mock.Of<ILogger<WeatherController>>();

            WeatherSettings appSettings = new WeatherSettings()
            {
                BaseAddress = "https://api.openweathermap.org/data/2.5/weather",
                APIKeys = "8b7535b42fe1c551f18028f64e8688f7"
            };
            var mockIOption = new Mock<IOptions<WeatherSettings>>();
            mockIOption.Setup(ap => ap.Value).Returns(appSettings);

            var weatherController = new WeatherController(mockLogger, mockIOption.Object);

            /// Act
            var result = (BadRequestObjectResult)await weatherController.Get(string.Empty, string.Empty);

            /// Assert
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task Get_ShouldReturn400Status_InvalidAPIKey()
        {
            /// Arrange
            var mockLogger = Mock.Of<ILogger<WeatherController>>();

            WeatherSettings appSettings = new WeatherSettings()
            {
                BaseAddress = "https://api.openweathermap.org/data/2.5/weather",
                APIKeys = "invalid-api-key"
            };
            var mockIOption = new Mock<IOptions<WeatherSettings>>();
            mockIOption.Setup(ap => ap.Value).Returns(appSettings);

            var weatherController = new WeatherController(mockLogger, mockIOption.Object);

            /// Act
            var result = (UnauthorizedResult)await weatherController.Get("Melbourne", "AU");

            /// Assert
            Assert.Equal(401, result.StatusCode);
        }
    }
}
