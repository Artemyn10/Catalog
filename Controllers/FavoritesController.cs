using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.Data; // Для ApplicationDbContext
using System.Security.Claims;

namespace Catalog.Controllers
{
    [Authorize] // Доступ только для авторизованных пользователей
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var favoriteRecipes = await _context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.FavoriteRecipes)
                .Include(r => r.User)  // Автор рецепта
                .Include(r => r.Category)  // Категория
                .Include(r => r.Photos)  // Фото (если есть)
                .ToListAsync();

            return View(favoriteRecipes);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int recipeId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(); // Более подходящий ответ, если claim отсутствует
            }

            var user = await _context.Users
                .Include(u => u.FavoriteRecipes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe == null) return NotFound();

            if (user.FavoriteRecipes.Any(r => r.Id == recipeId))
            {
                user.FavoriteRecipes.Remove(recipe);
                TempData["Message"] = "Рецепт удалён из избранного";
            }
            else
            {
                user.FavoriteRecipes.Add(recipe);
                TempData["Message"] = "Рецепт добавлен в избранное";
            }

            await _context.SaveChangesAsync();

            // Явный редирект на Details в RecipesController
            return RedirectToAction("Details", "Recipes", new { id = recipeId });
        }
    }
}