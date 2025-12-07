using Microsoft.AspNetCore.Mvc;

//public class AccountController : Controller
//{
//    // GET: /Account/Login
//    public IActionResult Login()
//    {
//        return View();
//    }

//    // GET: /Account/Register
//    public IActionResult Register()
//    {
//        return View();
//    }
//}
public class AccountController : Controller
{
    public IActionResult Login() => View();
    public IActionResult Register() => View();

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}


