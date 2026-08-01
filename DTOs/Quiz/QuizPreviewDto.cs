namespace QuizBackend.DTOs.Quiz;

public class QuizPreviewDto
{
    public int QuizId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string RulesText { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public int QuestionCount { get; set; }
    public int TimerSeconds { get; set; }
    public int CorrectPoints { get; set; }
    public int WrongPoints { get; set; }
    public int DurationSeconds { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
