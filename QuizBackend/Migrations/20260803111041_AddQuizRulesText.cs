using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizRulesText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BronzeMinScore",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "GoldMinScore",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "SilverMinScore",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "CurrentQuestionStartedAt",
                table: "QuizSessions");

            migrationBuilder.AddColumn<string>(
                name: "RulesText",
                table: "Quizzes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RulesText",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "BronzeMinScore",
                table: "Quizzes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoldMinScore",
                table: "Quizzes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SilverMinScore",
                table: "Quizzes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentQuestionStartedAt",
                table: "QuizSessions",
                type: "TEXT",
                nullable: true);
        }
    }
}
