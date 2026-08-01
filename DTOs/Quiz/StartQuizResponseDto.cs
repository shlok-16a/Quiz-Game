namespace QuizBackend.DTOs.Quiz;

public class StartQuizResponseDto
{
    public int SessionId { get; set; }

    public int TotalQuestions { get; set; }

    public int DurationSeconds { get; set; }

    public string Title { get; set; } = string.Empty;

    public PlayQuestionDto FirstQuestion { get; set; } = null!;
}