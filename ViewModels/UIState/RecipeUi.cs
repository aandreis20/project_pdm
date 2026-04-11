namespace RecipesApp.ViewModels.UIState;

public sealed class RecipeUi
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string Ingredients { get; init; } = string.Empty;

    public string ImageUrl { get; init; } = "toast_egg.png";
}
