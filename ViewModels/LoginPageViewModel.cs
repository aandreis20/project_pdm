namespace RecipesApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RecipesApp.Views;

public partial class LoginPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [RelayCommand]
    private async Task SignInAsync()
    {
        await Shell.Current.GoToAsync("//AppTabs");
    }

    [RelayCommand]
    private async Task GoToSignUpAsync()
    {
        await Shell.Current.GoToAsync(nameof(SignUpPage));
    }
}
