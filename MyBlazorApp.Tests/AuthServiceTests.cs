using MyBlazorApp.Models;
using MyBlazorApp.Services;
using MyBlazorApp.Tests.TestInfrastructure;

namespace MyBlazorApp.Tests;

public class AuthServiceTests
{
    [Fact]
    public void IsLoggedIn_IsFalseInitially()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        Assert.False(authService.IsLoggedIn);
    }

    [Fact]
    public void CurrentUser_IsNullInitially()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        Assert.Null(authService.CurrentUser);
    }

    [Fact]
    public async Task Login_ReturnsTrueForValidCredentials()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        var result = await authService.LoginAsync("demo@clinic.local", "demo123");

        Assert.True(result);
    }

    [Fact]
    public async Task Login_SetsCurrentUserOnSuccess()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        await authService.LoginAsync("demo@clinic.local", "demo123");

        Assert.Equal("demo@clinic.local", authService.CurrentUser?.Email);
    }

    [Fact]
    public async Task Login_SetsIsLoggedInToTrueOnSuccess()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        await authService.LoginAsync("demo@clinic.local", "demo123");

        Assert.True(authService.IsLoggedIn);
    }

    [Theory]
    [InlineData("demo@clinic.local", "wrong-password")]
    [InlineData("missing@clinic.local", "demo123")]
    [InlineData("", "demo123")]
    [InlineData("demo@clinic.local", "")]
    public async Task Login_ReturnsFalseForInvalidCredentials(string email, string password)
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        var result = await authService.LoginAsync(email, password);

        Assert.False(result);
    }

    [Fact]
    public async Task Login_DoesNotSetCurrentUserOnFailure()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        await authService.LoginAsync("demo@clinic.local", "wrong-password");

        Assert.Null(authService.CurrentUser);
    }

    [Fact]
    public async Task Login_RaisesOnChangeWhenSuccessful()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);
        var callCount = 0;
        authService.OnChange += () => callCount++;

        await authService.LoginAsync("demo@clinic.local", "demo123");

        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task Login_DoesNotRaiseOnChangeWhenFailed()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);
        var callCount = 0;
        authService.OnChange += () => callCount++;

        await authService.LoginAsync("demo@clinic.local", "wrong-password");

        Assert.Equal(0, callCount);
    }

    [Fact]
    public async Task Login_StoresUserEmailInStateStore()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out var authStateStore);

        await authService.LoginAsync("demo@clinic.local", "demo123");

        Assert.Equal("demo@clinic.local", authStateStore.StoredEmail);
    }

    [Fact]
    public async Task Logout_ClearsCurrentUser()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);
        await authService.LoginAsync("demo@clinic.local", "demo123");

        await authService.LogoutAsync();

        Assert.Null(authService.CurrentUser);
    }

    [Fact]
    public async Task Logout_SetsIsLoggedInToFalse()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);
        await authService.LoginAsync("demo@clinic.local", "demo123");

        await authService.LogoutAsync();

        Assert.False(authService.IsLoggedIn);
    }

    [Fact]
    public async Task Logout_RaisesOnChange()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);
        var callCount = 0;
        authService.OnChange += () => callCount++;

        await authService.LogoutAsync();

        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task Logout_ClearsStateStore()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out var authStateStore);
        await authService.LoginAsync("demo@clinic.local", "demo123");

        await authService.LogoutAsync();

        Assert.Null(authStateStore.StoredEmail);
        Assert.Equal(1, authStateStore.ClearCallCount);
    }

    [Fact]
    public async Task LoginAndLogout_RaisesOnChangeTwice()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);
        var callCount = 0;
        authService.OnChange += () => callCount++;

        await authService.LoginAsync("demo@clinic.local", "demo123");
        await authService.LogoutAsync();

        Assert.Equal(2, callCount);
    }

    [Fact]
    public async Task InitializeAsync_LoadsCurrentUserFromStateStore()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out var authStateStore);
        await authStateStore.SetUserEmailAsync("demo@clinic.local");

        await authService.InitializeAsync();

        Assert.True(authService.IsLoggedIn);
        Assert.Equal("demo@clinic.local", authService.CurrentUser?.Email);
    }

    [Fact]
    public async Task InitializeAsync_ClearsMissingUserFromStateStore()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out var authStateStore);
        await authStateStore.SetUserEmailAsync("missing@example.com");

        await authService.InitializeAsync();

        Assert.False(authService.IsLoggedIn);
        Assert.Null(authStateStore.StoredEmail);
        Assert.Equal(1, authStateStore.ClearCallCount);
    }

    [Fact]
    public async Task InitializeAsync_SetsInitializedFlag()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        await authService.InitializeAsync();

        Assert.True(authService.IsInitialized);
    }

    [Fact]
    public async Task InitializeAsync_RaisesOnChange()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);
        var callCount = 0;
        authService.OnChange += () => callCount++;

        await authService.InitializeAsync();

        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task InitializeAsync_DoesNotRunTwice()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);
        var callCount = 0;
        authService.OnChange += () => callCount++;

        await authService.InitializeAsync();
        await authService.InitializeAsync();

        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task Register_ReturnsTrueForUniqueUser()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        var result = await authService.RegisterAsync(new User { Name = "Unique", Email = "unique@example.com", Password = "secret1" });

        Assert.True(result);
    }

    [Fact]
    public async Task Register_ReturnsFalseForDuplicateUser()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        var result = await authService.RegisterAsync(new User { Name = "Duplicate", Email = "demo@clinic.local", Password = "secret1" });

        Assert.False(result);
    }

    [Fact]
    public async Task Register_PersistsUserInDataService()
    {
        using var context = new TestDataServiceContext();
        var authService = CreateAuthService(context, out _);

        await authService.RegisterAsync(new User { Name = "Persisted", Email = "persisted@example.com", Password = "secret1" });

        Assert.NotNull(context.DataService.GetUserByEmail("persisted@example.com"));
    }

    private static AuthService CreateAuthService(TestDataServiceContext context, out InMemoryAuthStateStore authStateStore)
    {
        authStateStore = new InMemoryAuthStateStore();
        return new AuthService(context.DataService, authStateStore);
    }
}