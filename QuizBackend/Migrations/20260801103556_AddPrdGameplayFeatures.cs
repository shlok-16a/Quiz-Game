using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPrdGameplayFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BronzeMinScore",
                table: "Quizzes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Quizzes",
                type: "TEXT",
                nullable: true);

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
                name: "StartDate",
                table: "Quizzes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentQuestionStartedAt",
                table: "QuizSessions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionOrder",
                table: "QuizSessionQuestions",
                type: "TEXT",
                nullable: false,
                defaultValue: "1,2,3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BronzeMinScore",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "GoldMinScore",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "SilverMinScore",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "CurrentQuestionStartedAt",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "OptionOrder",
                table: "QuizSessionQuestions");
        }
    }
}
