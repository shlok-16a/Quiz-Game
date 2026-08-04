using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPerQuestionTimer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsePerQuestionTimer",
                table: "Quizzes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TimerSeconds",
                table: "QuizQuestions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsePerQuestionTimer",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "TimerSeconds",
                table: "QuizQuestions");
        }
    }
}
