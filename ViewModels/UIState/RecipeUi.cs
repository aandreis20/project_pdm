namespace RecipesApp.ViewModels.UIState;

public sealed class RecipeUi
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string Ingredients { get; init; } = string.Empty;

    public string ImageUrl { get; init; } = "toast_egg.png";
    public int? PrepTime { get; init; }
    public int? Calories { get; init; }
}
