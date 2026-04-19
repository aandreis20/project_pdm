using RecipesApp.ViewModels;

namespace RecipesApp.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackButtonTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnTermsTapped(object sender, EventArgs e)
    {
        try
        {
            Uri uri = new Uri("https://github.com/aandreis20/project_pdm");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Could not open the link.", "OK");
        }
    }
}