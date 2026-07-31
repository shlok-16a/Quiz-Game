using Microsoft.AspNetCore.Mvc;
using QuizBackend.DTOs.Question;
using QuizBackend.Services;

namespace QuizBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{
    private readonly QuestionService _questionService;

    public QuestionController(QuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuestionResponseDto>>> Get()
    {
        return Ok(await _questionService.GetAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<QuestionResponseDto>> Create(CreateQuestionDto dto)
    {
        var created = await _questionService.AddAsync(dto);

        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _questionService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}