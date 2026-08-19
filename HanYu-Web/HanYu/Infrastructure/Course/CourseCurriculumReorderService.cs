using HanYu.Application.Common.Models;
using HanYu.Application.Features.Course.Admin;
using HanYu.Application.Features.Course.Admin.Chapters.Lessons;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Caching;
using HanYu.Application.Interfaces.Course;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Course;

public sealed class CourseCurriculumReorderService : ICourseCurriculumReorderService
{
    private readonly IHanYuDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly ICourseCacheInvalidator _cacheInvalidator;

    public CourseCurriculumReorderService(
        IHanYuDbContext dbContext,
        ICurrentUserService currentUser,
        ICourseCacheInvalidator cacheInvalidator)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _cacheInvalidator = cacheInvalidator;
    }

    public async Task<Result<bool>> ReorderChaptersAsync(
        long courseId,
        ReorderCourseChaptersRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (courseId <= 0)
        {
            return Result.Failure<bool>(
                Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));
        }

        if (request.Items.Count == 0)
        {
            return Result.Failure<bool>(
                Error.Validation("Course.ChapterOrderRequired", "Danh sách Chapter không được rỗng."));
        }

        if (request.Items.Any(x => x.ChapterId <= 0 || x.SortOrder < 0))
        {
            return Result.Failure<bool>(
                Error.Validation("Course.InvalidChapterOrder", "ChapterId hoặc SortOrder không hợp lệ."));
        }

        if (request.Items.Select(x => x.ChapterId).Distinct().Count() != request.Items.Count)
        {
            return Result.Failure<bool>(
                Error.Validation("Course.DuplicateChapter", "Chapter bị lặp trong danh sách sắp xếp."));
        }

        if (request.Items.Select(x => x.SortOrder).Distinct().Count() != request.Items.Count)
        {
            return Result.Failure<bool>(
                Error.Validation("Course.DuplicateChapterSortOrder", "SortOrder Chapter bị trùng."));
        }

        var course = await _dbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<bool>(
                Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        if (course.Status != ContentStatus.Draft)
        {
            return Result.Failure<bool>(
                Error.Conflict("Course.NotEditable", "Chỉ Course Draft mới được reorder Chapter."));
        }

        var chapterIds = request.Items.Select(x => x.ChapterId).ToArray();
        var chapters = await _dbContext.CourseChapters
            .Where(x => x.CourseId == courseId && chapterIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (chapters.Count != chapterIds.Length)
        {
            return Result.Failure<bool>(
                Error.NotFound("Course.ChapterNotFound", "Có Chapter không tồn tại hoặc không thuộc Course."));
        }

        var userId = GetRequiredUserId();
        var finalOrders = request.Items.ToDictionary(x => x.ChapterId, x => x.SortOrder);
        var maxExistingOrder = await _dbContext.CourseChapters
            .Where(x => x.CourseId == courseId)
            .Select(x => (int?)x.SortOrder)
            .MaxAsync(cancellationToken) ?? 0;
        var temporaryBase = checked(maxExistingOrder + request.Items.Count + 1);

        // Phase 1: move every affected row to a unique, valid temporary position.
        // This avoids violating the unique (CourseId, SortOrder) index during swaps.
        for (var index = 0; index < chapters.Count; index++)
        {
            chapters[index].ChangeSortOrder(checked(temporaryBase + index), userId);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Phase 2: assign the requested final positions after the original slots are free.
        foreach (var chapter in chapters)
        {
            chapter.ChangeSortOrder(finalOrders[chapter.Id], userId);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _cacheInvalidator.InvalidatePublicCourseCacheAsync(cancellationToken);

        return Result.Success(true);
    }

    public async Task<Result> ReorderChapterLessonsAsync(
        long courseId,
        long chapterId,
        ReorderChapterLessonsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (courseId <= 0 || chapterId <= 0)
        {
            return Result.Failure(
                Error.Validation("Course.InvalidChapterId", "Course ID hoặc Chapter ID không hợp lệ."));
        }

        if (request.Items.Count == 0)
        {
            return Result.Failure(
                Error.Validation("Course.LessonOrderRequired", "Danh sách Lesson không được rỗng."));
        }

        if (request.Items.Any(x => x.LessonId <= 0 || x.SortOrder < 0))
        {
            return Result.Failure(
                Error.Validation("Course.InvalidLessonOrder", "LessonId hoặc SortOrder không hợp lệ."));
        }

        if (request.Items.Select(x => x.LessonId).Distinct().Count() != request.Items.Count)
        {
            return Result.Failure(
                Error.Validation("Course.DuplicateLesson", "Lesson bị lặp."));
        }

        if (request.Items.Select(x => x.SortOrder).Distinct().Count() != request.Items.Count)
        {
            return Result.Failure(
                Error.Validation("Course.DuplicateLessonSortOrder", "SortOrder Lesson bị trùng."));
        }

        var course = await _dbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure(Error.NotFound("Course.NotFound", "Không tìm thấy Course."));
        }

        if (course.Status != ContentStatus.Draft)
        {
            return Result.Failure(
                Error.Conflict("Course.NotEditable", "Chỉ Course Draft mới được reorder Lesson."));
        }

        var chapterExists = await _dbContext.CourseChapters
            .AsNoTracking()
            .AnyAsync(x => x.Id == chapterId && x.CourseId == courseId, cancellationToken);

        if (!chapterExists)
        {
            return Result.Failure(
                Error.NotFound("Course.ChapterNotFound", "Không tìm thấy Chapter."));
        }

        var lessonIds = request.Items.Select(x => x.LessonId).ToArray();
        var lessons = await _dbContext.Lessons
            .Where(x => x.CourseChapterId == chapterId && lessonIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (lessons.Count != lessonIds.Length)
        {
            return Result.Failure(
                Error.NotFound("Course.LessonNotFound", "Có Lesson không thuộc Chapter."));
        }

        var finalOrders = request.Items.ToDictionary(x => x.LessonId, x => x.SortOrder);
        var maxExistingOrder = await _dbContext.Lessons
            .Where(x => x.CourseChapterId == chapterId)
            .Select(x => (int?)x.SortOrder)
            .MaxAsync(cancellationToken) ?? 0;
        var temporaryBase = checked(maxExistingOrder + request.Items.Count + 1);

        // Lesson.ChangeOrder requires non-negative values, therefore use positive
        // temporary positions rather than the previous invalid negative values.
        for (var index = 0; index < lessons.Count; index++)
        {
            lessons[index].ChangeOrder(checked(temporaryBase + index));
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var lesson in lessons)
        {
            lesson.ChangeOrder(finalOrders[lesson.Id]);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _cacheInvalidator.InvalidatePublicCourseCacheAsync(cancellationToken);

        return Result.Success();
    }

    private Guid GetRequiredUserId()
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue || userId.Value == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Không xác định được người dùng hiện tại.");
        }

        return userId.Value;
    }
}
