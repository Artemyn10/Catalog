using System.ComponentModel.DataAnnotations;

namespace Catalog.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }  // Рекомендую сделать non-nullable (рецепт всегда имеет автора)

        public int? CategoryId { get; set; }

        public string Title { get; set; } = string.Empty;  // Рекомендую non-nullable для ключевого поля

        public string Description { get; set; } = string.Empty;

        public int CookingTime { get; set; }

        public string Difficulty { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public User User { get; set; } = null!;

        public Category? Category { get; set; }

        public List<RecipeIngredient> Ingredients { get; set; } = new();

        public List<RecipeStep> Steps { get; set; } = new();

        public List<RecipePhoto> Photos { get; set; } = new();

        // Новое: пользователи, добавившие рецепт в избранное
        public List<User> FavoritedByUsers { get; set; } = new();
    }
}