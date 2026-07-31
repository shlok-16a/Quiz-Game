using Microsoft.EntityFrameworkCore;
using QuizBackend.Data;
using QuizBackend.DTOs.Question;
using QuizBackend.Models;

namespace QuizBackend.Services;

public class QuestionService
{
    private readonly QuizDbContext _context;

    public QuestionService(QuizDbContext context)
    {
        _context = context;
    }

    public async Task<List<QuestionResponseDto>> GetAllAsync()
    {
        return await _context.Questions
            .Include(q => q.Category)
            .Select(q => new QuestionResponseDto
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                Option1 = q.Option1,
                Option2 = q.Option2,
                Option3 = q.Option3,
                CorrectOption = q.CorrectOption,
                Difficulty = q.Difficulty,
                CategoryId = q.CategoryId,
                CategoryName = q.Category.Name
            })
            .ToListAsync();
    }

    public async Task<QuestionResponseDto> AddAsync(CreateQuestionDto dto)
    {
        var category = await _context.QuizCategories.FindAsync(dto.CategoryId);

        if (category == null)
            throw new Exception("Category not found.");

        var question = new Question
        {
            QuestionText = dto.QuestionText,
            Option1 = dto.Option1,
            Option2 = dto.Option2,
            Option3 = dto.Option3,
            CorrectOption = dto.CorrectOption,
            Difficulty = dto.Difficulty,
            CategoryId = dto.CategoryId
        };

        _context.Questions.Add(question);

        await _context.SaveChangesAsync();

        return new QuestionResponseDto
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            Option1 = question.Option1,
            Option2 = question.Option2,
            Option3 = question.Option3,
            CorrectOption = question.CorrectOption,
            Difficulty = question.Difficulty,
            CategoryId = category.Id,
            CategoryName = category.Name
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var question = await _context.Questions.FindAsync(id);

        if (question == null)
            return false;

        _context.Questions.Remove(question);

        await _context.SaveChangesAsync();

        return true;
    }
}   