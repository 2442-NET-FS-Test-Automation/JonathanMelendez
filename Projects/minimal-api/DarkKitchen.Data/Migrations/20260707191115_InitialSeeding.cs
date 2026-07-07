using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DarkKitchen.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Dishes_DishId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_DishId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "Ingredients");

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OriginCountry",
                table: "Dishes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DishIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DishId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishIngredients_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email", "Name" },
                values: new object[,]
                {
                    { 1, "jonathan@example.com", "Jonathan" },
                    { 2, "paula@example.com", "Paula" },
                    { 3, "takis@example.com", "Takis" },
                    { 4, "lemon@example.com", "Lemon" },
                    { 5, "bhy@example.com", "Bahji" }
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Description", "Enabled", "Name", "OriginCountry", "Price" },
                values: new object[,]
                {
                    { 1, "Traditional Latin American custard dessert popular in countries like Peru, Chile, and Colombia. Similar to flan or crème brûlée.", true, "Leche Asada", "Chile", 0m },
                    { 2, "Round, deep-fried cheese fritters that are crispy on the outside and soft and airy on the inside.", true, "Colombian Buñuelos", "Colombia", 0m },
                    { 3, "Traditional, creamy rice pudding", true, "Arroz con leche", "Spain", 0m },
                    { 4, "Classic Latin American pastry made by folding dough into a half-moon shape, filling it with melty cheese, and deep-frying it until the exterior is crispy and the inside is gooey.", true, "Fried cheese empanadas", "Mexico", 0m },
                    { 5, "Iconic, ultra-rich dessert renowned for its dense, velvety texture.", true, "New York cheesecake", "United States", 0m },
                    { 6, "Rich, comforting pasta dish consisting of long, flat fettuccine noodles coated in a velvety, indulgent sauce.", true, "Fettucine alfredo", "Italy", 0m },
                    { 7, "Savory Latin American casserole. It features a base of seasoned ground beef (pino) layered with hard-boiled eggs, black olives, and raisins.", true, "Potato Pie", "Chile", 0m },
                    { 8, "Traditional Dutch bite-sized snacks consisting of a rich, thick meat ragout enveloped in a crispy, deep-fried breadcrumb crust.", true, "Bitterballen", "Netherlands", 0m },
                    { 9, "", true, "Fatteh", "Egypt", 0m },
                    { 10, "Southern-style, pressure-fried chicken. Each piece is coated in a proprietary blend of 11 herbs and spices, resulting in a signature golden-brown, crispy exterior that shatters upon the first bite, giving way to piping-hot, exceptionally tender and juicy meat.", true, "Kentucky Fried Chicken", "United States", 0m }
                });

            migrationBuilder.InsertData(
                table: "Ingredients",
                columns: new[] { "Id", "Name", "Stock", "Unit" },
                values: new object[,]
                {
                    { 1, "White Rice", 5m, 2 },
                    { 2, "Water", 20m, 3 },
                    { 3, "Salt", 1m, 2 },
                    { 4, "Milk", 5m, 3 },
                    { 5, "Butter", 1m, 2 },
                    { 6, "Sugar", 2m, 2 },
                    { 7, "Condensed Milk", 50m, 0 },
                    { 8, "Egg", 50m, 4 },
                    { 9, "Flour", 5m, 2 },
                    { 10, "Chicken", 5m, 2 },
                    { 11, "Cheese", 8m, 2 },
                    { 12, "Potato", 6m, 2 },
                    { 13, "Tomato", 15m, 4 },
                    { 14, "Ground Beef", 3m, 2 },
                    { 15, "Onion", 10m, 4 },
                    { 16, "Cream", 5m, 2 },
                    { 17, "Pasta", 3m, 2 }
                });

            migrationBuilder.InsertData(
                table: "DishIngredients",
                columns: new[] { "Id", "DishId", "IngredientId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 4, 0.25m },
                    { 2, 1, 8, 4m },
                    { 3, 1, 6, 0.2m },
                    { 4, 2, 8, 1m },
                    { 5, 2, 11, 0.2m },
                    { 6, 2, 6, 0.1m },
                    { 7, 2, 3, 0.002m },
                    { 8, 2, 4, 0.1m },
                    { 9, 3, 1, 0.2m },
                    { 10, 3, 2, 0.4m },
                    { 11, 3, 4, 0.8m },
                    { 12, 3, 3, 0.001m },
                    { 13, 3, 5, 0.05m },
                    { 14, 3, 6, 0.2m },
                    { 15, 3, 7, 12m },
                    { 16, 4, 9, 0.6m },
                    { 17, 4, 3, 0.05m },
                    { 18, 4, 4, 0.02m },
                    { 19, 4, 8, 1m },
                    { 20, 4, 11, 0.4m },
                    { 21, 5, 5, 0.1m },
                    { 22, 5, 6, 0.4m },
                    { 23, 5, 11, 1m },
                    { 24, 5, 9, 0.1m },
                    { 25, 5, 8, 3m },
                    { 26, 6, 16, 0.3m },
                    { 27, 6, 5, 0.1m },
                    { 28, 6, 9, 0.01m },
                    { 29, 6, 11, 0.1m },
                    { 30, 6, 17, 0.25m },
                    { 31, 7, 12, 1m },
                    { 32, 7, 5, 0.1m },
                    { 33, 7, 3, 0.02m },
                    { 34, 7, 8, 2m },
                    { 35, 7, 15, 1m },
                    { 36, 7, 13, 3m },
                    { 37, 7, 14, 0.3m },
                    { 38, 8, 5, 0.1m },
                    { 39, 8, 9, 0.2m },
                    { 40, 8, 15, 0.5m },
                    { 41, 8, 14, 0.5m },
                    { 42, 8, 3, 0.03m },
                    { 43, 8, 8, 2m },
                    { 44, 9, 14, 0.3m },
                    { 45, 9, 15, 1m },
                    { 46, 9, 13, 2m },
                    { 47, 9, 1, 0.5m },
                    { 48, 9, 5, 0.1m },
                    { 49, 9, 3, 0.01m },
                    { 50, 10, 10, 2m },
                    { 51, 10, 8, 1m },
                    { 52, 10, 9, 0.5m },
                    { 53, 10, 6, 0.01m },
                    { 54, 10, 3, 0.01m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishIngredients_DishId",
                table: "DishIngredients",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishIngredients_IngredientId",
                table: "DishIngredients",
                column: "IngredientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishIngredients");

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "OriginCountry",
                table: "Dishes");

            migrationBuilder.AddColumn<int>(
                name: "DishId",
                table: "Ingredients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_DishId",
                table: "Ingredients",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Dishes_DishId",
                table: "Ingredients",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id");
        }
    }
}
