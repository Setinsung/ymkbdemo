using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YmKB.Migrators.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class contextchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuantizedList",
                table: "QuantizedList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OnlyChatHistory",
                table: "OnlyChatHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KnowledgeDb",
                table: "KnowledgeDb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KbDocFile",
                table: "KbDocFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KbApp",
                table: "KbApp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AIModel",
                table: "AIModel");

            migrationBuilder.RenameTable(
                name: "QuantizedList",
                newName: "QuantizedLists");

            migrationBuilder.RenameTable(
                name: "OnlyChatHistory",
                newName: "OnlyChatHistories");

            migrationBuilder.RenameTable(
                name: "KnowledgeDb",
                newName: "KnowledgeDbs");

            migrationBuilder.RenameTable(
                name: "KbDocFile",
                newName: "KbDocFiles");

            migrationBuilder.RenameTable(
                name: "KbApp",
                newName: "KbApps");

            migrationBuilder.RenameTable(
                name: "AIModel",
                newName: "AIModels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuantizedLists",
                table: "QuantizedLists",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OnlyChatHistories",
                table: "OnlyChatHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KnowledgeDbs",
                table: "KnowledgeDbs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KbDocFiles",
                table: "KbDocFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KbApps",
                table: "KbApps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AIModels",
                table: "AIModels",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuantizedLists",
                table: "QuantizedLists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OnlyChatHistories",
                table: "OnlyChatHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KnowledgeDbs",
                table: "KnowledgeDbs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KbDocFiles",
                table: "KbDocFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KbApps",
                table: "KbApps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AIModels",
                table: "AIModels");

            migrationBuilder.RenameTable(
                name: "QuantizedLists",
                newName: "QuantizedList");

            migrationBuilder.RenameTable(
                name: "OnlyChatHistories",
                newName: "OnlyChatHistory");

            migrationBuilder.RenameTable(
                name: "KnowledgeDbs",
                newName: "KnowledgeDb");

            migrationBuilder.RenameTable(
                name: "KbDocFiles",
                newName: "KbDocFile");

            migrationBuilder.RenameTable(
                name: "KbApps",
                newName: "KbApp");

            migrationBuilder.RenameTable(
                name: "AIModels",
                newName: "AIModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuantizedList",
                table: "QuantizedList",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OnlyChatHistory",
                table: "OnlyChatHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KnowledgeDb",
                table: "KnowledgeDb",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KbDocFile",
                table: "KbDocFile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KbApp",
                table: "KbApp",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AIModel",
                table: "AIModel",
                column: "Id");
        }
    }
}
