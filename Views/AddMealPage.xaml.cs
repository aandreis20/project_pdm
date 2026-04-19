using Microsoft.Maui.Controls;
using RecipesApp.ViewModels;

namespace RecipesApp.Views;

public partial class AddMealPage : ContentPage
{
    public AddMealPage(AddMealPlanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // preload recipes when page is created
        _ = viewModel.LoadRecipesCommand.ExecuteAsync(null);
    }
}