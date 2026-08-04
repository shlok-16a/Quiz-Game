namespace QuizBackend.DTOs.Category;

public class UploadCoverImageResultDto
{
    /// <summary>Relative path served by the API, e.g. /uploads/categories/abc.jpg</summary>
    public string Url { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public long SizeBytes { get; set; }
}
