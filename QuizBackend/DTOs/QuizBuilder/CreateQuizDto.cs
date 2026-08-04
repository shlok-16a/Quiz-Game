namespace QuizBackend.DTOs.QuizBuilder;

public class CreateQuizDto
{
    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string RulesText { get; set; } = string.Empty;

    public int QuestionCount { get; set; } = 10;

    /// <summary>Default seconds allotted to each question.</summary>
    public int DurationSeconds { get; set; } = 10;

    /// <summary>When true, each assigned question can have its own timer.</summary>
    public bool UsePerQuestionTimer { get; set; } = false;

    /// <summary>Percent of question time within which a correct answer earns bonus. 0 = off.</summary>
    public int BonusTimePercent { get; set; } = 0;

    public int BonusPoints { get; set; } = 0;

    public bool IsActive { get; set; } = false;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
