public class Recipe
{
    public int Id { get; set; }
    public int? UserId { get; set; }        // Может быть NULL
    public int? CategoryId { get; set; }    // Может быть NULL
    public string? Title { get; set; }      // Может быть NULL
    public string? Description { get; set; } // Может быть NULL
    public int? CookingTime { get; set; }   // Может быть NULL
    public string? Difficulty { get; set; } // Может быть NULL
    public DateTime? CreatedAt { get; set; } // Может быть NULL, будет CURRENT_TIMESTAMP по умолчанию

    // Навигационные свойства
    public User? User { get; set; }
    public Category? Category { get; set; }
    public List<RecipeIngredient> Ingredients { get; set; } = new();
    public List<RecipeStep> Steps { get; set; } = new();
    public List<RecipePhoto> Photos { get; set; } = new();
}