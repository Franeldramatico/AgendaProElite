using SQLite;

namespace AgendaProElite.Models;

[Table("Collaborations")]
public class Collaboration
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int EventId { get; set; }

    [Indexed]
    public int UserId { get; set; }

    public string Permission { get; set; } = "View"; // View, Edit, Owner

    public string ShareCode { get; set; } = string.Empty; // QR code identifier

    public string? QRCodeImage { get; set; } // Base64 encoded QR image

    public DateTime? ExpiresAt { get; set; }

    public bool IsAccepted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool IsDeleted { get; set; }
}

