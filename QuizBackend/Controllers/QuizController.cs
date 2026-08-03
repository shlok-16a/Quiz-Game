using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizBackend.DTOs.Quiz;
using QuizBackend.DTOs.QuizBuilder;
using QuizBackend.Services;

namespace QuizBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly QuizService _quizService;
    private readonly QuizBuilderService _quizBuilderService;

    public QuizController(QuizService quizService, QuizBuilderService quizBuilderService)
    {
        _quizService = quizService;
        _quizBuilderService = quizBuilderService;
    }

    [HttpGet("available")]
    public async Task<ActionResult<List<QuizResponseDto>>> GetAvailable()
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        return Ok(await _quizBuilderService.GetActiveAsync(userId));
    }

    [HttpPost("start")]
    public async Task<ActionResult<StartQuizResponseDto>> StartQuiz(StartQuizRequestDto dto)
    {
        try
        {
            var userId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var result = await _quizService.StartQuizAsync(userId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("answer")]
    public async Task<ActionResult<SubmitAnswerResponseDto>> SubmitAnswer(
        SubmitAnswerRequestDto dto)
    {
        var result = await _quizService.SubmitAnswerAsync(dto);

        return Ok(result);
    }

    [HttpGet("result/{sessionId}")]
    public async Task<ActionResult<QuizResultDto>> GetResult(int sessionId)
    {
        var result = await _quizService.GetQuizResultAsync(sessionId);

        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<QuizHistoryDto>>> GetHistory()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var result = await _quizService.GetQuizHistoryAsync(userId);

        return Ok(result);
    }
}
