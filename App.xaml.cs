using AgendaProElite.Services;

namespace AgendaProElite;

public partial class App : Application
{
	private readonly ThemeService _themeService;

	public App(ThemeService themeService)
	{
		InitializeComponent();
		_themeService = themeService;
		
		// Apply initial theme
		_themeService.ApplyTheme(_themeService.CurrentTheme);
		
		MainPage = new AppShell();
	}

	protected override async void OnStart()
	{
		// Initialize services on app start
		var databaseService = Handler?.MauiContext?.Services.GetService<DatabaseService>();
		if (databaseService != null)
		{
			await databaseService.InitializeAsync();
		}
	}
}

