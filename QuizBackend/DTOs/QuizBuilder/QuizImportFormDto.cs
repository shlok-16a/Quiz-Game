using Microsoft.AspNetCore.Http;

namespace QuizBackend.DTOs.QuizBuilder;

public class QuizImportFormDto
{
    public IFormFile File { get; set; } = null!;
}
