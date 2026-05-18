using SQLite;
using EventerMobile.Models;

namespace EventerMobile.Database;

public class EventRepository : IRepository<Event>
{
    private readonly SQLiteAsyncConnection _db;

    public EventRepository(DatabaseContext context)
    {
        _db = context.Connection;
    }

    // -----------------------------------------------
    // CRUD
    // -----------------------------------------------

    public Task<List<Event>> GetAllAsync()
        => _db.Table<Event>().ToListAsync();

    public Task<Event?> GetByIdAsync(int id)
        => _db.Table<Event>().Where(e => e.Id == id).FirstOrDefaultAsync();

    public Task<int> InsertAsync(Event entity)
        => _db.InsertAsync(entity);

    public Task<int> UpdateAsync(Event entity)
        => _db.UpdateAsync(entity);

    public Task<int> DeleteAsync(int id)
        => _db.DeleteAsync<Event>(id);

    // -----------------------------------------------
    // Бізнес-методи
    // -----------------------------------------------

    // Пошук подій за категорією
    public Task<List<Event>> GetByCategoryAsync(string category)
        => _db.Table<Event>()
              .Where(e => e.Category == category)
              .ToListAsync();

    // Пошук подій за містом через location_id
    public async Task<List<Event>> GetByCityAsync(string city)
    {
        // Знаходимо всі локації у місті
        var locationIds = (await _db.Table<Locations>()
                                    .Where(l => l.City == city)
                                    .ToListAsync())
                                    .Select(l => l.Id)
                                    .ToHashSet();

        return await _db.Table<Event>()
                        .Where(e => locationIds.Contains(e.LocationId))
                        .ToListAsync();
    }

    // Отримати майбутні події (дата більша за сьогодні)
    public async Task<List<Event>> GetUpcomingAsync()
    {
        var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        return await _db.Table<Event>()
                        .Where(e => string.Compare(e.StartTime, now) > 0)
                        .OrderBy(e => e.StartTime)
                        .ToListAsync();
    }

    // Пошук за ціновим діапазоном
    public Task<List<Event>> GetByPriceRangeAsync(double minPrice, double maxPrice)
        => _db.Table<Event>()
              .Where(e => e.Price >= minPrice && e.Price <= maxPrice)
              .ToListAsync();

    // Пошук за назвою (часткове співпадіння)
    public async Task<List<Event>> SearchByTitleAsync(string query)
    {
        var all = await _db.Table<Event>().ToListAsync();
        return all.Where(e => e.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                  .ToList();
    }

    // Отримати подію з назвою локації та організатора (для UI)
    public async Task<Event?> GetWithDetailsAsync(int eventId)
    {
        var ev = await GetByIdAsync(eventId);
        if (ev is null) return null;

        var location  = await _db.Table<Locations>().Where(l => l.Id == ev.LocationId).FirstOrDefaultAsync();
        var organizer = await _db.Table<Organizer>().Where(o => o.Id == ev.OrganizerId).FirstOrDefaultAsync();

        ev.LocationName  = location?.Address  ?? string.Empty;
        ev.OrganizerName = organizer?.Name ?? string.Empty;

        return ev;
    }

    // Отримати всі події з деталями (для списку у UI)
    public async Task<List<Event>> GetAllWithDetailsAsync()
    {
        var events    = await _db.Table<Event>().ToListAsync();
        var locations = await _db.Table<Locations>().ToListAsync();
        var organizers = await _db.Table<Organizer>().ToListAsync();

        // Словники для швидкого пошуку
        var locationMap  = locations.ToDictionary(l => l.Id);
        var organizerMap = organizers.ToDictionary(o => o.Id);

        foreach (var ev in events)
        {
            ev.LocationName  = locationMap.TryGetValue(ev.LocationId, out var loc) ? loc.Address : string.Empty;
            ev.OrganizerName = organizerMap.TryGetValue(ev.OrganizerId, out var org) ? org.Name : string.Empty;
        }

        return events;
    }
}
