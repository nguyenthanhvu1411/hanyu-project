using HanYu.Application.Interfaces.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace HanYu.Infrastructure.Course;

public sealed class CourseCacheInvalidator
    : ICourseCacheInvalidator
{
    private const string LessonPublicGenerationKey =
        "lesson:public:generation";

    private readonly IDistributedCache _cache;
    private readonly ICacheService _lessonCache;

    public CourseCacheInvalidator(
        IDistributedCache cache,
        ICacheService lessonCache)
    {
        _cache = cache;
        _lessonCache = lessonCache;
    }

    public async Task InvalidatePublicCourseCacheAsync(
        CancellationToken cancellationToken = default)
    {
        await _cache.SetStringAsync(
            CourseCacheKeys.Version,
            Guid.NewGuid().ToString("N"),
            cancellationToken);

        // Lesson public list uses a generation-based cache. Lesson mutations that
        // affect Course curriculum already flow through this invalidator, so rotate
        // the Lesson generation as part of the same public-content invalidation.
        await _lessonCache.RemoveAsync(
            LessonPublicGenerationKey,
            cancellationToken);
    }
}
