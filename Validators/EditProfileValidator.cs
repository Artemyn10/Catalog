using FluentValidation;
using Catalog.Models;

namespace Catalog.Validators
{
    public class EditProfileValidator : AbstractValidator<EditProfileDto>
    {
        public EditProfileValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательно")
                .Length(2, 50).WithMessage("Имя должно быть от 2 до 50 символов");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный формат email");

            When(x => !string.IsNullOrEmpty(x.NewPassword), () =>
            {
                RuleFor(x => x.CurrentPassword)
                    .NotEmpty().WithMessage("Текущий пароль обязателен для изменения");

                RuleFor(x => x.NewPassword)
                    .MinimumLength(6).WithMessage("Новый пароль не менее 6 символов")
                    .Matches(@"[A-Z]").WithMessage("Пароль должен содержать заглавную букву")
                    .Matches(@"[a-z]").WithMessage("Пароль должен содержать строчную букву")
                    .Matches(@"[0-9]").WithMessage("Пароль должен содержать цифру");

                RuleFor(x => x.ConfirmNewPassword)
                    .Equal(x => x.NewPassword).WithMessage("Пароли не совпадают");
            });
        }
    }
}