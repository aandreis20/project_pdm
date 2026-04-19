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

    private List<RecipeUi> _allRecipesFromDb = new();

    public RecipesPageViewModel(IRecipeService recipeService)
    {
        this.recipeService = recipeService;
    }

    [ObservableProperty]
    private bool isShowingInspirations = true;

    [ObservableProperty]
    private bool isShowingAllRecipes = false;

    [ObservableProperty]
    private ObservableCollection<RecipeUi> recipes = new();

    [ObservableProperty]
    private ObservableCollection<RecipeUi> displayedAllRecipes = new();

    public List<string> FilterCategories { get; } = new() { "Toate", "Mic dejun", "Prânz", "Cină" };
    public List<string> SortOptions { get; } = new() { "Cele mai noi", "Timp preparare (Crescător)", "Calorii (Crescător)" };

    [ObservableProperty]
    private string selectedCategory = "Toate";
    partial void OnSelectedCategoryChanged(string value) => ApplyFiltersAndSort();

    [ObservableProperty]
    private string selectedSort = "Cele mai noi";
    partial void OnSelectedSortChanged(string value) => ApplyFiltersAndSort();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [RelayCommand]
    private void SwitchTab(string tabName)
    {
        if (tabName == "Inspirations")
        {
            IsShowingInspirations = true;
            IsShowingAllRecipes = false;
        }
        else if (tabName == "AllRecipes")
        {
            IsShowingInspirations = false;
            IsShowingAllRecipes = true;
        }
    }

    [RelayCommand]
    private async Task LoadRecipesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var recipesFromSupabase = await recipeService.GetRecipesAsync();

            _allRecipesFromDb = recipesFromSupabase.Select(recipe => new RecipeUi
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

            var breakfast = _allRecipesFromDb.Where(r => r.Category == "Mic dejun").OrderBy(x => random.Next()).FirstOrDefault();
            var lunch = _allRecipesFromDb.Where(r => r.Category == "Prânz").OrderBy(x => random.Next()).FirstOrDefault();
            var dinner = _allRecipesFromDb.Where(r => r.Category == "Cină").OrderBy(x => random.Next()).FirstOrDefault();

            if (breakfast != null) dailyInspirations.Add(breakfast);
            if (lunch != null) dailyInspirations.Add(lunch);
            if (dinner != null) dailyInspirations.Add(dinner);

            Recipes = new ObservableCollection<RecipeUi>(dailyInspirations);

            ApplyFiltersAndSort();
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

        DisplayedAllRecipes = new ObservableCollection<RecipeUi>(filtered);
    }

    [RelayCommand]
    private async Task GoToSettingsAsync() => await Shell.Current.GoToAsync(nameof(SettingsPage));

    [RelayCommand]
    private async Task OpenRecipeAsync(RecipeUi selectedRecipe)
    {
        if (selectedRecipe == null) return;

        var page = MauiProgram.Services.GetService<RecipeDetailsPage>();
        if (page == null) return;

        if (page.BindingContext is RecipeDetailsViewModel vm)
        {
            vm.RecipeId = selectedRecipe.Id;
        }

        await Shell.Current.Navigation.PushModalAsync(page);
    }
}