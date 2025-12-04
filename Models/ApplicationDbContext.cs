using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<RecipeStep> RecipeSteps { get; set; }
    public DbSet<RecipePhoto> RecipePhotos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка имен таблиц
        //modelBuilder.Entity<User>().ToTable("users");
        //modelBuilder.Entity<Category>().ToTable("categories");
        //modelBuilder.Entity<Recipe>().ToTable("recipes");
        //modelBuilder.Entity<RecipeIngredient>().ToTable("recipe_ingredients");
        //modelBuilder.Entity<RecipeStep>().ToTable("recipe_steps");
        //modelBuilder.Entity<RecipePhoto>().ToTable("recipe_photos");

        // Настройка значений по умолчанию
        modelBuilder.Entity<Recipe>()   
            .Property(r => r.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Остальные настройки...
    }
}