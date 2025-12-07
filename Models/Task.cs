using SQLite;

namespace AgendaProElite.Models;

[Table("Tasks")]
public class Task
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Indexed]
    public DateTime? DueDate { get; set; }

    public int Priority { get; set; } // 0=Low, 1=Medium, 2=High, 3=Urgent (AI calculated)

    public int AIPriority { get; set; } // AI calculated priority based on patterns

    public bool IsCompleted { get; set; }

    public int Points { get; set; } // Gamification points

    public int ParentTaskId { get; set; } // For subtasks

    public int CategoryId { get; set; }

    public string Color { get; set; } = "#FF6B6B";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public DateTime? CompletedAt { get; set; }

    public int EstimatedMinutes { get; set; }

    public int ActualMinutes { get; set; }

    public string? HabitId { get; set; } // Link to habit tracking

    public bool IsSynced { get; set; }

    public bool IsDeleted { get; set; }
}

