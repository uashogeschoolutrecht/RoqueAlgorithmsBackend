using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeNewsBackend.Migrations.Sitemap
{
    public partial class addedLinksNotYetVisited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinksNotYetVisited",
                schema: "public",
                table: "GenerateProgress",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinksNotYetVisited",
                schema: "public",
                table: "GenerateProgress");
        }
    }
}
