namespace RecipesApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RecipesApp.Services;

public partial class ProfilePageViewModel : ObservableObject
{
    private readonly ISupabaseAuthService authService;

    public ProfilePageViewModel(ISupabaseAuthService authService)
    {
        this.authService = authService;
        Email = authService.CurrentUserEmail ?? "Not signed in";
    }

    [ObservableProperty]
    private string email = "Not signed in";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = string.Empty;
        IsBusy = true;

        try
        {
            await authService.SignOutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception exception)
        {
            ErrorMessage = string.IsNullOrWhiteSpace(exception.Message)
                ? "Logoutul a esuat."
                : exception.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
