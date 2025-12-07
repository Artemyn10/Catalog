using System.ComponentModel.DataAnnotations.Schema;
namespace Catalog.Models
{
    public class RecipeIngredient
    {
        public int Id { get; set; }

        [ForeignKey("Recipe")]  // ✅ Явно указываем, что это внешний ключ к свойству Recipe
        public int RecipeId { get; set; }
        public string? IngredientName { get; set; }
        public string? Amount { get; set; }

        public Recipe Recipe { get; set; }
    }
}