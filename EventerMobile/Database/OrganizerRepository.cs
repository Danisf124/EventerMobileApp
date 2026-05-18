using SQLite;
using EventerMobile.Models;

namespace EventerMobile.Database;

public class OrganizerRepository : IRepository<Organizer>
{
    private readonly SQLiteAsyncConnection _db;

    public OrganizerRepository(DatabaseContext context)
    {
        _db = context.Connection;
    }

    // -----------------------------------------------
    // CRUD
    // -----------------------------------------------

    public Task<List<Organizer>> GetAllAsync()
        => _db.Table<Organizer>().ToListAsync();

    public Task<Organizer?> GetByIdAsync(int id)
        => _db.Table<Organizer>().Where(o => o.Id == id).FirstOrDefaultAsync();

    public Task<int> InsertAsync(Organizer entity)
        => _db.InsertAsync(entity);

    public Task<int> UpdateAsync(Organizer entity)
        => _db.UpdateAsync(entity);

    public Task<int> DeleteAsync(int id)
        => _db.DeleteAsync<Organizer>(id);

    // -----------------------------------------------
    // Бізнес-методи
    // -----------------------------------------------

    // Пошук організатора за іменем (часткове співпадіння)
    public async Task<List<Organizer>> SearchByNameAsync(string query)
    {
        var all = await _db.Table<Organizer>().ToListAsync();
        return all.Where(o => o.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                  .ToList();
    }

    // Скільки подій має організатор
    public async Task<int> GetEventsCountAsync(int organizerId)
    {
        return await _db.Table<Event>()
                        .Where(e => e.OrganizerId == organizerId)
                        .CountAsync();
    }

    // Отримати всі події конкретного організатора
    public async Task<List<Event>> GetEventsByOrganizerAsync(int organizerId)
    {
        return await _db.Table<Event>()
                        .Where(e => e.OrganizerId == organizerId)
                        .ToListAsync();
    }
}
