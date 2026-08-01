using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizBackend.DTOs.Quiz;
using QuizBackend.Services;

namespace QuizBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly QuizService _quizService;

    public QuizController(QuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<StartQuizResponseDto>> StartQuiz(StartQuizRequestDto dto)
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var result = await _quizService.StartQuizAsync(userId, dto);

        return Ok(result);
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
}