using HanYu.Application.Common.Models;
using HanYu.Application.Features.Lesson.Public;

namespace HanYu.Application.Interfaces.Lesson;

public interface IPublicLessonService
{
    Task<Result<PublicLessonDetailDto>>
        GetAccessibleLessonAsync(
            Guid publicId,
            CancellationToken cancellationToken = default);

    Task<Result>
        StartAsync(
            Guid publicId,
            CancellationToken cancellationToken = default);

    Task<Result>
        CompleteAsync(
            Guid publicId,
            string idempotencyKey,
            CancellationToken cancellationToken = default);
}
