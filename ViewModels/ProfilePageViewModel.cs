namespace RecipesApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

public partial class ProfilePageViewModel : ObservableObject
{
    [RelayCommand]
    private async Task LogoutAsync()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
