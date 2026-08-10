using HanYu.Application.Common.Models;
using HanYu.Application.Features.Learning.Mapping;
using HanYu.Application.Features.Learning.Public.Activities;
using HanYu.Application.Features.Learning.Public.Dashboard;
using HanYu.Application.Features.Learning.Public.Goal;
using HanYu.Application.Features.Learning.Public.Summary;
using HanYu.Application.Interfaces.Learning;
using HanYu.Domain.Entities.Learning;
using HanYu.Domain.Constants;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Learning;

public sealed class LearningPublicService
    : ILearningPublicService
{
    private readonly HanYuDbContext _dbContext;

    public LearningPublicService(
        HanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<LearningGoalResponse>>
        GetMyGoalAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var goal =
            await _dbContext
                .Set<UserLearningGoal>()
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    (
                        x.Status ==
                            Domain.Enums.LearningGoalStatus.Active ||
                        x.Status ==
                            Domain.Enums.LearningGoalStatus.Paused
                    ))
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

        if (goal is null)
        {
            return Result.Failure<LearningGoalResponse>(
                Error.NotFound(
                    "Learning.GoalNotFound",
                    "Bạn chưa thiết lập mục tiêu học tập nào."));
        }

        return Result.Success(LearningMapper.ToPublicGoalResponse(goal));
    }

    public async Task<Result<LearningGoalResponse>>
        UpdateMyGoalAsync(
            Guid userId,
            UpdateLearningGoalRequest request,
            CancellationToken cancellationToken = default)
    {
        var goal =
            await _dbContext
                .Set<UserLearningGoal>()
                .Where(x =>
                    x.UserId == userId &&
                    (
                        x.Status ==
                            Domain.Enums.LearningGoalStatus.Active ||
                        x.Status ==
                            Domain.Enums.LearningGoalStatus.Paused
                    ))
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

        if (goal is null)
        {
            // If no active goal, create one
            goal = new UserLearningGoal(
                userId,
                request.TargetHskLevel,
                request.DailyGoalMinutes);
                
            goal.Update(
                request.TargetHskLevel,
                request.TargetDate,
                request.DailyGoalMinutes,
                request.DailyVocabularyGoal,
                request.WeeklyLessonGoal);

            _dbContext.Set<UserLearningGoal>().Add(goal);
        }
        else
        {
            try
            {
                goal.Update(
                    request.TargetHskLevel,
                    request.TargetDate,
                    request.DailyGoalMinutes,
                    request.DailyVocabularyGoal,
                    request.WeeklyLessonGoal);
            }
            catch (ArgumentException exception)
            {
                return Result.Failure<LearningGoalResponse>(
                    Error.Validation(
                        "Learning.Validation",
                        exception.Message));
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(LearningMapper.ToPublicGoalResponse(goal));
    }

    public async Task<Result<LearningGoalResponse>>
        PauseMyGoalAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var goal =
            await _dbContext
                .Set<UserLearningGoal>()
                .Where(x =>
                    x.UserId == userId &&
                    x.Status == Domain.Enums.LearningGoalStatus.Active)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

        if (goal is null)
        {
            return Result.Failure<LearningGoalResponse>(
                Error.NotFound(
                    "Learning.GoalNotFound",
                    "Không tìm thấy mục tiêu học tập đang hoạt động."));
        }

        goal.Pause();
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(LearningMapper.ToPublicGoalResponse(goal));
    }

    public async Task<Result<LearningGoalResponse>>
        ResumeMyGoalAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var goal =
            await _dbContext
                .Set<UserLearningGoal>()
                .Where(x =>
                    x.UserId == userId &&
                    x.Status == Domain.Enums.LearningGoalStatus.Paused)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

        if (goal is null)
        {
            return Result.Failure<LearningGoalResponse>(
                Error.NotFound(
                    "Learning.GoalNotFound",
                    "Không tìm thấy mục tiêu học tập đang tạm dừng."));
        }

        goal.Resume();
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(LearningMapper.ToPublicGoalResponse(goal));
    }

    public async Task<Result<PagedResult<LearningActivityResponse>>>
        GetMyActivitiesAsync(
            Guid userId,
            LearningActivityQuery query,
            CancellationToken cancellationToken = default)
    {
        var page = PaginationDefaults.NormalizePage(query.Page);
        var pageSize = PaginationDefaults.NormalizePageSize(query.PageSize);

        var source = _dbContext
            .Set<LearningActivity>()
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (query.ActivityType.HasValue)
        {
            source = source.Where(x => x.ActivityType == query.ActivityType.Value);
        }

        if (query.IsCompleted.HasValue)
        {
            source = source.Where(x => x.IsCompleted == query.IsCompleted.Value);
        }

        if (query.From.HasValue)
        {
            source = source.Where(x => x.StartedAt >= query.From.Value);
        }

        if (query.To.HasValue)
        {
            source = source.Where(x => x.StartedAt <= query.To.Value);
        }

        var total = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderByDescending(x => x.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        var responseItems = items.Select(LearningMapper.ToPublicActivityResponse).ToList();

        return Result.Success(new PagedResult<LearningActivityResponse>(responseItems, page, pageSize, total));
    }

    public async Task<Result<LearningActivityResponse>>
        GetMyActivityAsync(
            Guid userId,
            long activityId,
            CancellationToken cancellationToken = default)
    {
        var activity = await _dbContext
            .Set<LearningActivity>()
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Id == activityId)
            .FirstOrDefaultAsync(cancellationToken);

        if (activity is null)
        {
            return Result.Failure<LearningActivityResponse>(
                Error.NotFound(
                    "Learning.ActivityNotFound",
                    "Không tìm thấy hoạt động học tập."));
        }

        return Result.Success(LearningMapper.ToPublicActivityResponse(activity));
    }

    public async Task<Result<LearningSummaryResponse>>
        GetMySummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var summary = await _dbContext
            .Set<UserLearningSummary>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (summary is null)
        {
            return Result.Success(new LearningSummaryResponse(
                0, 0, 0, 0, 0, 0, 0, 0, 1, 0, null));
        }

        return Result.Success(LearningMapper.ToPublicSummaryResponse(summary));
    }

    public async Task<Result<LearningDashboardResponse>>
        GetDashboardAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var goalResult = await GetMyGoalAsync(userId, cancellationToken);
        var summaryResult = await GetMySummaryAsync(userId, cancellationToken);
        
        var goal = goalResult.IsSuccess ? goalResult.Value : null;
        var summary = summaryResult.Value!;

        var today = DateTimeOffset.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var todayActivities = await _dbContext
            .Set<LearningActivity>()
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.StartedAt >= today && x.StartedAt < tomorrow)
            .ToListAsync(cancellationToken);

        var todayLearningSeconds = todayActivities.Sum(x => x.DurationSeconds);
        var todayLearningMinutes = todayLearningSeconds / 60;
        var todayXp = todayActivities.Sum(x => x.XpEarned);
        var activitiesCount = todayActivities.Count;

        bool dailyGoalCompleted = false;
        if (goal is not null && goal.Status == Domain.Enums.LearningGoalStatus.Active)
        {
            dailyGoalCompleted = todayLearningMinutes >= goal.DailyGoalMinutes;
        }

        var dashboard = new LearningDashboardResponse(
            goal,
            summary,
            todayLearningMinutes,
            todayXp,
            activitiesCount,
            dailyGoalCompleted);

        return Result.Success(dashboard);
    }

}
