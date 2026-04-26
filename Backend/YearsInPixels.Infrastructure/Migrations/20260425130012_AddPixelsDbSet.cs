using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YearsInPixels.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPixelsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyPixel_Users_UserId",
                table: "DailyPixel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyPixel",
                table: "DailyPixel");

            migrationBuilder.DropIndex(
                name: "IX_DailyPixel_UserId",
                table: "DailyPixel");

            migrationBuilder.RenameTable(
                name: "DailyPixel",
                newName: "DailyPixels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyPixels",
                table: "DailyPixels",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPixels_UserId_Date",
                table: "DailyPixels",
                columns: new[] { "UserId", "Date" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPixels_Users_UserId",
                table: "DailyPixels",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyPixels_Users_UserId",
                table: "DailyPixels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyPixels",
                table: "DailyPixels");

            migrationBuilder.DropIndex(
                name: "IX_DailyPixels_UserId_Date",
                table: "DailyPixels");

            migrationBuilder.RenameTable(
                name: "DailyPixels",
                newName: "DailyPixel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyPixel",
                table: "DailyPixel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPixel_UserId",
                table: "DailyPixel",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPixel_Users_UserId",
                table: "DailyPixel",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
