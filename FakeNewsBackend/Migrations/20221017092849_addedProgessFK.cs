using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FakeNewsBackend.Migrations
{
    public partial class addedProgessFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Progress_Websites_WebsiteId",
                schema: "public",
                table: "Progress");

            migrationBuilder.AddColumn<int>(
                name: "WebsiteId",
                schema: "public",
                table: "Websites",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WebsiteId",
                schema: "public",
                table: "Progress",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Websites_WebsiteId",
                schema: "public",
                table: "Websites",
                column: "WebsiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_Progress_WebsiteId",
                schema: "public",
                table: "Websites",
                column: "WebsiteId",
                principalSchema: "public",
                principalTable: "Progress",
                principalColumn: "WebsiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Websites_Progress_WebsiteId",
                schema: "public",
                table: "Websites");

            migrationBuilder.DropIndex(
                name: "IX_Websites_WebsiteId",
                schema: "public",
                table: "Websites");

            migrationBuilder.DropColumn(
                name: "WebsiteId",
                schema: "public",
                table: "Websites");

            migrationBuilder.AlterColumn<int>(
                name: "WebsiteId",
                schema: "public",
                table: "Progress",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_Websites_WebsiteId",
                schema: "public",
                table: "Progress",
                column: "WebsiteId",
                principalSchema: "public",
                principalTable: "Websites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
