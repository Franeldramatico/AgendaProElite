using Microsoft.Maui.Controls;
using System.Linq;

namespace AgendaProElite.Services;

public class ThemeService
{
    private const string ThemeKey = "SelectedTheme";
    private const string DarkModeKey = "IsDarkMode";
    private const string AutoDarkModeKey = "AutoDarkMode";

    public string CurrentTheme { get; private set; } = "Default";
    public bool IsDarkMode { get; private set; }
    public bool AutoDarkMode { get; private set; }

    private readonly Dictionary<string, ResourceDictionary> _themes = new();

    public ThemeService()
    {
        LoadSettings();
        InitializeThemes();
    }

    private void LoadSettings()
    {
        CurrentTheme = Preferences.Get(ThemeKey, "Default");
        IsDarkMode = Preferences.Get(DarkModeKey, false);
        AutoDarkMode = Preferences.Get(AutoDarkModeKey, true);
    }

    private void InitializeThemes()
    {
        // Initialize 50+ premium themes
        // This is a simplified version - full implementation would have 50+ themes
        CreateTheme("Default", "#512BD4", "#FFFFFF", "#F5F5F5");
        CreateTheme("Ocean", "#0066CC", "#E3F2FD", "#BBDEFB");
        CreateTheme("Forest", "#2E7D32", "#E8F5E9", "#C8E6C9");
        CreateTheme("Sunset", "#FF6B35", "#FFF3E0", "#FFE0B2");
        CreateTheme("Midnight", "#1A237E", "#E8EAF6", "#C5CAE9");
        // ... Add 45+ more themes
    }

    private void CreateTheme(string name, string primaryColor, string backgroundColor, string surfaceColor)
    {
        var theme = new ResourceDictionary
        {
            { $"{name}PrimaryColor", Color.FromArgb(primaryColor) },
            { $"{name}BackgroundColor", Color.FromArgb(backgroundColor) },
            { $"{name}SurfaceColor", Color.FromArgb(surfaceColor) }
        };
        _themes[name] = theme;
    }

    public void ApplyTheme(string themeName)
    {
        CurrentTheme = themeName;
        Preferences.Set(ThemeKey, themeName);

        // Apply theme colors to application resources
        if (Application.Current != null && Application.Current.Resources != null)
        {
            // Remove old theme
            var oldTheme = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.ContainsKey("ThemeName"));
            if (oldTheme != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(oldTheme);
            }

            // Add new theme
            if (_themes.ContainsKey(themeName))
            {
                Application.Current.Resources.MergedDictionaries.Add(_themes[themeName]);
            }
        }
    }

    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        Preferences.Set(DarkModeKey, IsDarkMode);
        ApplyDarkMode();
    }

    private void ApplyDarkMode()
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
        }
    }

    public void SetAutoDarkMode(bool value)
    {
        AutoDarkMode = value;
        Preferences.Set(AutoDarkModeKey, value);
    }

    public void CheckAutoDarkMode()
    {
        if (AutoDarkMode)
        {
            var hour = DateTime.Now.Hour;
            var shouldBeDark = hour >= 20 || hour < 6; // 8 PM to 6 AM

            if (shouldBeDark != IsDarkMode)
            {
                IsDarkMode = shouldBeDark;
                Preferences.Set(DarkModeKey, IsDarkMode);
                ApplyDarkMode();
            }
        }
    }

    public List<string> GetAvailableThemes()
    {
        return _themes.Keys.ToList();
    }
}

