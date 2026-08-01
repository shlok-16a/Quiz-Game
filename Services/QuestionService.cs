using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
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

    public async Task<List<QuestionResponseDto>> GetAllAsync(
        int? categoryId = null,
        string? search = null,
        string? difficulty = null)
    {
        var query = _context.Questions.AsQueryable();

        if (categoryId.HasValue && categoryId.Value > 0)
            query = query.Where(q => q.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(q => q.QuestionText.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(difficulty))
            query = query.Where(q => q.Difficulty == difficulty);

        return await query
            .OrderByDescending(q => q.Id)
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

    public async Task<bool> UpdateAsync(int id, UpdateQuestionDto dto)
    {
        var question = await _context.Questions.FindAsync(id);

        if (question == null)
            return false;

        var category = await _context.QuizCategories.FindAsync(dto.CategoryId);

        if (category == null)
            throw new Exception("Category not found.");

        question.QuestionText = dto.QuestionText;
        question.Option1 = dto.Option1;
        question.Option2 = dto.Option2;
        question.Option3 = dto.Option3;
        question.CorrectOption = dto.CorrectOption;
        question.Difficulty = dto.Difficulty;
        question.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();

        return true;
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

    public async Task<ImportQuestionsResultDto> ImportFromCsvAsync(int categoryId, Stream csvStream)
    {
        var result = new ImportQuestionsResultDto();

        var category = await _context.QuizCategories.FindAsync(categoryId);

        if (category == null)
            throw new Exception("Category not found.");

        using var reader = new StreamReader(csvStream, Encoding.UTF8);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            BadDataFound = null
        });

        var rowNumber = 1; // header
        var questionsToAdd = new List<Question>();

        await foreach (var record in csv.GetRecordsAsync<CsvQuestionRow>())
        {
            rowNumber++;

            var error = ValidateCsvRow(record, rowNumber);

            if (error != null)
            {
                result.Failed++;
                result.Errors.Add(error);
                continue;
            }

            questionsToAdd.Add(new Question
            {
                QuestionText = record.QuestionText.Trim(),
                Option1 = record.Option1.Trim(),
                Option2 = record.Option2.Trim(),
                Option3 = record.Option3.Trim(),
                CorrectOption = record.CorrectOption,
                Difficulty = string.IsNullOrWhiteSpace(record.Difficulty)
                    ? "Easy"
                    : record.Difficulty.Trim(),
                CategoryId = categoryId,
                CreatedBy = "Admin"
            });
        }

        if (questionsToAdd.Count > 0)
        {
            _context.Questions.AddRange(questionsToAdd);
            await _context.SaveChangesAsync();
            result.Imported = questionsToAdd.Count;
        }

        return result;
    }

    private static string? ValidateCsvRow(CsvQuestionRow row, int rowNumber)
    {
        if (string.IsNullOrWhiteSpace(row.QuestionText))
            return $"Row {rowNumber} : Question Text Missing";

        if (string.IsNullOrWhiteSpace(row.Option1) ||
            string.IsNullOrWhiteSpace(row.Option2) ||
            string.IsNullOrWhiteSpace(row.Option3))
            return $"Row {rowNumber} : Options Missing";

        if (row.CorrectOption < 1 || row.CorrectOption > 3)
            return $"Row {rowNumber} : Invalid Correct Option";

        return null;
    }

    private class CsvQuestionRow
    {
        public string QuestionText { get; set; } = string.Empty;
        public string Option1 { get; set; } = string.Empty;
        public string Option2 { get; set; } = string.Empty;
        public string Option3 { get; set; } = string.Empty;
        public int CorrectOption { get; set; }
        public string Difficulty { get; set; } = "Easy";
    }
}
