using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeNewsBackend.Migrations
{
    public partial class addedsitemap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasSiteMap",
                schema: "public",
                table: "Websites");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasSiteMap",
                schema: "public",
                table: "Websites",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
