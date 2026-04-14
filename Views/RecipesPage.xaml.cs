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
            viewModel.LoadRecipesCommand.CanExecute(null))
        {
            viewModel.LoadRecipesCommand.Execute(null);
        }
    }

    private async void OnAddRecipeClicked(object sender, TappedEventArgs e)
    { 
        await Shell.Current.GoToAsync("AddRecipePage");
    }
}
