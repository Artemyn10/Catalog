using System.ComponentModel.DataAnnotations;  // Для [Key] и [Required]

namespace Catalog.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Role { get; set; } = "User";

        public List<Recipe> Recipes { get; set; } = new();

        // Новые поля для подтверждения email
        public bool EmailConfirmed { get; set; } = false;  // Статус подтверждения

        public string? ConfirmationToken { get; set; }  // Одноразовый токен

        public DateTime? ConfirmationTokenExpiry { get; set; }  // Срок действия токена (24 часа)
    }
}