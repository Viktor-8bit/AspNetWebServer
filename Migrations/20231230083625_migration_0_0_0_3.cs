using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetWebServer.Migrations
{
    /// <inheritdoc />
    public partial class migration_0_0_0_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "CPU_load",
                table: "Utilizations",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Utilizations",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<float>(
                name: "RAM",
                table: "Utilizations",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Utilizations");

            migrationBuilder.DropColumn(
                name: "RAM",
                table: "Utilizations");

            migrationBuilder.AlterColumn<string>(
                name: "CPU_load",
                table: "Utilizations",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
