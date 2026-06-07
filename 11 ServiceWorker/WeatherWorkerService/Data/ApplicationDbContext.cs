using Microsoft.EntityFrameworkCore;
using WeatherWorkerService.Models;

namespace WeatherWorkerService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<WeatherMeasurement> WeatherMeasurements => Set<WeatherMeasurement>();
}
