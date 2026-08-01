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
}
