using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgendaProElite.Services;

namespace AgendaProElite.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string currentView = "Calendar";

    public MainViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    private async Task NavigateToCalendar()
    {
        CurrentView = "Calendar";
        await Shell.Current.GoToAsync("//CalendarPage");
    }

    [RelayCommand]
    private async Task NavigateToTasks()
    {
        CurrentView = "Tasks";
        await Shell.Current.GoToAsync("//TaskPage");
    }

    [RelayCommand]
    private async Task NavigateToSettings()
    {
        CurrentView = "Settings";
        await Shell.Current.GoToAsync("//SettingsPage");
    }
}

