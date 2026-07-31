namespace QuizBackend.DTOs.Quiz;

public class SubmitAnswerRequestDto
{
    public int SessionId { get; set; }

    public int QuestionId { get; set; }

    public int SelectedOption { get; set; }
}
