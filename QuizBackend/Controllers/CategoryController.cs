using Microsoft.AspNetCore.Mvc;
using QuizBackend.DTOs.Category;
using QuizBackend.Services;

namespace QuizBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;
    private readonly CoverImageService _coverImageService;

    public CategoryController(CategoryService categoryService, CoverImageService coverImageService)
    {
        _categoryService = categoryService;
        _coverImageService = coverImageService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryResponseDto>>> Get()
    {
        return Ok(await _categoryService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
            return NotFound($"Category with ID {id} not found.");

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> Create(CreateCategoryDto dto)
    {
        var created = await _categoryService.AddAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
    {
        var updated = await _categoryService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound($"Category with ID {id} not found.");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);

        if (!deleted)
            return NotFound($"Category with ID {id} not found.");

        return NoContent();
    }

    [HttpPost("upload-cover")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(CoverImageService.MaxBytes + 512_000)]
    public async Task<ActionResult<UploadCoverImageResultDto>> UploadCover(
        [FromForm] UploadCoverImageFormDto form)
    {
        try
        {
            var result = await _coverImageService.SaveAsync(form.File);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
