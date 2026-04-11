namespace RecipesApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RecipesApp.Services;

public partial class SignUpPageViewModel : ObservableObject
{
    private readonly ISupabaseAuthService authService;

    public SignUpPageViewModel(ISupabaseAuthService authService)
    {
        this.authService = authService;
    }

    [ObservableProperty]
    private string fullName = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasInfo))]
    private string infoMessage = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasInfo => !string.IsNullOrWhiteSpace(InfoMessage);

    [RelayCommand]
    private async Task SignUpAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = string.Empty;
        InfoMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(FullName) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "Completeaza toate campurile.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Parolele nu coincid.";
            return;
        }

        IsBusy = true;
        var result = await authService.SignUpAsync(FullName, Email, Password);
        IsBusy = false;

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "Contul nu a putut fi creat.";
            return;
        }

        var loginMessage = Uri.EscapeDataString("Cont creat. Verifica emailul pentru confirmare, apoi autentifica-te.");
        await Shell.Current.GoToAsync($"//LoginPage?info={loginMessage}");
    }

    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
