using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FakeNewsBackend.Migrations
{
    public partial class addedProgess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rules",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "Progress",
                schema: "public",
                columns: table => new
                {
                    WebsiteId = table.Column<int>(type: "integer", nullable: false),
                    CurrentlyWorkingOn = table.Column<string>(type: "text", nullable: false),
                    NumberOfLinkInSiteMap = table.Column<int>(type: "integer", nullable: false),
                    IsDone = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progress", x => x.WebsiteId);
                    table.ForeignKey(
                        name: "FK_Progress_Websites_WebsiteId",
                        column: x => x.WebsiteId,
                        principalSchema: "public",
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Progress",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "Rules",
                schema: "public",
                columns: table => new
                {
                    WebsiteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AllowedLinks = table.Column<string>(type: "text", nullable: false),
                    DisallowedLinks = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.WebsiteId);
                });
        }
    }
}
