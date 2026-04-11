namespace RecipesApp.Views;

using RecipesApp.ViewModels;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = new ProfilePageViewModel();
    }
}
