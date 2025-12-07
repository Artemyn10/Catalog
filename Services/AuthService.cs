using BCrypt.Net;
using Catalog.Data;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _db;

        public AuthService(ApplicationDbContext db)
        {
            _db = db;
        }

        // === РЕГИСТРАЦИЯ ===
        public async Task<(bool Success, string Error)> Register(string name, string email, string password)
        {
            if (await _db.Users.AnyAsync(u => u.Email == email))
                return (false, "Пользователь с таким Email уже существует");

            var user = new User
            {
                Username = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return (true, "");
        }

        // === ЛОГИН ===
        public async Task<User?> Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            bool ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return ok ? user : null;
        }
    }
}
