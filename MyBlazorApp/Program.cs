using MyBlazorApp.Components;
using MyBlazorApp.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

ConfigureServerUrls(builder);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<DataService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<IAuthStateStore, BrowserAuthStateStore>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static void ConfigureServerUrls(WebApplicationBuilder builder)
{
    var configuredUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
        ?? Environment.GetEnvironmentVariable("DOTNET_URLS");

    if (string.IsNullOrWhiteSpace(configuredUrls))
    {
        return;
    }

    var resolvedUrls = configuredUrls
        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(ResolveUrl)
        .ToArray();

    builder.WebHost.UseUrls(resolvedUrls);
}

static string ResolveUrl(string url)
{
    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
    {
        return url;
    }

    if (!IsLocalHost(uri.Host) || uri.Port <= 0 || IsPortAvailable(uri.Port))
    {
        return url;
    }

    var freePort = FindFreePort(uri.Port + 1);
    var builder = new UriBuilder(uri)
    {
        Port = freePort
    };

    Console.WriteLine($"Port {uri.Port} is already in use. Switching to {builder.Uri}.");
    return builder.Uri.ToString().TrimEnd('/');
}

static bool IsLocalHost(string host) =>
    string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
    || string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase)
    || string.Equals(host, "::1", StringComparison.OrdinalIgnoreCase);

static bool IsPortAvailable(int port)
{
    try
    {
        using var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();
        return true;
    }
    catch (SocketException)
    {
        return false;
    }
}

static int FindFreePort(int startingPort)
{
    for (var port = startingPort; port <= 65535; port++)
    {
        if (IsPortAvailable(port))
        {
            return port;
        }
    }

    throw new InvalidOperationException("No free localhost ports are available.");
}
