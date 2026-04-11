namespace RecipesApp.Services;

using RecipesApp.Data;

public interface IRecipeService
{
    Task<IReadOnlyList<Recipe>> GetRecipesAsync();
}
