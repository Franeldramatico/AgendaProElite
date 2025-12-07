using AgendaProElite.Models;
using AgendaProElite.Services;
using ZXing;
using ZXing.Common;

namespace AgendaProElite.Services;

public class CollaborationService
{
    private readonly DatabaseService _databaseService;

    public CollaborationService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<string> GenerateShareCodeAsync(int eventId, int userId, string permission = "View", DateTime? expiresAt = null)
    {
        var shareCode = Guid.NewGuid().ToString("N")[..16].ToUpper();
        
        var collaboration = new Collaboration
        {
            EventId = eventId,
            UserId = userId,
            Permission = permission,
            ShareCode = shareCode,
            ExpiresAt = expiresAt
        };

        await _databaseService.SaveCollaborationAsync(collaboration);
        return shareCode;
    }

    public async Task<string> GenerateQRCodeAsync(int eventId, int userId, string permission = "View")
    {
        var shareCode = await GenerateShareCodeAsync(eventId, userId, permission);
        var qrData = $"agendapro://share/{shareCode}";

        // Generate QR code image
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Height = 300,
                Width = 300
            }
        };

        var bitmap = writer.Write(qrData);
        
        // Convert bitmap to base64 (simplified - would need proper image conversion)
        var qrImageBase64 = ""; // Placeholder
        
        var collaboration = (await _databaseService.GetCollaborationsByEventAsync(eventId))
            .FirstOrDefault(c => c.ShareCode == shareCode);
        
        if (collaboration != null)
        {
            collaboration.QRCodeImage = qrImageBase64;
            await _databaseService.SaveCollaborationAsync(collaboration);
        }

        return qrImageBase64;
    }

    public async Task<bool> AcceptCollaborationAsync(string shareCode, int userId)
    {
        var collaborations = await _databaseService.GetCollaborationsByEventAsync(0);
        var collaboration = collaborations.FirstOrDefault(c => c.ShareCode == shareCode);

        if (collaboration != null && 
            (!collaboration.ExpiresAt.HasValue || collaboration.ExpiresAt.Value > DateTime.Now))
        {
            collaboration.UserId = userId;
            collaboration.IsAccepted = true;
            await _databaseService.SaveCollaborationAsync(collaboration);
            return true;
        }

        return false;
    }

    public async Task<List<Collaboration>> GetUserCollaborationsAsync(int userId)
    {
        // This would require a new method in DatabaseService
        // For now, return empty list
        return new List<Collaboration>();
    }
}

