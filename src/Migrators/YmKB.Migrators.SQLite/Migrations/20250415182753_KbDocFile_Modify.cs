using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YmKB.Migrators.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class KbDocFile_Modify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileGuid",
                table: "KbDocFiles");

            migrationBuilder.AddColumn<string>(
                name: "SegmentPattern",
                table: "KbDocFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SegmentPattern",
                table: "KbDocFiles");

            migrationBuilder.AddColumn<string>(
                name: "FileGuid",
                table: "KbDocFiles",
                type: "TEXT",
                maxLength: 450,
                nullable: false,
                defaultValue: "");
        }
    }
}
