using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizDifficulty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "Quizzes",
                type: "TEXT",
                nullable: false,
                defaultValue: "Mixed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Quizzes");
        }
    }
}
