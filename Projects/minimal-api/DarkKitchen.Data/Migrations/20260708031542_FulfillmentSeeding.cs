using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DarkKitchen.Data.Migrations
{
    /// <inheritdoc />
    public partial class FulfillmentSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "FulfillmentEvents");

            migrationBuilder.AddColumn<int>(
                name: "Result",
                table: "FulfillmentEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "FulfillmentEvents",
                columns: new[] { "Id", "FulfilledAtUtc", "OrderId", "Result" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 7, 20, 15, 11, 0, DateTimeKind.Utc), 1, 0 },
                    { 2, new DateTime(2026, 7, 7, 20, 33, 25, 0, DateTimeKind.Utc), 2, 1 },
                    { 3, new DateTime(2026, 7, 7, 21, 11, 57, 0, DateTimeKind.Utc), 3, 0 }
                });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CompletedUtc",
                value: new DateTime(2026, 7, 7, 20, 15, 7, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "CompletedUtc",
                value: new DateTime(2026, 7, 7, 20, 33, 21, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "CompletedUtc",
                value: new DateTime(2026, 7, 7, 21, 11, 35, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FulfillmentEvents",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FulfillmentEvents",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FulfillmentEvents",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Result",
                table: "FulfillmentEvents");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "FulfillmentEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CompletedUtc",
                value: null);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "CompletedUtc",
                value: null);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "CompletedUtc",
                value: null);
        }
    }
}
