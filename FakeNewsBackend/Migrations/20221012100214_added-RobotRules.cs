using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FakeNewsBackend.Migrations
{
    public partial class addedRobotRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RobotRules_Websites_WebsiteId",
                schema: "public",
                table: "RobotRules");

            migrationBuilder.AddColumn<int>(
                name: "rulesWebsiteId",
                schema: "public",
                table: "Websites",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WebsiteId",
                schema: "public",
                table: "RobotRules",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Websites_rulesWebsiteId",
                schema: "public",
                table: "Websites",
                column: "rulesWebsiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_RobotRules_rulesWebsiteId",
                schema: "public",
                table: "Websites",
                column: "rulesWebsiteId",
                principalSchema: "public",
                principalTable: "RobotRules",
                principalColumn: "WebsiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Websites_RobotRules_rulesWebsiteId",
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

            migrationBuilder.AlterColumn<int>(
                name: "WebsiteId",
                schema: "public",
                table: "RobotRules",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_RobotRules_Websites_WebsiteId",
                schema: "public",
                table: "RobotRules",
                column: "WebsiteId",
                principalSchema: "public",
                principalTable: "Websites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
