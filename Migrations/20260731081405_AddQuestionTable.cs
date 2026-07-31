using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CorrectPoints",
                table: "QuizCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "QuizCategories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "QuizCategories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionCount",
                table: "QuizCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionTimerSeconds",
                table: "QuizCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RulesText",
                table: "QuizCategories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "QuizCategories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WrongPoints",
                table: "QuizCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionText = table.Column<string>(type: "TEXT", nullable: false),
                    Option1 = table.Column<string>(type: "TEXT", nullable: false),
                    Option2 = table.Column<string>(type: "TEXT", nullable: false),
                    Option3 = table.Column<string>(type: "TEXT", nullable: false),
                    CorrectOption = table.Column<int>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_QuizCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "QuizCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CategoryId",
                table: "Questions",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropColumn(
                name: "CorrectPoints",
                table: "QuizCategories");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "QuizCategories");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "QuizCategories");

            migrationBuilder.DropColumn(
                name: "QuestionCount",
                table: "QuizCategories");

            migrationBuilder.DropColumn(
                name: "QuestionTimerSeconds",
                table: "QuizCategories");

            migrationBuilder.DropColumn(
                name: "RulesText",
                table: "QuizCategories");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "QuizCategories");

            migrationBuilder.DropColumn(
                name: "WrongPoints",
                table: "QuizCategories");
        }
    }
}
