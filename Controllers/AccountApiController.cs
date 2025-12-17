//using Catalog.Services;
//using Microsoft.AspNetCore.Mvc;
//using Catalog.Models;





//namespace Catalog.Controllers
//{
//    [ApiController]
//    [Route("api/account")]
//    public class AccountApiController : ControllerBase
//    {
//        private readonly AuthService _auth;

//        public AccountApiController(AuthService auth)
//        {
//            _auth = auth;
//        }

//        [HttpPost("register")]
//        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
//        {
//            var (success, error) = await _auth.Register(dto.name, dto.email, dto.password);
//            return Ok(new { success, error });
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginDto dto)
//        {
//            var user = await _auth.Login(dto.email, dto.password);

//            if (user == null)
//                return Ok(new { success = false, error = "Неверный email или пароль" });

//            return Ok(new { success = true });
//        }

//        [HttpGet("logout")]
//        public async Task<IActionResult> Logout()
//        {
//            await _auth.Logout();
//            return Redirect("/");
//        }
//    }


//    public record RegisterDto(string name, string email, string password);
//    public record LoginDto(string email, string password);
//}


using Catalog.Services;
using Microsoft.AspNetCore.Mvc;
using Catalog.Models;  // Для RegisterDto, LoginDto

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountApiController : ControllerBase
    {
        private readonly AuthService _auth;

        public AccountApiController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);  // Автоматическая валидация от [ApiController]

            var (success, error) = await _auth.Register(dto.Name, dto.Email, dto.Password);
            if (!success)
                return BadRequest(new { success = false, error });

            return Ok(new { success = true });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _auth.Login(dto.Email, dto.Password);
            if (user == null)
                return BadRequest(new { success = false, error = "Неверный email или пароль" });

            return Ok(new { success = true });
        }

        [HttpPost("logout")]  // Изменено на POST для безопасности
        public async Task<IActionResult> Logout()
        {
            await _auth.Logout();
            return Ok(new { success = true });
        }
    }

    // DTO (без изменений, но убедитесь в валидации)
    public record RegisterDto(string Name, string Email, string Password);
    public record LoginDto(string Email, string Password);
}