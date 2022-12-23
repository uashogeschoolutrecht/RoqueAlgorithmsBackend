using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeNewsBackend.Migrations
{
    public partial class addedCheckForSitemapAndRobotRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasRules",
                schema: "public",
                table: "Websites",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSitemap",
                schema: "public",
                table: "Websites",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasRules",
                schema: "public",
                table: "Websites");

            migrationBuilder.DropColumn(
                name: "HasSitemap",
                schema: "public",
                table: "Websites");
        }
    }
}
