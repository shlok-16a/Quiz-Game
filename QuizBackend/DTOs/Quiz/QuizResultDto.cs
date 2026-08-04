namespace QuizBackend.DTOs.Quiz;

public class QuizResultDto
{
    public int SessionId { get; set; }

    public int Score { get; set; }

    public int CorrectAnswers { get; set; }

    public int WrongAnswers { get; set; }

    public int SkippedAnswers { get; set; }

    public int TotalQuestions { get; set; }

    /// <summary>Total speed-bonus points earned across the quiz.</summary>
    public int BonusPoints { get; set; }

    /// <summary>How many answers earned the speed bonus.</summary>
    public int BonusAnswers { get; set; }

    public double Percentage { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    /// <summary>Wall-clock seconds from quiz start to finish.</summary>
    public int DurationSeconds { get; set; }

    /// <summary>1-based rank among completed attempts for this quiz (score, then finish time).</summary>
    public int? Rank { get; set; }

    public int TotalCompletions { get; set; }
}
