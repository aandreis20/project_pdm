namespace RecipesApp.Views;

using Microsoft.Extensions.DependencyInjection;
using RecipesApp.ViewModels;

public partial class SignUpPage : ContentPage
{
    public SignUpPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<SignUpPageViewModel>();
    }
}
