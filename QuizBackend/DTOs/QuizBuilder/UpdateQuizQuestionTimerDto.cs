namespace QuizBackend.DTOs.QuizBuilder;

public class UpdateQuizQuestionTimerDto
{
    /// <summary>Seconds allotted to this question (minimum 1).</summary>
    public int TimerSeconds { get; set; }
}
