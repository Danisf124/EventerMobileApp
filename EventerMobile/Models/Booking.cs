using SQLite;
namespace EventerMobile.Models;

[Table("Bookings")]
public class Booking
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public int UserId { get; set; }

    [NotNull]
    public int EventId { get; set; }

    public int TicketCount { get; set; } = 1;

    [Unique]
    public string? QrCode { get; set; }

    // --- Поля НЕ з БД (для зручності у UI) ---
    [Ignore]
    public string EventTitle { get; set; } = string.Empty;

    [Ignore]
    public double TotalPrice { get; set; }
}