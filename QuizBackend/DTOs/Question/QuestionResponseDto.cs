namespace QuizBackend.DTOs.Question;

public class QuestionResponseDto
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string Option1 { get; set; } = string.Empty;

    public string Option2 { get; set; } = string.Empty;

    public string Option3 { get; set; } = string.Empty;

    public int CorrectOption { get; set; }

    public string Difficulty { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Seconds for this question inside a quiz (0 = use quiz default).
    /// Only populated for quiz-question list endpoints.
    /// </summary>
    public int TimerSeconds { get; set; }
}