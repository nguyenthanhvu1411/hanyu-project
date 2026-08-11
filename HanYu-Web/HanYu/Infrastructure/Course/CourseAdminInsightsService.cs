using HanYu.Application.Common.Models;
using HanYu.Application.Features.Course.Admin.Insights;
using HanYu.Application.Interfaces.Course;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Operations;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Course;

public sealed class CourseAdminInsightsService : ICourseAdminInsightsService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IHanYuDbContext _dbContext;

    public CourseAdminInsightsService(IHanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<CourseHistoryItemDto>>> GetHistoryAsync(
        long courseId,
        CancellationToken cancellationToken = default)
    {
        var course = await _dbContext.Courses
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<IReadOnlyList<CourseHistoryItemDto>>(
                Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        var auditLogs = await _dbContext.Set<AuditLog>()
            .AsNoTracking()
            .Where(x => x.EntityType == "Course" && x.EntityId == courseId.ToString())
            .OrderByDescending(x => x.OccurredAt)
            .Take(200)
            .ToListAsync(cancellationToken);

        var userIds = auditLogs
            .Where(x => x.UserId.HasValue)
            .Select(x => x.UserId!.Value)
            .Concat(new[]
            {
                course.CreatedById ?? Guid.Empty,
                course.UpdatedById ?? Guid.Empty,
                course.PublishedById ?? Guid.Empty,
                course.ArchivedById ?? Guid.Empty,
                course.DeletedById ?? Guid.Empty
            })
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        var users = await _dbContext.Set<User>()
            .AsNoTracking()
            .Include(x => x.Profile)
            .Where(x => userIds.Contains(x.Id))
            .ToDictionaryAsync(
                x => x.Id,
                x => x.Profile != null ? x.Profile.DisplayName : (x.Email ?? x.UserName ?? "Người dùng"),
                cancellationToken);

        string? UserName(Guid? id)
            => id.HasValue && users.TryGetValue(id.Value, out var name) ? name : null;

        var result = auditLogs
            .Select(x => new CourseHistoryItemDto(
                x.Id,
                x.Action,
                ToActionLabel(x.Action),
                x.UserId,
                UserName(x.UserId),
                x.OldValuesJson,
                x.NewValuesJson,
                x.ChangedPropertiesJson,
                x.IpAddress,
                x.CorrelationId,
                x.OccurredAt))
            .ToList();

        // Course audit was introduced after some Course records already existed.
        // These lifecycle rows are persisted timestamps from Course itself, not fabricated demo data.
        AddLifecycle(result, "created", "Tạo khóa học", course.CreatedById, UserName(course.CreatedById), course.CreatedAt);

        if (course.UpdatedAt > course.CreatedAt.AddSeconds(1))
        {
            AddLifecycle(result, "updated", "Cập nhật khóa học", course.UpdatedById, UserName(course.UpdatedById), course.UpdatedAt);
        }

        if (course.PublishedAt.HasValue)
        {
            AddLifecycle(result, "published", "Xuất bản khóa học", course.PublishedById, UserName(course.PublishedById), course.PublishedAt.Value);
        }

        if (course.ArchivedAt.HasValue)
        {
            AddLifecycle(result, "archived", "Lưu trữ khóa học", course.ArchivedById, UserName(course.ArchivedById), course.ArchivedAt.Value);
        }

        if (course.DeletedAt.HasValue)
        {
            AddLifecycle(result, "deleted", "Xóa khóa học", course.DeletedById, UserName(course.DeletedById), course.DeletedAt.Value);
        }

        var ordered = result
            .OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.Id ?? 0)
            .ToArray();

        return Result.Success<IReadOnlyList<CourseHistoryItemDto>>(ordered);
    }

    public async Task<Result<CourseStatisticsDto>> GetStatisticsAsync(
        long courseId,
        CancellationToken cancellationToken = default)
    {
        var course = await _dbContext.Courses
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseStatisticsDto>(
                Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        var chapterQuery = _dbContext.CourseChapters
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.CourseId == courseId && x.DeletedAt == null);

        var totalChapters = await chapterQuery.CountAsync(cancellationToken);
        var activeChapters = await chapterQuery.CountAsync(x => x.IsActive, cancellationToken);

        var lessonIds = await _dbContext.Lessons
            .AsNoTracking()
            .Where(x => x.CourseChapter != null && x.CourseChapter.CourseId == courseId)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        var progressRows = await _dbContext.Set<UserLessonProgress>()
            .AsNoTracking()
            .Where(x => lessonIds.Contains(x.LessonId))
            .Select(x => new
            {
                x.UserId,
                x.LessonId,
                x.Status,
                x.CompletionPercent
            })
            .ToListAsync(cancellationToken);

        var studentGroups = progressRows.GroupBy(x => x.UserId).ToArray();
        var totalLessons = lessonIds.Length;
        var totalStudents = studentGroups.Length;
        var completedStudents = totalLessons == 0
            ? 0
            : studentGroups.Count(group =>
                group.Count(x => x.Status == LessonProgressStatus.Completed) >= totalLessons);
        var inProgressStudents = totalStudents - completedStudents;

        var averageCompletion = totalStudents == 0 || totalLessons == 0
            ? 0m
            : Math.Round(
                studentGroups.Average(group =>
                    group.Sum(x => x.CompletionPercent) / totalLessons),
                2,
                MidpointRounding.AwayFromZero);

        return Result.Success(new CourseStatisticsDto(
            courseId,
            totalChapters,
            activeChapters,
            totalLessons,
            totalStudents,
            inProgressStudents,
            completedStudents,
            averageCompletion,
            course.EstimatedMinutes));
    }

    public async Task<Result<PagedResult<CourseStudentDto>>> GetStudentsAsync(
        long courseId,
        CourseStudentsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var courseExists = await _dbContext.Courses
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Id == courseId, cancellationToken);

        if (!courseExists)
        {
            return Result.Failure<PagedResult<CourseStudentDto>>(
                Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        var lessonIds = await _dbContext.Lessons
            .AsNoTracking()
            .Where(x => x.CourseChapter != null && x.CourseChapter.CourseId == courseId)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        if (lessonIds.Length == 0)
        {
            return Result.Success(new PagedResult<CourseStudentDto>([], 1, NormalizePageSize(query.PageSize), 0));
        }

        var progressRows = await _dbContext.Set<UserLessonProgress>()
            .AsNoTracking()
            .Where(x => lessonIds.Contains(x.LessonId))
            .Select(x => new
            {
                x.UserId,
                x.LessonId,
                x.Status,
                x.CompletionPercent,
                x.StartedAt,
                x.LastAccessedAt,
                x.CompletedAt
            })
            .ToListAsync(cancellationToken);

        var userIds = progressRows.Select(x => x.UserId).Distinct().ToArray();
        var users = await _dbContext.Set<User>()
            .AsNoTracking()
            .Include(x => x.Profile)
            .Where(x => userIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var totalLessons = lessonIds.Length;
        var rows = progressRows
            .GroupBy(x => x.UserId)
            .Select(group =>
            {
                users.TryGetValue(group.Key, out var user);
                var completedLessons = group.Count(x => x.Status == LessonProgressStatus.Completed);
                var startedLessons = group.Count(x => x.Status != LessonProgressStatus.NotStarted);
                var completion = Math.Round(
                    group.Sum(x => x.CompletionPercent) / totalLessons,
                    2,
                    MidpointRounding.AwayFromZero);
                var completed = completedLessons >= totalLessons;

                return new CourseStudentDto(
                    group.Key,
                    user?.Email ?? string.Empty,
                    user?.Profile?.DisplayName ?? user?.UserName ?? user?.Email ?? "Người dùng",
                    startedLessons,
                    completedLessons,
                    totalLessons,
                    completion,
                    completed ? "completed" : "in_progress",
                    group.Where(x => x.StartedAt.HasValue).Select(x => x.StartedAt).Min(),
                    group.Where(x => x.LastAccessedAt.HasValue).Select(x => x.LastAccessedAt).Max(),
                    completed ? group.Where(x => x.CompletedAt.HasValue).Select(x => x.CompletedAt).Max() : null);
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var keyword = query.Search.Trim();
            rows = rows
                .Where(x =>
                    x.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    x.DisplayName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var status = query.Status.Trim().ToLowerInvariant();
            rows = rows.Where(x => x.Status == status).ToList();
        }

        rows = rows
            .OrderByDescending(x => x.LastAccessedAt)
            .ThenBy(x => x.DisplayName)
            .ToList();

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = NormalizePageSize(query.PageSize);
        var total = rows.Count;
        var items = rows.Skip((page - 1) * pageSize).Take(pageSize).ToArray();

        return Result.Success(new PagedResult<CourseStudentDto>(items, page, pageSize, total));
    }

    private static int NormalizePageSize(int pageSize)
        => pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

    private static void AddLifecycle(
        ICollection<CourseHistoryItemDto> result,
        string action,
        string label,
        Guid? userId,
        string? userDisplayName,
        DateTimeOffset occurredAt)
    {
        if (result.Any(x =>
                x.Action.Equals(action, StringComparison.OrdinalIgnoreCase) &&
                Math.Abs((x.OccurredAt - occurredAt).TotalSeconds) < 2))
        {
            return;
        }

        result.Add(new CourseHistoryItemDto(
            null,
            action,
            label,
            userId,
            userDisplayName,
            null,
            null,
            null,
            null,
            null,
            occurredAt));
    }

    private static string ToActionLabel(string action)
        => action.Trim().ToLowerInvariant() switch
        {
            "create" or "created" => "Tạo khóa học",
            "update" or "updated" => "Cập nhật khóa học",
            "publish" or "published" => "Xuất bản khóa học",
            "archive" or "archived" => "Lưu trữ khóa học",
            "restore" or "restored" => "Khôi phục khóa học",
            "delete" or "deleted" => "Xóa khóa học",
            "approve" or "approved" => "Duyệt khóa học",
            "reject" or "rejected" => "Từ chối khóa học",
            "submit-review" or "submitted-review" => "Gửi duyệt khóa học",
            _ => action
        };
}
