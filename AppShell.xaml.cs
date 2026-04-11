namespace RecipesApp;

using RecipesApp.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
    }
}
