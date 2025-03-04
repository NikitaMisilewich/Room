using ChatRoom.Components;
using ChatRoom.Components.DB;
using ChatRoom.Components.Models;
using ChatRoom.Components.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавляем поддержку Razor-компонентов и Blazor Server
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();

// Подключаем Entity Framework Core с SQLite
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=blazorapp.db"));

builder.Services.AddScoped<UserSessionData>();
builder.Services.AddScoped<LocalStorageService>();

// Добавляем AuthService
builder.Services.AddScoped<AuthService>();

// Добавляем кастомный AuthenticationStateProvider
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Добавляем поддержку авторизации
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<MessageService>();

// Подключаем SignalR
builder.Services.AddSignalR();

var app = builder.Build();

app.UseRouting();

// Настройки обработки ошибок
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        db.Users.Add(new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"), Role = "Admin" });
        db.Users.Add(new User { Username = "student", PasswordHash = BCrypt.Net.BCrypt.HashPassword("student"), Role = "User" });
        db.SaveChanges();
    }
}

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub(); // Подключение Blazor
    endpoints.MapHub<ChatHub>("/chatHub"); // Подключение SignalR
    endpoints.MapRazorPages(); // Подключение Razor Pages
});

// ВОЗВРАЩАЕМ AddInteractiveServerRenderMode()
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
