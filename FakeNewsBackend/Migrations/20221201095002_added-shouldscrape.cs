using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeNewsBackend.Migrations
{
    public partial class addedshouldscrape : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isWhitelisted",
                schema: "public",
                table: "Websites",
                newName: "IsWhitelisted");

            migrationBuilder.AddColumn<bool>(
                name: "ShouldNotBeScraped",
                schema: "public",
                table: "Websites",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShouldNotBeScraped",
                schema: "public",
                table: "Websites");

            migrationBuilder.RenameColumn(
                name: "IsWhitelisted",
                schema: "public",
                table: "Websites",
                newName: "isWhitelisted");
        }
    }
}
