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
        var quiz = await _context.Quizzes
            .Include(q => q.Category)
            .Include(q => q.QuizQuestions)
            .ThenInclude(qq => qq.Question)
            .FirstOrDefaultAsync(q => q.Id == dto.QuizId);

        if (quiz == null)
            throw new Exception("Quiz not found.");

        if (!quiz.IsActive)
            throw new Exception("This quiz is not active.");

        var pool = quiz.QuizQuestions
            .OrderBy(qq => qq.QuestionOrder)
            .Select(qq => qq.Question)
            .Where(q => q.IsActive)
            .ToList();

        if (!pool.Any())
            throw new Exception("This quiz has no questions.");

        var poolIds = pool.Select(q => q.Id).ToHashSet();

        var seenIds = await _context.SeenQuestions
            .Where(s =>
                s.UserId == userId &&
                s.CategoryId == quiz.CategoryId &&
                poolIds.Contains(s.QuestionId))
            .Select(s => s.QuestionId)
            .ToListAsync();

        var unseen = pool.Where(q => !seenIds.Contains(q.Id)).ToList();

        // Pool exhausted for this user → reset seen for this quiz's questions, then reuse full pool
        if (!unseen.Any())
        {
            var seenToClear = _context.SeenQuestions.Where(s =>
                s.UserId == userId &&
                s.CategoryId == quiz.CategoryId &&
                poolIds.Contains(s.QuestionId));

            _context.SeenQuestions.RemoveRange(seenToClear);
            await _context.SaveChangesAsync();

            unseen = pool;
        }

        var takeCount = quiz.QuestionCount > 0
            ? Math.Min(quiz.QuestionCount, unseen.Count)
            : unseen.Count;

        var selectedQuestions = unseen
            .OrderBy(_ => Guid.NewGuid())
            .Take(takeCount)
            .ToList();

        var session = new QuizSession
        {
            UserId = userId,
            QuizId = quiz.Id,
            CategoryId = quiz.CategoryId,
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
                CategoryId = quiz.CategoryId,
                QuestionId = question.Id
            });
        }

        await _context.SaveChangesAsync();

        var firstQuestion = selectedQuestions.First();

        return new StartQuizResponseDto
        {
            SessionId = session.Id,
            TotalQuestions = selectedQuestions.Count,
            DurationSeconds = quiz.DurationSeconds,
            Title = quiz.Title,
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

        bool isCorrect;
        int points;

        var category = await _context.QuizCategories
            .FirstOrDefaultAsync(c => c.Id == session.CategoryId);

        if (category == null)
        {
            throw new Exception("Category not found.");
        }

        if (dto.SelectedOption == 0)
        {
            isCorrect = false;
            points = 0;
        }
        else
        {
            isCorrect = question.CorrectOption == dto.SelectedOption;

            points = isCorrect
                ? category.CorrectPoints
                : category.WrongPoints;
        }

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

        int skippedAnswers = answers.Count(a => a.SelectedOption == 0);

        int wrongAnswers = answers.Count(a =>
            !a.IsCorrect && a.SelectedOption != 0);

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
            SkippedAnswers = skippedAnswers,
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