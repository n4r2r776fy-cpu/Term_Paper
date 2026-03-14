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
        var authService = new AuthService(context.DataService);

        Assert.False(authService.IsLoggedIn);
    }

    [Fact]
    public void CurrentUser_IsNullInitially()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);

        Assert.Null(authService.CurrentUser);
    }

    [Fact]
    public void Login_ReturnsTrueForValidCredentials()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);

        var result = authService.Login("demo@clinic.local", "demo123");

        Assert.True(result);
    }

    [Fact]
    public void Login_SetsCurrentUserOnSuccess()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);

        authService.Login("demo@clinic.local", "demo123");

        Assert.Equal("demo@clinic.local", authService.CurrentUser?.Email);
    }

    [Fact]
    public void Login_SetsIsLoggedInToTrueOnSuccess()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);

        authService.Login("demo@clinic.local", "demo123");

        Assert.True(authService.IsLoggedIn);
    }

    [Theory]
    [InlineData("demo@clinic.local", "wrong-password")]
    [InlineData("missing@clinic.local", "demo123")]
    [InlineData("", "demo123")]
    [InlineData("demo@clinic.local", "")]
    public void Login_ReturnsFalseForInvalidCredentials(string email, string password)
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);

        var result = authService.Login(email, password);

        Assert.False(result);
    }

    [Fact]
    public void Login_DoesNotSetCurrentUserOnFailure()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);

        authService.Login("demo@clinic.local", "wrong-password");

        Assert.Null(authService.CurrentUser);
    }

    [Fact]
    public void Login_RaisesOnChangeWhenSuccessful()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);
        var callCount = 0;
        authService.OnChange += () => callCount++;

        authService.Login("demo@clinic.local", "demo123");

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void Login_DoesNotRaiseOnChangeWhenFailed()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);
        var callCount = 0;
        authService.OnChange += () => callCount++;

        authService.Login("demo@clinic.local", "wrong-password");

        Assert.Equal(0, callCount);
    }

    [Fact]
    public void Logout_ClearsCurrentUser()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);
        authService.Login("demo@clinic.local", "demo123");

        authService.Logout();

        Assert.Null(authService.CurrentUser);
    }

    [Fact]
    public void Logout_SetsIsLoggedInToFalse()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);
        authService.Login("demo@clinic.local", "demo123");

        authService.Logout();

        Assert.False(authService.IsLoggedIn);
    }

    [Fact]
    public void Logout_RaisesOnChange()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);
        var callCount = 0;
        authService.OnChange += () => callCount++;

        authService.Logout();

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void LoginAndLogout_RaisesOnChangeTwice()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);
        var callCount = 0;
        authService.OnChange += () => callCount++;

        authService.Login("demo@clinic.local", "demo123");
        authService.Logout();

        Assert.Equal(2, callCount);
    }

    [Fact]
    public void Register_ReturnsTrueForUniqueUser()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);

        var result = authService.Register(new User { Name = "Unique", Email = "unique@example.com", Password = "secret1" });

        Assert.True(result);
    }

    [Fact]
    public void Register_ReturnsFalseForDuplicateUser()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);

        var result = authService.Register(new User { Name = "Duplicate", Email = "demo@clinic.local", Password = "secret1" });

        Assert.False(result);
    }

    [Fact]
    public void Register_PersistsUserInDataService()
    {
        using var context = new TestDataServiceContext();
        var authService = new AuthService(context.DataService);

        authService.Register(new User { Name = "Persisted", Email = "persisted@example.com", Password = "secret1" });

        Assert.NotNull(context.DataService.GetUserByEmail("persisted@example.com"));
    }
}