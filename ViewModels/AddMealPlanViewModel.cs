using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipesApp.Data;
using RecipesApp.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace RecipesApp.ViewModels;

public partial class AddMealPlanViewModel : ObservableObject, IQueryAttributable
{
    private readonly MealPlanService _mealPlanService;
    private readonly IRecipeService _recipeService;
    private readonly ISupabaseAuthService _authService;

    public AddMealPlanViewModel(MealPlanService mealPlanService, IRecipeService recipeService, ISupabaseAuthService authService)
    {
        _mealPlanService = mealPlanService;
        _recipeService = recipeService;
        _authService = authService;
    }

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private string selectedMealType = "Breakfast";

    [ObservableProperty]
    private ObservableCollection<Recipe> recipes = new();

    [ObservableProperty]
    private Recipe? selectedRecipe;

    [ObservableProperty]
    private bool isSaving;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public List<string> MealTypes { get; } = new() { "Breakfast", "Lunch", "Dinner", "Snack", "Other" };

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("date", out var d) && DateTime.TryParse(d?.ToString(), out var dt))
            SelectedDate = dt;
    }

    [RelayCommand]
    private async Task LoadRecipesAsync()
    {
        try
        {
            var list = await _recipeService.GetRecipesAsync();
            Recipes = new ObservableCollection<Recipe>(list);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
    // Replace or update the existing SaveAsync and CancelAsync methods so they close the modal stack
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsSaving) return;

        // Validate
        if (SelectedRecipe == null)
        {
            ErrorMessage = "Please select a recipe.";
            await Shell.Current.DisplayAlert("Validation", ErrorMessage, "OK");
            return;
        }

        IsSaving = true;
        ErrorMessage = string.Empty;

        try
        {
            var userId = _authService.CurrentUserId ?? Guid.Empty;
            var mealPlan = new MealPlan
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RecipeId = SelectedRecipe.Id,
                PlanDate = DateOnly.FromDateTime(SelectedDate),
                MealType = SelectedMealType
            };

            await _mealPlanService.AddMealPlanAsync(mealPlan);

            // Give user feedback and close modal
            await Shell.Current.DisplayAlert("Success", "Meal plan added.", "OK");
            await Shell.Current.Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await Shell.Current.DisplayAlert("Error", $"Could not save meal: {ex.Message}", "OK");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.Navigation.PopModalAsync();
    }
}