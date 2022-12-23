using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeNewsBackend.Migrations
{
    public partial class Librecht : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Websites_Rules_rulesWebsiteId",
                schema: "public",
                table: "Websites");

            migrationBuilder.DropIndex(
                name: "IX_Websites_rulesWebsiteId",
                schema: "public",
                table: "Websites");

            migrationBuilder.DropColumn(
                name: "rulesWebsiteId",
                schema: "public",
                table: "Websites");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "rulesWebsiteId",
                schema: "public",
                table: "Websites",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Websites_rulesWebsiteId",
                schema: "public",
                table: "Websites",
                column: "rulesWebsiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_Rules_rulesWebsiteId",
                schema: "public",
                table: "Websites",
                column: "rulesWebsiteId",
                principalSchema: "public",
                principalTable: "Rules",
                principalColumn: "WebsiteId");
        }
    }
}
