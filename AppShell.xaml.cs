namespace RecipesApp;

using RecipesApp.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
        Routing.RegisterRoute("AddRecipePage", typeof(Views.AddRecipePage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(Views.SettingsPage));
    }
}
