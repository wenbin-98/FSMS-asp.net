using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSMS_asp.net.Data.Migrations
{
    public partial class addProductsUpdatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("InvoicesModel", null, "Invoices");
            migrationBuilder.RenameTable("ProductsModel", null, "Products");
            migrationBuilder.RenameTable("CustomersModel", null, "Customers");
            migrationBuilder.RenameTable("DOrderDetails", null, "DOrder Details");
            migrationBuilder.RenameTable("InvoiceDetailsModel", null, "Invoice Details");
            migrationBuilder.RenameTable("QuotationDetails", null, "Quotation Details");
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("Invoices", null, "InvoicesModel");
            migrationBuilder.RenameTable("Products", null, "ProductsModel");
            migrationBuilder.RenameTable("Customers", null, "CustomersModel");
            migrationBuilder.RenameTable("DOrder Details", null, "DOrderDetails");
            migrationBuilder.RenameTable("Invoice Details", null, "InvoiceDetailsModel");
            migrationBuilder.RenameTable("Quotation Details", null, "QuotationDetails");
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUsers");
        }
    }
}
