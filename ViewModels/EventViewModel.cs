using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgendaProElite.Models;
using AgendaProElite.Services;
using System.Collections.ObjectModel;

namespace AgendaProElite.ViewModels;

public partial class EventViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly AIService _aiService;

    [ObservableProperty]
    private Event currentEvent = new();

    [ObservableProperty]
    private ObservableCollection<Category> categories = new();

    [ObservableProperty]
    private Category? selectedCategory;

    [ObservableProperty]
    private bool isNewEvent;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? optimalTimeSuggestion;

    public EventViewModel(DatabaseService databaseService, AIService aiService)
    {
        _databaseService = databaseService;
        _aiService = aiService;
    }

    [RelayCommand]
    private async Task LoadEventAsync(int eventId)
    {
        IsLoading = true;
        try
        {
            IsNewEvent = eventId == 0;
            if (IsNewEvent)
            {
                CurrentEvent = new Event
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddHours(1),
                    Color = "#512BD4"
                };
            }
            else
            {
                CurrentEvent = await _databaseService.GetEventAsync(eventId) ?? new Event();
            }

            await LoadCategoriesAsync();
            if (CurrentEvent.CategoryId > 0)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == CurrentEvent.CategoryId);
            }

            if (IsNewEvent)
            {
                await GetOptimalTimeSuggestion();
            }
        }
        finally
        {
            IsLoading = false;
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

    [RelayCommand]
    private async Task SaveEventAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentEvent.Title))
        {
            await Shell.Current.DisplayAlert("Error", "El título es requerido", "OK");
            return;
        }

        if (SelectedCategory != null)
        {
            CurrentEvent.CategoryId = SelectedCategory.Id;
            CurrentEvent.Color = SelectedCategory.Color;
        }

        IsLoading = true;
        try
        {
            await _databaseService.SaveEventAsync(CurrentEvent);
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteEventAsync()
    {
        if (CurrentEvent.Id > 0)
        {
            var confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Eliminar este evento?", "Sí", "No");
            if (confirm)
            {
                await _databaseService.DeleteEventAsync(CurrentEvent);
                await Shell.Current.GoToAsync("..");
            }
        }
    }

    [RelayCommand]
    private async Task GetOptimalTimeSuggestion()
    {
        OptimalTimeSuggestion = await _aiService.GetOptimalTimeSuggestionsAsync(CurrentEvent.StartDate);
    }

    [RelayCommand]
    private async Task ApplyOptimalTime()
    {
        if (!string.IsNullOrEmpty(OptimalTimeSuggestion))
        {
            // Parse and apply suggestion
            // This would parse the suggestion and update CurrentEvent.StartDate and EndDate
        }
    }
}

