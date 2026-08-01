namespace QuizBackend.DTOs.QuizBuilder;

public class CreateQuizDto
{
    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int QuestionCount { get; set; } = 10;

    public int DurationSeconds { get; set; } = 600;

    public bool IsActive { get; set; } = false;
}
