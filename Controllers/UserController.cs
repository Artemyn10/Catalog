using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.Data;  // Namespace для ApplicationDbContext
using Catalog.Models;  // Namespace для User и Recipe
using System.Security.Claims;

namespace Catalog.Controllers
{
    [Authorize]  // Доступ только для авторизованных пользователей
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
                return RedirectToAction("Login", "Account");  // Редирект, если claim отсутствует
            }

            // Загрузка пользователя с рецептами
            var user = await _context.Users
                .Include(u => u.Recipes)  // Загрузка связанных рецептов
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();  // Пользователь не найден
            }

            return View(user);
        }
    }
}