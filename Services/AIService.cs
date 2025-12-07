using AgendaProElite.Models;
using AgendaProElite.Services;

namespace AgendaProElite.Services;

public class AIService
{
    private readonly DatabaseService _databaseService;

    public AIService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<int> CalculateTaskPriorityAsync(Task task)
    {
        // AI-based priority calculation
        // Factors: due date proximity, estimated time, historical completion patterns
        
        var priority = 0; // Low by default

        // Check due date urgency
        if (task.DueDate.HasValue)
        {
            var daysUntilDue = (task.DueDate.Value - DateTime.Now).TotalDays;
            if (daysUntilDue < 0)
                priority = 3; // Urgent - overdue
            else if (daysUntilDue <= 1)
                priority = 3; // Urgent
            else if (daysUntilDue <= 3)
                priority = 2; // High
            else if (daysUntilDue <= 7)
                priority = 1; // Medium
        }

        // Check if it's a subtask of a high-priority task
        if (task.ParentTaskId > 0)
        {
            var parentTask = await _databaseService.GetTaskAsync(task.ParentTaskId);
            if (parentTask != null && parentTask.Priority >= 2)
            {
                priority = Math.Max(priority, parentTask.Priority - 1);
            }
        }

        // Analyze historical patterns (simplified)
        var similarTasks = await _databaseService.GetTasksAsync(true);
        var completedSimilar = similarTasks
            .Where(t => t.CategoryId == task.CategoryId && t.IsCompleted)
            .ToList();

        if (completedSimilar.Any())
        {
            var avgCompletionTime = completedSimilar.Average(t => t.ActualMinutes);
            if (task.EstimatedMinutes > avgCompletionTime * 1.5)
            {
                priority = Math.Min(priority + 1, 3); // Increase priority for complex tasks
            }
        }

        return priority;
    }

    public async Task<string> GetOptimalTimeSuggestionsAsync(DateTime preferredDate)
    {
        // Analyze user's calendar patterns to suggest optimal times
        var events = await _databaseService.GetEventsAsync(
            preferredDate.AddDays(-30), 
            preferredDate.AddDays(30));

        // Find free time slots
        var busyHours = events
            .Where(e => e.StartDate.Date == preferredDate.Date)
            .SelectMany(e => Enumerable.Range(e.StartDate.Hour, (e.EndDate.Hour - e.StartDate.Hour) + 1))
            .Distinct()
            .ToList();

        // Suggest morning (9-11) or afternoon (14-16) if available
        var suggestions = new List<string>();
        
        if (!busyHours.Contains(9) && !busyHours.Contains(10))
        {
            suggestions.Add($"Mañana: {preferredDate.Date:dd/MM/yyyy} 09:00 - 11:00 (Mejor productividad)");
        }
        
        if (!busyHours.Contains(14) && !busyHours.Contains(15))
        {
            suggestions.Add($"Tarde: {preferredDate.Date:dd/MM/yyyy} 14:00 - 16:00 (Enfoque óptimo)");
        }

        if (!suggestions.Any())
        {
            // Find first available slot
            for (int hour = 9; hour <= 17; hour++)
            {
                if (!busyHours.Contains(hour))
                {
                    suggestions.Add($"Disponible: {preferredDate.Date:dd/MM/yyyy} {hour:00}:00");
                    break;
                }
            }
        }

        return suggestions.Any() 
            ? string.Join("\n", suggestions) 
            : "No hay horarios disponibles sugeridos";
    }

    public async Task<List<DateTime>> SuggestFocusTimeBlocksAsync(DateTime date, int durationMinutes = 60)
    {
        // Suggest time blocks for focused work
        var events = await _databaseService.GetEventsAsync(date.Date, date.Date.AddDays(1).AddTicks(-1));
        
        var suggestions = new List<DateTime>();
        var startHour = 9;
        var endHour = 17;

        for (int hour = startHour; hour < endHour; hour++)
        {
            var slotStart = date.Date.AddHours(hour);
            var slotEnd = slotStart.AddMinutes(durationMinutes);

            // Check if slot is free
            var isFree = !events.Any(e => 
                (e.StartDate < slotEnd && e.EndDate > slotStart));

            if (isFree)
            {
                suggestions.Add(slotStart);
            }
        }

        return suggestions;
    }

    public async Task AutoBlockFocusTimeAsync()
    {
        // Automatically block focus time based on patterns
        var today = DateTime.Today;
        var suggestions = await SuggestFocusTimeBlocksAsync(today, 60);

        if (suggestions.Any())
        {
            // Create a focus time event (this would be implemented based on user preferences)
            var optimalTime = suggestions.First();
            // User would need to confirm or auto-create based on settings
        }
    }

    public async Task<int> CalculateTaskPointsAsync(Task task)
    {
        // Gamification: Calculate points based on task characteristics
        var basePoints = 10;
        var priorityMultiplier = task.Priority + 1;
        var estimatedTimeMultiplier = Math.Max(1, task.EstimatedMinutes / 30);

        return basePoints * priorityMultiplier * estimatedTimeMultiplier;
    }
}

