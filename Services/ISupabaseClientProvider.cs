namespace RecipesApp.Services;

public interface ISupabaseClientProvider
{
    Supabase.Client Client { get; }

    Task EnsureInitializedAsync();
}
