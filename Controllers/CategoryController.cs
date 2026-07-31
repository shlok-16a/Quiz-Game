using Microsoft.AspNetCore.Mvc;

namespace QuizBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    public IActionResult GetCategories()
    {
        var categories = new[]
        {
            new { Id = 1, Name = "BGMI Quiz" },
            new { Id = 2, Name = "Valorant Quiz" },
            new { Id = 3, Name = "Free Fire Quiz" }
        };

        return Ok(categories);
    }
}