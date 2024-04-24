using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AspNetWebServer.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gin", ",,");

            migrationBuilder.CreateTable(
                name: "infoSecuritySpecialists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Login = table.Column<string>(type: "text", nullable: false),
                    HashPassword = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_infoSecuritySpecialists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pcs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hostname = table.Column<string>(type: "text", nullable: false),
                    Online = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pcs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MountedProcesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MonutedIndex = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ProcessId = table.Column<int>(type: "integer", nullable: false),
                    PcSenderId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MountedProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MountedProcesses_Pcs_PcSenderId",
                        column: x => x.PcSenderId,
                        principalTable: "Pcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Login = table.Column<string>(type: "text", nullable: false),
                    PCId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Pcs_PCId",
                        column: x => x.PCId,
                        principalTable: "Pcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Utilizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PcId = table.Column<int>(type: "integer", nullable: false),
                    CPU_load = table.Column<float>(type: "real", nullable: false),
                    RAM = table.Column<float>(type: "real", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Utilizations_Pcs_PcId",
                        column: x => x.PcId,
                        principalTable: "Pcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ProcessId = table.Column<int>(type: "integer", nullable: false),
                    MountedProcessId = table.Column<int>(type: "integer", nullable: false),
                    PcSenderId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeStarted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessActions_MountedProcesses_MountedProcessId",
                        column: x => x.MountedProcessId,
                        principalTable: "MountedProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessActions_Pcs_PcSenderId",
                        column: x => x.PcSenderId,
                        principalTable: "Pcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MountedProcesses_PcSenderId",
                table: "MountedProcesses",
                column: "PcSenderId");

            migrationBuilder.CreateIndex(
                name: "MountedProcess_Id_MonutedIndex_Index",
                table: "MountedProcesses",
                columns: new[] { "Id", "MonutedIndex" })
                .Annotation("Npgsql:IndexMethod", "btree");

            migrationBuilder.CreateIndex(
                name: "Pc_Id_Hostname_Index",
                table: "Pcs",
                columns: new[] { "Id", "hostname" })
                .Annotation("Npgsql:IndexMethod", "btree");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessActions_MountedProcessId",
                table: "ProcessActions",
                column: "MountedProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessActions_PcSenderId",
                table: "ProcessActions",
                column: "PcSenderId");

            migrationBuilder.CreateIndex(
                name: "ProcessAction_Id_Date_MountedProcessId_Index",
                table: "ProcessActions",
                columns: new[] { "Id", "Date", "MountedProcessId" })
                .Annotation("Npgsql:IndexMethod", "btree");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PCId",
                table: "Users",
                column: "PCId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizations_PcId",
                table: "Utilizations",
                column: "PcId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "infoSecuritySpecialists");

            migrationBuilder.DropTable(
                name: "ProcessActions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Utilizations");

            migrationBuilder.DropTable(
                name: "MountedProcesses");

            migrationBuilder.DropTable(
                name: "Pcs");
        }
    }
}
