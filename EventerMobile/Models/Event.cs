using SQLite;

namespace EventerMobile.Models;

[Table("Events")]
public class Event
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public int LocationId { get; set; }

    [NotNull]
    public int OrganizerId { get; set; }

    [NotNull]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    // кіно / концерт / виставка / спорт / інше
    [NotNull]
    public string Category { get; set; } = string.Empty;

    // Зберігаємо як рядок 'YYYY-MM-DD HH:MM' — SQLite не має типу DateTime
    [NotNull]
    public string StartTime { get; set; } = string.Empty;

    public double Price { get; set; } = 0;

    // --- Поля НЕ з БД (для зручності у UI) ---
    [Ignore]
    public string LocationName { get; set; } = string.Empty;

    [Ignore]
    public string OrganizerName { get; set; } = string.Empty;
}