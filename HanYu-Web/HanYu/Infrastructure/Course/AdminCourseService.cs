using HanYu.Application.Common.Models;
using HanYu.Application.Features.Course.Admin;
using HanYu.Application.Features.Course.Admin.Chapters;
using HanYu.Application.Features.Course.Admin.Chapters.Lessons;
using HanYu.Application.Features.Course.Admin.Prerequisites;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Course;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Entities.Course;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using HanYu.Application.Interfaces.Caching;

using CourseEntity = HanYu.Domain.Entities.Course.Course;

namespace HanYu.Infrastructure.Course;

public sealed class AdminCourseService : IAdminCourseService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IHanYuDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly ICourseCacheInvalidator _courseCacheInvalidator;

    public AdminCourseService(
        IHanYuDbContext dbContext,
        ICurrentUserService currentUser,
        ICourseCacheInvalidator courseCacheInvalidator)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _courseCacheInvalidator = courseCacheInvalidator;
    }

    // ============================================================
    // LIST
    // ============================================================

    public async Task<Result<PagedResult<AdminCourseListItemDto>>> GetCoursesAsync(
        AdminCourseQuery request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? DefaultPageSize : Math.Min(request.PageSize, MaxPageSize);

        IQueryable<CourseEntity> query = _dbContext.Courses.AsNoTracking();

        if (request.IncludeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var keyword = request.Search.Trim();
            query = query.Where(
                x => EF.Functions.ILike(x.Code, $"%{keyword}%") ||
                     EF.Functions.ILike(x.Slug, $"%{keyword}%") ||
                     EF.Functions.ILike(x.TitleVi, $"%{keyword}%"));
        }

        if (request.HskLevelId.HasValue)
        {
            query = query.Where(x => x.HskLevelId == request.HskLevelId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (request.IsFeatured.HasValue)
        {
            query = query.Where(x => x.IsFeatured == request.IsFeatured.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        query = ApplySort(query, request.SortBy, request.SortDescending);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminCourseListItemDto(
                x.Id,
                x.PublicId,
                x.Code,
                x.Slug,
                x.TitleVi,
                x.HskLevelId,
                x.HskLevel != null ? x.HskLevel.Code : null,
                x.HskLevel != null ? x.HskLevel.NameVi : null,
                x.CoverImageUrl,
                x.SortOrder,
                x.EstimatedMinutes,
                x.Status,
                x.IsActive,
                x.IsFeatured,
                x.Chapters.Count(chapter => chapter.DeletedAt == null),
                x.PublishedAt,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<AdminCourseListItemDto>(items, page, pageSize, total);
        return Result.Success(result);
    }

    // ============================================================
    // DETAIL
    // ============================================================

    public async Task<Result<AdminCourseDetailDto>> GetCourseAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));
        }

        var course = await GetDetailQuery(includeDeleted: true)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        return Result.Success(CourseAdminMapper.ToDetailDto(course));
    }

    // ============================================================
    // CREATE
    // ============================================================

    public async Task<Result<AdminCourseDetailDto>> CreateCourseAsync(
        CreateCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = GetRequiredUserId();
        var code = NormalizeCode(request.Code);
        var slug = NormalizeSlug(request.Slug);

        var codeExists = await _dbContext.Courses
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Code == code, cancellationToken);

        if (codeExists)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Conflict("Course.CodeExists", "Mã khóa học đã tồn tại."));
        }

        var slugExists = await _dbContext.Courses
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Slug == slug, cancellationToken);

        if (slugExists)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Conflict("Course.SlugExists", "Slug khóa học đã tồn tại."));
        }

        var hskValidation = await ValidateHskLevelAsync(request.HskLevelId, cancellationToken);
        if (!hskValidation.IsValid)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation(hskValidation.Code!, hskValidation.Message!));
        }

        CourseEntity course;
        try
        {
            course = new CourseEntity(
                code: code,
                slug: slug,
                titleVi: request.TitleVi,
                hskLevelId: request.HskLevelId,
                sortOrder: request.SortOrder,
                shortDescriptionVi: request.ShortDescriptionVi,
                descriptionVi: request.DescriptionVi,
                coverImageUrl: request.CoverImageUrl,
                estimatedMinutes: request.EstimatedMinutes);

            course.SetCreatedBy(userId);
            course.SetFeatured(request.IsFeatured, userId);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.ValidationFailed", exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Failure("Course.InvalidOperation", exception.Message));
        }

        _dbContext.Courses.Add(course);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var saved = await GetDetailQuery()
            .FirstAsync(x => x.Id == course.Id, cancellationToken);

        await InvalidatePublicCacheAsync(cancellationToken);

        return Result.Success(CourseAdminMapper.ToDetailDto(saved));
    }

    // ============================================================
    // UPDATE
    // ============================================================

    public async Task<Result<AdminCourseDetailDto>> UpdateCourseAsync(
        long id,
        UpdateCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id <= 0)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));
        }

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        if (course.ConcurrencyToken != request.ConcurrencyToken)
        {
            return ConcurrencyFailure();
        }

        var code = NormalizeCode(request.Code);
        var slug = NormalizeSlug(request.Slug);

        var codeExists = await _dbContext.Courses
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Id != id && x.Code == code, cancellationToken);

        if (codeExists)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Conflict("Course.CodeExists", "Mã khóa học đã tồn tại."));
        }

        var slugExists = await _dbContext.Courses
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Id != id && x.Slug == slug, cancellationToken);

        if (slugExists)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Conflict("Course.SlugExists", "Slug khóa học đã tồn tại."));
        }

        var hskValidation = await ValidateHskLevelAsync(request.HskLevelId, cancellationToken);
        if (!hskValidation.IsValid)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation(hskValidation.Code!, hskValidation.Message!));
        }

        try
        {
            course.Update(
                code: code,
                slug: slug,
                titleVi: request.TitleVi,
                hskLevelId: request.HskLevelId,
                sortOrder: request.SortOrder,
                shortDescriptionVi: request.ShortDescriptionVi,
                descriptionVi: request.DescriptionVi,
                coverImageUrl: request.CoverImageUrl,
                estimatedMinutes: request.EstimatedMinutes,
                updatedById: userId);

            course.SetFeatured(request.IsFeatured, userId);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.ValidationFailed", exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Failure("Course.InvalidState", exception.Message));
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await InvalidatePublicCacheAsync(cancellationToken);

        return await GetDetailResultAsync(id, cancellationToken);
    }

    // ============================================================
    // VALIDATE
    // ============================================================

    public async Task<Result<CourseValidationResultDto>> ValidateCourseAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Failure<CourseValidationResultDto>(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));
        }

        var course = await GetDetailQuery().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseValidationResultDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        var issues = new List<CourseValidationIssueDto>();

        if (string.IsNullOrWhiteSpace(course.Code))
        {
            issues.Add(new CourseValidationIssueDto("Course.CodeRequired", "Khóa học chưa có mã.", "code"));
        }

        if (string.IsNullOrWhiteSpace(course.Slug))
        {
            issues.Add(new CourseValidationIssueDto("Course.SlugRequired", "Khóa học chưa có slug.", "slug"));
        }

        if (string.IsNullOrWhiteSpace(course.TitleVi))
        {
            issues.Add(new CourseValidationIssueDto("Course.TitleRequired", "Khóa học chưa có tên.", "titleVi"));
        }

        var activeChapters = course.Chapters.Where(x => !x.IsDeleted && x.IsActive).ToList();
        if (activeChapters.Count == 0)
        {
            issues.Add(new CourseValidationIssueDto("Course.ChapterRequired", "Khóa học phải có ít nhất một chương hoạt động.", "chapters"));
        }

        if (course.HskLevelId.HasValue && (course.HskLevel is null || !course.HskLevel.IsActive))
        {
            issues.Add(new CourseValidationIssueDto("Course.InvalidHskLevel", "HSK Level không tồn tại hoặc đang bị vô hiệu.", "hskLevelId"));
        }

        foreach (var prerequisite in course.Prerequisites.Where(x => !x.IsDeleted))
        {
            if (prerequisite.RequiredCourse.IsDeleted || prerequisite.RequiredCourse.Status == ContentStatus.Archived)
            {
                issues.Add(new CourseValidationIssueDto("Course.InvalidPrerequisite", $"Khóa học tiên quyết '{prerequisite.RequiredCourse.TitleVi}' không còn hợp lệ.", "prerequisites"));
            }
        }

        return Result.Success(new CourseValidationResultDto(issues.Count == 0, issues));
    }

    // ============================================================
    // SUBMIT REVIEW
    // ============================================================

    public async Task<Result<AdminCourseDetailDto>> SubmitForReviewAsync(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteWorkflowAsync(
            id,
            request.ConcurrencyToken,
            static (course, userId) => course.SubmitForReview(userId),
            invalidatePublicCache: false,
            cancellationToken);
    }

    // ============================================================
    // APPROVE
    // ============================================================

    public async Task<Result<AdminCourseDetailDto>> ApproveAsync(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));
        }

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        if (course.ConcurrencyToken != request.ConcurrencyToken)
        {
            return ConcurrencyFailure();
        }

        if (course.CreatedById.HasValue && course.CreatedById.Value == userId)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Forbidden("Course.SelfApprovalNotAllowed", "Người tạo khóa học không được tự duyệt khóa học."));
        }

        try
        {
            course.Approve(userId);
        }
        catch (InvalidOperationException exception)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Failure("Course.InvalidState", exception.Message));
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetDetailResultAsync(id, cancellationToken);
    }

    // ============================================================
    // REJECT
    // ============================================================

    public async Task<Result<AdminCourseDetailDto>> RejectAsync(
        long id,
        RejectCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.RejectReasonRequired", "Lý do từ chối không được để trống."));
        }

        return await ExecuteWorkflowAsync(
            id,
            request.ConcurrencyToken,
            (course, userId) => course.Reject(request.Reason, userId),
            invalidatePublicCache: false,
            cancellationToken);
    }

    // ============================================================
    // PUBLISH
    // ============================================================

    public async Task<Result<AdminCourseDetailDto>> PublishAsync(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateCourseAsync(id, cancellationToken);

        if (!validation.IsSuccess)
        {
            return Result.Failure<AdminCourseDetailDto>(validation.Error);
        }

        if (!validation.Value.IsValid)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.NotPublishable", string.Join(" | ", validation.Value.Issues.Select(x => x.Message))));
        }

        return await ExecuteWorkflowAsync(
            id,
            request.ConcurrencyToken,
            static (course, userId) => course.Publish(userId),
            invalidatePublicCache: true,
            cancellationToken);
    }

    // ============================================================
    // SCHEDULE PUBLISH
    // ============================================================

    public async Task<Result> SchedulePublishAsync(
        long id,
        ScheduleCoursePublishRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Failure(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));
        }

        if (request.PublishAt <= DateTimeOffset.UtcNow)
        {
            return Result.Failure(Error.Validation("Course.InvalidPublishTime", "Thời gian publish phải ở tương lai."));
        }

        var course = await _dbContext.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (course is null)
        {
            return Result.Failure(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        if (course.ConcurrencyToken != request.ConcurrencyToken)
        {
            return Result.Failure(Error.Conflict("CONCURRENCY.CONFLICT", "Khóa học đã được thay đổi."));
        }

        if (course.Status != ContentStatus.Approved)
        {
            return Result.Failure(Error.Failure("Course.NotApproved", "Chỉ khóa học Approved mới được lên lịch publish."));
        }

        return Result.Failure(Error.Failure("Course.SchedulingNotConfigured", "Background publish scheduler chưa được cấu hình."));
    }

    // ============================================================
    // ARCHIVE
    // ============================================================

    public async Task<Result<AdminCourseDetailDto>> ArchiveAsync(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteWorkflowAsync(
            id,
            request.ConcurrencyToken,
            static (course, userId) => course.Archive(userId),
            invalidatePublicCache: true,
            cancellationToken);
    }

    // ============================================================
    // RESTORE ARCHIVED
    // ============================================================

    public async Task<Result<AdminCourseDetailDto>> RestoreAsync(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteWorkflowAsync(
            id,
            request.ConcurrencyToken,
            static (course, userId) => course.RestoreToDraft(userId),
            invalidatePublicCache: true,
            cancellationToken);
    }

    // ============================================================
    // DELETE
    // ============================================================

    public async Task<Result> DeleteAsync(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Failure(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));
        }

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (course is null)
        {
            return Result.Failure(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        if (course.ConcurrencyToken != request.ConcurrencyToken)
        {
            return Result.Failure(Error.Conflict("CONCURRENCY.CONFLICT", "Khóa học đã được thay đổi."));
        }

        if (course.Status == ContentStatus.Published)
        {
            return Result.Failure(Error.Failure("Course.PublishedCannotDelete", "Không thể xóa khóa học đang Published. Hãy Archive khóa học trước."));
        }

        course.SoftDelete(userId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await InvalidatePublicCacheAsync(cancellationToken);

        return Result.Success();
    }

    // ============================================================
    // RESTORE DELETED
    // ============================================================

    public async Task<Result<AdminCourseDetailDto>> RestoreDeletedAsync(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));
        }

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt != null, cancellationToken);

        if (course is null)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.NotFound("Course.DeletedNotFound", "Không tìm thấy khóa học đã xóa."));
        }

        if (course.ConcurrencyToken != request.ConcurrencyToken)
        {
            return ConcurrencyFailure();
        }

        var codeExists = await _dbContext.Courses.IgnoreQueryFilters().AnyAsync(x => x.Id != course.Id && x.DeletedAt == null && x.Code == course.Code, cancellationToken);

        if (codeExists)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Conflict("Course.RestoreCodeConflict", "Không thể restore vì mã khóa học đã được sử dụng."));
        }

        var slugExists = await _dbContext.Courses.IgnoreQueryFilters().AnyAsync(x => x.Id != course.Id && x.DeletedAt == null && x.Slug == course.Slug, cancellationToken);

        if (slugExists)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Conflict("Course.RestoreSlugConflict", "Không thể restore vì slug đã được sử dụng."));
        }

        course.Restore(userId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await InvalidatePublicCacheAsync(cancellationToken);

        return await GetDetailResultAsync(id, cancellationToken);
    }


    // ============================================================
    // WORKFLOW HELPER
    // ============================================================

    private async Task<Result<AdminCourseDetailDto>> ExecuteWorkflowAsync(
        long id,
        Guid concurrencyToken,
        Action<CourseEntity, Guid> action,
        bool invalidatePublicCache,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));
        }

        if (concurrencyToken == Guid.Empty)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.ConcurrencyTokenRequired", "ConcurrencyToken không hợp lệ."));
        }

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        if (course.ConcurrencyToken != concurrencyToken)
        {
            return ConcurrencyFailure();
        }

        try
        {
            action(course, userId);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Validation("Course.ValidationFailed", exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.Failure("Course.InvalidState", exception.Message));
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (invalidatePublicCache)
        {
            await InvalidatePublicCacheAsync(cancellationToken);
        }

        return await GetDetailResultAsync(id, cancellationToken);
    }

    private IQueryable<CourseEntity> GetDetailQuery(bool includeDeleted = false)
    {
        IQueryable<CourseEntity> query = _dbContext.Courses;

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return query
            .AsNoTracking()
            .Include(x => x.HskLevel)
            .Include(x => x.Chapters)
            .Include(x => x.Prerequisites)
            .ThenInclude(x => x.RequiredCourse);
    }

    private async Task<Result<AdminCourseDetailDto>> GetDetailResultAsync(long id, CancellationToken cancellationToken)
    {
        var course = await GetDetailQuery(includeDeleted: true).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<AdminCourseDetailDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        return Result.Success(CourseAdminMapper.ToDetailDto(course));
    }

    private async Task<ValidationState> ValidateHskLevelAsync(long? hskLevelId, CancellationToken cancellationToken)
    {
        if (!hskLevelId.HasValue)
        {
            return ValidationState.Valid();
        }

        if (hskLevelId.Value <= 0)
        {
            return ValidationState.Invalid("Course.InvalidHskLevel", "HSK Level ID không hợp lệ.");
        }

        var hskLevel = await _dbContext.HskLevels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == hskLevelId.Value, cancellationToken);

        if (hskLevel is null)
        {
            return ValidationState.Invalid("Course.HskLevelNotFound", "Không tìm thấy HSK Level.");
        }

        if (!hskLevel.IsActive)
        {
            return ValidationState.Invalid("Course.HskLevelInactive", "HSK Level đang bị vô hiệu.");
        }

        return ValidationState.Valid();
    }

    private static IQueryable<CourseEntity> ApplySort(IQueryable<CourseEntity> query, string? sortBy, bool descending)
    {
        var sort = sortBy?.Trim().ToLowerInvariant();

        return sort switch
        {
            "title" or "titlevi" => descending ? query.OrderByDescending(x => x.TitleVi) : query.OrderBy(x => x.TitleVi),
            "code" => descending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
            "status" => descending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            "createdat" => descending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
            "updatedat" => descending ? query.OrderByDescending(x => x.UpdatedAt) : query.OrderBy(x => x.UpdatedAt),
            "publishedat" => descending ? query.OrderByDescending(x => x.PublishedAt) : query.OrderBy(x => x.PublishedAt),
            "sortorder" => descending ? query.OrderByDescending(x => x.SortOrder).ThenByDescending(x => x.Id) : query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id),
            _ => query.OrderBy(x => x.SortOrder).ThenBy(x => x.TitleVi).ThenBy(x => x.Id)
        };
    }

    // ============================================================
    // CHAPTER CRUD
    // ============================================================

    public async Task<Result<bool>> ReorderChaptersAsync(
        long courseId,
        ReorderCourseChaptersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0) return Result.Failure<bool>(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses
            .Include(x => x.Chapters.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);

        if (course is null) return Result.Failure<bool>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));

        foreach (var item in request.Items)
        {
            var chapter = course.Chapters.FirstOrDefault(c => c.Id == item.ChapterId);
            if (chapter is not null)
            {
                chapter.ChangeSortOrder(item.SortOrder, userId);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await InvalidatePublicCacheAsync(cancellationToken);
        return Result.Success(true);
    }

    // ============================================================
    // CHAPTER - LIST
    // ============================================================

    public async Task<Result<IReadOnlyList<CourseChapterAdminDto>>>
        GetChaptersAsync(
            long courseId,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default)
    {
        if (courseId <= 0)
        {
            return Result.Failure<IReadOnlyList<CourseChapterAdminDto>>(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));
        }

        var courseExists =
            await _dbContext.Courses
                .IgnoreQueryFilters()
                .AnyAsync(
                    x => x.Id == courseId,
                    cancellationToken);

        if (!courseExists)
        {
            return Result.Failure<IReadOnlyList<CourseChapterAdminDto>>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        IQueryable<CourseChapter> query =
            _dbContext.CourseChapters
                .AsNoTracking()
                .Where(
                    x =>
                        x.CourseId ==
                        courseId);

        if (includeDeleted)
        {
            query =
                query.IgnoreQueryFilters();
        }

        var chapters =
            await query
                .OrderBy(
                    x => x.SortOrder)
                .ThenBy(
                    x => x.Id)
                .ToListAsync(
                    cancellationToken);

        IReadOnlyList<CourseChapterAdminDto> result =
            chapters
                .Select(
                    CourseChapterMapper.ToDto)
                .ToList();

        return Result.Success(
            result);
    }

    public async Task<Result<CourseChapterAdminDto>>
        GetChapterAsync(
            long courseId,
            long chapterId,
            CancellationToken cancellationToken = default)
    {
        if (
            courseId <= 0 ||
            chapterId <= 0)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.InvalidChapterId", "Course ID hoặc Chapter ID không hợp lệ."));
        }

        var chapter =
            await _dbContext.CourseChapters
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == chapterId &&
                        x.CourseId == courseId,
                    cancellationToken);

        if (chapter is null)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.ChapterNotFound", "Không tìm thấy Chapter."));
        }

        return Result.Success(
            CourseChapterMapper.ToDto(
                chapter));
    }

    public async Task<Result<CourseChapterAdminDto>>
        CreateChapterAsync(
            long courseId,
            CreateCourseChapterRequest request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        if (courseId <= 0)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.InvalidId", "Course ID không hợp lệ."));
        }

        var userId =
            GetRequiredUserId();

        var course =
            await _dbContext.Courses
                .FirstOrDefaultAsync(
                    x => x.Id == courseId,
                    cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.NotFound", "Không tìm thấy khóa học."));
        }

        if (course.Status !=
            ContentStatus.Draft)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.NotEditable", "Chỉ khóa học Draft mới được thay đổi cấu trúc Chapter."));
        }

        var duplicateOrder =
            await _dbContext.CourseChapters
                .AnyAsync(
                    x =>
                        x.CourseId == courseId &&
                        x.SortOrder == request.SortOrder,
                    cancellationToken);

        if (duplicateOrder)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.ChapterSortOrderExists", "SortOrder của Chapter đã tồn tại."));
        }

        CourseChapter chapter;

        try
        {
            chapter =
                new CourseChapter(
                    courseId:
                        courseId,

                    titleVi:
                        request.TitleVi,

                    sortOrder:
                        request.SortOrder,

                    descriptionVi:
                        request.DescriptionVi,

                    isActive:
                        request.IsActive);

            chapter.SetCreatedBy(
                userId);
        }
        catch (
            ArgumentException exception)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.ChapterValidationFailed", exception.Message));
        }

        _dbContext.CourseChapters.Add(
            chapter);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await InvalidatePublicCacheAsync(
            cancellationToken);

        return Result.Success(
            CourseChapterMapper.ToDto(
                chapter));
    }

    public async Task<Result<CourseChapterAdminDto>>
        UpdateChapterAsync(
            long courseId,
            long chapterId,
            UpdateCourseChapterRequest request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        var userId =
            GetRequiredUserId();

        var course =
            await _dbContext.Courses
                .FirstOrDefaultAsync(
                    x => x.Id == courseId,
                    cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.NotFound", "Không tìm thấy khóa học."));
        }

        if (course.Status !=
            ContentStatus.Draft)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.NotEditable", "Chỉ khóa học Draft mới được sửa Chapter."));
        }

        var chapter =
            await _dbContext.CourseChapters
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == chapterId &&
                        x.CourseId == courseId,
                    cancellationToken);

        if (chapter is null)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.ChapterNotFound", "Không tìm thấy Chapter."));
        }

        if (chapter.ConcurrencyToken !=
            request.ConcurrencyToken)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("CONCURRENCY.CONFLICT", "Chapter đã được thay đổi. Vui lòng tải lại."));
        }

        var duplicateOrder =
            await _dbContext.CourseChapters
                .AnyAsync(
                    x =>
                        x.CourseId == courseId &&
                        x.Id != chapterId &&
                        x.SortOrder ==
                            request.SortOrder,
                    cancellationToken);

        if (duplicateOrder)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.ChapterSortOrderExists", "SortOrder đã được Chapter khác sử dụng."));
        }

        try
        {
            chapter.Update(
                titleVi:
                    request.TitleVi,

                descriptionVi:
                    request.DescriptionVi,

                sortOrder:
                    request.SortOrder,

                isActive:
                    request.IsActive,

                updatedById:
                    userId);
        }
        catch (
            ArgumentException exception)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.ChapterValidationFailed", exception.Message));
        }
        catch (
            InvalidOperationException exception)
        {
            return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.ChapterInvalidState", exception.Message));
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await InvalidatePublicCacheAsync(
            cancellationToken);

        return Result.Success(
            CourseChapterMapper.ToDto(
                chapter));
    }

    public async Task<Result> DeleteChapterAsync(
        long courseId,
        long chapterId,
        CourseEntityWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0 || chapterId <= 0) return Result.Failure(Error.Validation("Course.InvalidId", "ID không hợp lệ."));

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);
        if (course is null) return Result.Failure(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));

        if (course.Status != ContentStatus.Draft) return Result.Failure(Error.Failure("Course.NotEditable", "Chỉ khóa học Draft mới được thay đổi cấu trúc."));

        var chapter = await _dbContext.CourseChapters.FirstOrDefaultAsync(x => x.Id == chapterId && x.CourseId == courseId, cancellationToken);
        if (chapter is null) return Result.Failure(Error.NotFound("Chapter.NotFound", "Không tìm thấy chương."));

        if (chapter.ConcurrencyToken != request.ConcurrencyToken)
            return Result.Failure(Error.Conflict("CONCURRENCY.CONFLICT", "Dữ liệu đã thay đổi."));

        chapter.Delete(userId);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await InvalidatePublicCacheAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<CourseChapterAdminDto>> RestoreChapterAsync(
        long courseId,
        long chapterId,
        CourseEntityWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0 || chapterId <= 0) return Result.Failure<CourseChapterAdminDto>(Error.Validation("Course.InvalidId", "ID không hợp lệ."));

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);
        if (course is null) return Result.Failure<CourseChapterAdminDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));

        if (course.Status != ContentStatus.Draft) return Result.Failure<CourseChapterAdminDto>(Error.Failure("Course.NotEditable", "Chỉ khóa học Draft mới được thay đổi cấu trúc."));

        var chapter = await _dbContext.CourseChapters.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == chapterId && x.CourseId == courseId && x.DeletedAt != null, cancellationToken);
        if (chapter is null) return Result.Failure<CourseChapterAdminDto>(Error.NotFound("Chapter.NotFound", "Không tìm thấy chương đã xóa."));

        if (chapter.ConcurrencyToken != request.ConcurrencyToken)
            return Result.Failure<CourseChapterAdminDto>(Error.Conflict("CONCURRENCY.CONFLICT", "Dữ liệu đã thay đổi."));

        chapter.Restore(userId);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await InvalidatePublicCacheAsync(cancellationToken);

        return Result.Success(CourseChapterMapper.ToDto(chapter));
    }

    // ============================================================
    // PREREQUISITE CRUD
    // ============================================================

    public async Task<Result<IReadOnlyList<CoursePrerequisiteAdminDto>>> GetPrerequisitesAsync(
        long courseId,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0) return Result.Failure<IReadOnlyList<CoursePrerequisiteAdminDto>>(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));

        IQueryable<CoursePrerequisite> query = _dbContext.CoursePrerequisites
            .AsNoTracking()
            .Include(x => x.RequiredCourse)
            .Where(x => x.CourseId == courseId);

        if (includeDeleted) query = query.IgnoreQueryFilters();

        var items = await query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync(cancellationToken);
        return Result.Success((IReadOnlyList<CoursePrerequisiteAdminDto>)items.Select(CoursePrerequisiteMapper.ToDto).ToList());
    }

    public async Task<Result<CoursePrerequisiteAdminDto>> GetPrerequisiteAsync(
        long courseId,
        long prerequisiteId,
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0 || prerequisiteId <= 0) return Result.Failure<CoursePrerequisiteAdminDto>(Error.Validation("Course.InvalidId", "ID không hợp lệ."));

        var item = await _dbContext.CoursePrerequisites
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.RequiredCourse)
            .FirstOrDefaultAsync(x => x.Id == prerequisiteId && x.CourseId == courseId, cancellationToken);

        if (item is null) return Result.Failure<CoursePrerequisiteAdminDto>(Error.NotFound("Prerequisite.NotFound", "Không tìm thấy điều kiện tiên quyết."));
        return Result.Success(CoursePrerequisiteMapper.ToDto(item));
    }

    public async Task<Result<CoursePrerequisiteAdminDto>> CreatePrerequisiteAsync(
        long courseId,
        CreateCoursePrerequisiteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0) return Result.Failure<CoursePrerequisiteAdminDto>(Error.Validation("Course.InvalidId", "Course ID không hợp lệ."));

        var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);
        if (course is null) return Result.Failure<CoursePrerequisiteAdminDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));

        if (course.Status != ContentStatus.Draft) return Result.Failure<CoursePrerequisiteAdminDto>(Error.Failure("Course.NotEditable", "Chỉ khóa học Draft mới được thay đổi cấu trúc."));

        var reqCourse = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == request.RequiredCourseId, cancellationToken);
        if (reqCourse is null) return Result.Failure<CoursePrerequisiteAdminDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học yêu cầu."));

        var pre = new CoursePrerequisite(
            courseId: courseId,
            requiredCourseId: request.RequiredCourseId,
            sortOrder: request.SortOrder,
            isRequired: request.IsRequired);

        // Preload RequiredCourse to satisfy Dto mapping
        pre.GetType().GetProperty(nameof(pre.RequiredCourse))?.SetValue(pre, reqCourse);
        pre.SetCreatedBy(GetRequiredUserId());

        _dbContext.CoursePrerequisites.Add(pre);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await InvalidatePublicCacheAsync(cancellationToken);

        return Result.Success(CoursePrerequisiteMapper.ToDto(pre));
    }

    public async Task<Result<CoursePrerequisiteAdminDto>> UpdatePrerequisiteAsync(
        long courseId,
        long prerequisiteId,
        UpdateCoursePrerequisiteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0 || prerequisiteId <= 0) return Result.Failure<CoursePrerequisiteAdminDto>(Error.Validation("Course.InvalidId", "ID không hợp lệ."));

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);
        if (course is null) return Result.Failure<CoursePrerequisiteAdminDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));

        if (course.Status != ContentStatus.Draft) return Result.Failure<CoursePrerequisiteAdminDto>(Error.Failure("Course.NotEditable", "Chỉ khóa học Draft mới được sửa."));

        var pre = await _dbContext.CoursePrerequisites
            .Include(x => x.RequiredCourse)
            .FirstOrDefaultAsync(x => x.Id == prerequisiteId && x.CourseId == courseId, cancellationToken);
        
        if (pre is null) return Result.Failure<CoursePrerequisiteAdminDto>(Error.NotFound("Prerequisite.NotFound", "Không tìm thấy điều kiện tiên quyết."));

        if (pre.ConcurrencyToken != request.ConcurrencyToken)
            return Result.Failure<CoursePrerequisiteAdminDto>(Error.Conflict("CONCURRENCY.CONFLICT", "Dữ liệu đã thay đổi."));

        if (pre.RequiredCourseId != request.RequiredCourseId)
        {
            var reqCourse = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == request.RequiredCourseId, cancellationToken);
            if (reqCourse is null) return Result.Failure<CoursePrerequisiteAdminDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học yêu cầu."));
            pre.GetType().GetProperty(nameof(pre.RequiredCourse))?.SetValue(pre, reqCourse);
        }

        pre.Update(
            requiredCourseId: request.RequiredCourseId,
            isRequired: request.IsRequired,
            sortOrder: request.SortOrder,
            updatedById: userId);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await InvalidatePublicCacheAsync(cancellationToken);

        return Result.Success(CoursePrerequisiteMapper.ToDto(pre));
    }

    public async Task<Result> DeletePrerequisiteAsync(
        long courseId,
        long prerequisiteId,
        CourseEntityWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0 || prerequisiteId <= 0) return Result.Failure(Error.Validation("Course.InvalidId", "ID không hợp lệ."));

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);
        if (course is null) return Result.Failure(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        if (course.Status != ContentStatus.Draft) return Result.Failure(Error.Failure("Course.NotEditable", "Chỉ khóa học Draft mới được thao tác."));

        var pre = await _dbContext.CoursePrerequisites.FirstOrDefaultAsync(x => x.Id == prerequisiteId && x.CourseId == courseId, cancellationToken);
        if (pre is null) return Result.Failure(Error.NotFound("Prerequisite.NotFound", "Không tìm thấy điều kiện tiên quyết."));

        if (pre.ConcurrencyToken != request.ConcurrencyToken)
            return Result.Failure(Error.Conflict("CONCURRENCY.CONFLICT", "Dữ liệu đã thay đổi."));

        pre.Delete(userId);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await InvalidatePublicCacheAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<CoursePrerequisiteAdminDto>> RestorePrerequisiteAsync(
        long courseId,
        long prerequisiteId,
        CourseEntityWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0 || prerequisiteId <= 0) return Result.Failure<CoursePrerequisiteAdminDto>(Error.Validation("Course.InvalidId", "ID không hợp lệ."));

        var userId = GetRequiredUserId();
        var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);
        if (course is null) return Result.Failure<CoursePrerequisiteAdminDto>(Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        if (course.Status != ContentStatus.Draft) return Result.Failure<CoursePrerequisiteAdminDto>(Error.Failure("Course.NotEditable", "Chỉ khóa học Draft mới được thao tác."));

        var pre = await _dbContext.CoursePrerequisites.IgnoreQueryFilters().Include(x => x.RequiredCourse).FirstOrDefaultAsync(x => x.Id == prerequisiteId && x.CourseId == courseId && x.DeletedAt != null, cancellationToken);
        if (pre is null) return Result.Failure<CoursePrerequisiteAdminDto>(Error.NotFound("Prerequisite.NotFound", "Không tìm thấy điều kiện tiên quyết."));

        if (pre.ConcurrencyToken != request.ConcurrencyToken)
            return Result.Failure<CoursePrerequisiteAdminDto>(Error.Conflict("CONCURRENCY.CONFLICT", "Dữ liệu đã thay đổi."));

        pre.Restore(userId);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await InvalidatePublicCacheAsync(cancellationToken);

        return Result.Success(CoursePrerequisiteMapper.ToDto(pre));
    }

    private async Task InvalidatePublicCacheAsync(CancellationToken cancellationToken)
    {
        await _courseCacheInvalidator.InvalidatePublicCourseCacheAsync(cancellationToken);
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

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Course code không được để trống.", nameof(code));
        }

        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Course slug không được để trống.", nameof(slug));
        }

        return slug.Trim().ToLowerInvariant();
    }

    private static Result<AdminCourseDetailDto> ConcurrencyFailure() => Result.Failure<AdminCourseDetailDto>(Error.Conflict("CONCURRENCY.CONFLICT", "Dữ liệu khóa học đã thay đổi. Vui lòng tải lại trước khi thực hiện thao tác."));

    private sealed record ValidationState(bool IsValid, string? Code, string? Message)
    {
        public static ValidationState Valid() => new(true, null, null);
        public static ValidationState Invalid(string code, string message) => new(false, code, message);
    }
    // ============================================================
    // CHAPTER LESSONS
    // ============================================================

    public async Task<Result<IReadOnlyList<CourseChapterLessonAdminDto>>>
        GetChapterLessonsAsync(
            long courseId,
            long chapterId,
            CancellationToken cancellationToken = default)
    {
        if (
            courseId <= 0 ||
            chapterId <= 0)
        {
            return Result.Failure<
                IReadOnlyList<CourseChapterLessonAdminDto>>(
                Error.Validation(
                    "Course.InvalidChapterId",
                    "Course ID hoặc Chapter ID không hợp lệ."));
        }

        var chapterExists =
            await _dbContext.CourseChapters
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.Id == chapterId &&
                        x.CourseId == courseId,
                    cancellationToken);

        if (!chapterExists)
        {
            return Result.Failure<
                IReadOnlyList<CourseChapterLessonAdminDto>>(
                Error.NotFound(
                    "Course.ChapterNotFound",
                    "Không tìm thấy Chapter."));
        }

        var lessons =
            await _dbContext.Lessons
                .AsNoTracking()
                .Where(
                    x =>
                        x.CourseChapterId ==
                        chapterId)
                .OrderBy(
                    x => x.SortOrder)
                .ThenBy(
                    x => x.Id)
                .ToListAsync(
                    cancellationToken);

        IReadOnlyList<CourseChapterLessonAdminDto>
            result =
                lessons
                    .Select(
                        CourseChapterLessonMapper.ToDto)
                    .ToList();

        return Result.Success(
            result);
    }

    public async Task<Result<CourseChapterLessonAdminDto>>
        AssignLessonToChapterAsync(
            long courseId,
            long chapterId,
            AssignLessonToChapterRequest request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        if (
            courseId <= 0 ||
            chapterId <= 0 ||
            request.LessonId <= 0)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.Validation(
                    "Course.InvalidLessonAssignment",
                    "Course, Chapter hoặc Lesson ID không hợp lệ."));
        }

        var course =
            await _dbContext.Courses
                .FirstOrDefaultAsync(
                    x => x.Id == courseId,
                    cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.NotFound(
                    "Course.NotFound",
                    "Không tìm thấy Course."));
        }

        if (course.Status !=
            ContentStatus.Draft)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.Validation(
                    "Course.NotEditable",
                    "Chỉ Course Draft mới được thay đổi cấu trúc Lesson."));
        }

        var chapter =
            await _dbContext.CourseChapters
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == chapterId &&
                        x.CourseId == courseId,
                    cancellationToken);

        if (chapter is null)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.NotFound(
                    "Course.ChapterNotFound",
                    "Chapter không tồn tại hoặc không thuộc Course."));
        }

        var lesson =
            await _dbContext.Lessons
                .FirstOrDefaultAsync(
                    x => x.Id == request.LessonId,
                    cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.NotFound(
                    "Course.LessonNotFound",
                    "Không tìm thấy Lesson."));
        }

        if (lesson.CourseChapterId.HasValue)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.Conflict(
                    "Course.LessonAlreadyAssigned",
                    "Lesson đã thuộc một Chapter. " +
                    "Hãy dùng thao tác Move thay vì Assign."));
        }

        var orderExists =
            await _dbContext.Lessons
                .AnyAsync(
                    x =>
                        x.CourseChapterId == chapterId &&
                        x.SortOrder == request.SortOrder,
                    cancellationToken);

        if (orderExists)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.Conflict(
                    "Course.LessonSortOrderExists",
                    "SortOrder đã được Lesson khác sử dụng."));
        }

        try
        {
            lesson.AssignToChapter(
                chapterId,
                request.SortOrder);
        }
        catch (
            Exception exception)
            when (
                exception is
                    ArgumentException or
                    InvalidOperationException)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.Failure(
                    "Course.LessonAssignmentFailed",
                    exception.Message));
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await InvalidatePublicCacheAsync(
            cancellationToken);

        return Result.Success(
            CourseChapterLessonMapper.ToDto(
                lesson));
    }

    public async Task<Result<CourseChapterLessonAdminDto>>
        MoveLessonAsync(
            long courseId,
            long chapterId,
            long lessonId,
            MoveLessonToChapterRequest request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        if (request.TargetChapterId <= 0)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.Validation(
                    "Course.InvalidTargetChapter",
                    "Target Chapter không hợp lệ."));
        }

        var course =
            await _dbContext.Courses
                .FirstOrDefaultAsync(
                    x => x.Id == courseId,
                    cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.NotFound(
                    "Course.NotFound",
                    "Không tìm thấy Course."));
        }

        if (course.Status !=
            ContentStatus.Draft)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.Validation(
                    "Course.NotEditable",
                    "Chỉ Course Draft mới được Move Lesson."));
        }

        var sourceChapterExists =
            await _dbContext.CourseChapters
                .AnyAsync(
                    x =>
                        x.Id == chapterId &&
                        x.CourseId == courseId,
                    cancellationToken);

        if (!sourceChapterExists)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.NotFound(
                    "Course.SourceChapterNotFound",
                    "Source Chapter không tồn tại."));
        }

        var targetChapterExists =
            await _dbContext.CourseChapters
                .AnyAsync(
                    x =>
                        x.Id ==
                            request.TargetChapterId &&
                        x.CourseId ==
                            courseId,
                    cancellationToken);

        if (!targetChapterExists)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.NotFound(
                    "Course.TargetChapterNotFound",
                    "Target Chapter không tồn tại hoặc không thuộc Course."));
        }

        var lesson =
            await _dbContext.Lessons
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == lessonId &&
                        x.CourseChapterId ==
                            chapterId,
                    cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.NotFound(
                    "Course.LessonNotFound",
                    "Lesson không thuộc Source Chapter."));
        }

        var orderConflict =
            await _dbContext.Lessons
                .AnyAsync(
                    x =>
                        x.CourseChapterId ==
                            request.TargetChapterId &&
                        x.Id != lesson.Id &&
                        x.SortOrder ==
                            request.SortOrder,
                    cancellationToken);

        if (orderConflict)
        {
            return Result.Failure<CourseChapterLessonAdminDto>(
                Error.Conflict(
                    "Course.LessonSortOrderExists",
                    "Target Chapter đã có Lesson ở vị trí này."));
        }

        lesson.MoveToChapter(
            request.TargetChapterId,
            request.SortOrder);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await InvalidatePublicCacheAsync(
            cancellationToken);

        return Result.Success(
            CourseChapterLessonMapper.ToDto(
                lesson));
    }

    public async Task<Result>
        RemoveLessonFromChapterAsync(
            long courseId,
            long chapterId,
            long lessonId,
            CancellationToken cancellationToken = default)
    {
        var course =
            await _dbContext.Courses
                .FirstOrDefaultAsync(
                    x => x.Id == courseId,
                    cancellationToken);

        if (course is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Course.NotFound",
                    "Không tìm thấy Course."));
        }

        if (course.Status !=
            ContentStatus.Draft)
        {
            return Result.Failure(
                Error.Validation(
                    "Course.NotEditable",
                    "Chỉ Course Draft mới được bỏ Lesson khỏi Chapter."));
        }

        var lesson =
            await _dbContext.Lessons
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == lessonId &&
                        x.CourseChapterId ==
                            chapterId &&
                        x.CourseChapter != null &&
                        x.CourseChapter.CourseId ==
                            courseId,
                    cancellationToken);

        if (lesson is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Course.LessonNotFound",
                    "Lesson không thuộc Chapter."));
        }

        /*
         * Không xóa Lesson.
         * Chỉ bỏ association.
         */
        lesson.RemoveFromChapter();

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await InvalidatePublicCacheAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result>
        ReorderChapterLessonsAsync(
            long courseId,
            long chapterId,
            ReorderChapterLessonsRequest request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        if (request.Items.Count == 0)
        {
            return Result.Failure(
                Error.Validation(
                    "Course.LessonOrderRequired",
                    "Danh sách Lesson không được rỗng."));
        }

        if (
            request.Items
                .Select(x => x.LessonId)
                .Distinct()
                .Count() !=
            request.Items.Count)
        {
            return Result.Failure(
                Error.Validation(
                    "Course.DuplicateLesson",
                    "Lesson bị lặp."));
        }

        if (
            request.Items
                .Select(x => x.SortOrder)
                .Distinct()
                .Count() !=
            request.Items.Count)
        {
            return Result.Failure(
                Error.Validation(
                    "Course.DuplicateLessonSortOrder",
                    "SortOrder bị trùng."));
        }

        if (
            request.Items.Any(
                x =>
                    x.LessonId <= 0 ||
                    x.SortOrder < 0))
        {
            return Result.Failure(
                Error.Validation(
                    "Course.InvalidLessonOrder",
                    "LessonId hoặc SortOrder không hợp lệ."));
        }

        var course =
            await _dbContext.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == courseId,
                    cancellationToken);

        if (course is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Course.NotFound",
                    "Không tìm thấy Course."));
        }

        if (course.Status !=
            ContentStatus.Draft)
        {
            return Result.Failure(
                Error.Validation(
                    "Course.NotEditable",
                    "Chỉ Course Draft mới được reorder Lesson."));
        }

        var chapterExists =
            await _dbContext.CourseChapters
                .AnyAsync(
                    x =>
                        x.Id == chapterId &&
                        x.CourseId == courseId,
                    cancellationToken);

        if (!chapterExists)
        {
            return Result.Failure(
                Error.NotFound(
                    "Course.ChapterNotFound",
                    "Không tìm thấy Chapter."));
        }

        var lessonIds =
            request.Items
                .Select(x => x.LessonId)
                .ToArray();

        var lessons =
            await _dbContext.Lessons
                .Where(
                    x =>
                        x.CourseChapterId ==
                            chapterId &&
                        lessonIds.Contains(
                            x.Id))
                .ToListAsync(
                    cancellationToken);

        if (lessons.Count !=
            lessonIds.Length)
        {
            return Result.Failure(
                Error.NotFound(
                    "Course.LessonNotFound",
                    "Có Lesson không thuộc Chapter."));
        }

        var orders =
            request.Items
                .ToDictionary(
                    x => x.LessonId,
                    x => x.SortOrder);

        // Phase 1: Set temporary negative SortOrder to bypass unique constraint (CourseChapterId, SortOrder)
        int tempIndex = 1;
        foreach (var lesson in lessons)
        {
            lesson.ChangeOrder(-tempIndex++);
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        // Phase 2: Set final requested SortOrder
        foreach (var lesson in lessons)
        {
            lesson.ChangeOrder(
                orders[lesson.Id]);
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await InvalidatePublicCacheAsync(
            cancellationToken);

        return Result.Success();
    }
}
