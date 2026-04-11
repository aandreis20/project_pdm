namespace RecipesApp.Services;

public sealed record AuthResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    bool RequiresEmailConfirmation = false)
{
    public static AuthResult Success(bool requiresEmailConfirmation = false)
    {
        return new AuthResult(true, null, requiresEmailConfirmation);
    }

    public static AuthResult Failure(string errorMessage)
    {
        return new AuthResult(false, errorMessage);
    }
}
