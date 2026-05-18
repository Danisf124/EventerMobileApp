using SQLite;
using EventerMobile.Models;

namespace EventerMobile.Database;

public class DatabaseContext
{
    private readonly SQLiteAsyncConnection _db;

    // Назва файлу бази даних
    private const string DatabaseFilename = "events_app.db3";

    // Шлях до файлу (зберігається у локальній папці додатку)
    private static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    public DatabaseContext()
    {
        _db = new SQLiteAsyncConnection(DatabasePath, 
            SQLiteOpenFlags.ReadWrite | 
            SQLiteOpenFlags.Create | 
            SQLiteOpenFlags.SharedCache);
    }

    // -----------------------------------------------
    // Ініціалізація — створення таблиць при першому запуску
    // -----------------------------------------------
    public async Task InitAsync()
    {
        // Створює таблиці якщо їх немає (не видаляє існуючі дані)
        await _db.CreateTableAsync<Locations>();
        await _db.CreateTableAsync<Organizer>();
        await _db.CreateTableAsync<Event>();
        await _db.CreateTableAsync<Booking>();

        // Заповнюємо тестовими даними якщо БД порожня
        await SeedDataAsync();
    }

    // -----------------------------------------------
    // Тестові дані (seed) — запускається один раз
    // -----------------------------------------------
    private async Task SeedDataAsync()
    {
        var locationsCount = await _db.Table<Location>().CountAsync();
        if (locationsCount > 0) return; // дані вже є — нічого не робимо

        // Локації
        var locations = new List<Locations>
        {
            new() { City = "Київ",  Address = "вул. Хрещатик, 1",    Latitude = 50.4501, Longitude = 30.5234 },
            new() { City = "Київ",  Address = "просп. Перемоги, 50", Latitude = 50.4589, Longitude = 30.4945 },
            new() { City = "Львів", Address = "пл. Ринок, 1",        Latitude = 49.8419, Longitude = 24.0315 },
        };
        await _db.InsertAllAsync(locations);

        // Організатори
        var organizers = new List<Organizer>
        {
            new() { Name = "EventPro UA", ContactInfo = "eventpro@gmail.com" },
            new() { Name = "ConcertLife", ContactInfo = "+380671234567" },
            new() { Name = "KyivCinema",  ContactInfo = "info@kyivcinema.ua" },
        };
        await _db.InsertAllAsync(organizers);

        // Події
        var events = new List<Event>
        {
            new() { LocationId = 1, OrganizerId = 1, Title = "Джазовий вечір",  Category = "концерт", StartTime = "2025-06-15 19:00", Price = 350, Description = "Живий джаз у центрі міста" },
            new() { LocationId = 2, OrganizerId = 2, Title = "Рок-фестиваль",   Category = "концерт", StartTime = "2025-06-20 17:00", Price = 500, Description = "Найкращі рок-гурти України" },
            new() { LocationId = 3, OrganizerId = 3, Title = "Прем'єра фільму", Category = "кіно",    StartTime = "2025-06-10 20:00", Price = 150, Description = "Новинка українського кіно" },
            new() { LocationId = 1, OrganizerId = 1, Title = "Арт-виставка",    Category = "виставка", StartTime = "2025-07-01 10:00", Price = 80,  Description = "Сучасне мистецтво" },
        };
        await _db.InsertAllAsync(events);

        // Бронювання
        var bookings = new List<Booking>
        {
            new() { UserId = 1, EventId = 1, TicketCount = 2, QrCode = "QR-001-ABC" },
            new() { UserId = 1, EventId = 3, TicketCount = 1, QrCode = "QR-002-DEF" },
            new() { UserId = 2, EventId = 2, TicketCount = 4, QrCode = "QR-003-GHI" },
        };
        await _db.InsertAllAsync(bookings);
    }

    // -----------------------------------------------
    // Доступ до з'єднання для репозиторіїв
    // -----------------------------------------------
    public SQLiteAsyncConnection Connection => _db;
}