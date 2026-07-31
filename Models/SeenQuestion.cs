namespace QuizBackend.Models;

public class SeenQuestion
{
    public int Id { get; set; }

    // User who has seen the question
    public int UserId { get; set; }

    // Category in which it was seen
    public int CategoryId { get; set; }

    // Question that was seen
    public int QuestionId { get; set; }

    // When it was seen
    public DateTime SeenAt { get; set; } = DateTime.UtcNow;
}