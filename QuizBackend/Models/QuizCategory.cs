using System.ComponentModel.DataAnnotations;

namespace QuizBackend.Models;

public class QuizCategory
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // Cover image shown in app
    public string CoverImageUrl { get; set; } = string.Empty;

    // Rules displayed before quiz starts
    public string RulesText { get; set; } = string.Empty;

    // Configurable scoring
    public int CorrectPoints { get; set; } = 10;
    public int WrongPoints { get; set; } = -5;

    // Quiz configuration
    public int QuestionCount { get; set; } = 10;
    public int QuestionTimerSeconds { get; set; } = 10;

    // Scheduling
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Property
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}