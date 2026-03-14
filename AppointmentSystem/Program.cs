using AppointmentSystem.Models;
using AppointmentSystem.Data;
using AppointmentSystem.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
.AddInteractiveServerComponents().AddInteractiveServerComponents();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=appointments.db"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<AppointmentSystem.Components.App>()
.AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!db.Doctors.Any()) // Якщо в базі ще немає жодного майстра
    {
        db.Doctors.AddRange(
            new Doctor { Name = "Олександр", Specialization = "Стрижки та бороди" },
            new Doctor { Name = "Марія", Specialization = "Манікюр" },
            new Doctor { Name = "Дмитро", Specialization = "Тату-майстер" }
        );
        db.SaveChanges();
    }
}
app.Run();