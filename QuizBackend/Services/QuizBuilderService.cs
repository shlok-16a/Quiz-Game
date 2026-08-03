using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using QuizBackend.Data;
using QuizBackend.DTOs.Question;
using QuizBackend.DTOs.QuizBuilder;
using QuizBackend.Models;

namespace QuizBackend.Services;

public class QuizBuilderService
{
    private readonly QuizDbContext _context;

    public QuizBuilderService(QuizDbContext context)
    {
        _context = context;
    }

    public async Task<List<QuizResponseDto>> GetAllAsync()
    {
        var quizzes = await _context.Quizzes
            .Include(q => q.Category)
            .Include(q => q.QuizQuestions)
            .OrderByDescending(q => q.Id)
            .ToListAsync();

        return quizzes.Select(Map).ToList();
    }

    public async Task<List<QuizResponseDto>> GetActiveAsync(int? userId = null)
    {
        var now = DateTime.UtcNow;

        var quizzes = await _context.Quizzes
            .Include(q => q.Category)
            .Include(q => q.QuizQuestions)
            .Where(q =>
                q.IsActive &&
                q.QuizQuestions.Any() &&
                (q.StartDate == null || q.StartDate <= now) &&
                (q.EndDate == null || q.EndDate >= now))
            .OrderByDescending(q => q.Id)
            .ToListAsync();

        HashSet<int>? attemptedQuizIds = null;
        if (userId.HasValue)
        {
            attemptedQuizIds = (await _context.QuizSessions
                .Where(s => s.UserId == userId.Value && s.QuizId != null)
                .Select(s => s.QuizId!.Value)
                .Distinct()
                .ToListAsync()).ToHashSet();
        }

        return quizzes.Select(q =>
        {
            var dto = Map(q);
            if (attemptedQuizIds != null)
                dto.HasAttempted = attemptedQuizIds.Contains(q.Id);
            return dto;
        }).ToList();
    }

    public async Task<QuizResponseDto?> GetByIdAsync(int id)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Category)
            .Include(q => q.QuizQuestions)
            .FirstOrDefaultAsync(q => q.Id == id);

        return quiz == null ? null : Map(quiz);
    }

    public async Task<QuizResponseDto> CreateAsync(CreateQuizDto dto)
    {
        var category = await _context.QuizCategories.FindAsync(dto.CategoryId)
            ?? throw new Exception("Category not found.");

        ValidateSchedule(dto.StartDate, dto.EndDate);

        var quiz = new Quiz
        {
            Title = string.IsNullOrWhiteSpace(dto.Title) ? $"{category.Name} Quiz" : dto.Title.Trim(),
            CategoryId = dto.CategoryId,
            QuestionCount = dto.QuestionCount <= 0 ? 10 : dto.QuestionCount,
            DurationSeconds = dto.DurationSeconds <= 0 ? 10 : dto.DurationSeconds,
            IsActive = dto.IsActive,
            StartDate = ToUtc(dto.StartDate),
            EndDate = ToUtc(dto.EndDate)
        };

        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(quiz.Id))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateQuizDto dto)
    {
        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz == null) return false;

        ValidateSchedule(dto.StartDate, dto.EndDate);

        quiz.Title = dto.Title.Trim();
        quiz.QuestionCount = dto.QuestionCount;
        quiz.DurationSeconds = dto.DurationSeconds;
        quiz.IsActive = dto.IsActive;
        quiz.StartDate = ToUtc(dto.StartDate);
        quiz.EndDate = ToUtc(dto.EndDate);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetActiveAsync(int id, bool isActive)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.QuizQuestions)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quiz == null) return false;

        if (isActive && !quiz.QuizQuestions.Any())
            throw new Exception("Cannot activate a quiz with no questions.");

        quiz.IsActive = isActive;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.QuizQuestions)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quiz == null) return false;

        _context.QuizQuestions.RemoveRange(quiz.QuizQuestions);
        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<QuestionResponseDto>> GetQuizQuestionsAsync(int quizId)
    {
        return await _context.QuizQuestions
            .Include(qq => qq.Question)
            .ThenInclude(q => q.Category)
            .Where(qq => qq.QuizId == quizId)
            .OrderBy(qq => qq.QuestionOrder)
            .Select(qq => new QuestionResponseDto
            {
                Id = qq.Question.Id,
                QuestionText = qq.Question.QuestionText,
                Option1 = qq.Question.Option1,
                Option2 = qq.Question.Option2,
                Option3 = qq.Question.Option3,
                CorrectOption = qq.Question.CorrectOption,
                Difficulty = qq.Question.Difficulty,
                CategoryId = qq.Question.CategoryId,
                CategoryName = qq.Question.Category.Name
            })
            .ToListAsync();
    }

    public async Task AddQuestionsAsync(int quizId, List<int> questionIds)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.QuizQuestions)
            .FirstOrDefaultAsync(q => q.Id == quizId)
            ?? throw new Exception("Quiz not found.");

        var existingIds = quiz.QuizQuestions.Select(q => q.QuestionId).ToHashSet();
        var nextOrder = quiz.QuizQuestions.Any()
            ? quiz.QuizQuestions.Max(q => q.QuestionOrder) + 1
            : 1;

        var validQuestions = await _context.Questions
            .Where(q => questionIds.Contains(q.Id) && q.CategoryId == quiz.CategoryId && q.IsActive)
            .ToListAsync();

        foreach (var question in validQuestions)
        {
            if (existingIds.Contains(question.Id))
                continue;

            _context.QuizQuestions.Add(new QuizQuestion
            {
                QuizId = quizId,
                QuestionId = question.Id,
                QuestionOrder = nextOrder++
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveQuestionAsync(int quizId, int questionId)
    {
        var link = await _context.QuizQuestions
            .FirstOrDefaultAsync(q => q.QuizId == quizId && q.QuestionId == questionId)
            ?? throw new Exception("Question not found in this quiz.");

        _context.QuizQuestions.Remove(link);
        await _context.SaveChangesAsync();
    }

    public async Task AddRandomQuestionsAsync(int quizId, int count)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.QuizQuestions)
            .FirstOrDefaultAsync(q => q.Id == quizId)
            ?? throw new Exception("Quiz not found.");

        if (count <= 0)
            count = quiz.QuestionCount;

        var existingIds = quiz.QuizQuestions.Select(q => q.QuestionId).ToHashSet();

        // Questions already used in other quizzes of the same category.
        var usedInOtherQuizzes = await _context.QuizQuestions
            .Where(qq => qq.Quiz.CategoryId == quiz.CategoryId && qq.QuizId != quizId)
            .Select(qq => qq.QuestionId)
            .Distinct()
            .ToListAsync();
        var usedElsewhereIds = usedInOtherQuizzes.ToHashSet();

        var pool = await _context.Questions
            .Where(q => q.CategoryId == quiz.CategoryId && q.IsActive && !existingIds.Contains(q.Id))
            .ToListAsync();

        // Prefer unique (unused in other quizzes) first; only reuse after that pool is exhausted.
        var unused = pool.Where(q => !usedElsewhereIds.Contains(q.Id)).OrderBy(_ => Guid.NewGuid()).ToList();
        var alreadyUsed = pool.Where(q => usedElsewhereIds.Contains(q.Id)).OrderBy(_ => Guid.NewGuid()).ToList();

        var selected = unused.Concat(alreadyUsed).Take(count).ToList();

        if (!selected.Any())
            throw new Exception("No available questions left in this category.");

        var nextOrder = quiz.QuizQuestions.Any()
            ? quiz.QuizQuestions.Max(q => q.QuestionOrder) + 1
            : 1;

        foreach (var question in selected)
        {
            _context.QuizQuestions.Add(new QuizQuestion
            {
                QuizId = quizId,
                QuestionId = question.Id,
                QuestionOrder = nextOrder++
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<ImportQuestionsResultDto> ImportCsvToQuizAsync(int quizId, Stream csvStream)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.QuizQuestions)
            .FirstOrDefaultAsync(q => q.Id == quizId)
            ?? throw new Exception("Quiz not found.");

        var result = new ImportQuestionsResultDto();
        var newQuestions = new List<Question>();

        using var reader = new StreamReader(csvStream, Encoding.UTF8);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            BadDataFound = null
        });

        var rowNumber = 1;

        await foreach (var record in csv.GetRecordsAsync<CsvQuestionRow>())
        {
            rowNumber++;

            if (string.IsNullOrWhiteSpace(record.QuestionText))
            {
                result.Failed++;
                result.Errors.Add($"Row {rowNumber} : Question Text Missing");
                continue;
            }

            if (string.IsNullOrWhiteSpace(record.Option1) ||
                string.IsNullOrWhiteSpace(record.Option2) ||
                string.IsNullOrWhiteSpace(record.Option3))
            {
                result.Failed++;
                result.Errors.Add($"Row {rowNumber} : Options Missing");
                continue;
            }

            if (record.CorrectOption < 1 || record.CorrectOption > 3)
            {
                result.Failed++;
                result.Errors.Add($"Row {rowNumber} : Invalid Correct Option");
                continue;
            }

            newQuestions.Add(new Question
            {
                QuestionText = record.QuestionText.Trim(),
                Option1 = record.Option1.Trim(),
                Option2 = record.Option2.Trim(),
                Option3 = record.Option3.Trim(),
                CorrectOption = record.CorrectOption,
                Difficulty = string.IsNullOrWhiteSpace(record.Difficulty) ? "Easy" : record.Difficulty.Trim(),
                CategoryId = quiz.CategoryId,
                CreatedBy = "Admin"
            });
        }

        if (newQuestions.Count == 0)
            return result;

        _context.Questions.AddRange(newQuestions);
        await _context.SaveChangesAsync();

        var nextOrder = quiz.QuizQuestions.Any()
            ? quiz.QuizQuestions.Max(q => q.QuestionOrder) + 1
            : 1;

        foreach (var question in newQuestions)
        {
            _context.QuizQuestions.Add(new QuizQuestion
            {
                QuizId = quizId,
                QuestionId = question.Id,
                QuestionOrder = nextOrder++
            });
        }

        await _context.SaveChangesAsync();
        result.Imported = newQuestions.Count;
        return result;
    }

    private static void ValidateSchedule(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && endDate <= startDate)
            throw new Exception("Active end time must be after the start time.");
    }

    private static DateTime? ToUtc(DateTime? value)
    {
        if (!value.HasValue) return null;
        return value.Value.Kind switch
        {
            DateTimeKind.Utc => value.Value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        };
    }

    private static QuizResponseDto Map(Quiz q) => new()
    {
        Id = q.Id,
        Title = q.Title,
        CategoryId = q.CategoryId,
        CategoryName = q.Category.Name,
        QuestionCount = q.QuestionCount,
        AssignedQuestions = q.QuizQuestions.Count,
        DurationSeconds = q.DurationSeconds,
        IsActive = q.IsActive,
        StartDate = AsUtc(q.StartDate),
        EndDate = AsUtc(q.EndDate),
        CreatedAt = AsUtc(q.CreatedAt)
    };

    private static DateTime? AsUtc(DateTime? value)
        => value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;

    private static DateTime AsUtc(DateTime value)
        => DateTime.SpecifyKind(value, DateTimeKind.Utc);

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
