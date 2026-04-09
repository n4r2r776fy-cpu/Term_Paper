using Microsoft.EntityFrameworkCore;
using AppointmentSystem.Data;
using AppointmentSystem.Models;
using AppointmentSystem.Components;
using AppointmentSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Сервіси
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();
builder.Services.AddHttpClient(); 
builder.Services.AddScoped<BookingService>();

// ВИПРАВЛЕНО: додана дужка в кінці UseSqlite
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 2. Middleware
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// 3. База та Майстри
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try 
    {
        context.Database.EnsureCreated();
        
       if (!context.Doctors.Any())
{
    context.Doctors.AddRange(
        new Doctor { 
            Name = "Дмитро", 
            Specialization = "Барбер", 
            Bio = "Мастер небезпечного гоління та ідеальних фейдів." 
        },
        new Doctor { 
            Name = "Анна", 
            Specialization = "Перукар-стиліст", 
            Bio = "Експерт з колористики та сучасних жіночих стрижок." 
        },
        new Doctor { 
            Name = "Олена", 
            Specialization = "Візажист", 
            Bio = "Створює витончені образи для особливих подій." 
        }
    );
    context.SaveChanges();
}
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }
}

app.Run();