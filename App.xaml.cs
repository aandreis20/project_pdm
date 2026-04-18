using RecipesApp.Services; // Ensure this matches your namespace for ISettingsService

namespace RecipesApp;

public partial class App : Application
{
    private readonly ISettingsService _settingsService;

    // 1. Inject the settings service so we can read the saved theme
    public App(ISettingsService settingsService)
    {
        InitializeComponent();
        _settingsService = settingsService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = new Window(new AppShell());

        // Tell the app to wait until the UI thread is fully ready before switching colors
        Application.Current.Dispatcher.Dispatch(() =>
        {
            Application.Current.UserAppTheme = _settingsService.ThemePreference switch
            {
                "Dark" => AppTheme.Dark,
                "Light" => AppTheme.Light,
                _ => AppTheme.Unspecified
            };
        });

        return window;
    }
}