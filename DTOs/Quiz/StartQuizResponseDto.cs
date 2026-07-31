namespace QuizBackend.DTOs.Quiz;

public class StartQuizResponseDto
{
    public int SessionId { get; set; }

    public int TotalQuestions { get; set; }

    public PlayQuestionDto FirstQuestion { get; set; } = null!;
}