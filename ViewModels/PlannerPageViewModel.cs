using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RecipesApp.Data;
using RecipesApp.Services;
using Microsoft.Maui.ApplicationModel;
using System.Linq;
using RecipesApp.Views;

namespace RecipesApp.ViewModels;

public partial class PlannerPageViewModel : ObservableObject
{
    private readonly MealPlanService _mealPlanService;
    private readonly ISupabaseAuthService _authService;

    public PlannerPageViewModel(MealPlanService mealPlanService, ISupabaseAuthService authService)
    {
        _mealPlanService = mealPlanService;
        _authService = authService;
    }

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ObservableCollection<MealSection> Sections { get; } = new();

    public int TotalPrepTime => Sections
        .SelectMany(s => s.Meals)
        .Sum(m => m.Recipe?.PrepTime ?? 0);

    public int TotalCalories => Sections
        .SelectMany(s => s.Meals)
        .Sum(m => m.Recipe?.Calories ?? 0);

    [RelayCommand]
    public async Task LoadMealsAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        ErrorMessage = string.Empty;
        Sections.Clear();

        try
        {
            var userId = _authService.CurrentUserId;
            if (userId == null || userId == Guid.Empty)
            {
                ErrorMessage = "User not authenticated.";
                return;
            }

            var date = DateOnly.FromDateTime(SelectedDate);
            var grouped = await _mealPlanService.GetMealsGroupedByTypeAsync(userId.Value, date);

            // preferred ordering for display
            var order = new[] { "Breakfast", "Lunch", "Dinner", "Snack", "Other" };
            foreach (var key in order)
            {
                if (grouped.TryGetValue(key, out var list) && list.Any())
                    Sections.Add(new MealSection(key, list));
            }

            // add remaining meal types
            foreach (var kvp in grouped)
            {
                if (!order.Contains(kvp.Key))
                    Sections.Add(new MealSection(kvp.Key, kvp.Value));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(TotalPrepTime));
            OnPropertyChanged(nameof(TotalCalories));
        }
    }

    [RelayCommand]
    private void PreviousDay() => SelectedDate = SelectedDate.AddDays(-1);

    [RelayCommand]
    private void NextDay() => SelectedDate = SelectedDate.AddDays(1);

    partial void OnSelectedDateChanged(DateTime value)
    {
        Sections.Clear();
        OnPropertyChanged(nameof(TotalPrepTime));
        OnPropertyChanged(nameof(TotalCalories));
        _ = LoadMealsAsync();
    }

    [RelayCommand]
    private async Task DeleteMealAsync(MealPlan? mealPlan)
    {
        if (mealPlan == null) return;
        if (IsLoading) return;

        IsLoading = true;
        try
        {
            // Delete on server
            await _mealPlanService.DeleteMealPlanAsync(mealPlan.Id);

            // Update UI on main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // find the section containing this meal
                var section = Sections.FirstOrDefault(s => s.Meals.Contains(mealPlan));
                if (section != null)
                {
                    section.Meals.Remove(mealPlan);

                    // if section is empty remove it from the collection
                    if (section.Meals.Count == 0)
                    {
                        Sections.Remove(section);
                    }
                    else
                    {
                        // refresh the section in the ObservableCollection so UI updates bindings if needed
                        var idx = Sections.IndexOf(section);
                        if (idx >= 0)
                        {
                            Sections.RemoveAt(idx);
                            Sections.Insert(idx, section);
                        }
                    }
                }

                OnPropertyChanged(nameof(TotalPrepTime));
                OnPropertyChanged(nameof(TotalCalories));
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Could not delete meal: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task OpenRecipeAsync(Recipe? recipe)
    {
        if (recipe == null) return;

        var page = MauiProgram.Services.GetService<RecipeDetailsPage>();
        if (page == null)
        {
            await Shell.Current.DisplayAlert("Error", "Unable to open recipe details.", "OK");
            return;
        }

        if (page.BindingContext is RecipeDetailsViewModel vm)
        {
            vm.RecipeId = recipe.Id;
        }

        await Shell.Current.Navigation.PushModalAsync(page);
    }
}

public class MealSection
{
    public string MealType { get; }
    public List<MealPlan> Meals { get; }

    public MealSection(string mealType, List<MealPlan> meals)
    {
        MealType = mealType;
        Meals = meals;
    }
}