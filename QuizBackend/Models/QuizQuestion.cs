namespace QuizBackend.Models;

public class QuizQuestion
{
    public int Id { get; set; }

    public int QuizId { get; set; }

    public Quiz Quiz { get; set; } = null!;

    public int QuestionId { get; set; }

    public Question Question { get; set; } = null!;

    public int QuestionOrder { get; set; }

    /// <summary>
    /// Seconds allotted to this question when the quiz uses per-question timers.
    /// 0 means fall back to the quiz default DurationSeconds.
    /// </summary>
    public int TimerSeconds { get; set; } = 0;
}
