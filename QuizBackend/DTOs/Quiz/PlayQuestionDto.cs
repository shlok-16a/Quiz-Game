namespace QuizBackend.DTOs.Quiz;

public class PlayQuestionDto
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string Option1 { get; set; } = string.Empty;

    public string Option2 { get; set; } = string.Empty;

    public string Option3 { get; set; } = string.Empty;

    public int QuestionNumber { get; set; }

    public int CurrentScore { get; set; }

    public int TimerSeconds { get; set; }
}