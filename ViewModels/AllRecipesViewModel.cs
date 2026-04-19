namespace RecipesApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipesApp.Services;
using RecipesApp.ViewModels.UIState;
using RecipesApp.Views;
using System.Collections.ObjectModel;

public partial class AllRecipesViewModel : ObservableObject
{
    private readonly IRecipeService recipeService;
    private List<RecipeUi> _allRecipesFromDb = new();

    public AllRecipesViewModel(IRecipeService recipeService)
    {
        this.recipeService = recipeService;
    }

    [ObservableProperty]
    private ObservableCollection<RecipeUi> displayedRecipes = new(); 

    [ObservableProperty]
    private bool isBusy;

    public List<string> FilterCategories { get; } = new() { "Toate", "Mic dejun", "Prânz", "Cină" };
    public List<string> SortOptions { get; } = new() { "Cele mai noi", "Timp preparare (Crescător)", "Calorii (Crescător)" };

    [ObservableProperty]
    private string selectedCategory = "Toate";

    partial void OnSelectedCategoryChanged(string value) => ApplyFiltersAndSort();

    [ObservableProperty]
    private string selectedSort = "Cele mai noi";

    partial void OnSelectedSortChanged(string value) => ApplyFiltersAndSort();

    [RelayCommand]
    private async Task LoadAllRecipesAsync()
    {
        IsBusy = true;
        var recipes = await recipeService.GetRecipesAsync();

        _allRecipesFromDb = recipes.Select(r => new RecipeUi
        {
            Id = r.Id,
            Title = r.Title,
            Category = r.Category ?? string.Empty,
            Description = r.Description ?? string.Empty,
            Ingredients = r.Ingredients ?? string.Empty,
            ImageUrl = string.IsNullOrWhiteSpace(r.ImageUrl) ? "toast_egg.png" : r.ImageUrl,
            PrepTime = r.PrepTime,
            Calories = r.Calories
        }).ToList();

        ApplyFiltersAndSort();
        IsBusy = false;
    }

    private void ApplyFiltersAndSort()
    {
        var filtered = _allRecipesFromDb.AsEnumerable();

        if (SelectedCategory != "Toate")
        {
            filtered = filtered.Where(r => r.Category == SelectedCategory);
        }

        if (SelectedSort == "Timp preparare (Crescător)")
        {
            filtered = filtered.OrderBy(r => r.PrepTime ?? int.MaxValue);
        }
        else if (SelectedSort == "Calorii (Crescător)")
        {
            filtered = filtered.OrderBy(r => r.Calories ?? int.MaxValue);
        }

        DisplayedRecipes = new ObservableCollection<RecipeUi>(filtered);
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task EditRecipeAsync(RecipeUi selectedRecipe)
    {
        if (selectedRecipe == null) return;
        await Shell.Current.GoToAsync($"AddRecipePage?recipeId={selectedRecipe.Id}");
    }

    [RelayCommand]
    private async Task OpenRecipeAsync(RecipeUi selectedRecipe)
    {
        if (selectedRecipe == null) return;

        // Resolve details page from DI
        var page = MauiProgram.Services.GetService<RecipeDetailsPage>();
        if (page == null)
        {
            await Shell.Current.DisplayAlert("Error", "Unable to open recipe details.", "OK");
            return;
        }

        // Pass id to VM so it loads the recipe
        if (page.BindingContext is RecipeDetailsViewModel vm)
        {
            vm.RecipeId = selectedRecipe.Id;
        }

        // Present modally to match existing app behavior
        await Shell.Current.Navigation.PushModalAsync(page);
    }
}
