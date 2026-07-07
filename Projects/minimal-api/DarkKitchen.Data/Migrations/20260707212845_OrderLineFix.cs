using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkKitchen.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderLineFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_DishId",
                table: "OrderLines",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Dishes_DishId",
                table: "OrderLines",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Dishes_DishId",
                table: "OrderLines");

            migrationBuilder.DropIndex(
                name: "IX_OrderLines_DishId",
                table: "OrderLines");
        }
    }
}
