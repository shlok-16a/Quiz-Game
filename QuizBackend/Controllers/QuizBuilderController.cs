using Microsoft.AspNetCore.Mvc;
using QuizBackend.DTOs.Question;
using QuizBackend.DTOs.QuizBuilder;
using QuizBackend.Services;

namespace QuizBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizBuilderController : ControllerBase
{
    private readonly QuizBuilderService _service;

    public QuizBuilderController(QuizBuilderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuizResponseDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("active")]
    public async Task<ActionResult<List<QuizResponseDto>>> GetActive()
        => Ok(await _service.GetActiveAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<QuizResponseDto>> GetById(int id)
    {
        var quiz = await _service.GetByIdAsync(id);
        return quiz == null ? NotFound() : Ok(quiz);
    }

    [HttpPost]
    public async Task<ActionResult<QuizResponseDto>> Create(CreateQuizDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateQuizDto dto)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/active")]
    public async Task<IActionResult> SetActive(int id, SetQuizActiveDto dto)
    {
        try
        {
            var ok = await _service.SetActiveAsync(id, dto.IsActive);
            return ok ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("{id}/questions")]
    public async Task<ActionResult<List<QuestionResponseDto>>> GetQuestions(int id)
        => Ok(await _service.GetQuizQuestionsAsync(id));

    [HttpPost("{id}/questions")]
    public async Task<IActionResult> AddQuestions(int id, AddQuizQuestionsDto dto)
    {
        try
        {
            await _service.AddQuestionsAsync(id, dto.QuestionIds);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}/questions/{questionId}")]
    public async Task<IActionResult> RemoveQuestion(int id, int questionId)
    {
        try
        {
            await _service.RemoveQuestionAsync(id, questionId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/questions/{questionId}/timer")]
    public async Task<IActionResult> UpdateQuestionTimer(
        int id,
        int questionId,
        UpdateQuizQuestionTimerDto dto)
    {
        try
        {
            await _service.UpdateQuestionTimerAsync(id, questionId, dto.TimerSeconds);
            return NoContent();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/random")]
    public async Task<IActionResult> AddRandom(int id, RandomQuizQuestionsDto dto)
    {
        try
        {
            await _service.AddRandomQuestionsAsync(id, dto.Count);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/import")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ImportQuestionsResultDto>> Import(
        int id,
        [FromForm] QuizImportFormDto form)
    {
        if (form.File == null || form.File.Length == 0)
            return BadRequest("CSV file is required.");

        try
        {
            await using var stream = form.File.OpenReadStream();
            var result = await _service.ImportCsvToQuizAsync(id, stream);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
