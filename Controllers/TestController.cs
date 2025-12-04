using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class TestController : Controller
{
    private readonly ApplicationDbContext _context;

    public TestController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var recipesCount = await _context.Recipes.CountAsync();
            var usersCount = await _context.Users.CountAsync();

            return Content($"✅ БД подключена! Рецептов: {recipesCount}, Пользователей: {usersCount}");
        }
        catch (Exception ex)
        {
            return Content($"❌ Ошибка: {ex.Message}");
        }
    }
}   