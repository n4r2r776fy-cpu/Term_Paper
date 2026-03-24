using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using AppointmentSystem.Data;
using AppointmentSystem.Models;
using AppointmentSystem.Components;

var builder = WebApplication.CreateBuilder(args);

// 1. Blazor & Controllers
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();
builder.Services.AddHttpClient(); 

// 2. Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Auth
var key = Encoding.ASCII.GetBytes("SuperSecretKey_DoNotShare_123456789");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 4. Pipeline
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// 5. Safe Seeding (БЕЗПЕЧНИЙ ЗАПУСК)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try 
    {
        // 1. Створюємо базу
        db.Database.EnsureCreated();
        
        // 2. Даємо SQLite "видихнути" (актуально для повільних дисків)
        Thread.Sleep(1000); 

        // 3. Перевіряємо користувачів через Try-Catch
        try 
        {
            if (!db.Users.Any())
            {
                db.Users.Add(new User 
                { 
                    Username = "admin", 
                    PasswordHash = "admin", 
                    Role = "Admin" 
                });
                db.SaveChanges();
                Console.WriteLine("[Система]: Адміна створено успішно.");
            }
        }
        catch (Exception)
        {
            Console.WriteLine("[Система]: Повторна спроба доступу до таблиць...");
            // Якщо не вийшло з першого разу, пробуємо ще раз через секунду
            Thread.Sleep(1000);
            if (!db.Users.Any()) 
            {
                db.Users.Add(new User { Username = "admin", PasswordHash = "admin", Role = "Admin" });
                db.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Критична помилка бази]: {ex.Message}");
    }
}

app.Run();