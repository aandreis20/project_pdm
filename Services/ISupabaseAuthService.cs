namespace RecipesApp.Services;

public interface ISupabaseAuthService
{
    string? CurrentUserEmail { get; }
    Guid? CurrentUserId { get; }

    Task<AuthResult> SignInAsync(string email, string password);

    Task<AuthResult> SignUpAsync(string fullName, string email, string password);

    Task SignOutAsync();
}
