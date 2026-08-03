namespace QuizBackend.DTOs.Question;

public class ImportQuestionsResultDto
{
    public int Imported { get; set; }

    public int Failed { get; set; }

    public List<string> Errors { get; set; } = new();
}
