using HanYu.Application.Common.Models;
using HanYu.Application.Features.Lesson.Admin.Assets;
using HanYu.Application.Features.Lesson.Admin.Lessons;
using HanYu.Application.Features.Lesson.Admin.Prerequisites;
using HanYu.Application.Features.Lesson.Admin.Sections;
using HanYu.Application.Features.Lesson.Admin.Vocabulary;
using HanYu.Application.Features.Lesson.Mapping;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Caching;
using HanYu.Application.Interfaces.Lesson;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Entities.Learning;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Entities.Course;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HanYu.Infrastructure.Lesson;

public sealed class LessonAdminService : ILessonAdminService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IHanYuDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly ICourseCacheInvalidator
        _courseCacheInvalidator;

    public LessonAdminService(
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

    public async Task<Result<PagedResult<AdminLessonListItemDto>>> GetLessonsAsync(
        AdminLessonQuery request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? DefaultPageSize : Math.Min(request.PageSize, MaxPageSize);

        IQueryable<Domain.Entities.Lesson.Lesson> query = _dbContext.Lessons.AsNoTracking();

        if (request.IncludeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        // SEARCH
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var keyword = request.Search.Trim();
            query = query.Where(
                x => EF.Functions.ILike(x.Slug, $"%{keyword}%") ||
                     EF.Functions.ILike(x.TitleVi, $"%{keyword}%"));
        }

        // COURSE
        if (request.CourseId.HasValue)
        {
            query = query.Where(
                x => x.CourseChapter != null &&
                     x.CourseChapter.CourseId == request.CourseId.Value);
        }

        // CHAPTER
        if (request.ChapterId.HasValue)
        {
            query = query.Where(
                x => x.CourseChapterId == request.ChapterId.Value);
        }

        // HSK
        if (request.HskLevelId.HasValue)
        {
            query = query.Where(
                x => x.HskLevelId == request.HskLevelId.Value);
        }

        // TOPIC
        if (request.TopicId.HasValue)
        {
            query = query.Where(
                x => x.TopicId == request.TopicId.Value);
        }

        // STATUS
        if (request.Status.HasValue)
        {
            query = query.Where(
                x => x.Status == request.Status.Value);
        }

        // FEATURED
        if (request.IsFeatured.HasValue)
        {
            query = query.Where(
                x => x.IsFeatured == request.IsFeatured.Value);
        }

        var total = await query.CountAsync(cancellationToken);

        query = ApplySort(query, request.SortBy, request.SortDescending);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminLessonListItemDto(
                x.Id,
                x.PublicId,
                x.CourseChapter != null ? x.CourseChapter.CourseId : null,
                x.CourseChapter != null ? x.CourseChapter.Course.TitleVi : null,
                x.CourseChapterId,
                x.CourseChapter != null ? x.CourseChapter.TitleVi : null,
                x.HskLevelId,
                x.HskLevel != null ? x.HskLevel.Code : null,
                x.TopicId,
                x.Slug,
                x.TitleVi,
                x.ShortDescriptionVi,
                x.SortOrder,
                x.EstimatedMinutes,
                x.Difficulty,
                x.IsFeatured,
                x.Status,
                x.Version,
                x.PublishedAt,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);

        return Result.Success(
            new PagedResult<AdminLessonListItemDto>(
                items,
                page,
                pageSize,
                total));
    }

    // ============================================================
    // DETAIL
    // ============================================================

    public async Task<Result<AdminLessonDetailDto>> GetLessonAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.InvalidId", "Lesson ID không hợp lệ."));
        }

        var lesson = await GetDetailQuery(includeDeleted: true)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.NotFound", "Không tìm thấy Lesson."));
        }

        return Result.Success(LessonAdminMapper.ToDetailDto(lesson));
    }

    // ============================================================
    // CREATE
    // ============================================================

    public async Task<Result<AdminLessonDetailDto>> CreateLessonAsync(
        CreateLessonRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var userId = GetRequiredUserId();
        var slug = NormalizeSlug(request.Slug);

        var slugExists = await _dbContext.Lessons
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Slug == slug, cancellationToken);

        if (slugExists)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.SlugExists", "Slug Lesson đã tồn tại."));
        }

        var chapterValidation = await ValidateChapterAsync(
            request.CourseChapterId, request.SortOrder, null, cancellationToken);

        if (!chapterValidation.IsValid)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure(chapterValidation.Code!, chapterValidation.Message!));
        }

        if (request.TopicId.HasValue)
        {
            var topicExists = await _dbContext.Set<Topic>()
                .AnyAsync(x => x.Id == request.TopicId.Value, cancellationToken);

            if (!topicExists)
            {
                return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.TopicNotFound", "Không tìm thấy Topic."));
            }
        }

        var hskExists = await _dbContext.HskLevels
            .AnyAsync(x => x.Id == request.HskLevelId, cancellationToken);

        if (!hskExists)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.HskNotFound", "Không tìm thấy HSK Level."));
        }

        Domain.Entities.Lesson.Lesson lesson;

        try
        {
            lesson = new Domain.Entities.Lesson.Lesson(
                request.HskLevelId, slug, request.TitleVi, request.SortOrder);

            lesson.UpdateCore(
                request.HskLevelId, slug, request.TitleVi, request.ShortDescriptionVi,
                request.DescriptionVi, request.ObjectiveVi, request.SortOrder,
                request.EstimatedMinutes, request.Difficulty);

            lesson.AssignTopic(request.TopicId);
            lesson.UpdateCover(request.CoverImageUrl);
            lesson.SetFeatured(request.IsFeatured);

            if (request.CourseChapterId.HasValue)
            {
                lesson.AssignCourseChapter(request.CourseChapterId);
            }

            lesson.SetCreatedBy(userId);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.ValidationFailed", exception.Message));
        }

        _dbContext.Lessons.Add(lesson);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (lesson.CourseChapterId.HasValue)
        {
            await InvalidateCourseCacheAsync(cancellationToken);
        }

        return await GetDetailResultAsync(lesson.Id, cancellationToken);
    }

    // ============================================================
    // UPDATE
    // ============================================================

    public async Task<Result<AdminLessonDetailDto>> UpdateLessonAsync(
        long id,
        UpdateLessonRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id <= 0)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.InvalidId", "Lesson ID không hợp lệ."));
        }

        var lesson = await _dbContext.Lessons.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.NotFound", "Không tìm thấy Lesson."));
        }

        if (lesson.Version != request.Version)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("CONCURRENCY.CONFLICT", "Lesson đã được thay đổi. Vui lòng tải lại dữ liệu."));
        }

        var slug = NormalizeSlug(request.Slug);

        var slugExists = await _dbContext.Lessons
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Id != id && x.Slug == slug, cancellationToken);

        if (slugExists)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.SlugExists", "Slug đã được Lesson khác sử dụng."));
        }

        var chapterValidation = await ValidateChapterAsync(
            request.CourseChapterId, request.SortOrder, id, cancellationToken);

        if (!chapterValidation.IsValid)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure(chapterValidation.Code!, chapterValidation.Message!));
        }

        var oldChapterId = lesson.CourseChapterId;

        try
        {
            if (lesson.CourseChapterId != request.CourseChapterId)
            {
                lesson.AssignCourseChapter(request.CourseChapterId);
            }

            lesson.UpdateCore(
                request.HskLevelId, slug, request.TitleVi, request.ShortDescriptionVi,
                request.DescriptionVi, request.ObjectiveVi, request.SortOrder,
                request.EstimatedMinutes, request.Difficulty);

            lesson.AssignTopic(request.TopicId);
            lesson.UpdateCover(request.CoverImageUrl);
            lesson.SetFeatured(request.IsFeatured);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.ValidationFailed", exception.Message));
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (oldChapterId.HasValue || lesson.CourseChapterId.HasValue)
        {
            await InvalidateCourseCacheAsync(cancellationToken);
        }

        return await GetDetailResultAsync(id, cancellationToken);
    }

    // ============================================================
    // VALIDATE
    // ============================================================

    public async Task<Result<LessonValidationResultDto>> ValidateLessonAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _dbContext.Lessons
            .AsNoTracking()
            .Include(x => x.HskLevel)
            .Include(x => x.CourseChapter).ThenInclude(x => x!.Course)
            .Include(x => x.Sections)
            .Include(x => x.LessonVocabularies)
            .Include(x => x.Assets)
            .Include(x => x.Prerequisites)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<LessonValidationResultDto>(Error.Failure("Lesson.NotFound", "Không tìm thấy Lesson."));
        }

        var issues = new List<LessonValidationIssueDto>();

        if (string.IsNullOrWhiteSpace(lesson.Slug))
        {
            issues.Add(new("Lesson.SlugRequired", "Lesson chưa có Slug.", "slug"));
        }

        if (string.IsNullOrWhiteSpace(lesson.TitleVi))
        {
            issues.Add(new("Lesson.TitleRequired", "Lesson chưa có tiêu đề.", "titleVi"));
        }

        if (lesson.HskLevel is null)
        {
            issues.Add(new("Lesson.HskNotFound", "HSK Level không tồn tại.", "hskLevelId"));
        }

        if (lesson.CourseChapterId.HasValue)
        {
            if (lesson.CourseChapter is null)
            {
                issues.Add(new("Lesson.ChapterNotFound", "Chapter không tồn tại.", "courseChapterId"));
            }
            else
            {
                if (lesson.CourseChapter.IsDeleted)
                {
                    issues.Add(new("Lesson.ChapterDeleted", "Chapter đã bị xóa.", "courseChapterId"));
                }
                if (!lesson.CourseChapter.IsActive)
                {
                    issues.Add(new("Lesson.ChapterInactive", "Chapter đang bị vô hiệu.", "courseChapterId"));
                }
                if (lesson.CourseChapter.Course.IsDeleted)
                {
                    issues.Add(new("Lesson.CourseDeleted", "Course đã bị xóa.", "courseChapterId"));
                }
            }
        }

        if (!lesson.Sections.Any(x => !x.IsDeleted))
        {
            issues.Add(new("Lesson.SectionRequired", "Lesson phải có ít nhất một Section.", "sections"));
        }

        return Result.Success(new LessonValidationResultDto(issues.Count == 0, issues));
    }

    // ============================================================
    // SUBMIT REVIEW
    // ============================================================

    public async Task<Result<AdminLessonDetailDto>> SubmitForReviewAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateLessonAsync(id, cancellationToken);
        if (!validation.IsSuccess)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure(validation.Error.Code, validation.Error.Message));
        }
        if (!validation.Value.IsValid)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.ValidationFailed", string.Join(" | ", validation.Value.Issues.Select(x => x.Message))));
        }

        return await ExecuteWorkflowAsync(
            id, request.Version, static lesson => lesson.SubmitForReview(),
            invalidateCourse: false, cancellationToken);
    }

    // ============================================================
    // APPROVE
    // ============================================================

    public async Task<Result<AdminLessonDetailDto>> ApproveAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteWorkflowAsync(
            id, request.Version, static lesson => lesson.Approve(),
            invalidateCourse: false, cancellationToken);
    }

    // ============================================================
    // PUBLISH
    // ============================================================

    public async Task<Result<AdminLessonDetailDto>> PublishAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateLessonAsync(id, cancellationToken);
        if (!validation.IsSuccess)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure(validation.Error.Code, validation.Error.Message));
        }
        if (!validation.Value.IsValid)
        {
            return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.NotPublishable", string.Join(" | ", validation.Value.Issues.Select(x => x.Message))));
        }

        return await ExecuteWorkflowAsync(
            id, request.Version, static lesson => lesson.Publish(),
            invalidateCourse: true, cancellationToken);
    }

    // ============================================================
    // ARCHIVE
    // ============================================================

    public async Task<Result<AdminLessonDetailDto>> ArchiveAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteWorkflowAsync(
            id, request.Version, static lesson => lesson.Archive(),
            invalidateCourse: true, cancellationToken);
    }

    // ============================================================
    // RESTORE ARCHIVED
    // ============================================================

    public async Task<Result<AdminLessonDetailDto>> RestoreAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteWorkflowAsync(
            id, request.Version, static lesson => lesson.RestoreToDraft(),
            invalidateCourse: true, cancellationToken);
    }

    // ============================================================
    // DELETE
    // ============================================================

    public async Task<Result> DeleteAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _dbContext.Lessons.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (lesson is null) return Result.Failure(Error.Failure("Lesson.NotFound", "Không tìm thấy Lesson."));
        if (lesson.Version != request.Version) return Result.Failure(Error.Failure("CONCURRENCY.CONFLICT", "Lesson đã được thay đổi."));
        if (lesson.Status == ContentStatus.Published) return Result.Failure(Error.Failure("Lesson.PublishedCannotDelete", "Hãy Archive Lesson trước khi xóa."));

        var hadCourse = lesson.CourseChapterId.HasValue;
        lesson.SoftDelete(GetRequiredUserId());
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (hadCourse) await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success();
    }

    // ============================================================
    // RESTORE DELETED
    // ============================================================

    public async Task<Result<AdminLessonDetailDto>> RestoreDeletedAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _dbContext.Lessons.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt != null, cancellationToken);
        if (lesson is null) return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.DeletedNotFound", "Không tìm thấy Lesson đã xóa."));
        if (lesson.Version != request.Version) return Result.Failure<AdminLessonDetailDto>(Error.Failure("CONCURRENCY.CONFLICT", "Lesson đã được thay đổi."));

        var slugConflict = await _dbContext.Lessons.AnyAsync(x => x.Id != id && x.Slug == lesson.Slug, cancellationToken);
        if (slugConflict) return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.RestoreSlugConflict", "Slug của Lesson đã được sử dụng."));

        lesson.Restore(GetRequiredUserId());
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (lesson.CourseChapterId.HasValue) await InvalidateCourseCacheAsync(cancellationToken);

        return await GetDetailResultAsync(id, cancellationToken);
    }

    // ============================================================
    // WORKFLOW
    // ============================================================

    private async Task<Result<AdminLessonDetailDto>> ExecuteWorkflowAsync(
        long id,
        int expectedVersion,
        Action<Domain.Entities.Lesson.Lesson> action,
        bool invalidateCourse,
        CancellationToken cancellationToken)
    {
        var lesson = await _dbContext.Lessons.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (lesson is null) return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.NotFound", "Không tìm thấy Lesson."));
        if (lesson.Version != expectedVersion) return Result.Failure<AdminLessonDetailDto>(Error.Failure("CONCURRENCY.CONFLICT", "Lesson đã được thay đổi. Vui lòng tải lại."));

        try { action(lesson); } catch (InvalidOperationException exception) { return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.InvalidState", exception.Message)); }

        await _dbContext.SaveChangesAsync(cancellationToken);
        if (invalidateCourse && lesson.CourseChapterId.HasValue) await InvalidateCourseCacheAsync(cancellationToken);

        return await GetDetailResultAsync(id, cancellationToken);
    }

    // ============================================================
    // CHAPTER VALIDATION
    // ============================================================

    private async Task<ValidationState> ValidateChapterAsync(
        long? chapterId, int sortOrder, long? excludeLessonId, CancellationToken cancellationToken)
    {
        if (!chapterId.HasValue) return ValidationState.Valid();
        if (chapterId.Value <= 0) return ValidationState.Invalid("Lesson.InvalidChapter", "CourseChapterId không hợp lệ.");

        var chapter = await _dbContext.CourseChapters.AsNoTracking().Include(x => x.Course).FirstOrDefaultAsync(x => x.Id == chapterId.Value, cancellationToken);
        if (chapter is null) return ValidationState.Invalid("Lesson.ChapterNotFound", "Không tìm thấy Chapter.");
        if (chapter.IsDeleted) return ValidationState.Invalid("Lesson.ChapterDeleted", "Chapter đã bị xóa.");
        if (!chapter.IsActive) return ValidationState.Invalid("Lesson.ChapterInactive", "Chapter đang bị vô hiệu.");
        if (chapter.Course.IsDeleted) return ValidationState.Invalid("Lesson.CourseDeleted", "Course đã bị xóa.");
        if (chapter.Course.Status != ContentStatus.Draft) return ValidationState.Invalid("Lesson.CourseNotEditable", "Chỉ Course Draft mới được thay đổi cấu trúc Lesson.");

        var orderConflict = await _dbContext.Lessons.AnyAsync(
            x => x.CourseChapterId == chapterId.Value && x.SortOrder == sortOrder && (!excludeLessonId.HasValue || x.Id != excludeLessonId.Value), cancellationToken);

        if (orderConflict) return ValidationState.Invalid("Lesson.SortOrderExists", "Chapter đã có Lesson tại SortOrder này.");

        return ValidationState.Valid();
    }

    // ============================================================
    // DETAIL QUERY
    // ============================================================

    private IQueryable<Domain.Entities.Lesson.Lesson> GetDetailQuery(bool includeDeleted = false)
    {
        IQueryable<Domain.Entities.Lesson.Lesson> query = _dbContext.Lessons;
        if (includeDeleted) query = query.IgnoreQueryFilters();

        return query.AsNoTracking().Include(x => x.HskLevel).Include(x => x.Topic)
            .Include(x => x.CourseChapter).ThenInclude(x => x!.Course)
            .Include(x => x.Sections).Include(x => x.LessonVocabularies).Include(x => x.Assets).Include(x => x.Prerequisites);
    }

    private async Task<Result<AdminLessonDetailDto>> GetDetailResultAsync(long id, CancellationToken cancellationToken)
    {
        var lesson = await GetDetailQuery(includeDeleted: true).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (lesson is null) return Result.Failure<AdminLessonDetailDto>(Error.Failure("Lesson.NotFound", "Không tìm thấy Lesson."));
        return Result.Success(LessonAdminMapper.ToDetailDto(lesson));
    }

    // ============================================================
    // SORT
    // ============================================================

    private static IQueryable<Domain.Entities.Lesson.Lesson> ApplySort(
        IQueryable<Domain.Entities.Lesson.Lesson> query, string? sortBy, bool descending)
    {
        var sort = sortBy?.Trim().ToLowerInvariant();
        return sort switch
        {
            "title" or "titlevi" => descending ? query.OrderByDescending(x => x.TitleVi) : query.OrderBy(x => x.TitleVi),
            "createdat" => descending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
            "updatedat" => descending ? query.OrderByDescending(x => x.UpdatedAt) : query.OrderBy(x => x.UpdatedAt),
            "publishedat" => descending ? query.OrderByDescending(x => x.PublishedAt) : query.OrderBy(x => x.PublishedAt),
            "sortorder" => descending ? query.OrderByDescending(x => x.SortOrder) : query.OrderBy(x => x.SortOrder),
            _ => query.OrderBy(x => x.SortOrder).ThenBy(x => x.TitleVi).ThenBy(x => x.Id)
        };
    }

    // ============================================================
    // CACHE
    // ============================================================

    private Task InvalidateCourseCacheAsync(CancellationToken cancellationToken) => _courseCacheInvalidator.InvalidatePublicCourseCacheAsync(cancellationToken);

    // ============================================================
    // USER
    // ============================================================

    private Guid GetRequiredUserId()
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue || userId.Value == Guid.Empty) throw new UnauthorizedAccessException("Không xác định được người dùng hiện tại.");
        return userId.Value;
    }

    private static string NormalizeSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Slug không được để trống.", nameof(slug));
        return string.Join('-', slug.Trim().ToLowerInvariant().Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries));
    }

    private sealed record ValidationState(bool IsValid, string? Code, string? Message)
    {
        public static ValidationState Valid() => new(true, null, null);
        public static ValidationState Invalid(string code, string message) => new(false, code, message);
    }

    // =========================================================
    // SECTIONS
    // =========================================================

    public async Task<Result<
        IReadOnlyCollection<AdminLessonSectionResponse>>>
        GetSectionsAsync(
            long lessonId,
            CancellationToken cancellationToken = default)
    {
        if (!await LessonExistsAsync(
                lessonId,
                cancellationToken))
        {
            return Result.Failure<
                IReadOnlyCollection<AdminLessonSectionResponse>>(
                NotFound("Lesson"));
        }

        var values =
            await _dbContext.Set<LessonSection>()
                .AsNoTracking()
                .Where(x => x.LessonId == lessonId)
                .OrderBy(x => x.SortOrder)
                .ToArrayAsync(cancellationToken);

        return Result.Success<
            IReadOnlyCollection<AdminLessonSectionResponse>>(
            values
                .Select(LessonAdminMapper.ToSectionResponse)
                .ToArray());
    }

    public async Task<Result<AdminLessonSectionResponse>>
        CreateSectionAsync(
            long lessonId,
            CreateLessonSectionRequest request,
            CancellationToken cancellationToken = default)
    {
        var parent =
            await GetEditableLessonAsync(
                lessonId,
                cancellationToken);

        if (parent.IsFailure)
            return Result.Failure<AdminLessonSectionResponse>(
                parent.Error);

        var duplicateOrder =
            await _dbContext.Set<LessonSection>()
                .AnyAsync(
                    x =>
                        x.LessonId == lessonId &&
                        x.SortOrder == request.SortOrder,
                    cancellationToken);

        if (duplicateOrder)
        {
            return Result.Failure<AdminLessonSectionResponse>(
                Error.Conflict(
                    "LessonSection.DuplicateOrder",
                    "SortOrder đã tồn tại."));
        }

        var entity =
            new LessonSection(
                lessonId,
                request.SectionType,
                request.SortOrder,
                request.TitleVi);

        entity.UpdateContent(
            request.TitleVi,
            request.ContentVi);

        entity.SetRequired(
            request.IsRequired);

        entity.UpdateEstimatedTime(
            request.EstimatedSeconds);

        _dbContext.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success(
            LessonAdminMapper.ToSectionResponse(entity));
    }

    public async Task<Result<AdminLessonSectionResponse>>
        UpdateSectionAsync(
            long lessonId,
            long sectionId,
            UpdateLessonSectionRequest request,
            CancellationToken cancellationToken = default)
    {
        var parent =
            await GetEditableLessonAsync(
                lessonId,
                cancellationToken);

        if (parent.IsFailure)
            return Result.Failure<AdminLessonSectionResponse>(
                parent.Error);

        var entity =
            await _dbContext.Set<LessonSection>()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == sectionId &&
                        x.LessonId == lessonId,
                    cancellationToken);

        if (entity is null)
            return Result.Failure<AdminLessonSectionResponse>(
                NotFound("LessonSection"));

        var duplicateOrder =
            await _dbContext.Set<LessonSection>()
                .AnyAsync(
                    x =>
                        x.Id != sectionId &&
                        x.LessonId == lessonId &&
                        x.SortOrder == request.SortOrder,
                    cancellationToken);

        if (duplicateOrder)
        {
            return Result.Failure<AdminLessonSectionResponse>(
                Error.Conflict(
                    "LessonSection.DuplicateOrder",
                    "SortOrder đã tồn tại."));
        }

        entity.ChangeType(request.SectionType);
        entity.UpdateContent(
            request.TitleVi,
            request.ContentVi);
        entity.ChangeOrder(request.SortOrder);
        entity.SetRequired(request.IsRequired);
        entity.UpdateEstimatedTime(
            request.EstimatedSeconds);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success(
            LessonAdminMapper.ToSectionResponse(entity));
    }

    public async Task<Result> DeleteSectionAsync(
        long lessonId,
        long sectionId,
        CancellationToken cancellationToken = default)
    {
        var parent =
            await GetEditableLessonAsync(
                lessonId,
                cancellationToken);

        if (parent.IsFailure)
            return Result.Failure(parent.Error);

        var hasProgress =
            await _dbContext.Set<UserLessonSectionProgress>()
                .AnyAsync(
                    x => x.LessonSectionId == sectionId,
                    cancellationToken);

        if (hasProgress)
        {
            return Result.Failure(
                Error.Conflict(
                    "LessonSection.HasProgress",
                    "Section đã có dữ liệu học và không thể xóa."));
        }

        var entity =
            await _dbContext.Set<LessonSection>()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == sectionId &&
                        x.LessonId == lessonId,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("LessonSection"));

        _dbContext.Remove(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success();
    }

    // =========================================================
    // ASSETS
    // =========================================================

    public async Task<Result<
        IReadOnlyCollection<AdminLessonAssetResponse>>>
        GetAssetsAsync(
            long lessonId,
            CancellationToken cancellationToken = default)
    {
        var values =
            await _dbContext.Set<LessonAsset>()
                .AsNoTracking()
                .Where(x => x.LessonId == lessonId)
                .OrderBy(x => x.SortOrder)
                .ToArrayAsync(cancellationToken);

        return Result.Success<
            IReadOnlyCollection<AdminLessonAssetResponse>>(
            values.Select(
                    LessonAdminMapper.ToAssetResponse)
                .ToArray());
    }

    public async Task<Result<AdminLessonAssetResponse>>
        CreateAssetAsync(
            long lessonId,
            CreateLessonAssetRequest request,
            CancellationToken cancellationToken = default)
    {
        var parent =
            await GetEditableLessonAsync(
                lessonId,
                cancellationToken);

        if (parent.IsFailure)
            return Result.Failure<AdminLessonAssetResponse>(
                parent.Error);

        var audioValidation =
            await ValidateAudioAsync(
                request.AudioAssetId,
                cancellationToken);

        if (audioValidation.IsFailure)
            return Result.Failure<AdminLessonAssetResponse>(
                audioValidation.Error);

        var entity =
            new LessonAsset(
                lessonId,
                request.AssetType,
                request.SortOrder);

        entity.Update(
            request.Url,
            request.CaptionVi,
            request.AudioAssetId,
            request.SortOrder);

        _dbContext.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success(
            LessonAdminMapper.ToAssetResponse(entity));
    }

    public async Task<Result<AdminLessonAssetResponse>>
        UpdateAssetAsync(
            long lessonId,
            long assetId,
            UpdateLessonAssetRequest request,
            CancellationToken cancellationToken = default)
    {
        var parent =
            await GetEditableLessonAsync(
                lessonId,
                cancellationToken);

        if (parent.IsFailure)
            return Result.Failure<AdminLessonAssetResponse>(
                parent.Error);

        var audioValidation =
            await ValidateAudioAsync(
                request.AudioAssetId,
                cancellationToken);

        if (audioValidation.IsFailure)
            return Result.Failure<AdminLessonAssetResponse>(
                audioValidation.Error);

        var entity =
            await _dbContext.Set<LessonAsset>()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == assetId &&
                        x.LessonId == lessonId,
                    cancellationToken);

        if (entity is null)
            return Result.Failure<AdminLessonAssetResponse>(
                NotFound("LessonAsset"));

        entity.Update(
            request.Url,
            request.CaptionVi,
            request.AudioAssetId,
            request.SortOrder);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success(
            LessonAdminMapper.ToAssetResponse(entity));
    }

    public async Task<Result> DeleteAssetAsync(
        long lessonId,
        long assetId,
        CancellationToken cancellationToken = default)
    {
        var parent =
            await GetEditableLessonAsync(
                lessonId,
                cancellationToken);

        if (parent.IsFailure)
            return Result.Failure(parent.Error);

        var entity =
            await _dbContext.Set<LessonAsset>()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == assetId &&
                        x.LessonId == lessonId,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("LessonAsset"));

        _dbContext.Remove(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success();
    }

    // =========================================================
    // VOCABULARY
    // =========================================================

    public async Task<Result<
        IReadOnlyCollection<AdminLessonVocabularyResponse>>>
        GetVocabularyAsync(
            long lessonId,
            CancellationToken cancellationToken = default)
    {
        var values =
            await _dbContext.Set<LessonVocabulary>()
                .AsNoTracking()
                .Include(x => x.Vocabulary)
                .Where(x => x.LessonId == lessonId)
                .OrderBy(x => x.SortOrder)
                .ToArrayAsync(cancellationToken);

        return Result.Success<
            IReadOnlyCollection<AdminLessonVocabularyResponse>>(
            values
                .Select(
                    LessonAdminMapper.ToVocabularyResponse)
                .ToArray());
    }

    public async Task<Result<AdminLessonVocabularyResponse>>
        AttachVocabularyAsync(
            long lessonId,
            AttachLessonVocabularyRequest request,
            CancellationToken cancellationToken = default)
    {
        var parent =
            await GetEditableLessonAsync(
                lessonId,
                cancellationToken);

        if (parent.IsFailure)
            return Result.Failure<AdminLessonVocabularyResponse>(
                parent.Error);

        var vocabulary =
            await _dbContext.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == request.VocabularyId,
                    cancellationToken);

        if (vocabulary is null)
            return Result.Failure<AdminLessonVocabularyResponse>(
                Error.NotFound(
                    "Vocabulary.NotFound",
                    "Không tìm thấy Vocabulary."));

        if (vocabulary.Status ==
            ContentStatus.Archived)
        {
            return Result.Failure<AdminLessonVocabularyResponse>(
                Error.Conflict(
                    "Vocabulary.Archived",
                    "Không thể thêm Vocabulary Archived."));
        }

        var duplicate =
            await _dbContext.Set<LessonVocabulary>()
                .AnyAsync(
                    x =>
                        x.LessonId == lessonId &&
                        x.VocabularyId ==
                            request.VocabularyId,
                    cancellationToken);

        if (duplicate)
        {
            return Result.Failure<AdminLessonVocabularyResponse>(
                Error.Conflict(
                    "LessonVocabulary.Duplicate",
                    "Vocabulary đã tồn tại trong Lesson."));
        }

        var entity =
            new LessonVocabulary(
                lessonId,
                request.VocabularyId,
                request.SortOrder,
                request.IsRequired);

        _dbContext.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var loaded =
            await _dbContext.Set<LessonVocabulary>()
                .AsNoTracking()
                .Include(x => x.Vocabulary)
                .FirstAsync(
                    x =>
                        x.LessonId == lessonId &&
                        x.VocabularyId ==
                            request.VocabularyId,
                    cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success(
            LessonAdminMapper.ToVocabularyResponse(
                loaded));
    }

    public async Task<Result<AdminLessonVocabularyResponse>>
        UpdateVocabularyAsync(
            long lessonId,
            long vocabularyId,
            UpdateLessonVocabularyRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _dbContext.Set<LessonVocabulary>()
                .Include(x => x.Vocabulary)
                .FirstOrDefaultAsync(
                    x =>
                        x.LessonId == lessonId &&
                        x.VocabularyId == vocabularyId,
                    cancellationToken);

        if (entity is null)
            return Result.Failure<AdminLessonVocabularyResponse>(
                NotFound("LessonVocabulary"));

        entity.ChangeOrder(request.SortOrder);
        entity.SetRequired(request.IsRequired);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success(
            LessonAdminMapper.ToVocabularyResponse(entity));
    }

    public async Task<Result> DetachVocabularyAsync(
        long lessonId,
        long vocabularyId,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _dbContext.Set<LessonVocabulary>()
                .FirstOrDefaultAsync(
                    x =>
                        x.LessonId == lessonId &&
                        x.VocabularyId == vocabularyId,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("LessonVocabulary"));

        _dbContext.Remove(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success();
    }

    // =========================================================
    // PREREQUISITES
    // =========================================================

    public async Task<Result<
        IReadOnlyCollection<AdminLessonPrerequisiteResponse>>>
        GetPrerequisitesAsync(
            long lessonId,
            CancellationToken cancellationToken = default)
    {
        var values =
            await _dbContext.Set<LessonPrerequisite>()
                .AsNoTracking()
                .Include(x => x.RequiredLesson)
                .Where(x => x.LessonId == lessonId)
                .ToArrayAsync(cancellationToken);

        return Result.Success<
            IReadOnlyCollection<
                AdminLessonPrerequisiteResponse>>(
            values
                .Select(
                    LessonAdminMapper
                        .ToPrerequisiteResponse)
                .ToArray());
    }

    public async Task<Result<AdminLessonPrerequisiteResponse>>
        AddPrerequisiteAsync(
            long lessonId,
            AddLessonPrerequisiteRequest request,
            CancellationToken cancellationToken = default)
    {
        if (lessonId == request.RequiredLessonId)
        {
            return Result.Failure<
                AdminLessonPrerequisiteResponse>(
                Error.Validation(
                    "LessonPrerequisite.SelfReference",
                    "Lesson không thể prerequisite chính nó."));
        }

        if (!await LessonExistsAsync(
                lessonId,
                cancellationToken) ||
            !await LessonExistsAsync(
                request.RequiredLessonId,
                cancellationToken))
        {
            return Result.Failure<
                AdminLessonPrerequisiteResponse>(
                NotFound("Lesson"));
        }

        var duplicate =
            await _dbContext.Set<LessonPrerequisite>()
                .AnyAsync(
                    x =>
                        x.LessonId == lessonId &&
                        x.RequiredLessonId ==
                            request.RequiredLessonId,
                    cancellationToken);

        if (duplicate)
        {
            return Result.Failure<
                AdminLessonPrerequisiteResponse>(
                Error.Conflict(
                    "LessonPrerequisite.Duplicate",
                    "Prerequisite đã tồn tại."));
        }

        if (await WouldCreateCycleAsync(
                lessonId,
                request.RequiredLessonId,
                cancellationToken))
        {
            return Result.Failure<
                AdminLessonPrerequisiteResponse>(
                Error.Conflict(
                    "LessonPrerequisite.Cycle",
                    "Prerequisite sẽ tạo vòng lặp giữa các Lesson."));
        }

        var entity =
            new LessonPrerequisite(
                lessonId,
                request.RequiredLessonId);

        _dbContext.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var loaded =
            await _dbContext.Set<LessonPrerequisite>()
                .AsNoTracking()
                .Include(x => x.RequiredLesson)
                .FirstAsync(
                    x =>
                        x.LessonId == lessonId &&
                        x.RequiredLessonId ==
                            request.RequiredLessonId,
                    cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success(
            LessonAdminMapper
                .ToPrerequisiteResponse(loaded));
    }

    public async Task<Result> RemovePrerequisiteAsync(
        long lessonId,
        long requiredLessonId,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _dbContext.Set<LessonPrerequisite>()
                .FirstOrDefaultAsync(
                    x =>
                        x.LessonId == lessonId &&
                        x.RequiredLessonId ==
                            requiredLessonId,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("LessonPrerequisite"));

        _dbContext.Remove(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCourseCacheAsync(cancellationToken);

        return Result.Success();
    }

    // =========================================================
    // VALIDATION / HELPERS
    // =========================================================

    private async Task<Result> ValidatePublishableGraphAsync(
        long lessonId,
        CancellationToken cancellationToken)
    {
        var hasSections =
            await _dbContext.Set<LessonSection>()
                .AnyAsync(
                    x => x.LessonId == lessonId,
                    cancellationToken);

        if (!hasSections)
        {
            return Result.Failure(
                Error.Conflict(
                    "Lesson.NoSections",
                    "Lesson phải có ít nhất một Section."));
        }

        var invalidVocabulary =
            await _dbContext.Set<LessonVocabulary>()
                .Include(x => x.Vocabulary)
                .AnyAsync(
                    x =>
                        x.LessonId == lessonId &&
                        x.IsRequired &&
                        x.Vocabulary.Status !=
                            ContentStatus.Published,
                    cancellationToken);

        if (invalidVocabulary)
        {
            return Result.Failure(
                Error.Conflict(
                    "Lesson.InvalidVocabulary",
                    "Vocabulary bắt buộc phải Published trước khi publish Lesson."));
        }

        var invalidPrerequisite =
            await _dbContext.Set<LessonPrerequisite>()
                .Include(x => x.RequiredLesson)
                .AnyAsync(
                    x =>
                        x.LessonId == lessonId &&
                        x.RequiredLesson.Status !=
                            ContentStatus.Published,
                    cancellationToken);

        if (invalidPrerequisite)
        {
            return Result.Failure(
                Error.Conflict(
                    "Lesson.InvalidPrerequisite",
                    "Các prerequisite Lesson phải Published."));
        }

        return Result.Success();
    }

    private async Task<bool> WouldCreateCycleAsync(
        long lessonId,
        long requiredLessonId,
        CancellationToken cancellationToken)
    {
        var edges =
            await _dbContext.Set<LessonPrerequisite>()
                .AsNoTracking()
                .Select(x => new
                {
                    x.LessonId,
                    x.RequiredLessonId
                })
                .ToListAsync(cancellationToken);

        var graph =
            edges
                .GroupBy(x => x.LessonId)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(
                            y => y.RequiredLessonId)
                        .ToArray());

        var stack =
            new Stack<long>();

        var visited =
            new HashSet<long>();

        stack.Push(requiredLessonId);

        while (stack.Count > 0)
        {
            var current =
                stack.Pop();

            if (current == lessonId)
                return true;

            if (!visited.Add(current))
                continue;

            if (!graph.TryGetValue(
                    current,
                    out var next))
                continue;

            foreach (var value in next)
                stack.Push(value);
        }

        return false;
    }

    private async Task<Result>
        CheckDeleteDependenciesAsync(
            long lessonId,
            CancellationToken cancellationToken)
    {
        var progress =
            await _dbContext.Set<UserLessonProgress>()
                .AnyAsync(
                    x => x.LessonId == lessonId,
                    cancellationToken);

        if (progress)
        {
            return Result.Failure(
                Error.Conflict(
                    "Lesson.HasProgress",
                    "Lesson đã có dữ liệu học. Hãy Archive thay vì xóa."));
        }

        var isRequiredByAnotherLesson =
            await _dbContext.Set<LessonPrerequisite>()
                .AnyAsync(
                    x =>
                        x.RequiredLessonId == lessonId &&
                        x.LessonId != lessonId,
                    cancellationToken);

        if (isRequiredByAnotherLesson)
        {
            return Result.Failure(
                Error.Conflict(
                    "Lesson.IsPrerequisite",
                    "Lesson đang là prerequisite của Lesson khác."));
        }

        var relatedQuizzes = await _dbContext.Set<HanYu.Domain.Entities.Quiz.Quiz>()
                .AnyAsync(
                    x => x.LessonId == lessonId,
                    cancellationToken);

        if (relatedQuizzes)
        {
            return Result.Failure(
                Error.Conflict(
                    "Lesson.HasQuiz",
                    "Lesson đang được Quiz sử dụng."));
        }

        var bookmark =
            await _dbContext.Set<UserLessonBookmark>()
                .AnyAsync(
                    x => x.LessonId == lessonId,
                    cancellationToken);

        if (bookmark)
        {
            return Result.Failure(
                Error.Conflict(
                    "Lesson.HasBookmarks",
                    "Lesson đã được người dùng bookmark."));
        }

        return Result.Success();
    }

    private async Task<Result>
        ValidateReferencesAsync(
            long hskLevelId,
            long? topicId,
            long? courseChapterId,
            CancellationToken cancellationToken)
    {
        var hsk =
            await _dbContext.Set<HskLevel>()
                .AnyAsync(
                    x =>
                        x.Id == hskLevelId &&
                        x.IsActive,
                    cancellationToken);

        if (!hsk)
        {
            return Result.Failure(
                Error.Validation(
                    "Lesson.InvalidHsk",
                    "HSK Level không hợp lệ."));
        }

        if (topicId.HasValue)
        {
            var topic =
                await _dbContext.Set<Topic>()
                    .AnyAsync(
                        x => x.Id == topicId.Value,
                        cancellationToken);

            if (!topic)
            {
                return Result.Failure(
                    Error.Validation(
                        "Lesson.InvalidTopic",
                        "Topic không tồn tại."));
            }
        }

        return Result.Success();
    }

    private async Task<Result> ValidateAudioAsync(
        long? audioAssetId,
        CancellationToken cancellationToken)
    {
        if (!audioAssetId.HasValue)
            return Result.Success();

        var audio =
            await _dbContext.Set<AudioAsset>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == audioAssetId.Value,
                    cancellationToken);

        if (audio is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AudioAsset.NotFound",
                    "AudioAsset không tồn tại."));
        }

        if (audio.Status ==
            ContentStatus.Archived)
        {
            return Result.Failure(
                Error.Conflict(
                    "AudioAsset.Archived",
                    "AudioAsset đã Archived."));
        }

        return Result.Success();
    }

    private async Task<Result<
        Domain.Entities.Lesson.Lesson>>
        GetEditableLessonAsync(
            long id,
            CancellationToken cancellationToken)
    {
        var entity =
            await _dbContext.Set<Domain.Entities.Lesson.Lesson>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure<
                Domain.Entities.Lesson.Lesson>(
                NotFound("Lesson"));

        if (entity.Status ==
            ContentStatus.Archived)
        {
            return Result.Failure<
                Domain.Entities.Lesson.Lesson>(
                Error.Conflict(
                    "Lesson.Archived",
                    "Lesson Archived không thể chỉnh sửa."));
        }

        return Result.Success(entity);
    }

    private async Task<Result<
        Domain.Entities.Lesson.Lesson>>
        FindLessonAsync(
            long id,
            CancellationToken cancellationToken)
    {
        var entity =
            await _dbContext.Set<Domain.Entities.Lesson.Lesson>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        return entity is null
            ? Result.Failure<
                Domain.Entities.Lesson.Lesson>(
                NotFound("Lesson"))
            : Result.Success(entity);
    }

    private Task<bool> LessonExistsAsync(
        long id,
        CancellationToken cancellationToken)
        => _dbContext.Set<Domain.Entities.Lesson.Lesson>()
            .AnyAsync(
                x => x.Id == id,
                cancellationToken);

    private IQueryable<Domain.Entities.Lesson.Lesson>
        QueryLesson()
        => _dbContext.Set<Domain.Entities.Lesson.Lesson>()
            .Include(x => x.HskLevel)
            .Include(x => x.Topic);

    private static Error NotFound(
        string resource)
        => Error.NotFound(
            $"{resource}.NotFound",
            $"Không tìm thấy {resource}.");
}
