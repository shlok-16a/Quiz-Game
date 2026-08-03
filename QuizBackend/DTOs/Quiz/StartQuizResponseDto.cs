namespace QuizBackend.DTOs.Quiz;

public class StartQuizResponseDto
{
    public int SessionId { get; set; }

    public int TotalQuestions { get; set; }

    /// <summary>Seconds allotted to each question (from quiz admin setting).</summary>
    public int QuestionTimerSeconds { get; set; }

    /// <summary>Same as QuestionTimerSeconds (kept for older clients).</summary>
    public int DurationSeconds { get; set; }

    public string Title { get; set; } = string.Empty;

    public PlayQuestionDto FirstQuestion { get; set; } = null!;
}