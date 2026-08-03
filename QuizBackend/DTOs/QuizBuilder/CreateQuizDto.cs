namespace QuizBackend.DTOs.QuizBuilder;

public class CreateQuizDto
{
    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int QuestionCount { get; set; } = 10;

    /// <summary>Seconds allotted to each question.</summary>
    public int DurationSeconds { get; set; } = 10;

    public bool IsActive { get; set; } = false;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
