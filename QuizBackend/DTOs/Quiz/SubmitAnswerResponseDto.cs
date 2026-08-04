namespace QuizBackend.DTOs.Quiz;

public class SubmitAnswerResponseDto
{
    public bool IsCorrect { get; set; }

    public int Score { get; set; }

    public bool QuizCompleted { get; set; }

    /// <summary>1-3 index of the correct option (shown after timeout/skip).</summary>
    public int CorrectOption { get; set; }

    public int PointsAwarded { get; set; }

    public int BonusAwarded { get; set; }

    public PlayQuestionDto? NextQuestion { get; set; }
}
