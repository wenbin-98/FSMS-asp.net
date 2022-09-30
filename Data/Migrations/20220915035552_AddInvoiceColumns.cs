using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSMS_asp.net.Data.Migrations
{
    public partial class AddInvoiceColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "InvoicesModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "InvoicesModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerHpNo",
                table: "InvoicesModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "InvoicesModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "InvoicesModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "InvoicesModel");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "InvoicesModel");

            migrationBuilder.DropColumn(
                name: "CustomerHpNo",
                table: "InvoicesModel");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "InvoicesModel");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "InvoicesModel");
        }
    }
}
