using System.ComponentModel.DataAnnotations;

namespace QuizBackend.DTOs.Category;

public class CreateCategoryDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string CoverImageUrl { get; set; } = string.Empty;

    public string RulesText { get; set; } = string.Empty;

    public int CorrectPoints { get; set; }

    public int WrongPoints { get; set; }

    public int QuestionCount { get; set; }

    public int QuestionTimerSeconds { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }
}