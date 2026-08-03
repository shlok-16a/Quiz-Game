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
}