using AgendaProElite.Services;
using AgendaProElite.ViewModels;
using AgendaProElite.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace AgendaProElite;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseLocalNotification()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register Services
		builder.Services.AddSingleton<DatabaseService>();
		builder.Services.AddSingleton<NotificationService>();
		builder.Services.AddSingleton<SyncService>();
		builder.Services.AddSingleton<AIService>();
		builder.Services.AddSingleton<ThemeService>();
		builder.Services.AddSingleton<ExportService>();
		builder.Services.AddSingleton<OfflineService>();
		builder.Services.AddSingleton<CollaborationService>();
		builder.Services.AddSingleton<GoogleCalendarService>();
		builder.Services.AddSingleton<OutlookCalendarService>();

		// Register ViewModels
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<CalendarViewModel>();
		builder.Services.AddTransient<EventViewModel>();
		builder.Services.AddTransient<TaskViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

		// Register Views
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<CalendarPage>();
		builder.Services.AddTransient<EventDetailPage>();
		builder.Services.AddTransient<TaskPage>();
		builder.Services.AddTransient<SettingsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

