using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCardDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payment_CVV",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_CardName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_CardNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_Expiration",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "Payment_Reference",
                table: "Orders",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            // Orders placed before this migration had card data and no reference
            migrationBuilder.Sql("UPDATE Orders SET Payment_Reference = 'SIM-legacy' WHERE Payment_Reference = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payment_Reference",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "Payment_CVV",
                table: "Orders",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Payment_CardName",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Payment_CardNumber",
                table: "Orders",
                type: "nvarchar(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Payment_Expiration",
                table: "Orders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
