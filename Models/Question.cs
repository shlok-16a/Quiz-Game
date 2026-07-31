using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizBackend.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    public string Option1 { get; set; } = string.Empty;

    [Required]
    public string Option2 { get; set; } = string.Empty;

    [Required]
    public string Option3 { get; set; } = string.Empty;

    // 1,2 or 3
    public int CorrectOption { get; set; }

    public string Difficulty { get; set; } = "Easy";

    public string CreatedBy { get; set; } = "Admin";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Foreign Key
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public QuizCategory Category { get; set; } = null!;

    public ICollection<QuizSessionQuestion> SessionQuestions { get; set; }
        = new List<QuizSessionQuestion>();
}