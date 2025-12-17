using Microsoft.AspNetCore.Mvc;

//public class AccountController : Controller
//{
//    public IActionResult Login() => View();
//    public IActionResult Register() => View();

//    public IActionResult Denied() => View();
//}


using Catalog.Services;
using Catalog.Models;
using Catalog.Data;
using Microsoft.EntityFrameworkCore;  // Для RegisterDto, LoginDto

namespace Catalog.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _auth;
        private readonly ApplicationDbContext _db;  // Для прямого доступа к БД

        public AccountController(AuthService auth, ApplicationDbContext db)
        {
            _auth = auth;
            _db = db;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var user = await _auth.Login(dto.Email, dto.Password);
            if (user == null)
            {
                // Проверяем причину (опционально, для детализации)
                var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (dbUser != null && !dbUser.EmailConfirmed)
                {
                    ModelState.AddModelError("", "Подтвердите email перед входом. Проверьте почту или запросите повторную отправку.");
                }
                else
                {
                    ModelState.AddModelError("", "Неверный email или пароль");
                }
                return View(dto);
            }

            TempData["Success"] = "Вход выполнен успешно!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // FluentValidation автоматически заполнит ModelState
            if (!ModelState.IsValid) return View(dto);

            var (success, error) = await _auth.Register(dto.Name, dto.Email, dto.Password);
            if (!success)
            {
                ModelState.AddModelError("", error ?? "Ошибка регистрации");
                return View(dto);
            }

            TempData["Success"] = "Регистрация прошла успешно! Теперь войдите в систему.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _auth.Logout();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Denied()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.ConfirmationToken == token && u.ConfirmationTokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                TempData["Error"] = "Неверный или истёкший токен подтверждения";
                return RedirectToAction("Login");
            }

            user.EmailConfirmed = true;
            user.ConfirmationToken = null;  // Очистка токена после использования
            user.ConfirmationTokenExpiry = null;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Email подтверждён успешно! Теперь вы можете войти.";
            return RedirectToAction("Login");
        }
    }
}


