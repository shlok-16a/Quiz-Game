using Microsoft.AspNetCore.Http;

namespace QuizBackend.DTOs.Category;

public class UploadCoverImageFormDto
{
    public IFormFile File { get; set; } = null!;
}
