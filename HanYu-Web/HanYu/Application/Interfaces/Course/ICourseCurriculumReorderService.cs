using HanYu.Application.Common.Models;
using HanYu.Application.Features.Course.Admin;
using HanYu.Application.Features.Course.Admin.Chapters.Lessons;

namespace HanYu.Application.Interfaces.Course;

public interface ICourseCurriculumReorderService
{
    Task<Result<bool>> ReorderChaptersAsync(
        long courseId,
        ReorderCourseChaptersRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> ReorderChapterLessonsAsync(
        long courseId,
        long chapterId,
        ReorderChapterLessonsRequest request,
        CancellationToken cancellationToken = default);
}
