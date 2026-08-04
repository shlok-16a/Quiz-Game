using Microsoft.EntityFrameworkCore;
using QuizBackend.Data;
using QuizBackend.DTOs.Quiz;
using QuizBackend.Models;

namespace QuizBackend.Services;

/// <summary>
/// Ranks completed quiz sessions by score (desc), then shortest duration,
/// then who finished first (CompletedAt asc) when duration is tied.
/// Duration is server wall-clock time: CompletedAt − StartedAt.
/// </summary>
public class QuizRankingService
{
    private readonly QuizDbContext _context;

    public QuizRankingService(QuizDbContext context)
    {
        _context = context;
    }

    public async Task<QuizLeaderboardDto?> GetLeaderboardAsync(int quizId)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Category)
            .FirstOrDefaultAsync(q => q.Id == quizId);

        if (quiz == null)
            return null;

        var sessions = await _context.QuizSessions
            .Where(s => s.QuizId == quizId && s.IsCompleted && s.CompletedAt != null)
            .ToListAsync();

        sessions = RankSessions(sessions);

        var userIds = sessions.Select(s => s.UserId).Distinct().ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id);

        var entries = new List<LeaderboardEntryDto>();
        for (var i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];
            users.TryGetValue(session.UserId, out var user);

            entries.Add(new LeaderboardEntryDto
            {
                Rank = i + 1,
                SessionId = session.Id,
                UserId = session.UserId,
                FullName = user?.FullName ?? "Unknown",
                Email = user?.Email ?? "",
                Score = session.Score,
                StartedAt = session.StartedAt,
                CompletedAt = session.CompletedAt,
                DurationSeconds = DurationSeconds(session)
            });
        }

        return new QuizLeaderboardDto
        {
            QuizId = quiz.Id,
            QuizTitle = quiz.Title,
            CategoryName = quiz.Category?.Name ?? "",
            TotalCompletions = entries.Count,
            Entries = entries
        };
    }

    /// <summary>
    /// Rank of a completed session among all completions for the same quiz.
    /// Returns null if session/quiz missing or not completed.
    /// </summary>
    public async Task<(int Rank, int TotalCompletions)?> GetSessionRankAsync(int sessionId)
    {
        var session = await _context.QuizSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null || !session.QuizId.HasValue || !session.IsCompleted || !session.CompletedAt.HasValue)
            return null;

        var sessions = await _context.QuizSessions
            .Where(s => s.QuizId == session.QuizId && s.IsCompleted && s.CompletedAt != null)
            .ToListAsync();

        var ranked = RankSessions(sessions);
        var index = ranked.FindIndex(s => s.Id == sessionId);
        if (index < 0)
            return null;

        return (index + 1, ranked.Count);
    }

    /// <summary>Score high → low, then shorter duration, then earlier finish time, then session id.</summary>
    private static List<QuizSession> RankSessions(List<QuizSession> sessions)
        => sessions
            .OrderByDescending(s => s.Score)
            .ThenBy(DurationSeconds)
            .ThenBy(s => s.CompletedAt)
            .ThenBy(s => s.Id)
            .ToList();

    private static int DurationSeconds(QuizSession session)
    {
        if (!session.CompletedAt.HasValue)
            return int.MaxValue;

        return (int)Math.Max(0, (session.CompletedAt.Value - session.StartedAt).TotalSeconds);
    }
}
