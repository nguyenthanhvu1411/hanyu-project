using HanYu.Application.Common.Models;
using HanYu.Application.Features.Quiz.Admin.Attempts;
using HanYu.Application.Interfaces.Quiz;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Quiz;

public sealed class QuizAttemptAdminService : IQuizAttemptAdminService
{
    private readonly HanYuDbContext _dbContext;

    public QuizAttemptAdminService(HanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<AdminQuizAttemptResponse>>> GetAttemptsAsync(
        AdminQuizAttemptQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = PaginationDefaults.NormalizePage(query.Page);
        var pageSize = PaginationDefaults.NormalizePageSize(query.PageSize);
        var source = ApplyFilters(BaseQuery(), query);
        var total = await source.CountAsync(cancellationToken);
        var attempts = await source
            .OrderByDescending(x => x.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = attempts.Select(MapAttempt).ToList();
        return Result.Success(new PagedResult<AdminQuizAttemptResponse>(items, page, pageSize, total));
    }

    public async Task<Result<AdminQuizAttemptDetailResponse>> GetAttemptAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _dbContext.Set<QuizAttempt>()
            .AsNoTracking()
            .Include(x => x.User)
                .ThenInclude(x => x.Profile)
            .Include(x => x.Quiz)
            .Include(x => x.Answers)
                .ThenInclude(x => x.Question)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (attempt is null)
        {
            return Result.Failure<AdminQuizAttemptDetailResponse>(
                Error.NotFound("Quiz.AttemptNotFound", "Không tìm thấy lượt làm bài."));
        }

        var answers = attempt.Answers
            .OrderBy(x => x.Question.SortOrder)
            .Select(x => new AdminQuizAttemptAnswerResponse(
                x.Id,
                x.Question.Prompt,
                x.Question.PromptPinyin,
                x.AnswerText,
                x.IsCorrect,
                x.EarnedPoints,
                x.ResponseTimeMs,
                x.AnsweredAt))
            .ToList();

        return Result.Success(new AdminQuizAttemptDetailResponse(MapAttempt(attempt), answers));
    }

    public async Task<Result<AdminQuizAttemptStatisticsResponse>> GetStatisticsAsync(
        AdminQuizAttemptQuery query,
        CancellationToken cancellationToken = default)
    {
        var source = ApplyFilters(_dbContext.Set<QuizAttempt>().AsNoTracking(), query);
        var today = DateTimeOffset.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var total = await source.LongCountAsync(cancellationToken);
        var inProgress = await source.LongCountAsync(x => x.Status == QuizAttemptStatus.InProgress, cancellationToken);
        var submitted = await source.LongCountAsync(x => x.Status == QuizAttemptStatus.Submitted, cancellationToken);
        var passed = await source.LongCountAsync(x => x.IsPassed == true, cancellationToken);
        var failed = await source.LongCountAsync(x => x.IsPassed == false, cancellationToken);
        var attemptsToday = await source.LongCountAsync(x => x.StartedAt >= today && x.StartedAt < tomorrow, cancellationToken);
        var percentages = source.Where(x => x.Percentage.HasValue).Select(x => x.Percentage!.Value);
        var averagePercentage = await percentages.AnyAsync(cancellationToken)
            ? await percentages.AverageAsync(cancellationToken)
            : 0m;
        var evaluated = passed + failed;
        var passRate = evaluated > 0 ? Math.Round(passed * 100m / evaluated, 2) : 0m;

        return Result.Success(new AdminQuizAttemptStatisticsResponse(
            total,
            inProgress,
            submitted,
            passed,
            failed,
            Math.Round(averagePercentage, 2),
            passRate,
            attemptsToday));
    }

    private IQueryable<QuizAttempt> BaseQuery()
        => _dbContext.Set<QuizAttempt>()
            .AsNoTracking()
            .Include(x => x.User)
                .ThenInclude(x => x.Profile)
            .Include(x => x.Quiz);

    private static IQueryable<QuizAttempt> ApplyFilters(
        IQueryable<QuizAttempt> source,
        AdminQuizAttemptQuery query)
    {
        if (query.UserId.HasValue)
            source = source.Where(x => x.UserId == query.UserId.Value);
        if (query.QuizId.HasValue)
            source = source.Where(x => x.QuizId == query.QuizId.Value);
        if (query.Status.HasValue)
            source = source.Where(x => x.Status == query.Status.Value);
        if (query.IsPassed.HasValue)
            source = source.Where(x => x.IsPassed == query.IsPassed.Value);
        if (query.From.HasValue)
            source = source.Where(x => x.StartedAt >= query.From.Value);
        if (query.To.HasValue)
            source = source.Where(x => x.StartedAt <= query.To.Value);
        return source;
    }

    private static AdminQuizAttemptResponse MapAttempt(QuizAttempt attempt)
        => new(
            attempt.Id,
            attempt.UserId,
            attempt.User.Profile?.DisplayName ?? attempt.User.Email ?? attempt.User.UserName ?? "Học viên",
            attempt.User.Email ?? string.Empty,
            attempt.QuizId,
            attempt.Quiz.TitleVi,
            attempt.AttemptNumber,
            attempt.Status,
            attempt.Score,
            attempt.MaxScore,
            attempt.Percentage,
            attempt.IsPassed,
            attempt.CorrectAnswers,
            attempt.WrongAnswers,
            attempt.UnansweredQuestions,
            attempt.StartedAt,
            attempt.SubmittedAt,
            attempt.ExpiresAt,
            attempt.DurationSeconds);
}