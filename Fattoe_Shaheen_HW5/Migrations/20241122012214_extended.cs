using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fattoe_Shaheen_HW5.Migrations
{
    /// <inheritdoc />
    public partial class extended : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExtendedPrice",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtendedPrice",
                table: "OrderDetails");
        }
    }
}
