using Microsoft.EntityFrameworkCore;
using QuizBackend.Models;

namespace QuizBackend.Data;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options)
        : base(options)
    {
    }

    public DbSet<QuizCategory> QuizCategories { get; set; }

    public DbSet<Question> Questions { get; set; }

    public DbSet<SeenQuestion> SeenQuestions { get; set; }

    public DbSet<QuizSession> QuizSessions { get; set; }

    public DbSet<QuizSessionQuestion> QuizSessionQuestions { get; set; }

    public DbSet<UserAnswer> UserAnswers { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Quiz> Quizzes { get; set; }

    public DbSet<QuizQuestion> QuizQuestions { get; set; }
}