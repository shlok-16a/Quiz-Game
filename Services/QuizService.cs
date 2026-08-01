using QuizBackend.Data;
using Microsoft.EntityFrameworkCore;
using QuizBackend.DTOs.Quiz;
using QuizBackend.Models;

namespace QuizBackend.Services;

public class QuizService
{
    private readonly QuizDbContext _context;

    public QuizService(QuizDbContext context)
    {
        _context = context;
    }

    public async Task<StartQuizResponseDto> StartQuizAsync(int userId, StartQuizRequestDto dto)
    {
        // Step 1 - Validate Category
        var category = await _context.QuizCategories
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId);

        if (category == null)
        {
            throw new Exception("Category not found.");
        }

        // Step 2
        // Get Questions

        // Step 3
        // Remove Seen Questions

        // Step 4
        // Randomize

        // Step 5
        // Create Session

        // Step 6
        // Save Session Questions

        // Step 7
        // Return First Question

        var questions = await _context.Questions
            .Where(q => q.CategoryId == dto.CategoryId && q.IsActive)
            .ToListAsync();

        if (!questions.Any())
        {
            throw new Exception("No questions found for this category.");
        }

        // Randomize questions
        var selectedQuestions = questions
            .OrderBy(q => Guid.NewGuid())
            .Take(category.QuestionCount)
            .ToList();

        var session = new QuizSession
        {
            UserId = userId,
            CategoryId = dto.CategoryId,
            StartedAt = DateTime.UtcNow,
            Score = 0,
            CurrentQuestionIndex = 0,
            IsCompleted = false
        };

        _context.QuizSessions.Add(session);

        await _context.SaveChangesAsync();

        int order = 1;

        foreach (var question in selectedQuestions)
        {
            _context.QuizSessionQuestions.Add(new QuizSessionQuestion
            {
                QuizSessionId = session.Id,
                QuestionId = question.Id,
                QuestionOrder = order++
            });

            _context.SeenQuestions.Add(new SeenQuestion
            {
                UserId = userId,
                CategoryId = dto.CategoryId,
                QuestionId = question.Id
            });
        }

        await _context.SaveChangesAsync();

        var firstQuestion = selectedQuestions.First();

        return new StartQuizResponseDto
        {
            SessionId = session.Id,
            TotalQuestions = selectedQuestions.Count,
            FirstQuestion = new PlayQuestionDto
            {
                Id = firstQuestion.Id,
                QuestionText = firstQuestion.QuestionText,
                Option1 = firstQuestion.Option1,
                Option2 = firstQuestion.Option2,
                Option3 = firstQuestion.Option3
            }
        };
    }

    public async Task<SubmitAnswerResponseDto> SubmitAnswerAsync(
        SubmitAnswerRequestDto dto)
    {
        var session = await _context.QuizSessions
            .FirstOrDefaultAsync(s => s.Id == dto.SessionId);

        if (session == null)
        {
            throw new Exception("Quiz session not found.");
        }

        if (session.IsCompleted)
        {
            throw new Exception("Quiz has already been completed.");
        }

        var question = await _context.Questions
            .FirstOrDefaultAsync(q => q.Id == dto.QuestionId);

        if (question == null)
        {
            throw new Exception("Question not found.");
        }

        bool isCorrect = question.CorrectOption == dto.SelectedOption;

        var category = await _context.QuizCategories
            .FirstOrDefaultAsync(c => c.Id == session.CategoryId);

        if (category == null)
        {
            throw new Exception("Category not found.");
        }

        int points = isCorrect
            ? category.CorrectPoints
            : category.WrongPoints;

        _context.UserAnswers.Add(new UserAnswer
        {
            QuizSessionId = session.Id,
            QuestionId = question.Id,
            SelectedOption = dto.SelectedOption,
            IsCorrect = isCorrect,
            PointsAwarded = points
        });

        session.Score += points;
        session.CurrentQuestionIndex++;

        await _context.SaveChangesAsync();

        var sessionQuestions = await _context.QuizSessionQuestions
            .Where(q => q.QuizSessionId == session.Id)
            .OrderBy(q => q.QuestionOrder)
            .ToListAsync();

        if (session.CurrentQuestionIndex >= sessionQuestions.Count)
        {
            session.IsCompleted = true;
            session.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new SubmitAnswerResponseDto
            {
                IsCorrect = isCorrect,
                Score = session.Score,
                QuizCompleted = true,
                NextQuestion = null
            };
        }

        var nextQuestionId =
            sessionQuestions[session.CurrentQuestionIndex].QuestionId;

        var nextQuestion = await _context.Questions
            .FirstAsync(q => q.Id == nextQuestionId);

        return new SubmitAnswerResponseDto
        {
            IsCorrect = isCorrect,
            Score = session.Score,
            QuizCompleted = false,
            NextQuestion = new PlayQuestionDto
            {
                Id = nextQuestion.Id,
                QuestionText = nextQuestion.QuestionText,
                Option1 = nextQuestion.Option1,
                Option2 = nextQuestion.Option2,
                Option3 = nextQuestion.Option3
            }
        };
    }

    public async Task<QuizResultDto> GetQuizResultAsync(int sessionId)
    {
        var session = await _context.QuizSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
        {
            throw new Exception("Quiz session not found.");
        }

        var answers = await _context.UserAnswers
            .Where(a => a.QuizSessionId == sessionId)
            .ToListAsync();

        int correctAnswers = answers.Count(a => a.IsCorrect);

        int wrongAnswers = answers.Count(a => !a.IsCorrect);

        int totalQuestions = answers.Count;

        double percentage = totalQuestions == 0
            ? 0
            : (double)correctAnswers / totalQuestions * 100;

        return new QuizResultDto
        {
            SessionId = session.Id,
            Score = session.Score,
            CorrectAnswers = correctAnswers,
            WrongAnswers = wrongAnswers,
            TotalQuestions = totalQuestions,
            Percentage = percentage,
            CompletedAt = session.CompletedAt
        };
    }

    public async Task<List<QuizHistoryDto>> GetQuizHistoryAsync(int userId)
    {
        throw new NotImplementedException();
    }
}