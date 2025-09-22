using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuleEngine.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RuleExecutionAudits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RuleId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RuleVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    Input = table.Column<string>(type: "TEXT", nullable: true),
                    Output = table.Column<string>(type: "TEXT", nullable: true),
                    Success = table.Column<bool>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    Duration = table.Column<string>(type: "TEXT", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleExecutionAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RuleParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RuleId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RuleParameters_Rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleVersions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RuleId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    PredicateExpression = table.Column<string>(type: "TEXT", nullable: false),
                    ResultExpression = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "csharp"),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RuleVersions_Rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RuleExecutionAudits_ExecutedAt",
                table: "RuleExecutionAudits",
                column: "ExecutedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RuleExecutionAudits_RuleId",
                table: "RuleExecutionAudits",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleParameters_RuleId_Name",
                table: "RuleParameters",
                columns: new[] { "RuleId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rules_Name",
                table: "Rules",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_Status",
                table: "Rules",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RuleVersions_RuleId_IsActive",
                table: "RuleVersions",
                columns: new[] { "RuleId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_RuleVersions_RuleId_Version",
                table: "RuleVersions",
                columns: new[] { "RuleId", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RuleExecutionAudits");

            migrationBuilder.DropTable(
                name: "RuleParameters");

            migrationBuilder.DropTable(
                name: "RuleVersions");

            migrationBuilder.DropTable(
                name: "Rules");
        }
    }
}
