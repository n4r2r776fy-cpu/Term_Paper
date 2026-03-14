using Microsoft.AspNetCore.Hosting;
using MyBlazorApp.Services;

namespace MyBlazorApp.Tests.TestInfrastructure;

internal sealed class TestDataServiceContext : IDisposable
{
    private readonly string _rootPath;

    public TestDataServiceContext()
    {
        _rootPath = Path.Combine(Path.GetTempPath(), "MyBlazorApp.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_rootPath);

        Environment = new TestWebHostEnvironment
        {
            ContentRootPath = _rootPath,
            WebRootPath = _rootPath
        };

        DataService = new DataService(Environment);
    }

    public IWebHostEnvironment Environment { get; }

    public DataService DataService { get; }

    public string AppDataPath => Path.Combine(_rootPath, "App_Data");

    public string AppointmentsFilePath => Path.Combine(AppDataPath, "appointments.json");

    public string UsersFilePath => Path.Combine(AppDataPath, "users.json");

    public void Dispose()
    {
        if (Directory.Exists(_rootPath))
        {
            Directory.Delete(_rootPath, recursive: true);
        }
    }
}