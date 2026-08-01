namespace QuizBackend.Models;

public class QuizQuestion
{
    public int Id { get; set; }

    public int QuizId { get; set; }

    public Quiz Quiz { get; set; } = null!;

    public int QuestionId { get; set; }

    public Question Question { get; set; } = null!;

    public int QuestionOrder { get; set; }
}
