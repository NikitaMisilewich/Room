using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using ChatRoom.Components.DB;
using ChatRoom.Components.Models;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly CustomAuthStateProvider _authStateProvider; // Используем кастомный AuthenticationStateProvider

    public event Action OnUserAuthenticated;

    public AuthService(AppDbContext context, CustomAuthStateProvider authStateProvider)
    {
        _context = context;
        _authStateProvider = authStateProvider; // Инициализация
    }

    public async Task<bool> Register(string Fio, string username, string password, string Group)
    {
        try
        {
            // Проверка на существование пользователя с таким же логином
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                Console.WriteLine("Пользователь с таким логином уже существует.");
                return false;
            }

            // Проверка пароля на минимальные требования (например, длина)
            if (password.Length < 6)
            {
                Console.WriteLine("Пароль должен быть не менее 6 символов.");
                return false;
            }

            // Хеширование пароля
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Создание нового пользователя
            var user = new User
            {
                Fio = Fio,
                Username = username,
                Group = Group,
                PasswordHash = hashedPassword,
                Role = "Студент"
            };

            // Добавление пользователя в базу данных
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Пользователь {username} успешно зарегистрирован.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при регистрации: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> Authenticate(string username, string password)
    {
        try
        {
            Console.WriteLine(username);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Console.WriteLine("Неверный логин или пароль.");
                return false;
            }

            Console.WriteLine($"Аутентификация успешна для {user.Username}");

            // Создание claims и обновление AuthenticationStateProvider

            // Уведомляем компонент, чтобы он обновил UI
            OnUserAuthenticated?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при аутентификации: {ex.Message}");
            return false;
        }
    }

    // Метод для получения роли пользователя
    public async Task<string> GetUserRole(string username)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user?.Role ?? "Не найдено";  // Возвращаем роль или сообщение об ошибке
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении роли пользователя: {ex.Message}");
            return "Ошибка";
        }
    }

    // Метод для получения полной информации о пользователе
    public async Task<User> GetUserInfo(string username)
    {
        try
        {
            // Убедитесь, что запрос корректно ищет по логину
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении информации о пользователе: {ex.Message}");
            return null;
        }
    }
}

