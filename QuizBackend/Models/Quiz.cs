namespace QuizBackend.Models;

public class Quiz
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public QuizCategory Category { get; set; } = null!;

    /// <summary>Rules shown to players for this quiz.</summary>
    public string RulesText { get; set; } = string.Empty;

    /// <summary>Target number of questions for this quiz (per user).</summary>
    public int QuestionCount { get; set; } = 10;

    /// <summary>
    /// Difficulty filter for play-time question selection from the category bank.
    /// Easy / Medium / Hard / Mixed (any difficulty).
    /// </summary>
    public string Difficulty { get; set; } = "Mixed";

    /// <summary>Default time allowed for each question (seconds).</summary>
    public int DurationSeconds { get; set; } = 10;

    /// <summary>Legacy column; always false. Kept for existing database schema.</summary>
    public bool UsePerQuestionTimer { get; set; } = false;

    /// <summary>Legacy column; unused. Kept for existing database schema.</summary>
    public int BonusTimePercent { get; set; } = 0;

    /// <summary>Legacy column; unused. Kept for existing database schema.</summary>
    public int BonusPoints { get; set; } = 0;

    public bool IsActive { get; set; } = false;

    /// <summary>When the quiz becomes visible/playable for users (UTC).</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>When the quiz stops being visible/playable for users (UTC).</summary>
    public DateTime? EndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
}
