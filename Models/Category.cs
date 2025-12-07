using SQLite;

namespace AgendaProElite.Models;

[Table("Categories")]
public class Category
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = "#512BD4";

    public string Icon { get; set; } = "calendar";

    public string Type { get; set; } = "Event"; // Event, Task, or Both

    public bool IsDefault { get; set; }

    public int UserId { get; set; } // For multi-user support

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool IsDeleted { get; set; }
}

