using System.Text.Json.Serialization;

namespace WeatherApi.Models;

public sealed class OpenWeatherForecastResponse
{
    [JsonPropertyName("city")]
    public CityDto City { get; set; } = default!;

    [JsonPropertyName("list")]
    public List<ForecastItemDto> List { get; set; } = new();
}

public sealed class CityDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("country")]
    public string Country { get; set; } = "";
}

public sealed class ForecastItemDto
{
    [JsonPropertyName("dt_txt")]
    public string DtTxt { get; set; } = "";

    [JsonPropertyName("main")]
    public MainDto Main { get; set; } = default!;

    [JsonPropertyName("weather")]
    public List<WeatherDto> Weather { get; set; } = new();

    [JsonPropertyName("wind")]
    public WindDto? Wind { get; set; }
}

public sealed class MainDto
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public sealed class WeatherDto
{
    [JsonPropertyName("main")]
    public string Main { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
}

public sealed class WindDto
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}