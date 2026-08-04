using QuizBackend.DTOs.Category;

namespace QuizBackend.Services;

public class CoverImageService
{
    public const long MaxBytes = 2 * 1024 * 1024; // 2 MB

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    };

    private readonly IWebHostEnvironment _env;

    public CoverImageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<UploadCoverImageResultDto> SaveAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("Image file is required.");

        if (file.Length > MaxBytes)
            throw new Exception("Image must be smaller than 2 MB.");

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            throw new Exception("Only JPG, PNG, GIF, and WEBP images are allowed.");

        if (!string.IsNullOrWhiteSpace(file.ContentType) &&
            !AllowedContentTypes.Contains(file.ContentType))
            throw new Exception("Invalid image content type.");

        var webRoot = _env.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(webRoot);
        }

        var folder = Path.Combine(webRoot, "uploads", "categories");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(folder, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return new UploadCoverImageResultDto
        {
            Url = $"/uploads/categories/{fileName}",
            FileName = fileName,
            SizeBytes = file.Length
        };
    }
}
