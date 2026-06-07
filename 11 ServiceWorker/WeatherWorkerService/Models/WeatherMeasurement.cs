using System.ComponentModel.DataAnnotations;

namespace WeatherWorkerService.Models;

public class WeatherMeasurement
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public DateTime RetrievedAtUtc { get; set; }
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public double TemperatureMin { get; set; }
    public double TemperatureMax { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }

    [MaxLength(100)]
    public string? Description { get; set; }

    public static WeatherMeasurement FromWeatherData(WeatherData weatherData, string requestedCity)
    {
        var description = weatherData.Weather.FirstOrDefault()?.Description;

        return new WeatherMeasurement
        {
            City = string.IsNullOrWhiteSpace(weatherData.Name) ? requestedCity : weatherData.Name,
            RetrievedAtUtc = DateTime.UtcNow,
            Temperature = weatherData.Main.Temp,
            FeelsLike = weatherData.Main.FeelsLike,
            TemperatureMin = weatherData.Main.TempMin,
            TemperatureMax = weatherData.Main.TempMax,
            Pressure = weatherData.Main.Pressure,
            Humidity = weatherData.Main.Humidity,
            WindSpeed = weatherData.Wind?.Speed ?? 0,
            Description = description
        };
    }
}
