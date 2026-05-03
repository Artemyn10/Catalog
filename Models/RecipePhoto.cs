namespace Catalog.Models
{
    public class RecipePhoto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}