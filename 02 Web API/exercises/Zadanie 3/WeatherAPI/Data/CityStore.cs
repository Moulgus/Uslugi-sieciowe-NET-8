using WeatherApi.Models;

namespace WeatherApi.Data;

public static class CityStore
{
    private static readonly List<City> _cities = new()
    {
        new City { ID = 1, Name = "Warsaw" },
        new City { ID = 2, Name = "Krakow" }
    };

    private static int _nextID = 3;

    public static IReadOnlyList<City> GetAll() => _cities;

    public static City? GetByID(int id) => _cities.FirstOrDefault(c => c.ID == id);

    public static City Add(string name)
    {
        var city = new City { ID = _nextID++, Name = name };
        _cities.Add(city);
        return city;
    }

    public static bool Delete(int id)
    {
        var city = GetByID(id);
        if (city is null) return false;
        _cities.Remove(city);
        return true;
    }

    public static City? Update(int id, string name)
    {
        var city = GetByID(id);
        if (city is null) return null;
        city.Name = name;
        return city;
    }
}