using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoritesAndAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "RecipePhotos");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "IngredientName",
                table: "RecipeIngredients");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "RecipePhotos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RecipeIngredients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Quantity",
                table: "RecipeIngredients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "RecipeIngredients",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "RecipePhotos");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "RecipeIngredients");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "RecipePhotos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Amount",
                table: "RecipeIngredients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IngredientName",
                table: "RecipeIngredients",
                type: "text",
                nullable: true);
        }
    }
}
