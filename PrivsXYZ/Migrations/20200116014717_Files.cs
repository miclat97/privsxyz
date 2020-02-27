using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PrivsXYZ.Migrations
{
    public partial class Files : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileIdentityString = table.Column<string>(nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    IPv4Address = table.Column<string>(nullable: true),
                    IPv6Address = table.Column<string>(nullable: true),
                    Hostname = table.Column<string>(nullable: true),
                    File = table.Column<byte[]>(nullable: true),
                    ViewerIPv4 = table.Column<string>(nullable: true),
                    ViewerIPv6 = table.Column<string>(nullable: true),
                    ViewerHostname = table.Column<string>(nullable: true),
                    OpenedDate = table.Column<DateTime>(nullable: false),
                    FileName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "File");
        }
    }
}
