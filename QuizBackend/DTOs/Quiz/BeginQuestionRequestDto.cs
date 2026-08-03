namespace QuizBackend.DTOs.Quiz;

public class BeginQuestionRequestDto
{
    public int SessionId { get; set; }
    public int QuestionId { get; set; }
}

public class BeginQuestionResponseDto
{
    public int TimerSeconds { get; set; }
    public DateTime QuestionStartedAt { get; set; }
}
