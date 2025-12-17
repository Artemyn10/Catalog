using Catalog.Data;
using Catalog.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using FluentValidation.AspNetCore;
using FluentValidation;
using Catalog.Validators;

namespace Catalog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DB (без изменений)
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Cookie Auth (уточнены опции для безопасности и срока жизни)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/Denied";
                    options.LogoutPath = "/Account/Logout";  // Добавлено для явного логаута
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Срок жизни куки
                    options.SlidingExpiration = true;  // Продление при активности
                    options.Cookie.HttpOnly = true;  // Защита от XSS
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;  // HTTPS в продакшене
                });

            builder.Services.AddAuthorization();  // Поддержка ролей (без изменений)

            builder.Services.AddControllersWithViews();  // Без изменений

            builder.Services.AddHttpContextAccessor();  // Для AuthService (без изменений)

            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>(); // Без изменений
            builder.Services.AddFluentValidationAutoValidation();  // Автоматическая валидация в контроллерах
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();  // Сканирование валидаторов в сборке

            

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Middleware для аутентификации (порядок сохранён, без UseSession)
            app.UseAuthentication();
            app.UseAuthorization();

           
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}