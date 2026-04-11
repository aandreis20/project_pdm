namespace RecipesApp.Views;

using RecipesApp.ViewModels;

public partial class PlannerPage : ContentPage
{
    public PlannerPage()
    {
        InitializeComponent();
        BindingContext = new PlannerPageViewModel();
    }
}
