namespace QuizBackend.DTOs.QuizBuilder;

public class QuizResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string RulesText { get; set; } = string.Empty;

    public int CorrectPoints { get; set; }

    public int WrongPoints { get; set; }

    public int QuestionCount { get; set; }

    public int AssignedQuestions { get; set; }

    /// <summary>Default seconds allotted to each question.</summary>
    public int DurationSeconds { get; set; }

    public int QuestionTimerSeconds { get; set; }

    /// <summary>When true, each assigned question can have its own timer.</summary>
    public bool UsePerQuestionTimer { get; set; }

    public int BonusTimePercent { get; set; }

    public int BonusPoints { get; set; }

    public bool IsActive { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool HasAttempted { get; set; }

    public DateTime CreatedAt { get; set; }
}
