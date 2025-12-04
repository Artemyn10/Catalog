public class RecipePhoto
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string? Url { get; set; }

    public Recipe Recipe { get; set; }
}