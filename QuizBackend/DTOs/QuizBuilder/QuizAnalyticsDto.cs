namespace QuizBackend.DTOs.QuizBuilder;

public class QuizAnalyticsDto
{
    public int QuizId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalPlays { get; set; }
    public int CompletedPlays { get; set; }
    public double CompletionRate { get; set; }
    public double AverageScore { get; set; }
    public int? MostMissedQuestionId { get; set; }
    public string? MostMissedQuestionText { get; set; }
    public int MostMissedWrongCount { get; set; }
}
