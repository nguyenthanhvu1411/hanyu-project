using HanYu.Application.Common.Models;
using HanYu.Application.Features.Learning.Mapping;
using HanYu.Application.Features.Learning.Admin.Activities;
using HanYu.Application.Features.Learning.Admin.Goals;
using HanYu.Application.Features.Learning.Admin.Summaries;
using HanYu.Application.Interfaces.Learning;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Entities.Learning;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Learning;

public sealed class LearningAdminService
    : ILearningAdminService
{
    private readonly HanYuDbContext _dbContext;

    public LearningAdminService(
        HanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<AdminLearningGoalResponse>>
        CreateGoalAsync(
            CreateLearningGoalRequest request,
            CancellationToken cancellationToken = default)
    {
        var userExists =
            await _dbContext.Set<User>()
                .AnyAsync(
                    x => x.Id == request.UserId &&
                         x.DeletedAt == null,
                    cancellationToken);

        if (!userExists)
            return NotFound<AdminLearningGoalResponse>(
                "Learning.UserNotFound",
                "Không tìm thấy người dùng.");

        try
        {
            var goal =
                new UserLearningGoal(
                    request.UserId,
                    request.TargetHskLevel,
                    request.DailyGoalMinutes);

            goal.Update(
                request.TargetHskLevel,
                request.TargetDate,
                request.DailyGoalMinutes,
                request.DailyVocabularyGoal,
                request.WeeklyLessonGoal);

            _dbContext
                .Set<UserLearningGoal>()
                .Add(goal);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return Result.Success(
                LearningMapper.ToAdminGoalResponse(goal));
        }
        catch (ArgumentException exception)
        {
            return Validation<AdminLearningGoalResponse>(
                exception.Message);
        }
    }

    public async Task<Result<AdminLearningGoalResponse>>
        GetGoalAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var goal =
            await _dbContext
                .Set<UserLearningGoal>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        return goal is null
            ? NotFound<AdminLearningGoalResponse>(
                "Learning.GoalNotFound",
                "Không tìm thấy learning goal.")
            : Result.Success(LearningMapper.ToAdminGoalResponse(goal));
    }

    public async Task<
        Result<PagedResult<AdminLearningGoalResponse>>>
        GetGoalsAsync(
            AdminLearningGoalQuery query,
            CancellationToken cancellationToken = default)
    {
        var page =
            PaginationDefaults.NormalizePage(query.Page);

        var pageSize =
            PaginationDefaults.NormalizePageSize(query.PageSize);

        IQueryable<UserLearningGoal> source =
            _dbContext.Set<UserLearningGoal>()
                .AsNoTracking();

        if (query.UserId.HasValue)
        {
            source = source.Where(
                x => x.UserId ==
                     query.UserId.Value);
        }

        if (query.Status.HasValue)
        {
            source = source.Where(
                x => x.Status ==
                     query.Status.Value);
        }

        if (query.TargetHskLevel.HasValue)
        {
            source = source.Where(
                x => x.TargetHskLevel ==
                     query.TargetHskLevel.Value);
        }

        var total = await source.CountAsync(cancellationToken);

        var items =
            await source
                .OrderByDescending(
                    x => x.StartedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync(
                    cancellationToken);

        var responseItems = items
            .Select(LearningMapper.ToAdminGoalResponse)
            .ToList();

        return Result.Success(
            new PagedResult<AdminLearningGoalResponse>(
                responseItems,
                page,
                pageSize,
                total));
    }

    public async Task<Result<AdminLearningGoalResponse>>
        UpdateGoalAsync(
            long id,
            UpdateLearningGoalRequest request,
            CancellationToken cancellationToken = default)
    {
        var goal =
            await _dbContext
                .Set<UserLearningGoal>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (goal is null)
            return NotFound<AdminLearningGoalResponse>(
                "Learning.GoalNotFound",
                "Không tìm thấy learning goal.");

        try
        {
            goal.Update(
                request.TargetHskLevel,
                request.TargetDate,
                request.DailyGoalMinutes,
                request.DailyVocabularyGoal,
                request.WeeklyLessonGoal);

            ApplyGoalStatus(
                goal,
                request.Status);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return Result.Success(
                LearningMapper.ToAdminGoalResponse(goal));
        }
        catch (ArgumentException exception)
        {
            return Validation<
                AdminLearningGoalResponse>(
                exception.Message);
        }
    }

    public async Task<Result> DeleteGoalAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var goal =
            await _dbContext
                .Set<UserLearningGoal>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (goal is null)
            return Result.Failure(
                Error.NotFound(
                    "Learning.GoalNotFound",
                    "Không tìm thấy learning goal."));

        _dbContext
            .Set<UserLearningGoal>()
            .Remove(goal);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<
        Result<AdminLearningActivityResponse>>
        CreateActivityAsync(
            CreateLearningActivityRequest request,
            CancellationToken cancellationToken = default)
    {
        var userExists =
            await _dbContext.Set<User>()
                .AnyAsync(
                    x => x.Id == request.UserId,
                    cancellationToken);

        if (!userExists)
            return NotFound<
                AdminLearningActivityResponse>(
                "Learning.UserNotFound",
                "Không tìm thấy người dùng.");

        try
        {
            var activity =
                new LearningActivity(
                    request.UserId,
                    request.ActivityType,
                    request.LessonId,
                    request.VocabularyId,
                    request.QuizAttemptId,
                    request.FlashcardSessionId,
                    request.MetadataJson);

            activity.Update(
                request.ActivityType,
                request.LessonId,
                request.VocabularyId,
                request.QuizAttemptId,
                request.FlashcardSessionId,
                request.DurationSeconds,
                request.XpEarned,
                request.MetadataJson);

            if (request.IsCompleted)
            {
                activity.Complete(
                    request.DurationSeconds,
                    request.XpEarned);
            }

            _dbContext
                .Set<LearningActivity>()
                .Add(activity);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return Result.Success(
                LearningMapper.ToAdminActivityResponse(activity));
        }
        catch (ArgumentException exception)
        {
            return Validation<
                AdminLearningActivityResponse>(
                exception.Message);
        }
    }

    public async Task<
        Result<AdminLearningActivityResponse>>
        GetActivityAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var activity =
            await _dbContext
                .Set<LearningActivity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        return activity is null
            ? NotFound<
                AdminLearningActivityResponse>(
                "Learning.ActivityNotFound",
                "Không tìm thấy learning activity.")
            : Result.Success(
                LearningMapper.ToAdminActivityResponse(activity));
    }

    public async Task<
        Result<PagedResult<AdminLearningActivityResponse>>>
        GetActivitiesAsync(
            AdminLearningActivityQuery query,
            CancellationToken cancellationToken = default)
    {
        var page =
            PaginationDefaults.NormalizePage(query.Page);

        var pageSize =
            PaginationDefaults.NormalizePageSize(query.PageSize);

        IQueryable<LearningActivity> source =
            _dbContext
                .Set<LearningActivity>()
                .AsNoTracking();

        if (query.UserId.HasValue)
        {
            source = source.Where(
                x => x.UserId ==
                     query.UserId.Value);
        }

        if (query.ActivityType.HasValue)
        {
            source = source.Where(
                x => x.ActivityType ==
                     query.ActivityType.Value);
        }

        if (query.IsCompleted.HasValue)
        {
            source = source.Where(
                x => x.IsCompleted ==
                     query.IsCompleted.Value);
        }

        if (query.From.HasValue)
        {
            source = source.Where(
                x => x.StartedAt >=
                     query.From.Value);
        }

        if (query.To.HasValue)
        {
            source = source.Where(
                x => x.StartedAt <=
                     query.To.Value);
        }

        var total = await source.CountAsync(cancellationToken);

        var items =
            await source
                .OrderByDescending(
                    x => x.StartedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync(
                    cancellationToken);

        var responseItems = items
            .Select(LearningMapper.ToAdminActivityResponse)
            .ToList();

        return Result.Success(
            new PagedResult<AdminLearningActivityResponse>(
                responseItems,
                page,
                pageSize,
                total));
    }

    public async Task<
        Result<AdminLearningActivityResponse>>
        UpdateActivityAsync(
            long id,
            UpdateLearningActivityRequest request,
            CancellationToken cancellationToken = default)
    {
        var activity =
            await _dbContext
                .Set<LearningActivity>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (activity is null)
            return NotFound<
                AdminLearningActivityResponse>(
                "Learning.ActivityNotFound",
                "Không tìm thấy learning activity.");

        try
        {
            activity.Update(
                request.ActivityType,
                request.LessonId,
                request.VocabularyId,
                request.QuizAttemptId,
                request.FlashcardSessionId,
                request.DurationSeconds,
                request.XpEarned,
                request.MetadataJson);

            if (request.IsCompleted &&
                !activity.IsCompleted)
            {
                activity.Complete(
                    request.DurationSeconds,
                    request.XpEarned);
            }

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return Result.Success(
                LearningMapper.ToAdminActivityResponse(activity));
        }
        catch (ArgumentException exception)
        {
            return Validation<
                AdminLearningActivityResponse>(
                exception.Message);
        }
    }

    public async Task<Result>
        DeleteActivityAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var activity =
            await _dbContext
                .Set<LearningActivity>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (activity is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Learning.ActivityNotFound",
                    "Không tìm thấy learning activity."));
        }

        _dbContext
            .Set<LearningActivity>()
            .Remove(activity);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<
        Result<AdminLearningSummaryResponse>>
        GetSummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var summary =
            await _dbContext
                .Set<UserLearningSummary>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);

        return summary is null
            ? NotFound<
                AdminLearningSummaryResponse>(
                "Learning.SummaryNotFound",
                "Không tìm thấy learning summary.")
            : Result.Success(
                LearningMapper.ToAdminSummaryResponse(summary));
    }

    public async Task<
        Result<PagedResult<AdminLearningSummaryResponse>>>
        GetSummariesAsync(
            AdminLearningSummaryQuery query,
            CancellationToken cancellationToken = default)
    {
        var page = PaginationDefaults.NormalizePage(query.Page);

        var pageSize =
            PaginationDefaults.NormalizePageSize(query.PageSize);

        IQueryable<UserLearningSummary> source =
            _dbContext
                .Set<UserLearningSummary>()
                .AsNoTracking();

        if (query.UserId.HasValue)
        {
            source = source.Where(
                x => x.UserId ==
                     query.UserId.Value);
        }

        if (query.CurrentHskLevel.HasValue)
        {
            source = source.Where(
                x => x.CurrentHskLevel ==
                     query.CurrentHskLevel.Value);
        }

        var total = await source.CountAsync(cancellationToken);

        var items =
            await source
                .OrderByDescending(
                    x => x.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync(
                    cancellationToken);

        var responseItems = items
            .Select(LearningMapper.ToAdminSummaryResponse)
            .ToList();

        return Result.Success(
            new PagedResult<AdminLearningSummaryResponse>(
                responseItems,
                page,
                pageSize,
                total));
    }

    public async Task<
        Result<AdminLearningSummaryResponse>>
        UpdateSummaryAsync(
            Guid userId,
            UpdateLearningSummaryRequest request,
            CancellationToken cancellationToken = default)
    {
        var summary =
            await _dbContext
                .Set<UserLearningSummary>()
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);

        if (summary is null)
        {
            summary =
                new UserLearningSummary(userId);

            _dbContext
                .Set<UserLearningSummary>()
                .Add(summary);
        }

        try
        {
            summary.Update(
                request.TotalLearningSeconds,
                request.TotalLessonsCompleted,
                request.TotalVocabularyLearned,
                request.TotalVocabularyMastered,
                request.TotalReviews,
                request.TotalQuizAttempts,
                request.TotalQuizPassed,
                request.TotalXp,
                request.CurrentHskLevel,
                request.OverallMasteryPercent,
                request.LastLearningAt);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return Result.Success(
                LearningMapper.ToAdminSummaryResponse(summary));
        }
        catch (ArgumentException exception)
        {
            return Validation<
                AdminLearningSummaryResponse>(
                exception.Message);
        }
    }

    public async Task<
        Result<AdminLearningSummaryResponse>>
        RecomputeSummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var activities =
            await _dbContext
                .Set<LearningActivity>()
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId)
                .ToListAsync(
                    cancellationToken);

        var summary =
            await _dbContext
                .Set<UserLearningSummary>()
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);

        if (summary is null)
        {
            summary =
                new UserLearningSummary(
                    userId);

            _dbContext
                .Set<UserLearningSummary>()
                .Add(summary);
        }

        var totalSeconds =
            activities.Sum(
                x => x.DurationSeconds);

        var lessonCount =
            activities.Count(
                x =>
                    x.ActivityType ==
                    LearningActivityType.LessonCompleted &&
                    x.IsCompleted);

        var vocabLearned =
            activities.Count(
                x =>
                    x.ActivityType ==
                    LearningActivityType.VocabularyLearned);

        var reviews =
            activities.Count(
                x =>
                    x.ActivityType ==
                    LearningActivityType.VocabularyReviewed);

        var quizAttempts =
            activities.Count(
                x =>
                    x.ActivityType ==
                    LearningActivityType.QuizCompleted);

        var totalXp =
            activities.Sum(
                x => x.XpEarned);

        var lastLearningAt =
            activities
                .OrderByDescending(
                    x => x.StartedAt)
                .Select(
                    x => (DateTimeOffset?)x.StartedAt)
                .FirstOrDefault();

        summary.Update(
            totalSeconds,
            lessonCount,
            vocabLearned,
            totalVocabularyMastered: 0,
            reviews,
            quizAttempts,
            totalQuizPassed: 0,
            totalXp,
            currentHskLevel: summary.CurrentHskLevel,
            overallMasteryPercent:
                summary.OverallMasteryPercent,
            lastLearningAt);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            LearningMapper.ToAdminSummaryResponse(summary));
    }

    private static void ApplyGoalStatus(
        UserLearningGoal goal,
        LearningGoalStatus status)
    {
        switch (status)
        {
            case LearningGoalStatus.Active:
                if (goal.Status ==
                    LearningGoalStatus.Paused)
                {
                    goal.Resume();
                }
                break;

            case LearningGoalStatus.Paused:
                goal.Pause();
                break;

            case LearningGoalStatus.Completed:
                goal.Complete();
                break;

            case LearningGoalStatus.Cancelled:
                goal.Cancel();
                break;
        }
    }



    private static Result<T> NotFound<T>(
        string code,
        string message) =>
        Result.Failure<T>(
            Error.NotFound(code, message));

    private static Result<T> Validation<T>(
        string message) =>
        Result.Failure<T>(
            Error.Validation(
                "Learning.Validation",
                message));
}
