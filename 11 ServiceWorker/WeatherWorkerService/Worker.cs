using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WeatherWorkerService.Data;
using WeatherWorkerService.Models;

namespace WeatherWorkerService;

public class Worker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cities = _configuration.GetSection("OpenWeather:Cities").Get<string[]>()
            ?? ["Warszawa", "Chełm", "Lublin"];
        var intervalSeconds = _configuration.GetValue("OpenWeather:IntervalSeconds", 30);

        while (!stoppingToken.IsCancellationRequested)
        {
            await FetchAndSaveWeatherAsync(cities, stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }

    private async Task FetchAndSaveWeatherAsync(IEnumerable<string> cities, CancellationToken stoppingToken)
    {
        var apiKey = _configuration["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("OpenWeather API key is missing. Set OpenWeather:ApiKey in appsettings.json or user secrets.");
            return;
        }

        foreach (var city in cities)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";

            try
            {
                var response = await _httpClient.GetAsync(url, stoppingToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(stoppingToken);
                var weatherData = JsonConvert.DeserializeObject<WeatherData>(content);
                if (weatherData?.Main == null)
                {
                    _logger.LogWarning("Weather response for {City} did not contain expected data.", city);
                    continue;
                }

                var weather = WeatherMeasurement.FromWeatherData(weatherData, city);

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.WeatherMeasurements.Add(weather);
                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation(
                    "Saved weather for {City}: {Temperature} C, humidity {Humidity}%.",
                    weather.City,
                    weather.Temperature,
                    weather.Humidity);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error while fetching weather for {City}.", city);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error while saving weather for {City}.", city);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error while parsing weather response for {City}.", city);
            }
        }
    }
}
