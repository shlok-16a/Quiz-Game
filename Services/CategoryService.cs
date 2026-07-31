using Microsoft.EntityFrameworkCore;
using QuizBackend.Data;
using QuizBackend.Models;

namespace QuizBackend.Services;

public class CategoryService
{
    private readonly QuizDbContext _context;

    public CategoryService(QuizDbContext context)
    {
        _context = context;
    }

    public async Task<List<QuizCategory>> GetAllAsync()
    
    {
        return await _context.QuizCategories.ToListAsync();
    }

    public async Task<QuizCategory> AddAsync(QuizCategory category)
{
    _context.QuizCategories.Add(category);

    await _context.SaveChangesAsync();

    return category;
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