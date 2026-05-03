using System.ComponentModel.DataAnnotations;

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
        public List<Recipe> FavoriteRecipes { get; set; } = new();
        public bool EmailConfirmed { get; set; } = false;
        public string? ConfirmationToken { get; set; }
        public DateTime? ConfirmationTokenExpiry { get; set; }

        // Новое поле для аватара
        public string? AvatarUrl { get; set; } = null;
    }
}