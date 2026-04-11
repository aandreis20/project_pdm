namespace RecipesApp.Views;

using Microsoft.Extensions.DependencyInjection;
using RecipesApp.ViewModels;

public partial class PlannerPage : ContentPage
{
    public PlannerPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<PlannerPageViewModel>();
    }
}
