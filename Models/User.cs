using SQLite;

namespace AgendaProElite.Models;

[Table("Users")]
public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Name { get; set; } = string.Empty;

    [Indexed]
    public string Email { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Role { get; set; } = "Member"; // Owner, Admin, Member, Guest

    public bool IsActive { get; set; } = true;

    public int FamilyId { get; set; } // For family/multi-user groups

    public string? GoogleAccountId { get; set; }

    public string? OutlookAccountId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool IsDeleted { get; set; }
}

