# Weather App #

A simple weather app that can be used to view current weather for any location based on the OpenWeather exposed APIs

This app has two components included which are described as below:
1. __Weather API (Back-end)__
    - A GET API exposed at endpoint `api/weather` that accepts city and country as input and calls the OpenWeather API to retrieve the weather response object.  The API returns only the description field from the response object.
    - This service uses custom middleware to limit the API requests within a specified time window. This limit is defined at the action method via an attribute as shown below
    ```C#
        [HttpGet]
        [RequestLimit(MaxRequests = 5, TimeLimit = 60)]
        public async Task<ActionResult> Get(string city, string country) {...}
    ```
    - The API also requires an API key to be provided for authentication purposes. The custom request header accepted is `x-api-key` which should hold the value of the API key matching with app setting.
2. __Web App (Front-end)__
    - An ASP.NET Core web app that fronts the weather API
    - User case use the page to enter city and country and retrieve the weather result
    - User is limited to 5 weather reports in an hour due to rate limiting built into the backend service
    - An API key is passed from the front-end to authenticate the request

## Run Locally 

Using a command line, clone the project
```bash
  git clone https://github.com/dkapadia6/WeatherApp.git
```

### Build and Run API Service
Navigate to the API project directory

```bash
  cd SimpleWeatherApp.Api
```

Build the API project

```bash
  dotnet build
```

Run the API service

```bash
  dotnet run
```

The API service should now be running at below URL

```bash
  https://localhost:5001
```

### Build and Run Web App (Front-end)
Navigate to the Web project directory

```bash
  cd SimpleWeatherApp.Web
```

Build the Web project

```bash
  dotnet build
```

Run the web app

```bash
  dotnet run
```

The Web app should now be running at below URL

```bash
  https://localhost:5003
```

## Test Locally
To test the solution, simply navigate to the web app URL where you will be presented a form to input your data.
- Enter the `city` and select the `country`, click on the button to return weather report
- To test rate limiting, click on the button more than 5 times within 60 minutes and API should return a `429` response indicating rate limit has exceeded

## Author
- [@dkapadia6](https://www.github.com/dkapadia6)