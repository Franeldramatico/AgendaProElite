using Plugin.LocalNotification;
using AgendaProElite.Models;
using AgendaProElite.Services;

namespace AgendaProElite.Services;

public class NotificationService
{
    private readonly DatabaseService _databaseService;

    public NotificationService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task ScheduleReminderAsync(Reminder reminder)
    {
        var notification = new NotificationRequest
        {
            NotificationId = reminder.Id,
            Title = reminder.Title,
            Description = reminder.Message,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = reminder.ReminderTime,
                RepeatType = ParseRecurrenceType(reminder.RecurrenceRule)
            }
        };

        if (reminder.UseVibration)
        {
            notification.Android.VibrationPattern = ParseVibrationPattern(reminder.VibrationPattern);
        }

        if (reminder.UseVoice)
        {
            notification.Android.ChannelId = "voice_channel";
        }

        await LocalNotificationCenter.Current.Show(notification);
    }

    public async Task ScheduleEventReminderAsync(Event eventItem, int minutesBefore = 15)
    {
        var reminderTime = eventItem.StartDate.AddMinutes(-minutesBefore);
        if (reminderTime <= DateTime.Now)
            return;

        var notification = new NotificationRequest
        {
            NotificationId = eventItem.Id + 10000, // Offset to avoid conflicts
            Title = $"Recordatorio: {eventItem.Title}",
            Description = $"El evento comienza en {minutesBefore} minutos",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = reminderTime
            }
        };

        await LocalNotificationCenter.Current.Show(notification);
    }

    public async Task ScheduleGeofenceReminderAsync(Reminder reminder)
    {
        // Geofencing requires platform-specific implementation
        // This is a placeholder for the logic
        if (reminder.UseGeolocation && reminder.Latitude.HasValue && reminder.Longitude.HasValue)
        {
            // Platform-specific geofencing would be implemented here
            // For Android: Use LocationManager
            // For iOS: Use CoreLocation
        }
    }

    public async Task CancelReminderAsync(int reminderId)
    {
        await LocalNotificationCenter.Current.Cancel(reminderId);
    }

    public async Task CancelAllRemindersAsync()
    {
        await LocalNotificationCenter.Current.CancelAll();
    }

    private NotificationRepeat ParseRecurrenceType(string recurrenceRule)
    {
        if (string.IsNullOrEmpty(recurrenceRule))
            return NotificationRepeat.No;

        return recurrenceRule.ToLower() switch
        {
            "daily" => NotificationRepeat.Daily,
            "weekly" => NotificationRepeat.Weekly,
            "monthly" => NotificationRepeat.Monthly,
            "yearly" => NotificationRepeat.Yearly,
            _ => NotificationRepeat.No
        };
    }

    private long[] ParseVibrationPattern(string pattern)
    {
        return pattern switch
        {
            "Default" => new long[] { 0, 500 },
            "Double" => new long[] { 0, 200, 100, 200 },
            "Long" => new long[] { 0, 1000 },
            _ => new long[] { 0, 500 }
        };
    }

    public async Task ProcessPendingRemindersAsync()
    {
        var reminders = await _databaseService.GetRemindersAsync(
            DateTime.Now, 
            DateTime.Now.AddDays(7));

        foreach (var reminder in reminders)
        {
            if (reminder.UseGeolocation)
            {
                await ScheduleGeofenceReminderAsync(reminder);
            }
            else
            {
                await ScheduleReminderAsync(reminder);
            }
        }
    }
}

