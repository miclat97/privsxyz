using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PrivsXYZ.Migrations
{
    public partial class MessageViewDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedDate",
                table: "Message",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ViewerHostname",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViewerIPv4",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViewerIPv6",
                table: "Message",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenedDate",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ViewerHostname",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ViewerIPv4",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ViewerIPv6",
                table: "Message");
        }
    }
}
