using HanYu.Application.Common.Models;
using HanYu.Application.Features.Course.Admin;

using HanYu.Application.Features.Course.Admin.Chapters;
using HanYu.Application.Features.Course.Admin.Prerequisites;

namespace HanYu.Application.Interfaces.Course;

public interface IAdminCourseService
{
    Task<Result<PagedResult<AdminCourseListItemDto>>>
        GetCoursesAsync(
            AdminCourseQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminCourseDetailDto>>
        GetCourseAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result<AdminCourseDetailDto>>
        CreateCourseAsync(
            CreateCourseRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminCourseDetailDto>>
        UpdateCourseAsync(
            long id,
            UpdateCourseRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<CourseValidationResultDto>>
        ValidateCourseAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result<AdminCourseDetailDto>>
        SubmitForReviewAsync(
            long id,
            CourseWorkflowRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminCourseDetailDto>>
        ApproveAsync(
            long id,
            CourseWorkflowRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminCourseDetailDto>>
        RejectAsync(
            long id,
            RejectCourseRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminCourseDetailDto>>
        PublishAsync(
            long id,
            CourseWorkflowRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminCourseDetailDto>>
        ArchiveAsync(
            long id,
            CourseWorkflowRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminCourseDetailDto>>
        RestoreAsync(
            long id,
            CourseWorkflowRequest request,
            CancellationToken cancellationToken = default);

    Task<Result>
        DeleteAsync(
            long id,
            CourseWorkflowRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminCourseDetailDto>>
        RestoreDeletedAsync(
            long id,
            CourseWorkflowRequest request,
            CancellationToken cancellationToken = default);

    // ============================================================
    // CHAPTER
    // ============================================================

    Task<Result<IReadOnlyList<CourseChapterAdminDto>>>
        GetChaptersAsync(
            long courseId,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default);

    Task<Result<CourseChapterAdminDto>>
        GetChapterAsync(
            long courseId,
            long chapterId,
            CancellationToken cancellationToken = default);

    Task<Result<CourseChapterAdminDto>>
        CreateChapterAsync(
            long courseId,
            CreateCourseChapterRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<CourseChapterAdminDto>>
        UpdateChapterAsync(
            long courseId,
            long chapterId,
            UpdateCourseChapterRequest request,
            CancellationToken cancellationToken = default);

    Task<Result>
        DeleteChapterAsync(
            long courseId,
            long chapterId,
            CourseEntityWorkflowRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<CourseChapterAdminDto>>
        RestoreChapterAsync(
            long courseId,
            long chapterId,
            CourseEntityWorkflowRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<bool>> ReorderChaptersAsync(long courseId, ReorderCourseChaptersRequest request, CancellationToken cancellationToken = default);

    // ============================================================
    // PREREQUISITE
    // ============================================================

    Task<Result<IReadOnlyList<CoursePrerequisiteAdminDto>>>
        GetPrerequisitesAsync(
            long courseId,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default);

    Task<Result<CoursePrerequisiteAdminDto>>
        GetPrerequisiteAsync(
            long courseId,
            long prerequisiteId,
            CancellationToken cancellationToken = default);

    Task<Result<CoursePrerequisiteAdminDto>>
        CreatePrerequisiteAsync(
            long courseId,
            CreateCoursePrerequisiteRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<CoursePrerequisiteAdminDto>>
        UpdatePrerequisiteAsync(
            long courseId,
            long prerequisiteId,
            UpdateCoursePrerequisiteRequest request,
            CancellationToken cancellationToken = default);

    Task<Result>
        DeletePrerequisiteAsync(
            long courseId,
            long prerequisiteId,
            CourseEntityWorkflowRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<CoursePrerequisiteAdminDto>>
        RestorePrerequisiteAsync(
            long courseId,
            long prerequisiteId,
            CourseEntityWorkflowRequest request,
            CancellationToken cancellationToken = default);

    // ============================================================
    // CHAPTER LESSONS
    // ============================================================

    Task<Result<IReadOnlyList<HanYu.Application.Features.Course.Admin.Chapters.Lessons.CourseChapterLessonAdminDto>>>
        GetChapterLessonsAsync(
            long courseId,
            long chapterId,
            CancellationToken cancellationToken = default);

    Task<Result<HanYu.Application.Features.Course.Admin.Chapters.Lessons.CourseChapterLessonAdminDto>>
        AssignLessonToChapterAsync(
            long courseId,
            long chapterId,
            HanYu.Application.Features.Course.Admin.Chapters.Lessons.AssignLessonToChapterRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<HanYu.Application.Features.Course.Admin.Chapters.Lessons.CourseChapterLessonAdminDto>>
        MoveLessonAsync(
            long courseId,
            long chapterId,
            long lessonId,
            HanYu.Application.Features.Course.Admin.Chapters.Lessons.MoveLessonToChapterRequest request,
            CancellationToken cancellationToken = default);

    Task<Result>
        RemoveLessonFromChapterAsync(
            long courseId,
            long chapterId,
            long lessonId,
            CancellationToken cancellationToken = default);

    Task<Result>
        ReorderChapterLessonsAsync(
            long courseId,
            long chapterId,
            HanYu.Application.Features.Course.Admin.Chapters.Lessons.ReorderChapterLessonsRequest request,
            CancellationToken cancellationToken = default);
}
