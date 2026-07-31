using Microsoft.AspNetCore.Mvc;
using QuizBackend.Models;
using QuizBackend.Services;
using QuizBackend.DTOs.Category;

namespace QuizBackend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
public async Task<ActionResult<List<CategoryResponseDto>>> Get()
{
    return Ok(await _categoryService.GetAllAsync());
}

    [HttpPost]
public async Task<ActionResult<CategoryResponseDto>> Create(CreateCategoryDto dto)
{
    var created = await _categoryService.AddAsync(dto);

    return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
}

[HttpGet("{id}")]
public async Task<ActionResult<QuizCategory>> GetById(int id)
{
    var category = await _categoryService.GetByIdAsync(id);

    if (category == null)
    {
        return NotFound($"Category with ID {id} not found.");
    }

    return Ok(category);
}

[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, QuizCategory category)
{
    var updated = await _categoryService.UpdateAsync(id, category);

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
}