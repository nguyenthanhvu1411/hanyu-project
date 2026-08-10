using HanYu.Application.Common.Models;
using HanYu.Application.Features.Lesson.Public.Bookmarks;
using HanYu.Application.Features.Lesson.Public.Lessons;
using HanYu.Application.Features.Lesson.Public.Progress;

namespace HanYu.Application.Interfaces.Lesson;

public interface ILessonPublicService
{
    Task<Result<PagedResult<LessonListItemResponse>>> GetLessonsAsync(
        LessonQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<LessonDetailResponse>> GetLessonAsync(
        Guid? userId,
        Guid lessonPublicId,
        CancellationToken cancellationToken = default);

    Task<Result<LessonProgressResponse>> StartLessonAsync(
        Guid userId,
        Guid lessonPublicId,
        CancellationToken cancellationToken = default);

    Task<Result<LessonProgressResponse>> SaveProgressAsync(
        Guid userId,
        Guid lessonPublicId,
        SaveLessonProgressRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<LessonSectionProgressResponse>> SaveSectionProgressAsync(
        Guid userId,
        Guid lessonPublicId,
        Guid sectionPublicId,
        SaveSectionProgressRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<LessonProgressResponse>> CompleteLessonAsync(
        Guid userId,
        Guid lessonPublicId,
        CancellationToken cancellationToken = default);

    Task<Result> BookmarkAsync(
        Guid userId,
        Guid lessonPublicId,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveBookmarkAsync(
        Guid userId,
        Guid lessonPublicId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<LessonBookmarkResponse>>>
        GetBookmarksAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
}
