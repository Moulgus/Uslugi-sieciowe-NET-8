using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherApi.Models;

namespace WeatherApi.Services;

public sealed class OpenWeatherClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public OpenWeatherClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<OpenWeatherForecastResponse> GetForecastAsync(string city, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));

        var apiKey = _config["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Missing OpenWeather:ApiKey in configuration.");

        var url =
            $"/data/2.5/forecast?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric&lang=pl";

        using var resp = await _http.GetAsync(url, ct);

        if (resp.StatusCode == HttpStatusCode.NotFound)
            throw new KeyNotFoundException($"City '{city}' not found in OpenWeather.");

        resp.EnsureSuccessStatusCode();

        var data = await resp.Content.ReadFromJsonAsync<OpenWeatherForecastResponse>(JsonOpts, ct);
        return data ?? throw new InvalidOperationException("OpenWeather returned empty response.");
    }
}