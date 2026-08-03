namespace QuizBackend.Models;

public class UserAnswer
{
    public int Id { get; set; }

    public int QuizSessionId { get; set; }

    public int QuestionId { get; set; }

    public int SelectedOption { get; set; }

    public bool IsCorrect { get; set; }

    public int PointsAwarded { get; set; }

    public int TimeTakenSeconds { get; set; }

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}