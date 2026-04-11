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
}
