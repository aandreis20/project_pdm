namespace RecipesApp.Views;

using Microsoft.Extensions.DependencyInjection;
using RecipesApp.ViewModels;

public partial class LoginPage : ContentPage, IQueryAttributable
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<LoginPageViewModel>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is not LoginPageViewModel viewModel ||
            !query.TryGetValue("info", out var info) ||
            info is null)
        {
            return;
        }

        viewModel.ShowInfo(Uri.UnescapeDataString(info.ToString() ?? string.Empty));
    }
}
