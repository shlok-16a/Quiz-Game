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
}