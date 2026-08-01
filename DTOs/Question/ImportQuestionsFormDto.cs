using Microsoft.AspNetCore.Http;

namespace QuizBackend.DTOs.Question;

public class ImportQuestionsFormDto
{
    public IFormFile File { get; set; } = null!;

    public int CategoryId { get; set; }
}
