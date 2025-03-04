using ChatRoom.Components;
using ChatRoom.Components.DB;
using ChatRoom.Components.Models;
using ChatRoom.Components.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ��������� ��������� Razor-����������� � Blazor Server
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();

// ���������� Entity Framework Core � SQLite
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=blazorapp.db"));

builder.Services.AddScoped<UserSessionData>();
builder.Services.AddScoped<LocalStorageService>();

// ��������� AuthService
builder.Services.AddScoped<AuthService>();

// ��������� ��������� AuthenticationStateProvider
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// ��������� ��������� �����������
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<MessageService>();

// ���������� SignalR
builder.Services.AddSignalR();

var app = builder.Build();

app.UseRouting();

// ��������� ��������� ������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// ������������� ���� ������
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
    endpoints.MapBlazorHub(); // ����������� Blazor
    endpoints.MapHub<ChatHub>("/chatHub"); // ����������� SignalR
    endpoints.MapRazorPages(); // ����������� Razor Pages
});

// ���������� AddInteractiveServerRenderMode()
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
