using System.ComponentModel.DataAnnotations;

namespace QuizBackend.DTOs.Question;

public class UpdateQuestionDto
{
    [Required]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    public string Option1 { get; set; } = string.Empty;

    [Required]
    public string Option2 { get; set; } = string.Empty;

    [Required]
    public string Option3 { get; set; } = string.Empty;

    [Range(1, 3)]
    public int CorrectOption { get; set; }

    public string Difficulty { get; set; } = "Easy";

    public int CategoryId { get; set; }
}