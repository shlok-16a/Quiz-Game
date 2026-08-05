using QuizBackend.Data;
using Microsoft.EntityFrameworkCore;
using QuizBackend.DTOs.Quiz;
using QuizBackend.Models;

namespace QuizBackend.Services;

public class QuizService
{
    private readonly QuizDbContext _context;
    private readonly QuizRankingService _rankingService;

    public QuizService(QuizDbContext context, QuizRankingService rankingService)
    {
        _context = context;
        _rankingService = rankingService;
    }

    public async Task<StartQuizResponseDto> StartQuizAsync(int userId, StartQuizRequestDto dto)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Category)
            .Include(q => q.QuizQuestions)
            .ThenInclude(qq => qq.Question)
            .FirstOrDefaultAsync(q => q.Id == dto.QuizId);

        if (quiz == null)
            throw new Exception("Quiz not found.");

        if (!quiz.IsActive)
            throw new Exception("This quiz is not active.");

        var now = DateTime.UtcNow;
        if (quiz.StartDate.HasValue && now < quiz.StartDate.Value)
            throw new Exception("This quiz is not available yet.");

        if (quiz.EndDate.HasValue && now > quiz.EndDate.Value)
            throw new Exception("This quiz is no longer available.");

        var alreadyAttempted = await _context.QuizSessions
            .AnyAsync(s => s.UserId == userId && s.QuizId == dto.QuizId);

        if (alreadyAttempted)
            throw new Exception("You have already attempted this quiz.");

        // Use exactly the questions admin assigned to this quiz
        // (including later-deactivated ones so existing quizzes stay intact).
        var pool = quiz.QuizQuestions.ToList();

        if (!pool.Any())
            throw new Exception("This quiz has no questions.");

        var takeCount = quiz.QuestionCount > 0
            ? Math.Min(quiz.QuestionCount, pool.Count)
            : pool.Count;

        // Shuffle question order for this user session (independent of admin order).
        var selectedLinks = pool
            .OrderBy(_ => Guid.NewGuid())
            .Take(takeCount)
            .ToList();

        var session = new QuizSession
        {
            UserId = userId,
            QuizId = quiz.Id,
            CategoryId = quiz.CategoryId,
            StartedAt = DateTime.UtcNow,
            Score = 0,
            CurrentQuestionIndex = 0,
            IsCompleted = false
        };

        _context.QuizSessions.Add(session);
        await _context.SaveChangesAsync();

        var sessionQuestions = new List<QuizSessionQuestion>();
        int order = 1;
        foreach (var link in selectedLinks)
        {
            var sessionQuestion = new QuizSessionQuestion
            {
                QuizSessionId = session.Id,
                QuestionId = link.QuestionId,
                QuestionOrder = order++,
                OptionOrder = CreateShuffledOptionOrder()
            };

            sessionQuestions.Add(sessionQuestion);
            _context.QuizSessionQuestions.Add(sessionQuestion);
        }

        await _context.SaveChangesAsync();

        var firstSessionQuestion = sessionQuestions.First();
        var firstLink = selectedLinks.First();
        var defaultSeconds = quiz.DurationSeconds > 0 ? quiz.DurationSeconds : 10;
        var firstTimer = ResolveQuestionTimer(quiz, firstLink, defaultSeconds);

        return new StartQuizResponseDto
        {
            SessionId = session.Id,
            TotalQuestions = selectedLinks.Count,
            QuestionTimerSeconds = firstTimer,
            DurationSeconds = firstTimer,
            Title = quiz.Title,
            FirstQuestion = ToPlayQuestion(firstLink.Question, firstSessionQuestion.OptionOrder, firstTimer)
        };
    }

    public async Task<SubmitAnswerResponseDto> SubmitAnswerAsync(
        SubmitAnswerRequestDto dto)
    {
        var session = await _context.QuizSessions
            .FirstOrDefaultAsync(s => s.Id == dto.SessionId);

        if (session == null)
        {
            throw new Exception("Quiz session not found.");
        }

        if (session.IsCompleted)
        {
            throw new Exception("Quiz has already been completed.");
        }

        var sessionQuestion = await _context.QuizSessionQuestions
            .Include(sq => sq.Question)
            .FirstOrDefaultAsync(sq =>
                sq.QuizSessionId == dto.SessionId &&
                sq.QuestionId == dto.QuestionId);

        if (sessionQuestion?.Question == null)
        {
            throw new Exception("Question not found in this quiz session.");
        }

        var question = sessionQuestion.Question;
        var optionOrder = sessionQuestion.OptionOrder;

        var quiz = session.QuizId.HasValue
            ? await _context.Quizzes
                .Include(q => q.QuizQuestions)
                .FirstOrDefaultAsync(q => q.Id == session.QuizId.Value)
            : null;

        var category = await _context.QuizCategories
            .FirstOrDefaultAsync(c => c.Id == session.CategoryId);

        if (category == null)
        {
            throw new Exception("Category not found.");
        }

        bool isCorrect;
        int points = 0;
        int bonusAwarded = 0;
        var timeTaken = Math.Max(0, dto.TimeTakenSeconds);

        // Display position (1-3) from UI → original option index for scoring
        var selectedOriginal = dto.SelectedOption == 0
            ? 0
            : DisplayToOriginal(dto.SelectedOption, optionOrder);

        if (selectedOriginal == 0)
        {
            isCorrect = false;
            points = 0;
        }
        else
        {
            isCorrect = question.CorrectOption == selectedOriginal;

            if (isCorrect)
            {
                points = category.CorrectPoints;

                if (quiz != null)
                {
                    var defaultSeconds = quiz.DurationSeconds > 0 ? quiz.DurationSeconds : 10;
                    var link = quiz.QuizQuestions.FirstOrDefault(qq => qq.QuestionId == question.Id);
                    var questionTimer = link != null
                        ? ResolveQuestionTimer(quiz, link, defaultSeconds)
                        : defaultSeconds;

                    if (quiz.UsePerQuestionTimer)
                    {
                        // Flat bonus when answered within the first BonusTimePercent of this question's timer.
                        if (quiz.BonusPoints > 0 && quiz.BonusTimePercent > 0)
                        {
                            var bonusWindowSeconds = questionTimer * quiz.BonusTimePercent / 100.0;
                            if (timeTaken <= bonusWindowSeconds)
                            {
                                bonusAwarded = quiz.BonusPoints;
                                points += bonusAwarded;
                            }
                        }
                    }
                    else
                    {
                        // Global timer: remaining seconds on a correct answer are the bonus (time = bonus).
                        bonusAwarded = Math.Max(0, questionTimer - timeTaken);
                        points += bonusAwarded;
                    }
                }
            }
            else
            {
                points = category.WrongPoints;
            }
        }

        _context.UserAnswers.Add(new UserAnswer
        {
            QuizSessionId = session.Id,
            QuestionId = question.Id,
            SelectedOption = selectedOriginal,
            IsCorrect = isCorrect,
            PointsAwarded = points,
            TimeTakenSeconds = timeTaken
        });

        session.Score += points;
        session.CurrentQuestionIndex++;

        await _context.SaveChangesAsync();

        // Correct option as shown on screen (shuffled display index)
        var correctDisplayOption = OriginalToDisplay(question.CorrectOption, optionOrder);

        var sessionQuestions = await _context.QuizSessionQuestions
            .Include(sq => sq.Question)
            .Where(q => q.QuizSessionId == session.Id)
            .OrderBy(q => q.QuestionOrder)
            .ToListAsync();

        if (session.CurrentQuestionIndex >= sessionQuestions.Count)
        {
            session.IsCompleted = true;
            session.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new SubmitAnswerResponseDto
            {
                IsCorrect = isCorrect,
                Score = session.Score,
                QuizCompleted = true,
                CorrectOption = correctDisplayOption,
                PointsAwarded = points,
                BonusAwarded = bonusAwarded,
                NextQuestion = null
            };
        }

        var nextSessionQuestion = sessionQuestions[session.CurrentQuestionIndex];
        var defaultSecondsNext = quiz?.DurationSeconds > 0 ? quiz.DurationSeconds : 10;
        var nextLink = quiz?.QuizQuestions.FirstOrDefault(qq => qq.QuestionId == nextSessionQuestion.QuestionId);
        var nextTimer = quiz != null && nextLink != null
            ? ResolveQuestionTimer(quiz, nextLink, defaultSecondsNext)
            : defaultSecondsNext;

        return new SubmitAnswerResponseDto
        {
            IsCorrect = isCorrect,
            Score = session.Score,
            QuizCompleted = false,
            CorrectOption = correctDisplayOption,
            PointsAwarded = points,
            BonusAwarded = bonusAwarded,
            NextQuestion = ToPlayQuestion(nextSessionQuestion.Question, nextSessionQuestion.OptionOrder, nextTimer)
        };
    }

    public async Task<QuizResultDto> GetQuizResultAsync(int sessionId)
    {
        var session = await _context.QuizSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
        {
            throw new Exception("Quiz session not found.");
        }

        var answers = await _context.UserAnswers
            .Where(a => a.QuizSessionId == sessionId)
            .ToListAsync();

        int correctAnswers = answers.Count(a => a.IsCorrect);

        int skippedAnswers = answers.Count(a => a.SelectedOption == 0);

        int wrongAnswers = answers.Count(a =>
            !a.IsCorrect && a.SelectedOption != 0);

        int totalQuestions = answers.Count;

        var category = await _context.QuizCategories
            .FirstOrDefaultAsync(c => c.Id == session.CategoryId);

        var correctPoints = category?.CorrectPoints ?? 0;
        var bonusAnswers = answers.Count(a =>
            a.IsCorrect && a.PointsAwarded > correctPoints);
        var bonusPoints = answers
            .Where(a => a.IsCorrect && a.PointsAwarded > correctPoints)
            .Sum(a => a.PointsAwarded - correctPoints);

        double percentage = totalQuestions == 0
            ? 0
            : (double)correctAnswers / totalQuestions * 100;

        var durationSeconds = session.CompletedAt.HasValue
            ? (int)Math.Max(0, (session.CompletedAt.Value - session.StartedAt).TotalSeconds)
            : 0;

        int? rank = null;
        var totalCompletions = 0;
        var rankInfo = await _rankingService.GetSessionRankAsync(sessionId);
        if (rankInfo.HasValue)
        {
            rank = rankInfo.Value.Rank;
            totalCompletions = rankInfo.Value.TotalCompletions;
        }

        return new QuizResultDto
        {
            SessionId = session.Id,
            Score = session.Score,
            CorrectAnswers = correctAnswers,
            WrongAnswers = wrongAnswers,
            SkippedAnswers = skippedAnswers,
            TotalQuestions = totalQuestions,
            BonusPoints = bonusPoints,
            BonusAnswers = bonusAnswers,
            Percentage = percentage,
            StartedAt = session.StartedAt,
            CompletedAt = session.CompletedAt,
            DurationSeconds = durationSeconds,
            Rank = rank,
            TotalCompletions = totalCompletions
        };
    }

    public async Task<List<QuizHistoryDto>> GetQuizHistoryAsync(int userId)
    {
        throw new NotImplementedException();
    }

    private static int ResolveQuestionTimer(Quiz quiz, QuizQuestion link, int defaultSeconds)
    {
        if (quiz.UsePerQuestionTimer && link.TimerSeconds > 0)
            return link.TimerSeconds;

        return defaultSeconds > 0 ? defaultSeconds : 10;
    }

    /// <summary>Random display order of original options, e.g. "3,1,2".</summary>
    private static string CreateShuffledOptionOrder()
        => string.Join(",", new[] { 1, 2, 3 }.OrderBy(_ => Guid.NewGuid()));

    private static int[] ParseOptionOrder(string? optionOrder)
    {
        var parts = (optionOrder ?? "1,2,3")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length != 3)
            return new[] { 1, 2, 3 };

        var nums = new int[3];
        for (var i = 0; i < 3; i++)
        {
            if (!int.TryParse(parts[i], out nums[i]) || nums[i] < 1 || nums[i] > 3)
                return new[] { 1, 2, 3 };
        }

        if (nums.Distinct().Count() != 3)
            return new[] { 1, 2, 3 };

        return nums;
    }

    private static PlayQuestionDto ToPlayQuestion(Question question, string optionOrder, int timerSeconds)
    {
        var order = ParseOptionOrder(optionOrder);
        var options = new[] { question.Option1, question.Option2, question.Option3 };

        return new PlayQuestionDto
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            Option1 = options[order[0] - 1],
            Option2 = options[order[1] - 1],
            Option3 = options[order[2] - 1],
            TimerSeconds = timerSeconds > 0 ? timerSeconds : 10
        };
    }

    /// <summary>Map UI button (1-3) to original option index using session shuffle.</summary>
    private static int DisplayToOriginal(int displayOption, string optionOrder)
    {
        if (displayOption < 1 || displayOption > 3)
            return 0;

        var order = ParseOptionOrder(optionOrder);
        return order[displayOption - 1];
    }

    /// <summary>Map original correct option to UI button index after shuffle.</summary>
    private static int OriginalToDisplay(int originalOption, string optionOrder)
    {
        var order = ParseOptionOrder(optionOrder);
        var index = Array.IndexOf(order, originalOption);
        return index >= 0 ? index + 1 : originalOption;
    }
}
