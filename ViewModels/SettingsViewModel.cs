using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgendaProElite.Services;
using System.Collections.ObjectModel;

namespace AgendaProElite.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ThemeService _themeService;
    private readonly SyncService _syncService;

    [ObservableProperty]
    private string currentTheme;

    [ObservableProperty]
    private bool isDarkMode;

    [ObservableProperty]
    private bool autoDarkMode;

    [ObservableProperty]
    private bool isGoogleSynced;

    [ObservableProperty]
    private bool isOutlookSynced;

    [ObservableProperty]
    private ObservableCollection<string> themes = new();

    public SettingsViewModel(ThemeService themeService, SyncService syncService)
    {
        _themeService = themeService;
        _syncService = syncService;
        CurrentTheme = _themeService.CurrentTheme;
        IsDarkMode = _themeService.IsDarkMode;
        AutoDarkMode = _themeService.AutoDarkMode;
        
        var availableThemes = _themeService.GetAvailableThemes();
        foreach (var theme in availableThemes)
        {
            Themes.Add(theme);
        }
    }

    [RelayCommand]
    private async Task ChangeTheme(string themeName)
    {
        _themeService.ApplyTheme(themeName);
        CurrentTheme = themeName;
    }

    [RelayCommand]
    private void ToggleDarkMode()
    {
        _themeService.ToggleDarkMode();
        IsDarkMode = _themeService.IsDarkMode;
    }

    [RelayCommand]
    private void ToggleAutoDarkMode()
    {
        _themeService.SetAutoDarkMode(AutoDarkMode);
    }

    [RelayCommand]
    private async Task SyncGoogle()
    {
        await _syncService.SyncGoogleCalendarAsync();
        IsGoogleSynced = _syncService.IsGoogleSynced;
    }

    [RelayCommand]
    private async Task SyncOutlook()
    {
        await _syncService.SyncOutlookCalendarAsync();
        IsOutlookSynced = _syncService.IsOutlookSynced;
    }
}

