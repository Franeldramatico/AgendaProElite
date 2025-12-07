using SQLite;

namespace AgendaProElite.Models;

[Table("Reminders")]
public class Reminder
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    [Indexed]
    public DateTime ReminderTime { get; set; }

    public int EventId { get; set; } // Optional link to event

    public int TaskId { get; set; } // Optional link to task

    public bool IsCompleted { get; set; }

    public bool UseVoice { get; set; }

    public bool UseVibration { get; set; }

    public string VibrationPattern { get; set; } = "Default";

    public bool UseGeolocation { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public double? GeofenceRadius { get; set; } // in meters

    public string RecurrenceRule { get; set; } = string.Empty;

    public DateTime? RecurrenceEndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool IsSynced { get; set; }

    public bool IsDeleted { get; set; }
}

