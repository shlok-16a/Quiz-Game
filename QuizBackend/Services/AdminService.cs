using Microsoft.EntityFrameworkCore;
using QuizBackend.Data;
using QuizBackend.DTOs.Admin;

namespace QuizBackend.Services;

public class AdminService
{
    private readonly QuizDbContext _context;

    public AdminService(QuizDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return new DashboardStatsDto
        {
            TotalCategories = await _context.QuizCategories.CountAsync(),
            TotalQuestions = await _context.Questions.CountAsync(),
            TotalPlayers = await _context.Users.CountAsync(),
            TotalQuizSessions = await _context.QuizSessions.CountAsync()
        };
    }

    public async Task<DashboardReportDto> GetDashboardReportAsync(int mostMissedLimit = 10, int? categoryId = null)
    {
        if (mostMissedLimit < 1)
            mostMissedLimit = 10;

        var summary = await GetDashboardStatsAsync();

        var sessionsQuery = _context.QuizSessions.AsQueryable();
        if (categoryId.HasValue && categoryId.Value > 0)
            sessionsQuery = sessionsQuery.Where(s => s.CategoryId == categoryId.Value);

        var sessions = await sessionsQuery
            .Select(s => new
            {
                s.Id,
                s.CategoryId,
                s.QuizId,
                s.IsCompleted,
                s.Score
            })
            .ToListAsync();

        var categoriesQuery = _context.QuizCategories.AsQueryable();
        if (categoryId.HasValue && categoryId.Value > 0)
            categoriesQuery = categoriesQuery.Where(c => c.Id == categoryId.Value);

        var categories = await categoriesQuery
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();

        var quizzesQuery = _context.Quizzes.AsQueryable();
        if (categoryId.HasValue && categoryId.Value > 0)
            quizzesQuery = quizzesQuery.Where(q => q.CategoryId == categoryId.Value);

        var quizzes = await quizzesQuery
            .Select(q => new { q.Id, q.Title, q.CategoryId })
            .ToListAsync();

        var categoryNameById = categories.ToDictionary(c => c.Id, c => c.Name);

        var completedSessions = sessions.Where(s => s.IsCompleted).ToList();
        var overallCompletionRate = sessions.Count == 0
            ? 0
            : Math.Round(100.0 * completedSessions.Count / sessions.Count, 1);
        var overallAverageScore = completedSessions.Count == 0
            ? 0
            : Math.Round(completedSessions.Average(s => s.Score), 1);

        var categoryPlays = categories
            .Select(c =>
            {
                var catSessions = sessions.Where(s => s.CategoryId == c.Id).ToList();
                var completed = catSessions.Where(s => s.IsCompleted).ToList();
                return new CategoryPlayReportDto
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    TotalPlays = catSessions.Count,
                    CompletedPlays = completed.Count,
                    CompletionRate = catSessions.Count == 0
                        ? 0
                        : Math.Round(100.0 * completed.Count / catSessions.Count, 1),
                    AverageScore = completed.Count == 0
                        ? 0
                        : Math.Round(completed.Average(s => s.Score), 1)
                };
            })
            .OrderByDescending(c => c.TotalPlays)
            .ThenBy(c => c.CategoryName)
            .ToList();

        var quizPlays = quizzes
            .Select(q =>
            {
                var quizSessions = sessions.Where(s => s.QuizId == q.Id).ToList();
                var completed = quizSessions.Where(s => s.IsCompleted).ToList();
                categoryNameById.TryGetValue(q.CategoryId, out var categoryName);

                return new QuizPlayReportDto
                {
                    QuizId = q.Id,
                    QuizTitle = q.Title,
                    CategoryName = categoryName ?? "",
                    TotalPlays = quizSessions.Count,
                    CompletedPlays = completed.Count,
                    CompletionRate = quizSessions.Count == 0
                        ? 0
                        : Math.Round(100.0 * completed.Count / quizSessions.Count, 1),
                    AverageScore = completed.Count == 0
                        ? 0
                        : Math.Round(completed.Average(s => s.Score), 1)
                };
            })
            .OrderByDescending(q => q.TotalPlays)
            .ThenBy(q => q.QuizTitle)
            .ToList();

        var mostMissedQuery = _context.UserAnswers
            .Join(
                _context.Questions,
                a => a.QuestionId,
                q => q.Id,
                (a, q) => new { a, q })
            .Join(
                _context.QuizCategories,
                x => x.q.CategoryId,
                c => c.Id,
                (x, c) => new { x.a, x.q, CategoryName = c.Name, CategoryId = c.Id });

        if (categoryId.HasValue && categoryId.Value > 0)
            mostMissedQuery = mostMissedQuery.Where(x => x.CategoryId == categoryId.Value);

        var mostMissed = await mostMissedQuery
            .GroupBy(x => new { x.q.Id, x.q.QuestionText, x.CategoryName })
            .Select(g => new
            {
                g.Key.Id,
                g.Key.QuestionText,
                g.Key.CategoryName,
                TotalAttempts = g.Count(),
                CorrectCount = g.Count(x => x.a.IsCorrect),
                WrongCount = g.Count(x => !x.a.IsCorrect && x.a.SelectedOption != 0),
                SkippedCount = g.Count(x => x.a.SelectedOption == 0)
            })
            .OrderByDescending(x => x.WrongCount)
            .ThenBy(x => x.TotalAttempts == 0
                ? 0
                : (double)x.CorrectCount / x.TotalAttempts)
            .ThenByDescending(x => x.TotalAttempts)
            .Take(mostMissedLimit)
            .Select(x => new QuestionAccuracyDto
            {
                QuestionId = x.Id,
                QuestionText = x.QuestionText,
                CategoryName = x.CategoryName,
                TotalAttempts = x.TotalAttempts,
                CorrectCount = x.CorrectCount,
                WrongCount = x.WrongCount,
                SkippedCount = x.SkippedCount,
                AccuracyPercent = x.TotalAttempts == 0
                    ? 0
                    : Math.Round(100.0 * x.CorrectCount / x.TotalAttempts, 1)
            })
            .ToListAsync();

        return new DashboardReportDto
        {
            Summary = summary,
            OverallCompletionRate = overallCompletionRate,
            OverallAverageScore = overallAverageScore,
            CategoryPlays = categoryPlays,
            QuizPlays = quizPlays,
            MostMissedQuestions = mostMissed
        };
    }
}
