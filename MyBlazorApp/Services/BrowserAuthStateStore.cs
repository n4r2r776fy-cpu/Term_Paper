using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MyBlazorApp.Services;

public class BrowserAuthStateStore : IAuthStateStore
{
    private const string UserEmailKey = "auth.user.email";
    private readonly ProtectedSessionStorage _sessionStorage;

    public BrowserAuthStateStore(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async Task<string?> GetUserEmailAsync()
    {
        var result = await _sessionStorage.GetAsync<string>(UserEmailKey);
        return result.Success ? result.Value : null;
    }

    public async Task SetUserEmailAsync(string email)
    {
        await _sessionStorage.SetAsync(UserEmailKey, email);
    }

    public async Task ClearUserEmailAsync()
    {
        await _sessionStorage.DeleteAsync(UserEmailKey);
    }
}