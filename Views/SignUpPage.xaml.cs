namespace RecipesApp.Views;

using RecipesApp.ViewModels;

public partial class SignUpPage : ContentPage
{
    public SignUpPage()
    {
        InitializeComponent();
        BindingContext = new SignUpPageViewModel();
    }
}
