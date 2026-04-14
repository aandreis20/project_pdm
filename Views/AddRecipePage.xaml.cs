namespace RecipesApp.Views;

using RecipesApp.ViewModels;

public partial class AddRecipePage : ContentPage
{
    public AddRecipePage(AddRecipeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}