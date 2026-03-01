var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

// Random Temperature Generator

app.MapGet("/Temperature", () =>
{
    return Random.Shared.Next(-40, 40);
});

// Random Wind Direction Generator

var WindDirections = new[]
{
    "N", "NE", "E", "SE", "S", "SW", "W", "NW"
};

app.MapGet("/WindDirection", () =>
{
    var Id = Random.Shared.Next(0, WindDirections.Length);
    return WindDirections[Id];
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}