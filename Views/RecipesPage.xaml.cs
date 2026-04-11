namespace RecipesApp.Views;

using RecipesApp.ViewModels;

public partial class RecipesPage : ContentPage
{
    public RecipesPage()
    {
        InitializeComponent();
        BindingContext = new RecipesPageViewModel();
    }
}
