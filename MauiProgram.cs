using Microsoft.Extensions.Logging;
using RecipesApp.Services;
using RecipesApp.ViewModels;
using RecipesApp.Views;

namespace RecipesApp;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; } = null!;

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialSymbolsRounded_28pt-Bold.ttf", "MaterialIcons");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons"); // Must match XAML
                fonts.AddFont("NotoSerif-Italic.ttf", "NotoSerif"); // Must match XAML
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<ISupabaseClientProvider, SupabaseClientProvider>();
        builder.Services.AddSingleton<ISupabaseAuthService, SupabaseAuthService>();
        builder.Services.AddSingleton<IRecipeService, SupabaseRecipeService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<MealPlanService>();
        builder.Services.AddTransient<AddMealPlanViewModel>();
        builder.Services.AddTransient<AddMealPage>();
        builder.Services.AddTransient<LoginPageViewModel>();
        builder.Services.AddTransient<SignUpPageViewModel>();
        builder.Services.AddTransient<RecipesPageViewModel>();
        builder.Services.AddTransient<PlannerPageViewModel>();
        builder.Services.AddTransient<PlannerPage>();
        builder.Services.AddTransient<ProfilePageViewModel>();
        builder.Services.AddTransient<AddRecipeViewModel>();
        builder.Services.AddTransient<Views.AddRecipePage>();
        builder.Services.AddTransient<AllRecipesViewModel>();
        builder.Services.AddTransient<Views.AllRecipesPage>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SettingsPage>();


        var app = builder.Build();
        Services = app.Services;

        return app;
    }
}
