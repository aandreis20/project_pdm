namespace RecipesApp.Views;

using RecipesApp.ViewModels;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginPageViewModel();
    }
}
