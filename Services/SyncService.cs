using AgendaProElite.Models;
using AgendaProElite.Services;
using Microsoft.Maui.Networking;

namespace AgendaProElite.Services;

public class SyncService
{
    private readonly DatabaseService _databaseService;
    private readonly GoogleCalendarService _googleService;
    private readonly OutlookCalendarService _outlookService;

    public bool IsGoogleSynced { get; private set; }
    public bool IsOutlookSynced { get; private set; }

    public SyncService(
        DatabaseService databaseService,
        GoogleCalendarService googleService,
        OutlookCalendarService outlookService)
    {
        _databaseService = databaseService;
        _googleService = googleService;
        _outlookService = outlookService;
    }

    public async Task SyncGoogleCalendarAsync()
    {
        try
        {
            IsGoogleSynced = await _googleService.AuthenticateAsync();
            if (IsGoogleSynced)
            {
                await _googleService.SyncEventsAsync();
            }
        }
        catch (Exception ex)
        {
            // Handle error
            IsGoogleSynced = false;
        }
    }

    public async Task SyncOutlookCalendarAsync()
    {
        try
        {
            IsOutlookSynced = await _outlookService.AuthenticateAsync();
            if (IsOutlookSynced)
            {
                await _outlookService.SyncEventsAsync();
            }
        }
        catch (Exception ex)
        {
            // Handle error
            IsOutlookSynced = false;
        }
    }

    public async Task SyncAllAsync()
    {
        var current = Connectivity.NetworkAccess;
        if (current == NetworkAccess.Internet)
        {
            await SyncGoogleCalendarAsync();
            await SyncOutlookCalendarAsync();
        }
    }

    public async Task ResolveConflictsAsync(List<Event> localEvents, List<Event> remoteEvents)
    {
        // Conflict resolution logic
        foreach (var localEvent in localEvents)
        {
            var remoteEvent = remoteEvents.FirstOrDefault(e => 
                e.GoogleEventId == localEvent.GoogleEventId || 
                e.OutlookEventId == localEvent.OutlookEventId);

            if (remoteEvent != null)
            {
                // Use most recent update
                if (remoteEvent.UpdatedAt > localEvent.UpdatedAt)
                {
                    await _databaseService.SaveEventAsync(remoteEvent);
                }
                else
                {
                    // Push local changes to remote
                    if (!string.IsNullOrEmpty(localEvent.GoogleEventId))
                    {
                        await _googleService.UpdateEventAsync(localEvent);
                    }
                    if (!string.IsNullOrEmpty(localEvent.OutlookEventId))
                    {
                        await _outlookService.UpdateEventAsync(localEvent);
                    }
                }
            }
        }
    }
}

