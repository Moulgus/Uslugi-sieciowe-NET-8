using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<OpenWeatherClient>(client =>
{
    var baseUrl = builder.Configuration["OpenWeather:BaseUrl"] ?? "https://api.openweathermap.org";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

app.MapGet("/weather", async (string city, OpenWeatherClient ow, CancellationToken ct) =>
{
    try
    {
        var forecast = await ow.GetForecastAsync(city, ct);
        return Results.Ok(forecast);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem($"HTTP error while calling OpenWeather: {ex.Message}");
    }
});

app.Run();