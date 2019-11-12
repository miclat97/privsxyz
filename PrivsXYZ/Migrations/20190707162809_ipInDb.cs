using Microsoft.EntityFrameworkCore.Migrations;

namespace PrivsXYZ.Migrations
{
    public partial class ipInDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hostname",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPv4Address",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPv6Address",
                table: "Message",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hostname",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "IPv4Address",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "IPv6Address",
                table: "Message");
        }
    }
}
