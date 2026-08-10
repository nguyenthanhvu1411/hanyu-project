using HanYu.Application.Common.Models;
using HanYu.Application.Features.Course.Public;

namespace HanYu.Application.Interfaces.Course;

public interface IPublicCourseService
{
    Task<Result<PagedResult<PublicCourseListItemDto>>>
        GetPublishedCoursesAsync(
            PublicCourseQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<PublicCourseDetailDto>>
        GetCourseAsync(
            Guid publicId,
            CancellationToken cancellationToken = default);

    Task<Result<PublicCourseDetailDto>>
        GetCourseBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

    Task<Result<PublicCourseCurriculumDto>>
        GetCurriculumAsync(
            Guid publicId,
            CancellationToken cancellationToken = default);
}
