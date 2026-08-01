namespace QuizBackend.DTOs.Quiz;

public class QuizResultDto
{
    public int SessionId { get; set; }

    public int Score { get; set; }

    public int CorrectAnswers { get; set; }

    public int WrongAnswers { get; set; }

    public int SkippedAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public double Percentage { get; set; }

    public DateTime? CompletedAt { get; set; }
}
