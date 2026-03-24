using AppointmentSystem.Data;
using AppointmentSystem.Services; // Для нашого сервісу
using AppointmentSystem.Models;   // Для класу Doctor
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Додаємо сервіси до контейнера
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Налаштування бази даних
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=appointments.db"));

// 3. Реєстрація нашого сервісу логіки (Dependency Injection)
builder.Services.AddScoped<BookingService>();

var app = builder.Build();

// --- БЛОК АВТОЗАПОВНЕННЯ (SEED DATA) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!db.Doctors.Any())
    {
        db.Doctors.AddRange(
            new Doctor { Name = "Олександр Майстер", Specialization = "Тату" },
            new Doctor { Name = "Марія Стильна", Specialization = "Косметолог" },
            new Doctor { Name = "Іван Борода", Specialization = "Барбер" }
        );
        db.SaveChanges();
    }
}

// Конфігурація HTTP-запитів
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<AppointmentSystem.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();