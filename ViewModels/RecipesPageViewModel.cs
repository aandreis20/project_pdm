namespace RecipesApp.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RecipesApp.Services;
using RecipesApp.ViewModels.UIState;

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
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = string.Empty;
        IsBusy = true;

        try
        {
            var recipesFromSupabase = await recipeService.GetRecipesAsync();
            Recipes = new ObservableCollection<RecipeUi>(
                recipesFromSupabase.Select(recipe => new RecipeUi
                {
                    Title = recipe.Title,
                    Description = string.IsNullOrWhiteSpace(recipe.Description)
                        ? "No description yet."
                        : recipe.Description,
                    Ingredients = recipe.Ingredients ?? string.Empty,
                    Category = string.IsNullOrWhiteSpace(recipe.Category)
                        ? "Recipe"
                        : recipe.Category,
                    ImageUrl = string.IsNullOrWhiteSpace(recipe.ImageUrl)
                        ? "toast_egg.png"
                        : recipe.ImageUrl
                }));
        }
        catch (Exception exception)
        {
            ErrorMessage = string.IsNullOrWhiteSpace(exception.Message)
                ? "Retetele nu au putut fi incarcate din Supabase."
                : exception.Message;
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
}
