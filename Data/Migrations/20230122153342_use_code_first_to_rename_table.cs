using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSMS_asp.net.Data.Migrations
{
    public partial class use_code_first_to_rename_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOrder Details_Delivery Orders_DOrdersId",
                table: "DOrder Details");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice Details_Invoices_InvoiceId",
                table: "Invoice Details");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotation Details_Quotations_QuotationId",
                table: "Quotation Details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quotation Details",
                table: "Quotation Details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice Details",
                table: "Invoice Details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DOrder Details",
                table: "DOrder Details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.RenameTable(
                name: "Quotation Details",
                newName: "QuotationDetails");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "InvoicesModel");

            migrationBuilder.RenameTable(
                name: "Invoice Details",
                newName: "InvoiceDetailsModel");

            migrationBuilder.RenameTable(
                name: "DOrder Details",
                newName: "DOrderDetails");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "CustomersModel");

            migrationBuilder.RenameIndex(
                name: "IX_Quotation Details_QuotationId",
                table: "QuotationDetails",
                newName: "IX_QuotationDetails_QuotationId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice Details_InvoiceId",
                table: "InvoiceDetailsModel",
                newName: "IX_InvoiceDetailsModel_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_DOrder Details_DOrdersId",
                table: "DOrderDetails",
                newName: "IX_DOrderDetails_DOrdersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuotationDetails",
                table: "QuotationDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoicesModel",
                table: "InvoicesModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceDetailsModel",
                table: "InvoiceDetailsModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DOrderDetails",
                table: "DOrderDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomersModel",
                table: "CustomersModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DOrderDetails_Delivery Orders_DOrdersId",
                table: "DOrderDetails",
                column: "DOrdersId",
                principalTable: "Delivery Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetailsModel_InvoicesModel_InvoiceId",
                table: "InvoiceDetailsModel",
                column: "InvoiceId",
                principalTable: "InvoicesModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_Quotations_QuotationId",
                table: "QuotationDetails",
                column: "QuotationId",
                principalTable: "Quotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
