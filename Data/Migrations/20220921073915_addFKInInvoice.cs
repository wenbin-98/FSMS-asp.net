using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSMS_asp.net.Data.Migrations
{
    public partial class addFKInInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetailsModel_InvoiceId",
                table: "InvoiceDetailsModel",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetailsModel_InvoicesModel_InvoiceId",
                table: "InvoiceDetailsModel",
                column: "InvoiceId",
                principalTable: "InvoicesModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetailsModel_InvoicesModel_InvoiceId",
                table: "InvoiceDetailsModel");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetailsModel_InvoiceId",
                table: "InvoiceDetailsModel");
        }
    }
}
