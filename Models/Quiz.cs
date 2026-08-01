namespace QuizBackend.Models;

public class Quiz
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public QuizCategory Category { get; set; } = null!;

    /// <summary>Target number of questions for this quiz.</summary>
    public int QuestionCount { get; set; } = 10;

    /// <summary>Total time allowed for the whole quiz (seconds).</summary>
    public int DurationSeconds { get; set; } = 600;

    public bool IsActive { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
}
