namespace RecipesApp.Services;

using RecipesApp.Configuration;

public sealed class SupabaseClientProvider : ISupabaseClientProvider
{
    private readonly SemaphoreSlim initializeLock = new(1, 1);
    private bool isInitialized;

    public SupabaseClientProvider()
    {
        var options = new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = false
        };

        Client = new Supabase.Client(
            SupabaseSettings.Url,
            SupabaseSettings.PublishableKey,
            options);
    }

    public Supabase.Client Client { get; }

    public async Task EnsureInitializedAsync()
    {
        if (isInitialized)
        {
            return;
        }

        await initializeLock.WaitAsync();
        try
        {
            if (isInitialized)
            {
                return;
            }

            await Client.InitializeAsync();
            isInitialized = true;
        }
        finally
        {
            initializeLock.Release();
        }
    }
}
