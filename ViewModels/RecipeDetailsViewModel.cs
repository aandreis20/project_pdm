using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipesApp.Data;
using RecipesApp.Services;
using Microsoft.Maui.Controls;
using RecipesApp.Views;

namespace RecipesApp.ViewModels;

public partial class RecipeDetailsViewModel : ObservableObject, IQueryAttributable
{
    private readonly IRecipeService _recipeService;
    private readonly ISupabaseAuthService _authService;

    public RecipeDetailsViewModel(IRecipeService recipeService, ISupabaseAuthService authService)
    {
        _recipeService = recipeService;
        _authService = authService;
    }

    // Backing recipe id (used for navigation / loading)
    [ObservableProperty]
    private Guid recipeId;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string ingredients = string.Empty;

    [ObservableProperty]
    private string category = string.Empty;

    [ObservableProperty]
    private string imageUrl = "toast_egg.png";

    [ObservableProperty]
    private int? prepTime;

    [ObservableProperty]
    private int? calories;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isOwner;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    // IQueryAttributable: accept recipeId via Shell query params
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("recipeId", out var idObj) && Guid.TryParse(idObj?.ToString(), out var id))
        {
            RecipeId = id;
            _ = LoadRecipeAsync();
        }
    }

    [RelayCommand]
    private async Task LoadRecipeAsync()
    {
        if (IsBusy) return;
        if (RecipeId == Guid.Empty)
        {
            ErrorMessage = "Invalid recipe id.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var recipe = await _recipeService.GetRecipeByIdAsync(RecipeId);
            if (recipe == null)
            {
                ErrorMessage = "Recipe not found.";
                return;
            }

            Title = recipe.Title;
            Description = recipe.Description ?? string.Empty;
            Ingredients = recipe.Ingredients ?? string.Empty;
            Category = recipe.Category ?? string.Empty;
            ImageUrl = string.IsNullOrWhiteSpace(recipe.ImageUrl) ? "toast_egg.png" : recipe.ImageUrl;
            PrepTime = recipe.PrepTime;
            Calories = recipe.Calories;

            IsOwner = (_authService.CurrentUserId ?? Guid.Empty) == recipe.UserId;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load recipe: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditRecipeAsync()
    {
        if (RecipeId == Guid.Empty) return;

        // Resolve the existing Add/Edit recipe page from DI
        var addPage = MauiProgram.Services.GetService<AddRecipePage>();
        if (addPage == null)
        {
            await Shell.Current.DisplayAlert("Error", "Unable to open edit recipe page.", "OK");
            return;
        }

        // If the page VM supports query attributes, populate it so it loads the recipe for editing
        if (addPage.BindingContext is AddRecipeViewModel addVm)
        {
            // AddRecipeViewModel implements IQueryAttributable.ApplyQueryAttributes and will load the recipe
            addVm.ApplyQueryAttributes(new Dictionary<string, object>
            {
                ["recipeId"] = RecipeId.ToString()
            });
        }

        // Present edit page modally to match the app's modal patterns
        await Shell.Current.Navigation.PushModalAsync(addPage);
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    // Add this partial method implementation to the existing class.
    // It triggers loading when the generated `RecipeId` property is changed programmatically.
    partial void OnRecipeIdChanged(Guid value)
    {
        // Fire-and-forget is fine because the VM handles IsBusy and errors.
        _ = LoadRecipeAsync();
    }
}