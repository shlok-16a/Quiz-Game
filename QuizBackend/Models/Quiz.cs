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
    /// Answer correctly within this percent of the question timer to earn bonus
    /// (e.g. 40 on a 10s question = within first 4 seconds).
    /// 0 disables bonus.
    /// </summary>
    public int BonusTimePercent { get; set; } = 0;

    /// <summary>Extra points added on top of correct points when bonus is earned.</summary>
    public int BonusPoints { get; set; } = 0;

    public bool IsActive { get; set; } = false;

    /// <summary>When the quiz becomes visible/playable for users (UTC).</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>When the quiz stops being visible/playable for users (UTC).</summary>
    public DateTime? EndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
}
