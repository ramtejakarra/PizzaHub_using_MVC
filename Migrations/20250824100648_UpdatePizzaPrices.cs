using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePizzaPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Qunatity",
                table: "CartItems",
                newName: "Quantity");

            migrationBuilder.UpdateData(
                table: "Pizzas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageFileName", "Price" },
                values: new object[] { "margherita.jpg", 399.00m });

            migrationBuilder.UpdateData(
                table: "Pizzas",
                keyColumn: "Id",
                keyValue: 2,
                column: "Price",
                value: 297.00m);

            migrationBuilder.UpdateData(
                table: "Pizzas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Price" },
                values: new object[] { "A colorful mix of fresh veggies including bell peppers, olives, onions, and mushrooms", 258.00m });

            migrationBuilder.UpdateData(
                table: "Pizzas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Price" },
                values: new object[] { "A tropical treat with ham and pineapple", 366.00m });

            migrationBuilder.UpdateData(
                table: "Pizzas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Price" },
                values: new object[] { "Loaded with pepperoni, sausage, ham, and bacon for true meat lovers", 397.00m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "CartItems",
                newName: "Qunatity");

            migrationBuilder.UpdateData(
                table: "Pizzas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageFileName", "Price" },
                values: new object[] { "margherima.jpg", 5.99m });

            migrationBuilder.UpdateData(
                table: "Pizzas",
                keyColumn: "Id",
                keyValue: 2,
                column: "Price",
                value: 7.99m);

            migrationBuilder.UpdateData(
                table: "Pizzas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Price" },
                values: new object[] { "Grilled chicken, BBQ sauce, onions, and cheese", 8.99m });

            migrationBuilder.UpdateData(
                table: "Pizzas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Price" },
                values: new object[] { "A colorful mix of fresh veggies including bell peppers, olives, onions, and mushrooms", 6.99m });

            migrationBuilder.UpdateData(
                table: "Pizzas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Price" },
                values: new object[] { "A tropical treat with ham and pineapple", 7.49m });
        }
    }
}
