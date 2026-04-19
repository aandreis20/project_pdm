using Microsoft.Maui.Controls;
using RecipesApp.ViewModels;

namespace RecipesApp.Views;

public partial class RecipeDetailsPage : ContentPage
{
    public RecipeDetailsPage(RecipeDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Ensure the VM loads if it was provided a recipe id via query or property
        if (BindingContext is RecipeDetailsViewModel vm && vm.LoadRecipeCommand.CanExecute(null))
        {
            _ = vm.LoadRecipeCommand.ExecuteAsync(null);
        }
    }
}