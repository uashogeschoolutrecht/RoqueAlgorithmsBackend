using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeNewsBackend.Migrations.Similarity
{
    public partial class addedversion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "public",
                table: "Similarities",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "public",
                table: "Similarities");
        }
    }
}
