namespace QuizBackend.DTOs.QuizBuilder;

public class UpdateQuizDto
{
    public string Title { get; set; } = string.Empty;

    public string RulesText { get; set; } = string.Empty;

    public int QuestionCount { get; set; }

    public int DurationSeconds { get; set; }

    public bool UsePerQuestionTimer { get; set; }

    public int BonusTimePercent { get; set; }

    public int BonusPoints { get; set; }

    public bool IsActive { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
