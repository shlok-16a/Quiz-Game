namespace QuizBackend.DTOs.Quiz;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }

    public int SessionId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int Score { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    /// <summary>Wall-clock seconds from quiz start to finish (server timestamps).</summary>
    public int DurationSeconds { get; set; }
}
