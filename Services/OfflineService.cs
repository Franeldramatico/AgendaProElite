using AgendaProElite.Models;
using AgendaProElite.Services;
using Microsoft.Maui.Networking;

namespace AgendaProElite.Services;

public class OfflineService
{
    private readonly DatabaseService _databaseService;
    private readonly Queue<SyncAction> _syncQueue = new();

    public bool IsOnline { get; private set; }

    public OfflineService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        CheckConnectivity();
        Connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        CheckConnectivity();
        if (IsOnline)
        {
            ProcessSyncQueueAsync();
        }
    }

    private void CheckConnectivity()
    {
        var current = Connectivity.NetworkAccess;
        IsOnline = current == NetworkAccess.Internet;
    }

    public async Task<bool> IsOnlineAsync()
    {
        CheckConnectivity();
        return IsOnline;
    }

    public void QueueSyncAction(SyncAction action)
    {
        _syncQueue.Enqueue(action);
        if (IsOnline)
        {
            ProcessSyncQueueAsync();
        }
    }

    private async Task ProcessSyncQueueAsync()
    {
        while (_syncQueue.Count > 0 && IsOnline)
        {
            var action = _syncQueue.Dequeue();
            try
            {
                await ExecuteSyncActionAsync(action);
            }
            catch
            {
                // Re-queue on failure
                _syncQueue.Enqueue(action);
                break;
            }
        }
    }

    private async Task ExecuteSyncActionAsync(SyncAction action)
    {
        // Sync action will be handled by SyncService when connectivity is restored
        // This service just queues actions
        await Task.CompletedTask;
    }

    public async Task<List<Event>> GetCachedEventsAsync(DateTime startDate, DateTime endDate)
    {
        // Always return from local database (offline-first)
        return await _databaseService.GetEventsAsync(startDate, endDate);
    }
}

public enum SyncActionType
{
    CreateEvent,
    UpdateEvent,
    DeleteEvent,
    SyncAll
}

public class SyncAction
{
    public SyncActionType Type { get; set; }
    public Event? Event { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

