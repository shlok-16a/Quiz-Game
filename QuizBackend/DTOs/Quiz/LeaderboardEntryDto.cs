namespace QuizBackend.DTOs.Quiz;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime? CompletedAt { get; set; }
}
