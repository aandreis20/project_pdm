namespace RecipesApp.Views;

using Microsoft.Extensions.DependencyInjection;
using RecipesApp.ViewModels;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<LoginPageViewModel>();
    }
}
