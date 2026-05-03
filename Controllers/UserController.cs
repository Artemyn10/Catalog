using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.Data; // Namespace для ApplicationDbContext
using Catalog.Models; // Namespace для User и Recipe
using System.Security.Claims;

namespace Catalog.Controllers
{
    [Authorize] // Доступ только для авторизованных пользователей
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Profile()
        {
            // Получение ID текущего пользователя из claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("Login", "Account"); // Редирект, если claim отсутствует
            }

            // Загрузка пользователя с рецептами
            var user = await _context.Users
                .Include(u => u.Recipes) // Загрузка связанных рецептов
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(); // Пользователь не найден
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var dto = new EditProfileDto
            {
                Name = user.Username,
                Email = user.Email
            };

            ViewBag.CurrentAvatar = user.AvatarUrl  ;

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileDto dto, IFormFile? avatar)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.CurrentAvatar = user.AvatarUrl;
                return View(dto);
            }

            // Проверка текущего пароля, если меняется новый
            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                if (string.IsNullOrEmpty(dto.CurrentPassword) || !BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Текущий пароль неверный или не указан");
                    ViewBag.CurrentAvatar = user.AvatarUrl ?? "/images/default-avatar.png";
                    return View(dto);
                }
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            }

            // Проверка уникальности email (если изменился)
            if (dto.Email != user.Email && await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                ModelState.AddModelError("", "Этот email уже зарегистрирован");
                ViewBag.CurrentAvatar = user.AvatarUrl;
                return View(dto);
            }

            // Обработка аватара
            if (avatar != null && avatar.Length > 0)
            {
                if (avatar.Length > 2 * 1024 * 1024) // > 2 МБ
                {
                    ModelState.AddModelError("", "Файл слишком большой (максимум 2 МБ)");
                    ViewBag.CurrentAvatar = user.AvatarUrl ;
                    return View(dto);
                }

                var allowedTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedTypes.Contains(avatar.ContentType))
                {
                    ModelState.AddModelError("", "Только JPG или PNG");
                    ViewBag.CurrentAvatar = user.AvatarUrl;
                    return View(dto);
                }

                // Создание папки avatars, если не существует
                var avatarsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars");
                if (!Directory.Exists(avatarsPath))
                {
                    Directory.CreateDirectory(avatarsPath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(avatar.FileName);
                var filePath = Path.Combine(avatarsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(stream);
                }

                user.AvatarUrl = "/images/avatars/" + fileName;  // Обновляем только при успешной загрузке
            }
            // Если аватар не загружен — AvatarUrl остаётся прежним (или default из модели)

            // Обновление основных полей
            user.Username = dto.Name;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Профиль успешно обновлён";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(EditProfileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(dto.CurrentPassword) || string.IsNullOrEmpty(dto.NewPassword) || string.IsNullOrEmpty(dto.ConfirmNewPassword))
            {
                ModelState.AddModelError("", "Все поля пароля обязательны");
                ViewBag.CurrentAvatar = user.AvatarUrl;
                return View("Edit", new EditProfileDto { Name = user.Username, Email = user.Email });
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError("", "Текущий пароль неверный");
                ViewBag.CurrentAvatar = user.AvatarUrl;
                return View("Edit", new EditProfileDto { Name = user.Username, Email = user.Email });
            }

            if (dto.NewPassword != dto.ConfirmNewPassword)
            {
                ModelState.AddModelError("", "Новый пароль и подтверждение не совпадают");
                ViewBag.CurrentAvatar = user.AvatarUrl;
                return View("Edit", new EditProfileDto { Name = user.Username, Email = user.Email });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            TempData["SuccessPassword"] = "Пароль успешно изменён";
            return RedirectToAction("Edit");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAvatar()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.AvatarUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);  // Удаляем файл с диска
                }
                user.AvatarUrl = null;
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Аватар удалён (вернулся дефолт)";
            return RedirectToAction("Edit");
        }
    }
}
