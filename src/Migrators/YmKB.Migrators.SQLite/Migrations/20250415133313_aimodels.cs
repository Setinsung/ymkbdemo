using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YmKB.Migrators.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class aimodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    AIModelType = table.Column<string>(type: "TEXT", nullable: false),
                    Endpoint = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    ModelName = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    ModelKey = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    ModelDescription = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KbApp",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    KbAppType = table.Column<string>(type: "TEXT", nullable: false),
                    ChatModelId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    EmbeddingModelId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Temperature = table.Column<double>(type: "REAL", nullable: false),
                    Prompt = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ApiFunctionList = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    NativeFunctionList = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    KbIdList = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Relevance = table.Column<double>(type: "REAL", nullable: false),
                    MaxAskPromptSize = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxMatchesCount = table.Column<int>(type: "INTEGER", nullable: false),
                    AnswerTokens = table.Column<int>(type: "INTEGER", nullable: false),
                    PromptTemplate = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    NoReplyFoundTemplate = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KbApp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KbDocFile",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    KbId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    FileGuid = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    DataCount = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KbDocFile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeDb",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    ChatModelID = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    EmbeddingModelID = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    MaxTokensPerParagraph = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxTokensPerLine = table.Column<int>(type: "INTEGER", nullable: false),
                    OverlappingTokens = table.Column<int>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeDb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnlyChatHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    KbAppId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    MessageType = table.Column<string>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlyChatHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuantizedList",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    KbId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    KbDocFileId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Remark = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    ProcessTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantizedList", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIModel");

            migrationBuilder.DropTable(
                name: "KbApp");

            migrationBuilder.DropTable(
                name: "KbDocFile");

            migrationBuilder.DropTable(
                name: "KnowledgeDb");

            migrationBuilder.DropTable(
                name: "OnlyChatHistory");

            migrationBuilder.DropTable(
                name: "QuantizedList");
        }
    }
}
