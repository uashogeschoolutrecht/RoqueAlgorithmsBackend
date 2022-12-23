using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeNewsBackend.Migrations
{
    public partial class changedFKName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Websites_Progress_WebsiteId",
                schema: "public",
                table: "Websites");

            migrationBuilder.RenameColumn(
                name: "WebsiteId",
                schema: "public",
                table: "Websites",
                newName: "ProgressId");

            migrationBuilder.RenameIndex(
                name: "IX_Websites_WebsiteId",
                schema: "public",
                table: "Websites",
                newName: "IX_Websites_ProgressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_Progress_ProgressId",
                schema: "public",
                table: "Websites",
                column: "ProgressId",
                principalSchema: "public",
                principalTable: "Progress",
                principalColumn: "WebsiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Websites_Progress_ProgressId",
                schema: "public",
                table: "Websites");

            migrationBuilder.RenameColumn(
                name: "ProgressId",
                schema: "public",
                table: "Websites",
                newName: "WebsiteId");

            migrationBuilder.RenameIndex(
                name: "IX_Websites_ProgressId",
                schema: "public",
                table: "Websites",
                newName: "IX_Websites_WebsiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_Progress_WebsiteId",
                schema: "public",
                table: "Websites",
                column: "WebsiteId",
                principalSchema: "public",
                principalTable: "Progress",
                principalColumn: "WebsiteId");
        }
    }
}
