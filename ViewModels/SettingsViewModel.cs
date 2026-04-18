using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipesApp.Services; // Make sure this points to where your ISettingsService is!

namespace RecipesApp.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    // Binds to the secondary gray text in the UI
    [ObservableProperty]
    private string _themeText;

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        // When the page loads, format the saved string for the UI
        ThemeText = FormatThemeText(_settingsService.ThemePreference);
    }

    [RelayCommand]
    private async Task ChangeThemeAsync()
    {
        // Ask the user to pick a theme
        string action = await Application.Current.MainPage.DisplayActionSheet("Select Theme", "Cancel", null, "Light", "Dark", "System");

        // If they tap outside the box or hit Cancel, do nothing
        if (string.IsNullOrEmpty(action) || action == "Cancel")
            return;

        // 1. Save the string ("Light", "Dark", or "System") to local storage
        _settingsService.ThemePreference = action;

        // 2. Update the text on the UI
        ThemeText = FormatThemeText(action);

        // 3. Immediately switch the app's current theme to match
        Application.Current.UserAppTheme = action switch
        {
            "Dark" => AppTheme.Dark,
            "Light" => AppTheme.Light,
            _ => AppTheme.Unspecified // System Default
        };
    }

    // A tiny helper method to make the UI text look nice
    private string FormatThemeText(string preference)
    {
        return preference switch
        {
            "Dark" => "Dark Mode",
            "Light" => "Light Mode",
            _ => "System Default"
        };
    }

    [RelayCommand]
    private async Task AboutAsync()
    {
        await Application.Current.MainPage.DisplayAlert("About", "Editorial Kitchen v1.0", "OK");
    }

    [RelayCommand]
    private async Task TermsAsync()
    {
        // await Browser.Default.OpenAsync("https://yourwebsite.com/terms");
    }
}