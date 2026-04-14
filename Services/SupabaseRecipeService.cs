namespace RecipesApp.Services;

using RecipesApp.Data;

public sealed class SupabaseRecipeService : IRecipeService
{
    private readonly ISupabaseClientProvider supabase;

    public SupabaseRecipeService(ISupabaseClientProvider supabase)
    {
        this.supabase = supabase;
    }

    public async Task<IReadOnlyList<Recipe>> GetRecipesAsync()
    {
        await supabase.EnsureInitializedAsync();

        var response = await supabase.Client
            .From<Recipe>()
            .Order(recipe => recipe.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
            .Get();

        return response.Models;
    }

    public async Task AddRecipeAsync(Recipe recipe)
    {
        await supabase.EnsureInitializedAsync();

        var user = supabase.Client.Auth.CurrentUser;
        if (user != null && Guid.TryParse(user.Id, out var userId))
        {
            recipe.UserId = userId;
        }

        recipe.CreatedAt = DateTime.UtcNow;

        await supabase.Client.From<Recipe>().Insert(recipe);
    }

    public async Task<Recipe?> GetRecipeByIdAsync(Guid id)
    {
        await supabase.EnsureInitializedAsync();
        var response = await supabase.Client.From<Recipe>().Where(x => x.Id == id).Get();
        return response.Models.FirstOrDefault();
    }

    public async Task UpdateRecipeAsync(Recipe recipe)
    {
        await supabase.EnsureInitializedAsync();
        await supabase.Client.From<Recipe>().Update(recipe);
    }

    public async Task<string> UploadImageAsync(string localFilePath)
    {
        if (string.IsNullOrWhiteSpace(localFilePath) || !File.Exists(localFilePath))
            return string.Empty;

        try
        {
            await supabase.EnsureInitializedAsync();

            var fileBytes = File.ReadAllBytes(localFilePath);

            var fileExtension = Path.GetExtension(localFilePath) ?? ".jpg";
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            await supabase.Client.Storage
                .From("recipe_images")
                .Upload(fileBytes, uniqueFileName);

            var publicUrl = supabase.Client.Storage
                .From("recipe_images")
                .GetPublicUrl(uniqueFileName);

            return publicUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la upload imagine: {ex.Message}");
            return string.Empty;
        }
    }
}
