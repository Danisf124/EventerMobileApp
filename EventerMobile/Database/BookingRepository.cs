using SQLite;
using EventerMobile.Models;

namespace EventerMobile.Database;

public class BookingRepository : IRepository<Booking>
{
    private readonly SQLiteAsyncConnection _db;

    public BookingRepository(DatabaseContext context)
    {
        _db = context.Connection;
    }

    // -----------------------------------------------
    // CRUD
    // -----------------------------------------------

    public Task<List<Booking>> GetAllAsync()
        => _db.Table<Booking>().ToListAsync();

    public Task<Booking?> GetByIdAsync(int id)
        => _db.Table<Booking>().Where(b => b.Id == id).FirstOrDefaultAsync();

    public Task<int> InsertAsync(Booking entity)
        => _db.InsertAsync(entity);

    public Task<int> UpdateAsync(Booking entity)
        => _db.UpdateAsync(entity);

    public Task<int> DeleteAsync(int id)
        => _db.DeleteAsync<Booking>(id);

    // -----------------------------------------------
    // Бізнес-методи
    // -----------------------------------------------

    // Отримати всі бронювання конкретного користувача
    public Task<List<Booking>> GetByUserAsync(int userId)
        => _db.Table<Booking>()
              .Where(b => b.UserId == userId)
              .ToListAsync();

    // Підрахувати загальну кількість куплених квитків на подію
    public async Task<int> GetTotalTicketsByEventAsync(int eventId)
    {
        var bookings = await _db.Table<Booking>()
                                .Where(b => b.EventId == eventId)
                                .ToListAsync();
        return bookings.Sum(b => b.TicketCount);
    }

    // Перевірити чи користувач вже бронював цю подію
    public async Task<bool> HasUserBookedEventAsync(int userId, int eventId)
    {
        var booking = await _db.Table<Booking>()
                               .Where(b => b.UserId == userId && b.EventId == eventId)
                               .FirstOrDefaultAsync();
        return booking is not null;
    }

    // Створити нове бронювання з автоматичним QR-кодом
    public async Task<Booking?> CreateBookingAsync(int userId, int eventId, int ticketCount)
    {
        // Перевіряємо чи подія існує
        var ev = await _db.Table<Event>().Where(e => e.Id == eventId).FirstOrDefaultAsync();
        if (ev is null) return null;

        // Перевіряємо чи користувач вже бронював
        if (await HasUserBookedEventAsync(userId, eventId)) return null;

        var booking = new Booking
        {
            UserId      = userId,
            EventId     = eventId,
            TicketCount = ticketCount,
            QrCode      = GenerateQrCode(userId, eventId)
        };

        await _db.InsertAsync(booking);
        return booking;
    }

    // Отримати бронювання користувача з назвою події та загальною ціною
    public async Task<List<Booking>> GetUserBookingsWithDetailsAsync(int userId)
    {
        var bookings = await _db.Table<Booking>()
                                .Where(b => b.UserId == userId)
                                .ToListAsync();

        var events = await _db.Table<Event>().ToListAsync();
        var eventMap = events.ToDictionary(e => e.Id);

        foreach (var booking in bookings)
        {
            if (eventMap.TryGetValue(booking.EventId, out var ev))
            {
                booking.EventTitle  = ev.Title;
                booking.TotalPrice  = ev.Price * booking.TicketCount;
            }
        }

        return bookings;
    }

    // Скасувати бронювання (видалення)
    public async Task<bool> CancelBookingAsync(int bookingId, int userId)
    {
        var booking = await _db.Table<Booking>()
                               .Where(b => b.Id == bookingId && b.UserId == userId)
                               .FirstOrDefaultAsync();

        if (booking is null) return false;

        await _db.DeleteAsync(booking);
        return true;
    }

    // Генерація унікального QR-коду
    private static string GenerateQrCode(int userId, int eventId)
        => $"QR-U{userId}-E{eventId}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}
