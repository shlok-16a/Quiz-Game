namespace QuizBackend.DTOs.Quiz;

public class QuizHistoryDto
{
    public int SessionId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int Score { get; set; }

    public int CorrectAnswers { get; set; }

    public int WrongAnswers { get; set; }

    public double Percentage { get; set; }

    public DateTime? CompletedAt { get; set; }
}
