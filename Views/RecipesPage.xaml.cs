namespace RecipesApp.Views;

using Microsoft.Extensions.DependencyInjection;
using RecipesApp.ViewModels;

public partial class RecipesPage : ContentPage
{
    public RecipesPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<RecipesPageViewModel>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RecipesPageViewModel viewModel &&
            viewModel.Recipes.Count == 0 &&
            viewModel.LoadRecipesCommand.CanExecute(null))
        {
            viewModel.LoadRecipesCommand.Execute(null);
        }
    }
}
