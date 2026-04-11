namespace RecipesApp.Views;

using Microsoft.Extensions.DependencyInjection;
using RecipesApp.ViewModels;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<ProfilePageViewModel>();
    }
}
