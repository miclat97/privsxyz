using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PrivsXYZ.Migrations
{
    public partial class PhotoOpenDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedDate",
                table: "Photo",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ViewerHostname",
                table: "Photo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViewerIPv4",
                table: "Photo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViewerIPv6",
                table: "Photo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenedDate",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "ViewerHostname",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "ViewerIPv4",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "ViewerIPv6",
                table: "Photo");
        }
    }
}
