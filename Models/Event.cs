using SQLite;

namespace AgendaProElite.Models;

[Table("Events")]
public class Event
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Indexed]
    public DateTime StartDate { get; set; }

    [Indexed]
    public DateTime EndDate { get; set; }

    public bool IsAllDay { get; set; }

    public int CategoryId { get; set; }

    public string Location { get; set; } = string.Empty;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string RecurrenceRule { get; set; } = string.Empty;

    public DateTime? RecurrenceEndDate { get; set; }

    public string Color { get; set; } = "#512BD4";

    public bool IsCompleted { get; set; }

    public int Priority { get; set; } // 0=Low, 1=Medium, 2=High, 3=Urgent

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public string? GoogleEventId { get; set; }

    public string? OutlookEventId { get; set; }

    public bool IsSynced { get; set; }

    public bool IsDeleted { get; set; }
}

