namespace RecipesApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipesApp.Data;
using RecipesApp.Services;
using RecipesApp.ViewModels.UIState;

public partial class AddRecipeViewModel : ObservableObject, IQueryAttributable
{
    private readonly IRecipeService recipeService;
    private Guid? recipeIdToEdit = null; 

    public AddRecipeViewModel(IRecipeService recipeService)
    {
        this.recipeService = recipeService;
    }

    [ObservableProperty]
    private string pageTitle = "Add Recipe"; 

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string category = string.Empty;

    public List<string> Categories { get; } = new() { "Mic dejun", "Prânz", "Cină" };

    [ObservableProperty]
    private string ingredients = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string imageUrl = "toast_egg.png";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string prepTimeText = string.Empty;

    [ObservableProperty]
    private string caloriesText = string.Empty;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("recipeId", out var idObj) && Guid.TryParse(idObj.ToString(), out var id))
        {
            recipeIdToEdit = id;
            PageTitle = "Edit Recipe";
            await LoadExistingRecipeAsync(id);
        }
    }

    private async Task LoadExistingRecipeAsync(Guid id)
    {
        IsBusy = true;
        var recipe = await recipeService.GetRecipeByIdAsync(id);
        if (recipe != null)
        {
            Title = recipe.Title;
            Category = recipe.Category ?? string.Empty;
            Ingredients = recipe.Ingredients ?? string.Empty;
            Description = recipe.Description ?? string.Empty;
            ImageUrl = string.IsNullOrWhiteSpace(recipe.ImageUrl) ? "toast_egg.png" : recipe.ImageUrl;

            PrepTimeText = recipe.PrepTime.ToString();
            CaloriesText = recipe.Calories.ToString();
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task SaveRecipeAsync()
    {
        if (string.IsNullOrWhiteSpace(Title)) return;
        IsBusy = true;

        try
        {
            if (!string.IsNullOrWhiteSpace(ImageUrl) && !ImageUrl.StartsWith("http") && ImageUrl != "toast_egg.png")
            {
                var uploadedUrl = await recipeService.UploadImageAsync(ImageUrl);
                if (!string.IsNullOrWhiteSpace(uploadedUrl))
                {
                    ImageUrl = uploadedUrl;
                }
            }

            int? parsedPrepTime = int.TryParse(PrepTimeText, out var pt) ? pt : null;
            int? parsedCalories = int.TryParse(CaloriesText, out var cal) ? cal : null;

            if (recipeIdToEdit.HasValue)
            {
                var recipeToUpdate = await recipeService.GetRecipeByIdAsync(recipeIdToEdit.Value);
                if (recipeToUpdate != null)
                {
                    recipeToUpdate.Title = Title;
                    recipeToUpdate.Category = Category;
                    recipeToUpdate.Ingredients = Ingredients;
                    recipeToUpdate.Description = Description;
                    recipeToUpdate.ImageUrl = ImageUrl;

                    recipeToUpdate.PrepTime = parsedPrepTime;
                    recipeToUpdate.Calories = parsedCalories;

                    await recipeService.UpdateRecipeAsync(recipeToUpdate);
                }
            }
            else 
            {
                var newRecipe = new Recipe
                {
                    Title = Title,
                    Category = Category,
                    Ingredients = Ingredients,
                    Description = Description,
                    ImageUrl = ImageUrl,

                    PrepTime = parsedPrepTime,
                    Calories = parsedCalories
                };
                await recipeService.AddRecipeAsync(newRecipe);
            }

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PickImageAsync()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Alege o poză pentru rețetă",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                ImageUrl = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window?.Page != null)
            {
                await window.Page.DisplayAlertAsync("Eroare", $"Nu am putut selecta imaginea: {ex.Message}", "OK");
            }
        }
    }
}