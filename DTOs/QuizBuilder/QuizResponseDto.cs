namespace QuizBackend.DTOs.QuizBuilder;

public class QuizResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int QuestionCount { get; set; }

    public int AssignedQuestions { get; set; }

    public int DurationSeconds { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
