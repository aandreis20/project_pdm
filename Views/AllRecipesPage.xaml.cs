namespace RecipesApp.Views;

using RecipesApp.ViewModels;

public partial class AllRecipesPage : ContentPage
{
    public AllRecipesPage(AllRecipesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AllRecipesViewModel vm)
        {
            vm.LoadAllRecipesCommand.Execute(null);
        }
    }
}