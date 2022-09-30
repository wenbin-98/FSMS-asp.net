using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSMS_asp.net.Data.Migrations
{
    public partial class fixSomething : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PoNo",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "RefNo",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Delivery Orders");

            migrationBuilder.AddColumn<int>(
                name: "TotalQuantity",
                table: "Delivery Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalQuantity",
                table: "Delivery Orders");

            migrationBuilder.AddColumn<string>(
                name: "PoNo",
                table: "Quotations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefNo",
                table: "Quotations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Delivery Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
