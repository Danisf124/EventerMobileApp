using SQLite;
using EventerMobile.Models;

namespace EventerMobile.Database;

public class LocationRepository : IRepository<Locations>
{
    private readonly SQLiteAsyncConnection _db;

    public LocationRepository(DatabaseContext context)
    {
        _db = context.Connection;
    }

    // -----------------------------------------------
    // CRUD
    // -----------------------------------------------

    public Task<List<Locations>> GetAllAsync()
        => _db.Table<Locations>().ToListAsync();

    public Task<Locations?> GetByIdAsync(int id)
        => _db.Table<Locations>().Where(l => l.Id == id).FirstOrDefaultAsync();

    public Task<int> InsertAsync(Locations entity)
        => _db.InsertAsync(entity);

    public Task<int> UpdateAsync(Locations entity)
        => _db.UpdateAsync(entity);

    public Task<int> DeleteAsync(int id)
        => _db.DeleteAsync<Locations>(id);

    // -----------------------------------------------
    // Бізнес-методи
    // -----------------------------------------------

    // Отримати всі локації у конкретному місті
    public Task<List<Locations>> GetByCityAsync(string city)
        => _db.Table<Locations>()
              .Where(l => l.City == city)
              .ToListAsync();

    // Отримати список унікальних міст (для фільтрів у UI)
    public async Task<List<string>> GetAllCitiesAsync()
    {
        var locations = await _db.Table<Locations>().ToListAsync();
        return locations.Select(l => l.City).Distinct().OrderBy(c => c).ToList();
    }

    // Знайти найближчі локації за координатами (проста формула відстані)
    public async Task<List<Locations>> GetNearbyAsync(double lat, double lng, double radiusKm = 10)
    {
        var all = await _db.Table<Locations>().ToListAsync();
        return all.Where(l => GetDistanceKm(lat, lng, l.Latitude, l.Longitude) <= radiusKm)
                  .OrderBy(l => GetDistanceKm(lat, lng, l.Latitude, l.Longitude))
                  .ToList();
    }

    // Відстань між двома точками (формула Гаверсина)
    private static double GetDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
