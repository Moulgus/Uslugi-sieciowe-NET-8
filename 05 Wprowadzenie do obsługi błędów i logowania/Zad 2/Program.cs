using Serilog;
using WeatherApi.Data;
using WeatherApi.Models;
using WeatherApi.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddHttpClient<OpenWeatherClient>(client =>
{
    var baseUrl = builder.Configuration["OpenWeather:BaseUrl"] ?? "https://api.openweathermap.org";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/api/error");
}

app.UseSwagger();
app.UseSwaggerUI();

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

// GET /cities
app.MapGet("/cities", () => Results.Ok(CityStore.GetAll()))
   .WithName("GetCities");

// GET /cities/{id}
app.MapGet("/cities/{id:int}", (int id) =>
{
    var city = CityStore.GetByID(id);
    return city is null ? Results.NotFound() : Results.Ok(city);
})
.WithName("GetCityByID");

// POST /cities
app.MapPost("/cities", (CityCreateDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name))
        return Results.BadRequest(new { error = "Name is required." });

    var created = CityStore.Add(dto.Name.Trim());
    return Results.Created($"/cities/{created.ID}", created);
})
.WithName("CreateCity");

// PUT /cities/{id}
app.MapPut("/cities/{id:int}", (int id, CityUpdateDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name))
        return Results.BadRequest(new { error = "Name is required." });

    var updated = CityStore.Update(id, dto.Name.Trim());
    return updated is null ? Results.NotFound() : Results.Ok(updated);
})
.WithName("UpdateCity");

// DELETE /cities/{id}
app.MapDelete("/cities/{id:int}", (int id) =>
{
    var ok = CityStore.Delete(id);
    return ok ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteCity");

app.MapControllers();

app.Run();