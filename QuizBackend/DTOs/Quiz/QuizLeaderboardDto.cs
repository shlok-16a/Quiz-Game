namespace QuizBackend.DTOs.Quiz;

public class QuizLeaderboardDto
{
    public int QuizId { get; set; }

    public string QuizTitle { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int TotalCompletions { get; set; }

    public List<LeaderboardEntryDto> Entries { get; set; } = new();
}
