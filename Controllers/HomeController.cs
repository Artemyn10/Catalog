using Microsoft.AspNetCore.Mvc;

namespace RecipeCatalog.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }
        public IActionResult Services()
        {
            return View();
        }
        public IActionResult Recipes()
        {
            return View();
        }

    }
}
