namespace Catalog.Models
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public string Quantity { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}