using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgendaProElite.Models;
using AgendaProElite.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace AgendaProElite.ViewModels;

public partial class TaskViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly AIService _aiService;

    [ObservableProperty]
    private ObservableCollection<Task> tasks = new();

    [ObservableProperty]
    private ObservableCollection<Task> completedTasks = new();

    [ObservableProperty]
    private ObservableCollection<Category> categories = new();

    [ObservableProperty]
    private Task? selectedTask;

    [ObservableProperty]
    private bool showCompleted;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private int totalPoints;

    public TaskViewModel(DatabaseService databaseService, AIService aiService)
    {
        _databaseService = databaseService;
        _aiService = aiService;
    }

    [RelayCommand]
    private async Task LoadTasksAsync()
    {
        IsLoading = true;
        try
        {
            var tasksList = await _databaseService.GetTasksAsync(ShowCompleted);
            Tasks.Clear();
            CompletedTasks.Clear();

            foreach (var task in tasksList)
            {
                if (task.IsCompleted)
                {
                    CompletedTasks.Add(task);
                }
                else
                {
                    Tasks.Add(task);
                }
            }

            await LoadCategoriesAsync();
            CalculateTotalPoints();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadCategoriesAsync()
    {
        var categoriesList = await _databaseService.GetCategoriesAsync("Task");
        Categories.Clear();
        foreach (var category in categoriesList)
        {
            Categories.Add(category);
        }
    }

    [RelayCommand]
    private async Task CreateTask()
    {
        await Shell.Current.GoToAsync($"TaskDetailPage?taskId=0");
    }

    [RelayCommand]
    private async Task CompleteTask(Task task)
    {
        if (task != null && !task.IsCompleted)
        {
            await _databaseService.CompleteTaskAsync(task);
            TotalPoints += task.Points;
            await LoadTasksAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteTask(Task task)
    {
        if (task != null)
        {
            var confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Eliminar esta tarea?", "Sí", "No");
            if (confirm)
            {
                task.IsDeleted = true;
                await _databaseService.SaveTaskAsync(task);
                await LoadTasksAsync();
            }
        }
    }

    [RelayCommand]
    private async Task CalculateAIPriorities()
    {
        IsLoading = true;
        try
        {
            foreach (var task in Tasks)
            {
                var aiPriority = await _aiService.CalculateTaskPriorityAsync(task);
                task.AIPriority = aiPriority;
                await _databaseService.SaveTaskAsync(task);
            }
            await LoadTasksAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CalculateTotalPoints()
    {
        TotalPoints = CompletedTasks.Sum(t => t.Points);
    }
}

