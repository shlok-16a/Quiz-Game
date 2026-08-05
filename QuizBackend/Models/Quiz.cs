namespace QuizBackend.Models;

public class Quiz
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public QuizCategory Category { get; set; } = null!;

    /// <summary>Rules shown to players for this quiz.</summary>
    public string RulesText { get; set; } = string.Empty;

    /// <summary>Target number of questions for this quiz.</summary>
    public int QuestionCount { get; set; } = 10;

    /// <summary>Default time allowed for each question (seconds).</summary>
    public int DurationSeconds { get; set; } = 10;

    /// <summary>
    /// When true, each quiz question uses its own <see cref="QuizQuestion.TimerSeconds"/>.
    /// When false, every question uses <see cref="DurationSeconds"/>.
    /// </summary>
    public bool UsePerQuestionTimer { get; set; } = false;

    /// <summary>
    /// Used only when <see cref="UsePerQuestionTimer"/> is true.
    /// Answer correctly within this percent of the question timer to earn
    /// <see cref="BonusPoints"/> (e.g. 40 on a 10s question = within first 4 seconds).
    /// 0 disables the threshold bonus.
    /// When <see cref="UsePerQuestionTimer"/> is false, bonus is remaining seconds instead.
    /// </summary>
    public int BonusTimePercent { get; set; } = 0;

    /// <summary>
    /// Flat extra points when the per-question threshold bonus is earned.
    /// Ignored when using the global (same-for-all) timer — that mode awards remaining seconds.
    /// </summary>
    public int BonusPoints { get; set; } = 0;

    public bool IsActive { get; set; } = false;

    /// <summary>When the quiz becomes visible/playable for users (UTC).</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>When the quiz stops being visible/playable for users (UTC).</summary>
    public DateTime? EndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
}
