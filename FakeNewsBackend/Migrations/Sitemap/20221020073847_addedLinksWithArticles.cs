using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeNewsBackend.Migrations.Sitemap
{
    public partial class addedLinksWithArticles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LinksNotYetVisited",
                schema: "public",
                table: "GenerateProgress",
                newName: "LinksWithArticles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LinksWithArticles",
                schema: "public",
                table: "GenerateProgress",
                newName: "LinksNotYetVisited");
        }
    }
}
