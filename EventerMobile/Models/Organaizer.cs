using SQLite;

namespace EventerMobile.Models;

[Table("Organizers")]
public class Organizer
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Name { get; set; } = string.Empty;

    [NotNull]
    public string ContactInfo { get; set; } = string.Empty;
}