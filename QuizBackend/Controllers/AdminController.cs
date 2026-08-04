using Microsoft.AspNetCore.Mvc;
using QuizBackend.DTOs.Admin;
using QuizBackend.DTOs.Quiz;
using QuizBackend.Services;

namespace QuizBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AdminService _adminService;
    private readonly QuizRankingService _rankingService;

    public AdminController(AdminService adminService, QuizRankingService rankingService)
    {
        _adminService = adminService;
        _rankingService = rankingService;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        return Ok(await _adminService.GetDashboardStatsAsync());
    }

    /// <summary>
    /// Basic reporting: plays per category/quiz, completion rate, average score, most-missed questions.
    /// </summary>
    [HttpGet("report")]
    public async Task<ActionResult<DashboardReportDto>> GetReport(
        [FromQuery] int mostMissed = 10,
        [FromQuery] int? categoryId = null)
    {
        return Ok(await _adminService.GetDashboardReportAsync(mostMissed, categoryId));
    }

    /// <summary>
    /// Leaderboard for a quiz: highest score first; then shortest duration; then earlier finish time.
    /// </summary>
    [HttpGet("leaderboard/{quizId}")]
    public async Task<ActionResult<QuizLeaderboardDto>> GetLeaderboard(int quizId)
    {
        var leaderboard = await _rankingService.GetLeaderboardAsync(quizId);
        return leaderboard == null ? NotFound("Quiz not found.") : Ok(leaderboard);
    }
}
