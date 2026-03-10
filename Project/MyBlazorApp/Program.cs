using MyBlazorApp.Components;
using Domain_Entities;
using Repositories;
using Notification_System;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



// 1. Реєструємо репозиторії. 
// AddSingleton означає, що створиться один список для всієї програми (дані не зникнуть при оновленні сторінки)
builder.Services.AddSingleton<IRepository<Client>, ClientRepository>();
builder.Services.AddSingleton<IRepository<Appointment>, AppointmentRepository>();

// 2. Обираємо спосіб сповіщення (Поліморфізм у дії!)
// Якщо захочете змінити Email на SMS, просто поміняйте EmailNotification на SmsNotification тут.
builder.Services.AddSingleton<INotification, EmailNotification>();

// 3. Реєструємо менеджер сповіщень та сервіси бізнес-логіки
builder.Services.AddSingleton<ISubject, NotificationManager>();
builder.Services.AddSingleton<AppointmentService>();
builder.Services.AddSingleton<AuthService>();


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
