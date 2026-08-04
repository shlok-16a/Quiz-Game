using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizBonusPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BonusPoints",
                table: "Quizzes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusTimePercent",
                table: "Quizzes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusPoints",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "BonusTimePercent",
                table: "Quizzes");
        }
    }
}
