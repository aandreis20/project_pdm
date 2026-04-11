namespace RecipesApp.Services;

public sealed class SupabaseAuthService : ISupabaseAuthService
{
    private readonly ISupabaseClientProvider supabase;

    public SupabaseAuthService(ISupabaseClientProvider supabase)
    {
        this.supabase = supabase;
    }

    public string? CurrentUserEmail => supabase.Client.Auth.CurrentUser?.Email;

    public async Task<AuthResult> SignInAsync(string email, string password)
    {
        try
        {
            await supabase.EnsureInitializedAsync();

            var session = await supabase.Client.Auth.SignIn(email.Trim(), password);
            if (session?.User is null)
            {
                return AuthResult.Failure("Autentificarea nu a returnat un utilizator valid.");
            }

            return AuthResult.Success();
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(GetFriendlyError(exception));
        }
    }

    public async Task<AuthResult> SignUpAsync(string fullName, string email, string password)
    {
        try
        {
            await supabase.EnsureInitializedAsync();

            var session = await supabase.Client.Auth.SignUp(email.Trim(), password);
            if (session?.User is null)
            {
                return AuthResult.Failure("Contul nu a putut fi creat. Verifica setarile de email confirmation din Supabase.");
            }

            var requiresEmailConfirmation = string.IsNullOrWhiteSpace(session.AccessToken);
            return AuthResult.Success(requiresEmailConfirmation);
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(GetFriendlyError(exception));
        }
    }

    public async Task SignOutAsync()
    {
        await supabase.EnsureInitializedAsync();
        await supabase.Client.Auth.SignOut();
    }

    private static string GetFriendlyError(Exception exception)
    {
        var message = exception.Message;
        if (message.Contains("Invalid login credentials", StringComparison.OrdinalIgnoreCase))
        {
            return "Email sau parola incorecta.";
        }

        if (message.Contains("User already registered", StringComparison.OrdinalIgnoreCase))
        {
            return "Exista deja un cont cu acest email.";
        }

        if (message.Contains("Email not confirmed", StringComparison.OrdinalIgnoreCase))
        {
            return "Emailul trebuie confirmat inainte de autentificare.";
        }

        return string.IsNullOrWhiteSpace(message)
            ? "A aparut o eroare la comunicarea cu Supabase."
            : message;
    }
}
