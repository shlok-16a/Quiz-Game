using Microsoft.EntityFrameworkCore;
using QuizBackend.Data;
using QuizBackend.Models;
using QuizBackend.DTOs.Category;

namespace QuizBackend.Services;

public class CategoryService
{
    private readonly QuizDbContext _context;

    public CategoryService(QuizDbContext context)
    {
        _context = context;
    }

   public async Task<List<CategoryResponseDto>> GetAllAsync()
{
    return await _context.QuizCategories
        .Select(c => new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            CoverImageUrl = c.CoverImageUrl,
            RulesText = c.RulesText,
            CorrectPoints = c.CorrectPoints,
            WrongPoints = c.WrongPoints,
            QuestionCount = c.QuestionCount,
            QuestionTimerSeconds = c.QuestionTimerSeconds,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            IsActive = c.IsActive
        })
        .ToListAsync();
}

    public async Task<CategoryResponseDto> AddAsync(CreateCategoryDto dto)
{
    var category = new QuizCategory
    {
        Name = dto.Name,
        Description = dto.Description,
        CoverImageUrl = dto.CoverImageUrl,
        RulesText = dto.RulesText,
        CorrectPoints = dto.CorrectPoints,
        WrongPoints = dto.WrongPoints,
        QuestionCount = dto.QuestionCount,
        QuestionTimerSeconds = dto.QuestionTimerSeconds,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        IsActive = dto.IsActive
    };

    _context.QuizCategories.Add(category);
    await _context.SaveChangesAsync();

    return new CategoryResponseDto
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        CoverImageUrl = category.CoverImageUrl,
        RulesText = category.RulesText,
        CorrectPoints = category.CorrectPoints,
        WrongPoints = category.WrongPoints,
        QuestionCount = category.QuestionCount,
        QuestionTimerSeconds = category.QuestionTimerSeconds,
        StartDate = category.StartDate,
        EndDate = category.EndDate,
        IsActive = category.IsActive
    };
}

public async Task<QuizCategory?> GetByIdAsync(int id)
{
    return await _context.QuizCategories.FindAsync(id);
}

public async Task<bool> UpdateAsync(int id, QuizCategory updatedCategory)
{
    var category = await _context.QuizCategories.FindAsync(id);

    if (category == null)
        return false;

    category.Name = updatedCategory.Name;
    category.Description = updatedCategory.Description;
    category.IsActive = updatedCategory.IsActive;

    await _context.SaveChangesAsync();

    return true;
}

public async Task<bool> DeleteAsync(int id)
{
    var category = await _context.QuizCategories.FindAsync(id);

    if (category == null)
        return false;

    _context.QuizCategories.Remove(category);

    await _context.SaveChangesAsync();

    return true;
}
}