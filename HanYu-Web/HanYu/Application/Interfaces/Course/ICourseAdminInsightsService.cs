using HanYu.Application.Common.Models;
using HanYu.Application.Features.Course.Admin.Insights;

namespace HanYu.Application.Interfaces.Course;

public interface ICourseAdminInsightsService
{
    Task<Result<IReadOnlyList<CourseHistoryItemDto>>> GetHistoryAsync(
        long courseId,
        CancellationToken cancellationToken = default);

    Task<Result<CourseStatisticsDto>> GetStatisticsAsync(
        long courseId,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<CourseStudentDto>>> GetStudentsAsync(
        long courseId,
        CourseStudentsQuery query,
        CancellationToken cancellationToken = default);
}
