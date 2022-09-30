using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSMS_asp.net.Data.Migrations
{
    public partial class fixThing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOrderDetails_Delivery Orders_QuotationId",
                table: "DOrderDetails");

            migrationBuilder.RenameColumn(
                name: "QuotationId",
                table: "DOrderDetails",
                newName: "DOrdersId");

            migrationBuilder.RenameIndex(
                name: "IX_DOrderDetails_QuotationId",
                table: "DOrderDetails",
                newName: "IX_DOrderDetails_DOrdersId");

            migrationBuilder.AddForeignKey(
                name: "FK_DOrderDetails_Delivery Orders_DOrdersId",
                table: "DOrderDetails",
                column: "DOrdersId",
                principalTable: "Delivery Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOrderDetails_Delivery Orders_DOrdersId",
                table: "DOrderDetails");

            migrationBuilder.RenameColumn(
                name: "DOrdersId",
                table: "DOrderDetails",
                newName: "QuotationId");

            migrationBuilder.RenameIndex(
                name: "IX_DOrderDetails_DOrdersId",
                table: "DOrderDetails",
                newName: "IX_DOrderDetails_QuotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DOrderDetails_Delivery Orders_QuotationId",
                table: "DOrderDetails",
                column: "QuotationId",
                principalTable: "Delivery Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
