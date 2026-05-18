using SQLite;

namespace EventerMobile.Models;

[Table("Locations")]
public class Locations
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string City { get; set; } = string.Empty;

    [NotNull]
    public string Address { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}