using Microsoft.AspNetCore.Mvc;

[Route("api/account")]
public class AccountApiController : Controller
{
    // временное хранилище пользователей
    private static List<UserDto> Users = new List<UserDto>();

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            return BadRequest(new { error = "Заполните все поля." });

        //  проверка длины пароля
        if (model.Password.Length < 6)
            return BadRequest(new { error = "Пароль должен быть не меньше 6 символов." });

        if (Users.Any(u => u.Email == model.Email))
            return BadRequest(new { error = "Такой Email уже зарегистрирован." });

        Users.Add(new UserDto
        {
            Email = model.Email,
            Password = model.Password
        });

        return Ok(new { success = true });
    }


    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto model)
    {
        var user = Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

        if (user == null)
            return BadRequest(new { error = "Неверный Email или пароль" });

        return Ok(new { success = true });
    }
}

public class RegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
