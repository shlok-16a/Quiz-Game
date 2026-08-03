namespace QuizBackend.Models;

public class Quiz
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public QuizCategory Category { get; set; } = null!;

    /// <summary>Target number of questions for this quiz.</summary>
    public int QuestionCount { get; set; } = 10;

    /// <summary>Time allowed for each question (seconds).</summary>
    public int DurationSeconds { get; set; } = 10;

    public bool IsActive { get; set; } = false;

    /// <summary>When the quiz becomes visible/playable for users (UTC).</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>When the quiz stops being visible/playable for users (UTC).</summary>
    public DateTime? EndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
}
