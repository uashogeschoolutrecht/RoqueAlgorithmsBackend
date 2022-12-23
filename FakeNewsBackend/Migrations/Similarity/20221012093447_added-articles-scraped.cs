using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeNewsBackend.Migrations.Similarity
{
    public partial class addedarticlesscraped : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Similarities",
                schema: "public",
                columns: table => new
                {
                    OriginalWebsiteId = table.Column<int>(type: "integer", nullable: false),
                    FoundWebsiteId = table.Column<int>(type: "integer", nullable: false),
                    UrlToOriginalArticle = table.Column<string>(type: "text", nullable: false),
                    UrlToFoundArticle = table.Column<string>(type: "text", nullable: false),
                    SimilarityScore = table.Column<float>(type: "real", nullable: false),
                    OriginalLanguage = table.Column<int>(type: "integer", nullable: false),
                    FoundLanguage = table.Column<int>(type: "integer", nullable: false),
                    OriginalPostDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundPostDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Similarities", x => new { x.FoundWebsiteId, x.OriginalWebsiteId, x.UrlToFoundArticle, x.UrlToOriginalArticle });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Similarities",
                schema: "public");
        }
    }
}
