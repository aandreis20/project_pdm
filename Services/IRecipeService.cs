namespace RecipesApp.Services;

using RecipesApp.Data;

public interface IRecipeService
{
    Task<IReadOnlyList<Recipe>> GetRecipesAsync();
    Task AddRecipeAsync(Recipe recipe);
    Task<Recipe?> GetRecipeByIdAsync(Guid id);
    Task UpdateRecipeAsync(Recipe recipe);
    Task<string> UploadImageAsync(string localFilePath);
}
