using System.ComponentModel.DataAnnotations;

namespace QuizBackend.DTOs.Question;

public class BulkDeleteQuestionsDto
{
    [Required]
    [MinLength(1)]
    public List<int> Ids { get; set; } = new();
}
