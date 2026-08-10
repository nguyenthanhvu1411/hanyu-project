using HanYu.Application.Common.Models;
using HanYu.Application.Features.Lesson.Public;
using HanYu.Application.Interfaces.Lesson;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Lesson;

public sealed class PublicLessonService : IPublicLessonService
{
    private readonly IHanYuDbContext _dbContext;

    public PublicLessonService(IHanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ============================================================
    // DETAIL
    // ============================================================

    public async Task<Result<PublicLessonDetailDto>> GetAccessibleLessonAsync(
        Guid publicId,
        CancellationToken cancellationToken = default)
    {
        if (publicId == Guid.Empty)
        {
            return Result.Failure<PublicLessonDetailDto>(Error.Failure("Lesson.InvalidPublicId", "Lesson PublicId không hợp lệ."));
        }

        var lesson = await _dbContext.Lessons
            .AsNoTracking()
            .Where(x => x.PublicId == publicId && x.Status == ContentStatus.Published && x.DeletedAt == null)
            .Select(x => new
            {
                x.PublicId,
                x.Slug,
                x.TitleVi,
                x.ShortDescriptionVi,
                x.DescriptionVi,
                x.ObjectiveVi,
                x.CoverImageUrl,
                x.EstimatedMinutes,
                x.Difficulty,
                HskCode = x.HskLevel != null ? x.HskLevel.Code : string.Empty,
                HskNameVi = x.HskLevel != null ? x.HskLevel.NameVi : string.Empty,
                CoursePublicId = x.CourseChapter != null ? x.CourseChapter.Course.PublicId : (Guid?)null,
                CourseSlug = x.CourseChapter != null ? x.CourseChapter.Course.Slug : null,
                CourseTitleVi = x.CourseChapter != null ? x.CourseChapter.Course.TitleVi : null,
                CourseStatus = x.CourseChapter != null ? x.CourseChapter.Course.Status : (ContentStatus?)null,
                CourseActive = x.CourseChapter != null ? x.CourseChapter.Course.IsActive : true,
                CourseDeleted = x.CourseChapter != null ? x.CourseChapter.Course.DeletedAt : null,
                ChapterPublicId = x.CourseChapter != null ? x.CourseChapter.PublicId : (Guid?)null,
                ChapterTitleVi = x.CourseChapter != null ? x.CourseChapter.TitleVi : null,
                ChapterActive = x.CourseChapter != null ? x.CourseChapter.IsActive : true,
                ChapterDeleted = x.CourseChapter != null ? x.CourseChapter.DeletedAt : null,
                Sections = x.Sections
                    .Where(section => section.DeletedAt == null)
                    .OrderBy(section => section.SortOrder)
                    .Select(section => new PublicLessonSectionDto(
                        section.PublicId,
                        section.SectionType,
                        section.TitleVi,
                        section.ContentVi,
                        section.SortOrder,
                        section.IsRequired,
                        section.EstimatedSeconds))
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<PublicLessonDetailDto>(Error.Failure("Lesson.NotFound", "Không tìm thấy Lesson."));
        }

        // ========================================================
        // COURSE-AWARE ACCESS
        // ========================================================

        if (lesson.CoursePublicId.HasValue)
        {
            if (lesson.CourseDeleted.HasValue ||
                !lesson.CourseActive ||
                lesson.CourseStatus != ContentStatus.Published ||
                lesson.ChapterDeleted.HasValue ||
                !lesson.ChapterActive)
            {
                return Result.Failure<PublicLessonDetailDto>(Error.Failure("Lesson.NotAvailable", "Lesson hiện không khả dụng."));
            }
        }

        PublicLessonContextDto? context = null;
        if (lesson.CoursePublicId.HasValue && lesson.ChapterPublicId.HasValue)
        {
            context = new PublicLessonContextDto(
                lesson.CoursePublicId.Value,
                lesson.CourseSlug!,
                lesson.CourseTitleVi!,
                lesson.ChapterPublicId.Value,
                lesson.ChapterTitleVi!);
        }

        return Result.Success(
            new PublicLessonDetailDto(
                lesson.PublicId,
                lesson.Slug,
                lesson.TitleVi,
                lesson.ShortDescriptionVi,
                lesson.DescriptionVi,
                lesson.ObjectiveVi,
                lesson.CoverImageUrl,
                lesson.HskCode,
                lesson.HskNameVi,
                lesson.EstimatedMinutes,
                lesson.Difficulty,
                context,
                lesson.Sections));
    }

    // ============================================================
    // START
    // ============================================================

    public async Task<Result> StartAsync(
        Guid publicId,
        CancellationToken cancellationToken = default)
    {
        var accessible = await GetAccessibleLessonAsync(publicId, cancellationToken);
        if (!accessible.IsSuccess)
        {
            return Result.Failure(accessible.Error);
        }

        /*
         * TODO: nối LearningActivity / learning session.
         * Không fake ghi progress ở đây.
         */

        return Result.Success();
    }

    // ============================================================
    // COMPLETE
    // ============================================================

    public async Task<Result> CompleteAsync(
        Guid publicId,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return Result.Failure(Error.Failure("Lesson.IdempotencyKeyRequired", "Idempotency-Key là bắt buộc."));
        }

        var accessible = await GetAccessibleLessonAsync(publicId, cancellationToken);
        if (!accessible.IsSuccess)
        {
            return Result.Failure(accessible.Error);
        }

        /*
         * Không giả lập completion ở đây.
         * Bước Progress cần transaction:
         * UserLessonProgress + LearningActivity + OutboxEvent LessonCompleted
         * trong cùng transaction.
         */

        return Result.Failure(Error.Failure("Lesson.ProgressNotConfigured", "Lesson completion chưa được nối với Progress/Outbox."));
    }
}
