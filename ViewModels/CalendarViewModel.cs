using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgendaProElite.Models;
using AgendaProElite.Services;
using System.Collections.ObjectModel;

namespace AgendaProElite.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly AIService _aiService;

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private string currentView = "Month"; // Month, Week, Day, Agenda

    [ObservableProperty]
    private ObservableCollection<Event> events = new();

    [ObservableProperty]
    private ObservableCollection<Event> todayEvents = new();

    [ObservableProperty]
    private ObservableCollection<Category> categories = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private Event? selectedEvent;

    public CalendarViewModel(DatabaseService databaseService, AIService aiService)
    {
        _databaseService = databaseService;
        _aiService = aiService;
    }

    [RelayCommand]
    private async Task LoadEventsAsync()
    {
        IsLoading = true;
        try
        {
            var startDate = GetViewStartDate();
            var endDate = GetViewEndDate();
            var eventsList = await _databaseService.GetEventsAsync(startDate, endDate);
            
            Events.Clear();
            foreach (var evt in eventsList)
            {
                Events.Add(evt);
            }

            await LoadTodayEventsAsync();
            await LoadCategoriesAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadTodayEventsAsync()
    {
        var today = DateTime.Today;
        var todayEventsList = await _databaseService.GetEventsAsync(today, today.AddDays(1).AddTicks(-1));
        TodayEvents.Clear();
        foreach (var evt in todayEventsList)
        {
            TodayEvents.Add(evt);
        }
    }

    private async Task LoadCategoriesAsync()
    {
        var categoriesList = await _databaseService.GetCategoriesAsync();
        Categories.Clear();
        foreach (var category in categoriesList)
        {
            Categories.Add(category);
        }
    }

    private DateTime GetViewStartDate()
    {
        return CurrentView switch
        {
            "Day" => SelectedDate.Date,
            "Week" => SelectedDate.Date.AddDays(-(int)SelectedDate.DayOfWeek),
            "Month" => new DateTime(SelectedDate.Year, SelectedDate.Month, 1),
            _ => SelectedDate.Date
        };
    }

    private DateTime GetViewEndDate()
    {
        return CurrentView switch
        {
            "Day" => SelectedDate.Date.AddDays(1).AddTicks(-1),
            "Week" => SelectedDate.Date.AddDays(-(int)SelectedDate.DayOfWeek).AddDays(7).AddTicks(-1),
            "Month" => new DateTime(SelectedDate.Year, SelectedDate.Month, 1).AddMonths(1).AddTicks(-1),
            _ => SelectedDate.Date.AddDays(1).AddTicks(-1)
        };
    }

    [RelayCommand]
    private async Task ChangeView(string view)
    {
        CurrentView = view;
        await LoadEventsAsync();
    }

    [RelayCommand]
    private async Task PreviousPeriod()
    {
        SelectedDate = CurrentView switch
        {
            "Day" => SelectedDate.AddDays(-1),
            "Week" => SelectedDate.AddDays(-7),
            "Month" => SelectedDate.AddMonths(-1),
            _ => SelectedDate.AddDays(-1)
        };
        await LoadEventsAsync();
    }

    [RelayCommand]
    private async Task NextPeriod()
    {
        SelectedDate = CurrentView switch
        {
            "Day" => SelectedDate.AddDays(1),
            "Week" => SelectedDate.AddDays(7),
            "Month" => SelectedDate.AddMonths(1),
            _ => SelectedDate.AddDays(1)
        };
        await LoadEventsAsync();
    }

    [RelayCommand]
    private async Task GoToToday()
    {
        SelectedDate = DateTime.Today;
        await LoadEventsAsync();
    }

    [RelayCommand]
    private async Task CreateEvent()
    {
        await Shell.Current.GoToAsync($"EventDetailPage?eventId=0");
    }

    [RelayCommand]
    private async Task EditEvent(Event eventItem)
    {
        if (eventItem != null)
        {
            await Shell.Current.GoToAsync($"EventDetailPage?eventId={eventItem.Id}");
        }
    }

    [RelayCommand]
    private async Task DeleteEvent(Event eventItem)
    {
        if (eventItem != null)
        {
            await _databaseService.DeleteEventAsync(eventItem);
            await LoadEventsAsync();
        }
    }

    [RelayCommand]
    private async Task GetAISuggestions()
    {
        var suggestions = await _aiService.GetOptimalTimeSuggestionsAsync(SelectedDate);
        // Show suggestions to user
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
        LoadEventsAsync().Wait();
    }
}

