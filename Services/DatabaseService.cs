using SQLite;
using AgendaProElite.Models;
using System.Diagnostics;

namespace AgendaProElite.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _databasePath;

    public DatabaseService()
    {
        _databasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "agendapro_elite.db3");
    }

    public async Task InitializeAsync()
    {
        if (_database != null)
            return;

        _database = new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
        
        // Create tables with optimized indexes
        await CreateTablesAsync();
        await CreateIndexesAsync();
        
        // Initialize default data
        await InitializeDefaultDataAsync();
    }

    private async Task CreateTablesAsync()
    {
        await _database!.CreateTableAsync<Event>();
        await _database.CreateTableAsync<Task>();
        await _database.CreateTableAsync<Reminder>();
        await _database.CreateTableAsync<Category>();
        await _database.CreateTableAsync<User>();
        await _database.CreateTableAsync<Collaboration>();
    }

    private async Task CreateIndexesAsync()
    {
        // Optimized indexes for fast queries (10k events <1s)
        await _database!.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_events_startdate ON Events(StartDate)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_events_enddate ON Events(EndDate)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_events_category ON Events(CategoryId)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_events_daterange ON Events(StartDate, EndDate)");
        
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_tasks_duedate ON Tasks(DueDate)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_tasks_priority ON Tasks(Priority, AIPriority)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_tasks_parent ON Tasks(ParentTaskId)");
        
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_reminders_time ON Reminders(ReminderTime)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_reminders_event ON Reminders(EventId)");
        
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_categories_type ON Categories(Type)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_collaborations_event ON Collaborations(EventId)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_collaborations_user ON Collaborations(UserId)");
    }

    private async Task InitializeDefaultDataAsync()
    {
        // Check if default categories exist
        var categories = await _database!.Table<Category>().Where(c => c.IsDefault).ToListAsync();
        if (categories.Count == 0)
        {
            var defaultCategories = new List<Category>
            {
                new Category { Name = "Personal", Color = "#FF6B6B", Icon = "person", Type = "Both", IsDefault = true },
                new Category { Name = "Trabajo", Color = "#4ECDC4", Icon = "briefcase", Type = "Both", IsDefault = true },
                new Category { Name = "Familia", Color = "#45B7D1", Icon = "people", Type = "Both", IsDefault = true },
                new Category { Name = "Salud", Color = "#96CEB4", Icon = "heart", Type = "Both", IsDefault = true },
                new Category { Name = "Educación", Color = "#FFEAA7", Icon = "school", Type = "Both", IsDefault = true }
            };
            await _database.InsertAllAsync(defaultCategories);
        }
    }

    // Event CRUD Operations
    public async Task<List<Event>> GetEventsAsync(DateTime startDate, DateTime endDate)
    {
        await InitializeAsync();
        return await _database!.Table<Event>()
            .Where(e => e.StartDate >= startDate && e.StartDate <= endDate && !e.IsDeleted)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<Event?> GetEventAsync(int id)
    {
        await InitializeAsync();
        return await _database!.Table<Event>().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<int> SaveEventAsync(Event eventItem)
    {
        await InitializeAsync();
        eventItem.UpdatedAt = DateTime.Now;
        if (eventItem.Id == 0)
        {
            eventItem.CreatedAt = DateTime.Now;
            return await _database!.InsertAsync(eventItem);
        }
        else
        {
            return await _database.UpdateAsync(eventItem);
        }
    }

    public async Task<int> DeleteEventAsync(Event eventItem)
    {
        await InitializeAsync();
        eventItem.IsDeleted = true;
        eventItem.UpdatedAt = DateTime.Now;
        return await _database!.UpdateAsync(eventItem);
    }

    public async Task<List<Event>> SearchEventsAsync(string searchTerm)
    {
        await InitializeAsync();
        return await _database!.Table<Event>()
            .Where(e => (e.Title.Contains(searchTerm) || e.Description.Contains(searchTerm)) && !e.IsDeleted)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    // Task CRUD Operations
    public async Task<List<Task>> GetTasksAsync(bool includeCompleted = false)
    {
        await InitializeAsync();
        var query = _database!.Table<Task>().Where(t => !t.IsDeleted);
        if (!includeCompleted)
            query = query.Where(t => !t.IsCompleted);
        return await query.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ToListAsync();
    }

    public async Task<Task?> GetTaskAsync(int id)
    {
        await InitializeAsync();
        return await _database!.Table<Task>().FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Task>> GetSubtasksAsync(int parentTaskId)
    {
        await InitializeAsync();
        return await _database!.Table<Task>()
            .Where(t => t.ParentTaskId == parentTaskId && !t.IsDeleted)
            .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    public async Task<int> SaveTaskAsync(Task task)
    {
        await InitializeAsync();
        task.UpdatedAt = DateTime.Now;
        if (task.Id == 0)
        {
            task.CreatedAt = DateTime.Now;
            return await _database!.InsertAsync(task);
        }
        else
        {
            return await _database.UpdateAsync(task);
        }
    }

    public async Task<int> CompleteTaskAsync(Task task)
    {
        await InitializeAsync();
        task.IsCompleted = true;
        task.CompletedAt = DateTime.Now;
        task.UpdatedAt = DateTime.Now;
        return await _database!.UpdateAsync(task);
    }

    // Reminder Operations
    public async Task<List<Reminder>> GetRemindersAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await InitializeAsync();
        var query = _database!.Table<Reminder>().Where(r => !r.IsDeleted && !r.IsCompleted);
        if (startDate.HasValue)
            query = query.Where(r => r.ReminderTime >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(r => r.ReminderTime <= endDate.Value);
        return await query.OrderBy(r => r.ReminderTime).ToListAsync();
    }

    public async Task<int> SaveReminderAsync(Reminder reminder)
    {
        await InitializeAsync();
        reminder.UpdatedAt = DateTime.Now;
        if (reminder.Id == 0)
        {
            reminder.CreatedAt = DateTime.Now;
            return await _database!.InsertAsync(reminder);
        }
        else
        {
            return await _database.UpdateAsync(reminder);
        }
    }

    // Category Operations
    public async Task<List<Category>> GetCategoriesAsync(string? type = null)
    {
        await InitializeAsync();
        var query = _database!.Table<Category>().Where(c => !c.IsDeleted);
        if (!string.IsNullOrEmpty(type))
            query = query.Where(c => c.Type == type || c.Type == "Both");
        return await query.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<int> SaveCategoryAsync(Category category)
    {
        await InitializeAsync();
        category.UpdatedAt = DateTime.Now;
        if (category.Id == 0)
        {
            category.CreatedAt = DateTime.Now;
            return await _database!.InsertAsync(category);
        }
        else
        {
            return await _database.UpdateAsync(category);
        }
    }

    // User Operations
    public async Task<List<User>> GetUsersAsync()
    {
        await InitializeAsync();
        return await _database!.Table<User>().Where(u => !u.IsDeleted && u.IsActive).ToListAsync();
    }

    public async Task<int> SaveUserAsync(User user)
    {
        await InitializeAsync();
        user.UpdatedAt = DateTime.Now;
        if (user.Id == 0)
        {
            user.CreatedAt = DateTime.Now;
            return await _database!.InsertAsync(user);
        }
        else
        {
            return await _database.UpdateAsync(user);
        }
    }

    // Collaboration Operations
    public async Task<List<Collaboration>> GetCollaborationsByEventAsync(int eventId)
    {
        await InitializeAsync();
        return await _database!.Table<Collaboration>()
            .Where(c => c.EventId == eventId && !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> SaveCollaborationAsync(Collaboration collaboration)
    {
        await InitializeAsync();
        collaboration.UpdatedAt = DateTime.Now;
        if (collaboration.Id == 0)
        {
            collaboration.CreatedAt = DateTime.Now;
            return await _database!.InsertAsync(collaboration);
        }
        else
        {
            return await _database.UpdateAsync(collaboration);
        }
    }

    // Performance optimized bulk operations
    public async Task<List<Event>> GetEventsBulkAsync(DateTime startDate, DateTime endDate, int limit = 1000, int offset = 0)
    {
        await InitializeAsync();
        return await _database!.Table<Event>()
            .Where(e => e.StartDate >= startDate && e.StartDate <= endDate && !e.IsDeleted)
            .OrderBy(e => e.StartDate)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    // Backup and restore
    public async Task<bool> BackupDatabaseAsync(string backupPath)
    {
        try
        {
            await InitializeAsync();
            File.Copy(_databasePath, backupPath, true);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

