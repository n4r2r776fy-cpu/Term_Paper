namespace MyBlazorApp.Services;

public interface IAuthStateStore
{
    Task<string?> GetUserEmailAsync();

    Task SetUserEmailAsync(string email);

    Task ClearUserEmailAsync();
}