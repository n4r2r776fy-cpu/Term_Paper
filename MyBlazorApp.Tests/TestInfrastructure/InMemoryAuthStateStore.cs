using MyBlazorApp.Services;

namespace MyBlazorApp.Tests.TestInfrastructure;

internal sealed class InMemoryAuthStateStore : IAuthStateStore
{
    public string? StoredEmail { get; private set; }

    public int ClearCallCount { get; private set; }

    public Task<string?> GetUserEmailAsync() => Task.FromResult(StoredEmail);

    public Task SetUserEmailAsync(string email)
    {
        StoredEmail = email;
        return Task.CompletedTask;
    }

    public Task ClearUserEmailAsync()
    {
        StoredEmail = null;
        ClearCallCount++;
        return Task.CompletedTask;
    }
}