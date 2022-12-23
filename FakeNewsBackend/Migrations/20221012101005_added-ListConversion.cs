using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeNewsBackend.Migrations
{
    public partial class addedListConversion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Websites_RobotRules_rulesWebsiteId",
                schema: "public",
                table: "Websites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RobotRules",
                schema: "public",
                table: "RobotRules");

            migrationBuilder.RenameTable(
                name: "RobotRules",
                schema: "public",
                newName: "Rules",
                newSchema: "public");

            migrationBuilder.AddColumn<string>(
                name: "AllowedLinks",
                schema: "public",
                table: "Rules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisallowedLinks",
                schema: "public",
                table: "Rules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rules",
                schema: "public",
                table: "Rules",
                column: "WebsiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_Rules_rulesWebsiteId",
                schema: "public",
                table: "Websites",
                column: "rulesWebsiteId",
                principalSchema: "public",
                principalTable: "Rules",
                principalColumn: "WebsiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Websites_Rules_rulesWebsiteId",
                schema: "public",
                table: "Websites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rules",
                schema: "public",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "AllowedLinks",
                schema: "public",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "DisallowedLinks",
                schema: "public",
                table: "Rules");

            migrationBuilder.RenameTable(
                name: "Rules",
                schema: "public",
                newName: "RobotRules",
                newSchema: "public");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RobotRules",
                schema: "public",
                table: "RobotRules",
                column: "WebsiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_RobotRules_rulesWebsiteId",
                schema: "public",
                table: "Websites",
                column: "rulesWebsiteId",
                principalSchema: "public",
                principalTable: "RobotRules",
                principalColumn: "WebsiteId");
        }
    }
}
