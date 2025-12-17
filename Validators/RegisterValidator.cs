using FluentValidation;
using Catalog.Models;
using Catalog.Controllers;  // Namespace для RegisterDto

namespace Catalog.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательно")
                .Length(2, 50).WithMessage("Имя должно быть от 2 до 50 символов");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный формат email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Пароль не менее 6 символов")
                .Matches(@"[A-Z]").WithMessage("Пароль должен содержать заглавную букву")
                .Matches(@"[a-z]").WithMessage("Пароль должен содержать строчную букву")
                .Matches(@"[0-9]").WithMessage("Пароль должен содержать цифру");
        }
    }
}