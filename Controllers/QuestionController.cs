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
    public async Task<ActionResult<List<QuestionResponseDto>>> Get(
        [FromQuery] int? categoryId,
        [FromQuery] string? search,
        [FromQuery] string? difficulty)
    {
        return Ok(await _questionService.GetAllAsync(categoryId, search, difficulty));
    }

    [HttpPost]
    public async Task<ActionResult<QuestionResponseDto>> Create(CreateQuestionDto dto)
    {
        var created = await _questionService.AddAsync(dto);

        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateQuestionDto dto)
    {
        try
        {
            var updated = await _questionService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _questionService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ImportQuestionsResultDto>> Import(
        [FromForm] ImportQuestionsFormDto form)
    {
        var file = form.File;
        var categoryId = form.CategoryId;

        if (file == null || file.Length == 0)
            return BadRequest("CSV file is required.");

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only .csv files are supported.");

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await _questionService.ImportFromCsvAsync(categoryId, stream);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
