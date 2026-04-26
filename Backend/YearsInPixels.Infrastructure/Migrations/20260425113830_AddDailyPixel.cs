using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YearsInPixels.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyPixel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "DailyPixel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MoodValue = table.Column<int>(type: "INTEGER", nullable: false),
                    DayWord = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    JournalNote = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPixel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyPixel_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPixel_UserId",
                table: "DailyPixel",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPixel");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
