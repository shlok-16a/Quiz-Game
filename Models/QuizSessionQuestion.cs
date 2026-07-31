namespace QuizBackend.Models;

public class QuizSessionQuestion
{
    public int Id { get; set; }

    // Session this question belongs to
    public int QuizSessionId { get; set; }

    // Actual question
    public int QuestionId { get; set; }

    // Order in which it should appear
    public int QuestionOrder { get; set; }

    // Navigation Properties
    public QuizSession QuizSession { get; set; } = null!;

    public Question Question { get; set; } = null!;
}