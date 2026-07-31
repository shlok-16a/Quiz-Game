namespace QuizBackend.Models;

public class QuizSession
{
    public int Id { get; set; }

    // User playing the quiz
    public int UserId { get; set; }

    // Selected Category
    public int CategoryId { get; set; }

    // Current Score
    public int Score { get; set; } = 0;

    // Current Question Index (0-9)
    public int CurrentQuestionIndex { get; set; } = 0;

    // Quiz Started
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    // Quiz Finished
    public DateTime? CompletedAt { get; set; }

    // Quiz Completed?
    public bool IsCompleted { get; set; } = false;

    public ICollection<QuizSessionQuestion> SessionQuestions { get; set; }
        = new List<QuizSessionQuestion>();
}