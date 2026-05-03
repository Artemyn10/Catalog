namespace Catalog.Models
{
    public class EditProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;  // Для проверки
        public string NewPassword { get; set; } = string.Empty;      // Опционально
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}