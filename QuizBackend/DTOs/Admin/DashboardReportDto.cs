namespace QuizBackend.DTOs.Admin;

public class DashboardReportDto
{
    public DashboardStatsDto Summary { get; set; } = new();

    public double OverallCompletionRate { get; set; }

    public double OverallAverageScore { get; set; }

    public List<CategoryPlayReportDto> CategoryPlays { get; set; } = new();

    public List<QuizPlayReportDto> QuizPlays { get; set; } = new();

    public List<QuestionAccuracyDto> MostMissedQuestions { get; set; } = new();
}

public class CategoryPlayReportDto
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int TotalPlays { get; set; }

    public int CompletedPlays { get; set; }

    public double CompletionRate { get; set; }

    public double AverageScore { get; set; }
}

public class QuizPlayReportDto
{
    public int QuizId { get; set; }

    public string QuizTitle { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int TotalPlays { get; set; }

    public int CompletedPlays { get; set; }

    public double CompletionRate { get; set; }

    public double AverageScore { get; set; }
}

public class QuestionAccuracyDto
{
    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int TotalAttempts { get; set; }

    public int CorrectCount { get; set; }

    public int WrongCount { get; set; }

    public int SkippedCount { get; set; }

    /// <summary>Correct / total attempts as a percentage (0–100).</summary>
    public double AccuracyPercent { get; set; }
}
