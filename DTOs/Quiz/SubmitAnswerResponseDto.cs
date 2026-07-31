namespace QuizBackend.DTOs.Quiz;

public class SubmitAnswerResponseDto
{
    public bool IsCorrect { get; set; }

    public int Score { get; set; }

    public bool QuizCompleted { get; set; }

    public PlayQuestionDto? NextQuestion { get; set; }
}
