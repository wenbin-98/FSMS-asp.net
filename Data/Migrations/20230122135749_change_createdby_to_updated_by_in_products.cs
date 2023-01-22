using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSMS_asp.net.Data.Migrations
{
    public partial class change_createdby_to_updated_by_in_products : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Products",
                newName: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Products",
                newName: "CreatedBy");
        }
    }
}
