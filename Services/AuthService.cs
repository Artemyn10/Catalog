using Catalog.Data;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;

namespace Catalog.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _http;
        private readonly IEmailService _emailService;
        public AuthService(ApplicationDbContext db, IHttpContextAccessor http, IEmailService emailService)
        {
            _db = db;
            _http = http;
            _emailService = emailService;
        }

        public async Task<(bool success, string? error)> Register(string username, string email, string password)
        {
            if (await _db.Users.AnyAsync(u => u.Email == email))
                return (false, "Email уже зарегистрирован");

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false,  // Новый пользователь не подтверждён
                ConfirmationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),  // Генерация токена
                ConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24)  // Действителен 24 часа
            };

            // Первый зарегистрированный — автоматически админ
            if (!await _db.Users.AnyAsync())
                user.Role = "Admin";

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Отправка email с токеном (инъекция IEmailService в конструктор)
            await _emailService.SendConfirmationEmailAsync(email, username, user.ConfirmationToken);

            return (true, null);
        }

        public async Task<User?> Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Проверка существования, пароля и подтверждения email
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) || !user.EmailConfirmed)
                return null;

            // Создание ClaimsIdentity
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username ?? user.Email),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _http.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return user;
        }

        public async Task Logout()
        {
            await _http.HttpContext.SignOutAsync();
        }
    }
}
