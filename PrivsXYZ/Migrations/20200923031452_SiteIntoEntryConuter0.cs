using Microsoft.EntityFrameworkCore.Migrations;

namespace PrivsXYZ.Migrations
{
    public partial class SiteIntoEntryConuter0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "EntryCounter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Site",
                table: "EntryCounter");
        }
    }
}
