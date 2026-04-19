namespace RecipesApp.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RecipesApp.Services;
using RecipesApp.ViewModels.UIState;
using RecipesApp.Views;

public partial class RecipesPageViewModel : ObservableObject
{
    private readonly IRecipeService recipeService;

    public RecipesPageViewModel(IRecipeService recipeService)
    {
        this.recipeService = recipeService;
    }

    [ObservableProperty]
    private ObservableCollection<RecipeUi> recipes = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [RelayCommand]
    private async Task LoadRecipesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var recipesFromSupabase = await recipeService.GetRecipesAsync();

            var allRecipes = recipesFromSupabase.Select(recipe => new RecipeUi
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Category = recipe.Category ?? string.Empty,
                Description = recipe.Description ?? string.Empty,
                Ingredients = recipe.Ingredients ?? string.Empty,
                ImageUrl = string.IsNullOrWhiteSpace(recipe.ImageUrl) ? "toast_egg.png" : recipe.ImageUrl,
                PrepTime = recipe.PrepTime,
                Calories = recipe.Calories
            }).ToList();

            var random = new Random();
            var dailyInspirations = new List<RecipeUi>();

            var breakfast = allRecipes.Where(r => r.Category == "Mic dejun").OrderBy(x => random.Next()).FirstOrDefault();
            var lunch = allRecipes.Where(r => r.Category == "Prânz").OrderBy(x => random.Next()).FirstOrDefault();
            var dinner = allRecipes.Where(r => r.Category == "Cină").OrderBy(x => random.Next()).FirstOrDefault();

            if (breakfast != null) dailyInspirations.Add(breakfast);
            if (lunch != null) dailyInspirations.Add(lunch);
            if (dinner != null) dailyInspirations.Add(dinner);

            Recipes = new ObservableCollection<RecipeUi>(dailyInspirations);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load recipes: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }

    [RelayCommand]
    private async Task EditRecipeAsync(RecipeUi selectedRecipe)
    {
        if (selectedRecipe == null) return;

        await Shell.Current.GoToAsync($"AddRecipePage?recipeId={selectedRecipe.Id}");
    }

    [RelayCommand]
    private async Task GoToAllRecipesAsync()
    {
        await Shell.Current.GoToAsync("AllRecipesPage");
    }

    [RelayCommand]
    private async Task GoToSettingsAsync()
    {
        // Shell.Current.GoToAsync uses string-based URI routing
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    [RelayCommand]
    private async Task OpenRecipeAsync(RecipeUi selectedRecipe)
    {
        if (selectedRecipe == null) return;

        // Resolve the details page from DI (must be registered in MauiProgram)
        var page = MauiProgram.Services.GetService<RecipeDetailsPage>();
        if (page == null)
        {
            await Shell.Current.DisplayAlert("Error", "Unable to open recipe details.", "OK");
            return;
        }

        // Pass the id to the viewmodel so it loads the recipe
        if (page.BindingContext is RecipeDetailsViewModel vm)
        {
            vm.RecipeId = selectedRecipe.Id;
        }

        // Show as modal (keeps behavior consistent with other modal pages in the app)
        await Shell.Current.Navigation.PushModalAsync(page);
    }
}
