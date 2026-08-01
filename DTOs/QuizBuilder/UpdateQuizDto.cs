namespace QuizBackend.DTOs.QuizBuilder;

public class UpdateQuizDto
{
    public string Title { get; set; } = string.Empty;

    public int QuestionCount { get; set; }

    public int DurationSeconds { get; set; }

    public bool IsActive { get; set; }
}
