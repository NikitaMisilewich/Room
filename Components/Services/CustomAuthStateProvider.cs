using ChatRoom.Components.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly UserSessionData _userSessionService;

    public CustomAuthStateProvider(UserSessionData userSessionService)
    {
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
    }

    // Этот метод вызывается для получения состояния аутентификации
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Получаем данные пользователя из UserSessionService
        var username = _userSessionService.Username;
        var role = _userSessionService.Role;

        // Если данных нет, возвращаем анонимного пользователя
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(user);
        }

        // Если данные есть, создаем ClaimsPrincipal с полученными значениями
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "custom");
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationState(principal);
    }

    // Метод для обновления состояния аутентификации
    public void MarkUserAsAuthenticated(string username, string role)
    {
        _userSessionService.Username = username;
        _userSessionService.Role = role;

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "apiauth_type");
        var principal = new ClaimsPrincipal(identity);

        // Уведомляем систему об изменении состояния аутентификации
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public void MarkUserAsLoggedOut()
    {
        _userSessionService.Username = string.Empty;
        _userSessionService.Role = string.Empty;

        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }
}
