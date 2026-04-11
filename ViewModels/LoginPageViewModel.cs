namespace RecipesApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RecipesApp.Services;
using RecipesApp.Views;

public partial class LoginPageViewModel : ObservableObject
{
    private readonly ISupabaseAuthService authService;

    public LoginPageViewModel(ISupabaseAuthService authService)
    {
        this.authService = authService;
    }

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [RelayCommand]
    private async Task SignInAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Completeaza emailul si parola.";
            return;
        }

        IsBusy = true;
        var result = await authService.SignInAsync(Email, Password);
        IsBusy = false;

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "Autentificarea a esuat.";
            return;
        }

        await Shell.Current.GoToAsync("//AppTabs");
    }

    [RelayCommand]
    private async Task GoToSignUpAsync()
    {
        await Shell.Current.GoToAsync(nameof(SignUpPage));
    }
}
